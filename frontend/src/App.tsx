import { useMemo } from 'react'
import { useLocationContext } from './context/LocationContext'
import { WeatherList } from './components/WeatherList'
import { useWeather } from './hooks/useWeather'
import './App.css'

const HISTORICAL_DAYS_LIMIT = 7

const parseDateString = (value: string) => {
  const [year, month, day] = value.split('-').map(Number)

  if (Number.isNaN(year) || Number.isNaN(month) || Number.isNaN(day)) {
    return new Date(value)
  }

  const date = new Date(year, month - 1, day)
  date.setHours(0, 0, 0, 0)
  return date
}

function App() {
  const { status: locationStatus, error: locationError, refetch } = useLocationContext()
  const { status: weatherStatus, forecast, error: weatherError, refresh } = useWeather()

  const { todayForecast, upcomingForecast, historicalForecast, historicalDaysDisplayed } = useMemo(() => {
    if (!forecast) {
      return {
        todayForecast: undefined,
        upcomingForecast: [],
        historicalForecast: [],
        historicalDaysDisplayed: 0
      }
    }

    const today = new Date()
    today.setHours(0, 0, 0, 0)

    const tomorrow = new Date(today)
    tomorrow.setDate(tomorrow.getDate() + 1)

    const sortByDate = (a: { date: string }, b: { date: string }) =>
      parseDateString(a.date).getTime() - parseDateString(b.date).getTime()

    const sortedDaily = [...forecast.daily].sort(sortByDate)

    const exactToday = sortedDaily.find(item => parseDateString(item.date).getTime() === today.getTime())
    const todayItem =
      exactToday ??
      sortedDaily.find(item => parseDateString(item.date).getTime() > today.getTime()) ??
      sortedDaily.at(0)

    const upcomingItems = sortedDaily
      .filter(item => parseDateString(item.date) >= tomorrow)
      .slice(0, 7)

    const historicalItems = [...forecast.historical]
      .sort(sortByDate)
      .filter(item => parseDateString(item.date) < today)

    const limitedHistorical = historicalItems.slice(-HISTORICAL_DAYS_LIMIT)

    return {
      todayForecast: todayItem,
      upcomingForecast: upcomingItems,
      historicalForecast: limitedHistorical,
      historicalDaysDisplayed: limitedHistorical.length
    }
  }, [forecast])

  const isLoading = locationStatus === 'loading' || weatherStatus === 'loading'
  const hasError = locationStatus === 'error' || weatherStatus === 'error'

  return (
    <main className="app">
      <header className="app__header">
        <div>
          <h1>Clima en tu ubicación</h1>
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
          {todayForecast && (
            <article className="app__today" aria-live="polite">
              <header className="app__today-header">
                <h2>Clima de hoy</h2>
                <p>
                  {new Intl.DateTimeFormat('es-CL', {
                    weekday: 'long',
                    month: 'long',
                    day: 'numeric'
                  }).format(parseDateString(todayForecast.date))}
                </p>
              </header>
              <div className="app__today-temperature">
                <span className="app__today-temperature--primary">
                  {todayForecast.temperatureC.toFixed(1)}°C
                </span>
                <span className="app__today-temperature--secondary">
                  {todayForecast.temperatureF.toFixed(1)}°F
                </span>
              </div>
              <p className="app__today-summary">{todayForecast.summary}</p>
            </article>
          )}

          <section className="app__summary">
            <h2>Resumen de los próximos 7 días</h2>
            <p>
              Latitud: {forecast.location.latitude.toFixed(2)} | Longitud: {forecast.location.longitude.toFixed(2)}
            </p>
          </section>
          <WeatherList items={upcomingForecast} />
          <section className="app__summary">
            <h2>Histórico de los últimos {historicalDaysDisplayed} días</h2>
            <p>Datos recopilados para la misma ubicación detectada automáticamente.</p>
          </section>
          <WeatherList items={historicalForecast} />
        </>
      )}
    </main>
  )
}

export default App
