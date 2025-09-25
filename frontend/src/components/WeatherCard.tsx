import classNames from 'classnames'
import type { DailyForecast } from '../services/weatherService'

interface WeatherCardProps {
  forecast: DailyForecast
}

function parseForecastDate(value: string) {
  const [year, month, day] = value.split('-').map(Number)

  if (Number.isNaN(year) || Number.isNaN(month) || Number.isNaN(day)) {
    return new Date(value)
  }

  const date = new Date(year, month - 1, day)
  date.setHours(0, 0, 0, 0)
  return date
}

export function WeatherCard({ forecast }: WeatherCardProps) {
  const date = parseForecastDate(forecast.date)
  const formatter = new Intl.DateTimeFormat('es-CL', {
    weekday: 'long',
    month: 'short',
    day: 'numeric'
  })

  return (
    <article className="weather-card" aria-label={`Pronóstico para ${formatter.format(date)}`}>
      <header className="weather-card__header">
        <span className="weather-card__day">{formatter.format(date)}</span>
      </header>
      <div className="weather-card__body">
        <div className="weather-card__temperature">
          <span className="weather-card__temperature--primary">{forecast.temperatureC.toFixed(1)}°C</span>
          <span className="weather-card__temperature--secondary">{forecast.temperatureF.toFixed(1)}°F</span>
        </div>
        <p className={classNames('weather-card__summary')}>{forecast.summary}</p>
      </div>
    </article>
  )
}
