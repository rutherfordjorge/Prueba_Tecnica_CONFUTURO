declare module 'node:test' {
  type TestFn = () => void | Promise<void>
  export function test(name: string, fn: TestFn): void
}

declare module 'node:assert/strict' {
  export interface Assert {
    strictEqual<T>(actual: T, expected: T, message?: string): void
    ok(value: unknown, message?: string): asserts value
  }

  const assert: Assert
  export default assert
}
