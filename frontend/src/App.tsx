import { useLocationContext } from './context/LocationContext'
import { WeatherList } from './components/WeatherList'
import { useWeather } from './hooks/useWeather'
import './App.css'

function App() {
  const { status: locationStatus, location, error: locationError, refetch } = useLocationContext()
  const { status: weatherStatus, forecast, error: weatherError, refresh } = useWeather()

  const isLoading = locationStatus === 'loading' || weatherStatus === 'loading'
  const hasError = locationStatus === 'error' || weatherStatus === 'error'

  return (
    <main className="app">
      <header className="app__header">
        <div>
          <h1>Clima en tu ubicación</h1>
          {location && (
            <p className="app__subtitle">
              {location.city}, {location.region ? `${location.region}, ` : ''}{location.country}
            </p>
          )}
        </div>
        <div className="app__actions">
          <button type="button" onClick={refetch} className="app__button" aria-label="Actualizar ubicación">
            Actualizar ubicación
          </button>
          <button type="button" onClick={refresh} className="app__button" aria-label="Actualizar pronóstico">
            Actualizar pronóstico
          </button>
        </div>
      </header>

      {isLoading && <p className="app__status">Cargando información del clima...</p>}
      {hasError && <p className="app__status app__status--error">{locationError ?? weatherError}</p>}

      {forecast && !hasError && !isLoading && (
        <>
          <section className="app__summary">
            <h2>Resumen de los próximos 7 días</h2>
            <p>
              Latitud: {forecast.location.latitude.toFixed(2)} | Longitud: {forecast.location.longitude.toFixed(2)}
            </p>
          </section>
          <WeatherList items={forecast.daily} />
          <section className="app__summary">
            <h2>Histórico de los últimos 7 días</h2>
            <p>Datos recopilados para la misma ubicación detectada automáticamente.</p>
          </section>
          <WeatherList items={forecast.historical} />
        </>
      )}
    </main>
  )
}

export default App
