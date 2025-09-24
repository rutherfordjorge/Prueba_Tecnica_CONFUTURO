import { WeatherCard } from './WeatherCard'
import type { DailyForecast } from '../services/weatherService'

interface WeatherListProps {
  items: DailyForecast[]
}

export function WeatherList({ items }: WeatherListProps) {
  return (
    <section className="weather-list" aria-live="polite">
      {items.map(item => (
        <WeatherCard key={item.date} forecast={item} />
      ))}
    </section>
  )
}
