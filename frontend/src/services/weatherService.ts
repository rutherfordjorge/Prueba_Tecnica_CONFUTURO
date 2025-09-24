import { apiClient } from './apiClient'
import { z, type infer as Infer } from 'zod'

const dailySchema = z.object({
  date: z.string(),
  temperatureC: z.number(),
  temperatureF: z.number(),
  summary: z.string(),
  icon: z.string().optional()
})

const forecastSchema = z.object({
  location: z.object({
    city: z.string(),
    region: z.string().optional(),
    country: z.string(),
    latitude: z.number(),
    longitude: z.number()
  }),
  daily: z.array(dailySchema)
})

export type ForecastResponse = Infer<typeof forecastSchema>
export type DailyForecast = Infer<typeof dailySchema>

export async function fetchForecast(lat?: number, lon?: number): Promise<ForecastResponse> {
  const query = lat !== undefined && lon !== undefined ? `?latitude=${lat}&longitude=${lon}` : ''
  const response = await apiClient.get<ForecastResponse>(`/weather/forecast${query}`)
  return forecastSchema.parse(response.data)
}
