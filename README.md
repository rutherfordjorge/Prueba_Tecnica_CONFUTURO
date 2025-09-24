# Prueba_Tecnica_CONFUTURO
Desarrollar una aplicación que muestre la información del clima de los últimos 7 días, detectando la ubicación del usuario de manera automática

Requerimientos funcionales 

Frontend: 

Desarrollado en React con typescript. 

Debe implementar useContext para la gestión de estado global. 

Mostrar la información del clima correspondiente a la ubicación detectada. 

Backend: 

Desarrollado en C# .NET Core. 

Actuar como intermediario entre el frontend y las APIs externas. 

APIs a utilizar: 

Clima: OpenWeatherMap. 

Si conoces otra API totalmente gratuita que entregue la misma información, también puedes usarla. 

Geolocalización: ipgeolocation.io. 

Si conoces otra opción gratuita que cumpla el mismo propósito, también es válida. 

Consideraciones técnicas 

La aplicación debe tener una buena arquitectura (organización clara de carpetas, separación de responsabilidades y código mantenible). 

El backend debe ser la única capa que interactúe con las APIs externas (el frontend no debe consumir directamente las APIs de terceros). 
