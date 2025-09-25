# 📦 Prueba Técnica – CONFUTURO

Desarrollar una aplicación que muestre la información del clima de los últimos 7 días para una ubicación predeterminada, permitiendo opcionalmente consultar otras coordenadas manualmente.

---

## ✅ Requerimientos Funcionales

### 🌐 Frontend

- Desarrollado en **React** con **TypeScript**.
- Debe implementar `useContext` para la gestión de estado global.
- Mostrar la información del clima correspondiente a la ubicación detectada.

### ⚙️ Backend

- Desarrollado en **C# .NET Core**.
- Actuar como intermediario entre el frontend y las APIs externas.

---

## 🔗 APIs a Utilizar

### ☁️ Clima

- API principal sugerida: **[OpenWeatherMap](https://openweathermap.org/api)**
- Puedes utilizar otra API **totalmente gratuita** que entregue la misma información.
- En la solución implementada se utiliza **[Open-Meteo](https://open-meteo.com/)** como proveedor único y predeterminado.

---

## 🧱 Consideraciones Técnicas

- La aplicación debe tener **una buena arquitectura**:
  - Organización clara de carpetas.
  - Separación de responsabilidades.
  - Código limpio y mantenible.

- El **backend** debe ser la **única capa que interactúe con las APIs externas**.  
  El frontend **no debe** consumir directamente servicios de terceros.

---

## 📂 Estructura sugerida (opcional)
```plaintext
Prueba_Tecnica_CONFUTURO/
├── backend/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── WeatherApi/
│   ├── Program.cs
│   ├── Startup.cs
│   └── PruebaTecnicaConfuturo.csproj
├── frontend/
│   ├── public/
│   └── src/
│       ├── components/
│       ├── context/
│       ├── services/
│       ├── hooks/
│       ├── App.tsx
│       └── main.tsx
├── README.md
├── .gitignore
└── LICENSE

---

## 🚀 Cómo ejecutar el proyecto

### Backend (.NET 8 API)
1. Configura tus llaves en `backend/appsettings.json` o mediante variables de entorno:
   ```json
   "ExternalApis": {
     "Weather": {
       "ForecastBaseUrl": "https://api.open-meteo.com/",
       "HistoricalBaseUrl": "https://archive-api.open-meteo.com/",
       "Timezone": "auto"
     },
     "Geolocation": {
       "BaseUrl": "https://api.ipgeolocation.io/",
       "ApiKey": "TU_API_KEY"
     }
   }
   ```
2. Restaura dependencias y ejecuta la API:
   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```
3. Ejecuta los tests automatizados del backend:
   ```bash
   dotnet test
   ```
4. El servicio expone por defecto el endpoint:
   - `GET /api/weather/forecast?latitude={lat}&longitude={lon}`

> Si no se definen las llaves, la API responde con datos simulados para poder probar la solución sin depender de servicios externos.

### Frontend (React + Vite)
1. Define la URL base del backend en un archivo `.env` dentro de `frontend/`:
   ```env
   VITE_API_BASE_URL=http://localhost:5083/api
   ```
2. Ejecuta el cliente:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
3. Ejecuta los tests unitarios del frontend (utilizando el compilador de TypeScript y el runner nativo de Node.js):
   ```bash
   npm run test
   ```
4. La aplicación consume únicamente el backend propio y presenta el pronóstico de 7 días con estados de carga y error.

---

## 🧱 Arquitectura implementada

### Backend
- **Domain Driven Design básico** con entidades (`Location`, `DailyWeather`) y agregados (`ForecastReport`).
- Servicio especializado (`WeatherService`) que consume la API de Open-Meteo mediante `HttpClientFactory` y mapeos automáticos con **AutoMapper**.
- Servicio de geolocalización (`GeolocationService`) que utiliza **Refit** para consultar `ipgeolocation.io`, centralizando toda interacción con servicios externos.
- Validaciones con **FluentValidation** y manejo de errores resiliente: si la configuración es incompleta, se entrega un pronóstico simulado.
- Endpoints RESTful con documentación automática vía Swagger (entorno Development).

### Frontend
- Estado global mediante **Context + useReducer** para la ubicación detectada automáticamente desde el backend.
- **Custom hook `useWeather`** que coordina la obtención del pronóstico usando el contexto y peticiones con **axios** real.
- Componentización atómica (`WeatherCard`, `WeatherList`) y validación de respuestas con **zod**.
- Enrutamiento básico con **react-router-dom** y configuración de variables mediante **dotenv**.

---

## 🧪 Estrategia de validación
- **Frontend:** tests unitarios escritos en TypeScript y ejecutados con el `node:test` runner tras compilar con `tsc`, además de la build de Vite.
- **Backend:** suite de pruebas `xUnit` integrada en la solución para verificar los value objects críticos, ejecutable con `dotnet test`.

