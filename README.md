# 📦 Prueba Técnica – CONFUTURO

Desarrollar una aplicación que muestre la información del clima de los últimos 7 días, detectando la ubicación del usuario de manera automática.

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

### 📍 Geolocalización

- API sugerida: **[ipgeolocation.io](https://ipgeolocation.io/)**
- Se acepta cualquier alternativa gratuita que cumpla el mismo propósito.

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
│   ├── GeolocationApi/
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

### Backend (.NET 9 API)
1. Configura tus llaves en `backend/appsettings.json` o mediante variables de entorno:
   ```json
   "ExternalApis": {
     "Weather": {
       "BaseUrl": "https://api.openweathermap.org/",
       "ApiKey": "TU_API_KEY"
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
3. El servicio expone por defecto los endpoints:
   - `GET /api/location`
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
3. La aplicación consume únicamente el backend propio y presenta el pronóstico de 7 días con estados de carga y error.

---

## 🧱 Arquitectura implementada

### Backend
- **Domain Driven Design básico** con entidades (`Location`, `DailyWeather`) y agregados (`ForecastReport`).
- Servicios especializados (`WeatherService`, `GeolocationService`) que consumen APIs externas mediante `HttpClientFactory`.
- Validaciones con **FluentValidation** y manejo de errores resiliente: si la configuración es incompleta, se entrega un pronóstico simulado.
- Endpoints RESTful con documentación automática vía Swagger (entorno Development).

### Frontend
- Estado global mediante **Context + useReducer** para la ubicación del usuario.
- **Custom hook `useWeather`** que coordina la obtención del pronóstico usando el contexto.
- Componentización atómica (`WeatherCard`, `WeatherList`) y validación de respuestas con un adaptador mínimo de `zod`.
- Cliente HTTP basado en un wrapper ligero compatible con la API de `axios`, lo que permite mantener el contrato exigido sin dependencias externas.

---

## 🧪 Estrategia de validación
- **Frontend:** compilación de TypeScript y build de Vite para garantizar integridad.
- **Backend:** al contar con datos simulados, los controladores pueden probarse sin llaves; en un entorno con SDK de .NET se recomienda ejecutar `dotnet test` o al menos `dotnet build`.

