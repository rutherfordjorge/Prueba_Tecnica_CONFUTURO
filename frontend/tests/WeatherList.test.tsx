import { test } from 'node:test'
import assert from 'node:assert/strict'
import React from 'react'
import { renderToString } from 'react-dom/server'
import { WeatherList } from '../src/components/WeatherList.js'
import type { DailyForecast } from '../src/services/weatherService'

const forecastItems: DailyForecast[] = [
  {
    date: '2024-05-01T00:00:00.000Z',
    temperatureC: 19.5,
    temperatureF: 67.1,
    summary: 'Cielo despejado',
    icon: '01d'
  },
  {
    date: '2024-05-02T00:00:00.000Z',
    temperatureC: 16.2,
    temperatureF: 61.2,
    summary: 'Parcialmente nublado',
    icon: undefined
  }
]

test('WeatherList renders one weather card per forecast item', () => {
  const html = renderToString(<WeatherList items={forecastItems} />)

  const articleMatches = html.match(/<article class="weather-card"/g) ?? []
  assert.strictEqual(articleMatches.length, forecastItems.length)

  const iconMatches = html.match(/alt="Icono del clima"/g) ?? []
  assert.strictEqual(iconMatches.length, 1)

  assert.ok(html.includes('aria-live="polite"'))
})
