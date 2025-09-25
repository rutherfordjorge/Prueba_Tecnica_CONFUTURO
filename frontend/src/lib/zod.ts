type Parser<T> = (input: unknown) => T

export interface Schema<T> {
  parse(input: unknown): T
  optional(): Schema<T | undefined>
  nullable(): Schema<T | null>
  nullish(): Schema<T | null | undefined>
  transform<U>(mapper: (value: T) => U): Schema<U>
}

function createSchema<T>(parser: Parser<T>): Schema<T> {
  return {
    parse: parser,
    optional(): Schema<T | undefined> {
      return createSchema(input => {
        if (input === undefined) return undefined as T | undefined
        return parser(input)
      })
    },
    nullable(): Schema<T | null> {
      return createSchema(input => {
        if (input === null) return null
        return parser(input)
      })
    },
    nullish(): Schema<T | null | undefined> {
      return createSchema(input => {
        if (input === undefined || input === null) {
          return input as T | null | undefined
        }
        return parser(input)
      })
    },
    transform<U>(mapper: (value: T) => U): Schema<U> {
      return createSchema(input => mapper(parser(input)))
    }
  }
}

function string(): Schema<string> {
  return createSchema(input => {
    if (typeof input !== 'string') {
      throw new Error('Expected string')
    }
    return input
  })
}

function number(): Schema<number> {
  return createSchema(input => {
    if (typeof input !== 'number' || Number.isNaN(input)) {
      throw new Error('Expected number')
    }
    return input
  })
}

type SchemaShape = Record<string, Schema<unknown>>

type InferShape<T extends SchemaShape> = { [K in keyof T]: ReturnType<T[K]['parse']> }

function object<T extends SchemaShape>(shape: T): Schema<InferShape<T>> {
  return createSchema(input => {
    if (typeof input !== 'object' || input === null) {
      throw new Error('Expected object')
    }

    const result: Record<string, unknown> = {}
    for (const key in shape) {
      const schema = shape[key]
      result[key] = schema.parse((input as Record<string, unknown>)[key])
    }
    return result as InferShape<T>
  })
}

function array<T>(schema: Schema<T>): Schema<T[]> {
  return createSchema(input => {
    if (!Array.isArray(input)) {
      throw new Error('Expected array')
    }

    return input.map(item => schema.parse(item))
  })
}

export const z = {
  string,
  number,
  object,
  array
}

export type infer<T extends Schema<unknown>> = T extends Schema<infer U> ? U : never
