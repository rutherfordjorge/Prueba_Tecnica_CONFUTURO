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

function hasGeolocation(): boolean {
  return typeof navigator !== 'undefined' && 'geolocation' in navigator
}

function fromGeolocationPosition(position: GeolocationPosition): LocationDto {
  const { latitude, longitude } = position.coords

  return {
    city: 'Ubicación detectada',
    region: undefined,
    country: '',
    latitude,
    longitude
  }
}

function resolveWithFallback(resolve: (value: LocationDto) => void) {
  resolve({ ...fallbackLocation })
}

export async function fetchLocation(): Promise<LocationDto> {
  if (!hasGeolocation()) {
    return { ...fallbackLocation }
  }

  return await new Promise<LocationDto>((resolve) => {
    navigator.geolocation.getCurrentPosition(
      (position) => resolve(fromGeolocationPosition(position)),
      () => resolveWithFallback(resolve),
      {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 5 * 60 * 1000
      }
    )
  })
}

export function getDefaultLocation(): LocationDto {
  return { ...fallbackLocation }
}
