# Práctica 2 - Citizen API

## Descripción
En esta práctica desarrollé una API en .NET para registrar ciudadanos. La aplicación permite crear, listar, buscar, actualizar y eliminar ciudadanos. Además, al crear un ciudadano se genera un grupo sanguíneo aleatorio y se obtiene un objeto aleatorio desde una API externa. Los datos se guardan en un archivo CSV.

## Tecnologías usadas
- .NET 10
- ASP.NET Core Web API
- Swagger / Swashbuckle
- Serilog
- Git y GitHub
- Visual Studio Code

## Cómo ejecutar el proyecto
```bash
cd CitizenApi
dotnet restore
dotnet build
dotnet run
```

## Swagger

https://localhost:9070/swagger

## Aplicación de los 12 factores en mi proyecto

## 1. Codebase
En mi proyecto manejo una sola base de código y la controlo con Git. Trabajé con las ramas `main`, `develop` y `P2-001`, lo que me permitió organizar mejor los cambios y mantener un flujo de trabajo ordenado. Además, el repositorio se encuentra alojado en GitHub, por lo que el historial de cambios queda centralizado en un solo lugar.

## 2. Dependencies
Las dependencias de mi proyecto están declaradas de forma explícita en el archivo `.csproj`. Esto permite que cualquier persona que descargue el proyecto pueda restaurarlas con los comandos de .NET, sin necesidad de instalarlas manualmente una por una. Entre las dependencias principales están Swagger para documentar la API y Serilog para el sistema de logs.

## 3. Config
La configuración de la aplicación no está completamente escrita dentro del código. Utilicé `appsettings.json` para la configuración general y `appsettings.Development.json` para la configuración específica del entorno de desarrollo. En estos archivos definí, por ejemplo, la ruta del archivo CSV, la URL base de la API externa y la configuración de Serilog.

## 4. Backing Services
En mi proyecto utilicé una API externa para obtener el `PersonalAsset` de cada ciudadano y un archivo CSV para guardar los datos de forma persistente. La API externa sí funciona como un servicio de apoyo para la aplicación. 

## 5. Build, Release, Run
En el proyecto puedo separar de forma básica las etapas de compilación y ejecución. Por ejemplo, utilizo `dotnet build` para compilar y `dotnet run` para ejecutar la aplicación.

## 6. Processes
La aplicación funciona como un proceso web de ASP.NET Core. En una primera etapa trabajé con una lista en memoria, pero después cambié la persistencia a un archivo CSV, lo que redujo la dependencia del estado temporal en memoria. 

## 7. Port Binding
La aplicación expone su servicio directamente a través de un puerto configurado. En este proyecto trabajé con `https://localhost:9070`, definido en `launchSettings.json`. Esto permite que la API se ejecute directamente como un servicio accesible desde el navegador o desde herramientas como Swagger.

## 8. Concurrency
No implementé concurrencia ni escalamiento horizontal en este proyecto. La aplicación está pensada para ejecutarse como una sola instancia local durante el desarrollo. Por esa razón, este factor no se encuentra aplicado de manera completa.

## 9. Disposability
La aplicación inicia y se detiene rápidamente, lo cual facilita las pruebas y el desarrollo. Además, como los datos se guardan en un archivo CSV, al reiniciar la aplicación no se pierde la información registrada. Esto mejora el comportamiento del sistema.

## 10. Dev / Prod Parity
Separé la configuración general de la configuración de desarrollo mediante `appsettings.json` y `appsettings.Development.json`. Esto ayuda a no mezclar todo en un solo archivo y facilita adaptar el proyecto a distintos entornos. 

## 11. Logs
Implementé el registro de eventos usando Serilog. Los logs se envían tanto a consola como a archivo, lo que permite revisar qué sucede en la aplicación mientras se ejecuta. También registré eventos importantes como lectura y escritura del CSV, creación, actualización y eliminación de ciudadanos, llamadas a la API externa y errores capturados con `try-catch`.

## 12. Admin Processes
No implementé procesos administrativos separados, como scripts independientes para mantenimiento, migraciones o tareas especiales. Toda la lógica principal se ejecuta directamente desde la API. Por eso, este factor no está aplicado completamente en el proyecto.