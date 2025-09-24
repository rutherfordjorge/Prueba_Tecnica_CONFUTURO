import { useEffect, useState } from 'react'
import { fetchWeather } from '../services/weatherService'
import type { WeatherForecast } from '../services/weatherService'

const WeatherComponent = () => {
  const [data, setData] = useState<WeatherForecast[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchWeather()
      .then(setData)
      .catch(err => console.error("❌ Error en fetchWeather:", err))
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <p>Cargando clima...</p>

  return (
    <div>
      <h2>Pronóstico del Clima</h2>
      <ul>
        {data.map((item, index) => (
          <li key={index}>
            <strong>{item.date}</strong>: {item.temperatureC}°C / {item.temperatureF}°F – {item.summary}
          </li>
        ))}
      </ul>
    </div>
  )
}

export default WeatherComponent
