import { apiClient } from './apiClient'
import { z, type infer as Infer } from 'zod'

const locationSchema = z.object({
  city: z.string(),
  region: z
    .string()
    .nullish()
    .transform((value) => value ?? undefined),
  country: z.string(),
  latitude: z.number(),
  longitude: z.number()
})

export type LocationDto = Infer<typeof locationSchema>

export async function fetchLocation(): Promise<LocationDto> {
  const response = await apiClient.get<LocationDto>('/location')
  return locationSchema.parse(response.data)
}
