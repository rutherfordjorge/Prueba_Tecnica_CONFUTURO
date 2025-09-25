import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { LocationProvider } from './context/LocationContext'
import { BrowserRouter } from 'react-router-dom'
import { AppRouter } from './routes/AppRouter'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <LocationProvider>
        <AppRouter />
      </LocationProvider>
    </BrowserRouter>
  </StrictMode>,
)
