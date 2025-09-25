# 📋 Requirements Técnicos – Prueba Técnica CONFUTURO

Este documento define los requisitos técnicos completos del proyecto solicitado en la prueba técnica CONFUTURO, cubriendo tecnologías, librerías, buenas prácticas, diseño de arquitectura y detalles de implementación para asegurar un desarrollo estructurado, limpio y escalable.

---

## 🧰 Tecnologías Base

### 🖥️ Frontend
- **React** 18+
- **TypeScript**
- **Vite** (como herramienta de bundling)

### 🧠 Estado Global
- `useContext` + `useReducer` (para estado compartido)
- `React.Context` modularizado por feature

### 🌐 Backend
- **.NET Core 8.0** (o mínimo .NET 6.0)
- Arquitectura en capas (Controller, Service, Domain, Infrastructure)
- API REST JSON

---

## 📦 Librerías y Paquetes

### Frontend (NPM)
- `axios`: para consumo del backend
- `dotenv`: manejo de variables de entorno
- `react-router-dom`: navegación SPA (si aplica)
- `zod` o `yup`: validación de schemas
- `classnames`: manejo dinámico de clases CSS

### Backend (NuGet)
- `Swashbuckle.AspNetCore` – Swagger UI
- `Refit` o `RestSharp` – consumo de APIs externas
- `Microsoft.Extensions.Http`
- `Newtonsoft.Json` o `System.Text.Json`
- `AutoMapper`
- `FluentValidation`
- `Microsoft.AspNetCore.Cors`

---

## 📚 Patrones y Principios

### ✅ SOLID
- **S**ingle Responsibility: cada clase cumple una única función
- **O**pen/Closed: servicios extensibles, no modificables
- **L**iskov Substitution: respetar la herencia con interfaces
- **I**nterface Segregation: interfaces por módulo
- **D**ependency Inversion: inyección de dependencias (`IService`, `IHttpClient`, etc.)

### ⚙️ DDD (Domain-Driven Design)
- **Entidades**: `WeatherForecast`, `Location`
- **Value Objects**: `Coordinates`, `Temperature`
- **Agregados**: `ForecastReport`
- **Servicios de Dominio**: `ForecastService`
- **Repositorios (si aplica)**: simulados o mocks en esta prueba

### 🧼 YAGNI (You Aren’t Gonna Need It)
- Evitar anticipar funcionalidades no requeridas
- No agregar infraestructura si no es necesaria en esta prueba

### 🔐 Manejo de Errores / Try-Catch
- Uso de `try/catch` en todas las llamadas externas (API, DB)
- Uso de `ILogger` para logueo de excepciones
- Control de errores HTTP con códigos estándar (`400`, `404`, `500`)

---

## 🔌 Interconexiones

### Flujo General
```text
[Cliente] → [Frontend React] → [API .NET Core] → [API Open-Meteo]
```

### Detalles
- El Frontend **solicita al Backend** la data del clima
- El Backend **usa HTTPClient** o `Refit` para consultar una API meteorológica gratuita (OpenWeatherMap, Open-Meteo, etc.)
- El Backend retorna al Frontend **una respuesta consolidada y simplificada**

---

## 🌐 Endpoints Sugeridos

### GET /weather/current
- 📥 Parámetros: implícitos por IP detectada
- 📤 Devuelve: condiciones actuales + últimos 7 días

---

## 🧩 Componentes Frontend

- `WeatherCard.tsx`: muestra día, temperatura, ícono
- `WeatherList.tsx`: renderiza lista de 7 días
- `LocationContext.tsx`: contexto global para lat/lon/ciudad
- `useWeather.ts`: custom hook para lógica de consumo de clima

---

## 📁 Estructura de Carpetas Detallada

### Backend
```
backend/
├── Controllers/
│   └── WeatherController.cs
├── Services/
│   └── WeatherService.cs
├── Models/
│   └── ForecastDto.cs
├── WeatherApi/
├── Program.cs
├── Startup.cs
└── PruebaTecnicaConfuturo.csproj
```

### Frontend
```
frontend/src/
├── components/
│   ├── WeatherCard.tsx
│   └── WeatherList.tsx
├── context/
│   └── LocationContext.tsx
├── services/
│   └── weatherService.ts
├── hooks/
│   └── useWeather.ts
├── App.tsx
└── main.tsx
```

---

## 🧪 Validaciones Adicionales

- Validar que latitud y longitud estén presentes antes de llamar la API
- Mostrar estados de carga y error en UI
- Control de errores en todos los fetch/axios

---

## 🔚 Notas Finales

- Se valorará especialmente el uso correcto de patrones, respeto a separación de capas y claridad en el código.
- La prueba puede ser ejecutada localmente sin necesidad de base de datos.
- Es aceptable mockear la respuesta de APIs si hay problemas de límite gratuito.

