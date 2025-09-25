import { z } from 'zod'
import { apiClient } from './apiClient'

export type LocationDto = {
  city: string
  region?: string
  country: string
  latitude: number
  longitude: number
}

const fallbackLocation: LocationDto = {
  city: 'Santiago',
  region: 'Metropolitana',
  country: 'Chile',
  latitude: -33.4489,
  longitude: -70.6693
}

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

export async function fetchLocation(): Promise<LocationDto> {
  try {
    const response = await apiClient.get<LocationDto>('/location/resolve')
    return locationSchema.parse(response.data)
  } catch (error) {
    console.error('Failed to resolve location from backend', error)
    throw error
  }
}

export function getDefaultLocation(): LocationDto {
  return { ...fallbackLocation }
}
