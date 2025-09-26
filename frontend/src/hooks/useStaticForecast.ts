import { useCallback, useEffect, useMemo, useState } from 'react'
import { fetchForecast, type ForecastResponse } from '../services/weatherService'

type StaticForecastState = {
  status: 'idle' | 'loading' | 'ready' | 'error'
  forecast?: ForecastResponse
  error?: string
}

type UseStaticForecastOptions = {
  latitude: number
  longitude: number
  errorMessage?: string
}

const initialState: StaticForecastState = {
  status: 'idle'
}

export function useStaticForecast({ latitude, longitude, errorMessage }: UseStaticForecastOptions) {
  const [state, setState] = useState<StaticForecastState>(initialState)

  const loadForecast = useCallback(async () => {
    setState({ status: 'loading' })

    try {
      const data = await fetchForecast(latitude, longitude)
      setState({ status: 'ready', forecast: data })
    } catch (error) {
      console.error('Failed to fetch static forecast', error)
      setState({
        status: 'error',
        error: errorMessage ?? 'No se pudo obtener el pronóstico del clima.'
      })
    }
  }, [latitude, longitude, errorMessage])

  useEffect(() => {
    loadForecast()
  }, [loadForecast])

  return useMemo(
    () => ({
      ...state,
      refresh: loadForecast
    }),
    [state, loadForecast]
  )
}
