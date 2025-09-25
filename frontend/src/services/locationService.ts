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

export async function fetchLocation(): Promise<LocationDto> {
  return { ...fallbackLocation }
}

export function getDefaultLocation(): LocationDto {
  return { ...fallbackLocation }
}
