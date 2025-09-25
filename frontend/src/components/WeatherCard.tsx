import classNames from 'classnames'
import type { DailyForecast } from '../services/weatherService'

interface WeatherCardProps {
  forecast: DailyForecast
}

const WEATHER_THEME_RULES: Array<{ className: string; keywords: string[] }> = [
  { className: 'weather-card--storm', keywords: ['tormenta', 'tempestad', 'electrica'] },
  { className: 'weather-card--rainy', keywords: ['lluvia', 'llovizna', 'chubasco', 'chaparron'] },
  { className: 'weather-card--snowy', keywords: ['nieve', 'nevadas', 'nevando', 'nevada'] },
  { className: 'weather-card--foggy', keywords: ['niebla', 'neblina', 'bruma'] },
  { className: 'weather-card--windy', keywords: ['viento', 'ventoso', 'rafaga', 'brisa'] },
  {
    className: 'weather-card--partly-cloudy',
    keywords: ['parcialmente nublado', 'intervalos nubosos', 'nubes y sol', 'nuboso']
  },
  { className: 'weather-card--cloudy', keywords: ['nublado', 'cubierto', 'overcast'] },
  { className: 'weather-card--sunny', keywords: ['soleado', 'soleada', 'despejado', 'sol'] }
]

function normalizeSummary(value: string) {
  return value
    .trim()
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, '')
    .toLowerCase()
}

function getWeatherThemeClass(summary: string | undefined) {
  if (!summary) {
    return 'weather-card--default'
  }

  const normalized = normalizeSummary(summary)

  for (const theme of WEATHER_THEME_RULES) {
    if (theme.keywords.some(keyword => normalized.includes(keyword))) {
      return theme.className
    }
  }

  return 'weather-card--default'
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
  const themeClass = getWeatherThemeClass(forecast.summary)

  return (
    <article
      className={classNames('weather-card', themeClass)}
      aria-label={`Pronóstico para ${formatter.format(date)}`}
    >
      <span className="weather-card__effects" aria-hidden="true" />
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
