import { createContext, useContext, useEffect, useMemo, useReducer } from 'react'
import { fetchLocation, type LocationDto } from '../services/locationService'

type LocationStatus = 'idle' | 'loading' | 'ready' | 'error'

type LocationState = {
  status: LocationStatus
  location?: LocationDto
  error?: string
}

type LocationAction =
  | { type: 'REQUEST' }
  | { type: 'SUCCESS'; payload: LocationDto }
  | { type: 'FAILURE'; error: string }

type LocationContextValue = LocationState & {
  refetch: () => void
}

const initialState: LocationState = {
  status: 'idle'
}

function reducer(state: LocationState, action: LocationAction): LocationState {
  switch (action.type) {
    case 'REQUEST':
      return { status: 'loading' }
    case 'SUCCESS':
      return { status: 'ready', location: action.payload }
    case 'FAILURE':
      return { status: 'error', error: action.error }
    default:
      return state
  }
}

const LocationContext = createContext<LocationContextValue | undefined>(undefined)

export function LocationProvider({ children }: { children: React.ReactNode }) {
  const [state, dispatch] = useReducer(reducer, initialState)

  useEffect(() => {
    let isMounted = true

    async function load() {
      dispatch({ type: 'REQUEST' })
      try {
        const location = await fetchLocation()
        if (!isMounted) return
        dispatch({ type: 'SUCCESS', payload: location })
      } catch (error) {
        console.error('Failed to resolve location', error)
        if (!isMounted) return
        dispatch({ type: 'FAILURE', error: 'No se pudo obtener la ubicación.' })
      }
    }

    load()
    return () => {
      isMounted = false
    }
  }, [])

  const value = useMemo<LocationContextValue>(() => ({
    ...state,
    refetch: async () => {
      dispatch({ type: 'REQUEST' })
      try {
        const location = await fetchLocation()
        dispatch({ type: 'SUCCESS', payload: location })
      } catch (error) {
        console.error('Failed to resolve location', error)
        dispatch({ type: 'FAILURE', error: 'No se pudo obtener la ubicación.' })
      }
    }
  }), [state])

  return <LocationContext.Provider value={value}>{children}</LocationContext.Provider>
}

export function useLocationContext() {
  const context = useContext(LocationContext)
  if (!context) {
    throw new Error('useLocationContext debe usarse dentro de LocationProvider')
  }
  return context
}
