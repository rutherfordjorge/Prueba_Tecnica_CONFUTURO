export type AxiosResponse<T> = {
  data: T
  status: number
}

export type AxiosError = Error & {
  status?: number
}

type RequestConfig = {
  baseURL?: string
  headers?: Record<string, string>
}

const defaultConfig: RequestConfig = {
  headers: {
    'Content-Type': 'application/json'
  }
}

function mergeConfig(base: RequestConfig, override?: RequestConfig): RequestConfig {
  return {
    ...base,
    ...override,
    headers: {
      ...base.headers,
      ...override?.headers
    }
  }
}

async function request<T>(url: string, init?: RequestInit): Promise<AxiosResponse<T>> {
  const response = await fetch(url, init)
  const data = (await response.json()) as T

  if (!response.ok) {
    const error: AxiosError = new Error('Request failed')
    error.status = response.status
    throw error
  }

  return { data, status: response.status }
}

export function create(config?: RequestConfig) {
  const finalConfig = mergeConfig(defaultConfig, config)

  return {
    async get<T>(path: string, options?: RequestConfig): Promise<AxiosResponse<T>> {
      const cfg = mergeConfig(finalConfig, options)
      const url = cfg.baseURL ? `${cfg.baseURL}${path}` : path
      return request<T>(url, { headers: cfg.headers })
    }
  }
}

const axios = create()

export default axios
