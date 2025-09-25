import { createContext, useCallback, useContext, useEffect, useMemo, useReducer } from 'react'
import { fetchLocation, getDefaultLocation, type LocationDto } from '../services/locationService'

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
  status: 'idle',
  location: getDefaultLocation()
}

function reducer(state: LocationState, action: LocationAction): LocationState {
  switch (action.type) {
    case 'REQUEST':
      return { status: 'loading', location: state.location }
    case 'SUCCESS':
      return { status: 'ready', location: action.payload }
    case 'FAILURE':
      return { status: 'error', location: state.location, error: action.error }
    default:
      return state
  }
}

const LocationContext = createContext<LocationContextValue | undefined>(undefined)

export function LocationProvider({ children }: { children: React.ReactNode }) {
  const [state, dispatch] = useReducer(reducer, initialState)

  const resolveLocation = useCallback(async () => {
    dispatch({ type: 'REQUEST' })

    try {
      const location = await fetchLocation()
      dispatch({ type: 'SUCCESS', payload: location })
    } catch (error) {
      console.error('Failed to resolve location', error)
      dispatch({ type: 'FAILURE', error: 'No se pudo obtener la ubicación.' })
    }
  }, [])

  useEffect(() => {
    void resolveLocation()
  }, [resolveLocation])

  const value = useMemo<LocationContextValue>(() => ({
    ...state,
    refetch: resolveLocation
  }), [state, resolveLocation])

  return <LocationContext.Provider value={value}>{children}</LocationContext.Provider>
}

// eslint-disable-next-line react-refresh/only-export-components
export function useLocationContext() {
  const context = useContext(LocationContext)
  if (!context) {
    throw new Error('useLocationContext debe usarse dentro de LocationProvider')
  }
  return context
}
