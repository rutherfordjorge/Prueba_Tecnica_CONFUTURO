import { useCallback, useEffect, useMemo, useState } from 'react'
import { useLocationContext } from '../context/LocationContext'
import { fetchForecast, type ForecastResponse } from '../services/weatherService'

type WeatherState = {
  status: 'idle' | 'loading' | 'ready' | 'error'
  forecast?: ForecastResponse
  error?: string
}

const initialState: WeatherState = {
  status: 'idle'
}

export function useWeather() {
  const { location, status: locationStatus } = useLocationContext()
  const [state, setState] = useState<WeatherState>(initialState)

  const loadForecast = useCallback(async () => {
    if (!location) return

    setState({ status: 'loading' })
    try {
      const data = await fetchForecast(location.latitude, location.longitude)
      setState({ status: 'ready', forecast: data })
    } catch (error) {
      console.error('Failed to fetch forecast', error)
      setState({ status: 'error', error: 'No se pudo obtener el pronóstico del clima.' })
    }
  }, [location])

  useEffect(() => {
    if (locationStatus === 'ready' && location) {
      loadForecast()
    }
  }, [locationStatus, location, loadForecast])

  return useMemo(
    () => ({
      ...state,
      refresh: loadForecast
    }),
    [state, loadForecast]
  )
}
