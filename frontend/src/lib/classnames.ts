export default function classNames(
  ...inputs: Array<string | undefined | null | false | Record<string, boolean>>
): string {
  const classes: string[] = []

  inputs.forEach(input => {
    if (!input) return

    if (typeof input === 'string') {
      classes.push(input)
      return
    }

    Object.entries(input).forEach(([key, value]) => {
      if (value) classes.push(key)
    })
  })

  return classes.join(' ')
}
