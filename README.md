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
