// frontend/src/services/weatherService.ts

// 👇 INTERFAZ EXPORTADA
export interface WeatherForecast {
  date: string
  temperatureC: number
  temperatureF: number
  summary: string
}

// 👇 FUNCIÓN EXPORTADA
export async function fetchWeather(): Promise<WeatherForecast[]> {
  const baseUrl = import.meta.env.VITE_API_BASE_URL
  if (!baseUrl) {
    throw new Error("⚠️ VITE_API_BASE_URL no está definida en .env")
  }

  const res = await fetch(`${baseUrl}/weather`)
  if (!res.ok) {
    throw new Error("⚠️ Error al obtener el clima")
  }

  return res.json()
}
