import classNames from 'classnames'
import type { DailyForecast } from '../services/weatherService'

interface WeatherCardProps {
  forecast: DailyForecast
}

export function WeatherCard({ forecast }: WeatherCardProps) {
  const date = new Date(forecast.date)
  const formatter = new Intl.DateTimeFormat('es-CL', {
    weekday: 'long',
    month: 'short',
    day: 'numeric'
  })

  return (
    <article className="weather-card" aria-label={`Pronóstico para ${formatter.format(date)}`}>
      <header className="weather-card__header">
        <span className="weather-card__day">{formatter.format(date)}</span>
        {forecast.icon && (
          <img
            src={`https://openweathermap.org/img/wn/${forecast.icon}@2x.png`}
            alt="Icono del clima"
            className="weather-card__icon"
            loading="lazy"
          />
        )}
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
