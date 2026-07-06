# Zuli Airlines - Sistema de Reservas Aéreas

Zuli Airlines es una plataforma académica para gestionar la operación de una aerolínea. El sistema permite buscar vuelos, comprar boletos, consultar itinerarios, cancelar reservas, gestionar equipaje adicional y administrar recursos internos como usuarios, aviones, aeropuertos, rutas, vuelos y reportes.

El proyecto también contempla integración con APIs de otras aerolíneas para ofrecer conexiones externas y permitir que el viajero tramite su itinerario desde un único sitio web.

## Contenido

- [Características principales](#características-principales)
- [Arquitectura del proyecto](#arquitectura-del-proyecto)
- [Tecnologías](#tecnologías)
- [Prerrequisitos](#prerrequisitos)
- [Configuración local](#configuración-local)
- [Instalación y ejecución](#instalación-y-ejecución)
- [Contenedores y GitHub Actions](#contenedores-y-github-actions)
- [Pruebas](#pruebas)
- [Documentación API](#documentación-api)
- [Contribuyentes](#contribuyentes)
- [Autores](#autores)
- [Licencia](#licencia)
- [Estado del proyecto](#estado-del-proyecto)

## Características principales

- Búsqueda de vuelos por origen, destino y fecha.
- Compra de boletos y generación de confirmaciones de compra.
- Consulta de reservaciones e itinerarios.
- Cancelación de reservas mediante flujo de confirmación.
- Registro y compra de equipaje adicional.
- Administración de usuarios, roles y activación de cuentas.
- Administración de aeronaves, aeropuertos, rutas y vuelos.
- Reportes administrativos de ingresos y vuelos.
- Envío de correos para confirmaciones y procesos operativos.
- Generación de PDF y códigos QR para documentos de viaje.
- API externa para exponer disponibilidad de vuelos a otras aerolíneas.

## Arquitectura del proyecto

```text
zuli_airlines/
├── zuli_backend/        # API principal ASP.NET Core
├── zuli_external_API/   # API para integración con aerolíneas externas
├── zuli_frontend/       # Aplicación web Vue 3 + Vite
├── zuli_database/       # Scripts SQL de base de datos
└── .github/workflows/   # CI para pruebas automatizadas
```

La solución está separada por capas:

- `zuli_backend/zuli_backend`: API principal y configuración de servicios.
- `zuli_backend/zuli_buisiness`: lógica de negocio, validaciones, generación de PDFs, correos y utilidades.
- `zuli_backend/zuli_Repository`: acceso a datos mediante repositorios.
- `zuli_backend/zuli_Data`: entidades, DTOs, contexto Dapper y tipos compartidos.
- `zuli_external_API`: API independiente para integración con otras aerolíneas.
- `zuli_frontend`: interfaz de usuario para clientes y personal administrativo.

## Tecnologías

- .NET 10 / ASP.NET Core Web API.
- Vue 3, Vite y Tailwind CSS.
- SQL Server.
- Dapper y Microsoft.Data.SqlClient.
- NUnit, Moq y Selenium para pruebas.
- Mapster para mapeo de objetos.
- FluentValidation para validaciones.
- QuestPDF y QRCoder para documentos PDF y códigos QR.
- Scalar/OpenAPI para documentación de endpoints.

## Prerrequisitos

Antes de ejecutar el proyecto, instale:

- [.NET SDK 10.0.x](https://dotnet.microsoft.com/).
- [Node.js](https://nodejs.org/) y npm.
- SQL Server local o una instancia accesible.
- Git.

Opcional para pruebas de UI:

- Google Chrome o Chromium compatible con Selenium WebDriver.

## Configuración local

### Base de datos

1. Cree una base de datos SQL Server llamada `ZuliAirlines`.
2. Ejecute los scripts ubicados en `zuli_database/`.
3. Para reconstruir la base desde cero puede usar como referencia `zuli_database/39_new_Master_scripts.sql` y luego aplicar los scripts incrementales posteriores cuando corresponda.

### Variables y cadenas de conexión

Configure la cadena de conexión y los valores sensibles mediante variables de entorno, secretos de usuario de .NET o archivos locales no compartidos.

Valores requeridos por la API principal:

```text
ConnectionStrings__DefaultConnection=<cadena-de-conexion-sql-server>
settings__secretKey=<clave-para-jwt>
Frontend__BaseUrl=http://localhost:5173
EmailSettings__SmtpHost=<servidor-smtp>
EmailSettings__SmtpPort=<puerto-smtp>
EmailSettings__SmtpUser=<usuario-smtp>
EmailSettings__SmtpPassword=<password-smtp>
EmailSettings__FromEmail=<correo-remitente>
EmailSettings__FromName=Zuli Airlines
```

Valores relevantes para la API externa:

```text
ConnectionStrings__DefaultConnection=<cadena-de-conexion-sql-server>
settings__secretKey=<clave-para-jwt>
```

## Instalación y ejecución

### 1. Restaurar dependencias .NET

Desde la raíz del repositorio:

```bash
dotnet restore zuli_airlines.sln
```

### 2. Ejecutar backend principal

```bash
dotnet run --project zuli_backend/zuli_backend/zuli_backend.csproj --launch-profile https
```

Por defecto queda disponible en:

- `https://localhost:7034`
- `http://localhost:5001`

### 3. Ejecutar API externa

En otra terminal:

```bash
dotnet run --project zuli_external_API/zuli_backend/zuli_external_API.csproj --launch-profile https
```

Por defecto queda disponible en:

- `https://localhost:7064`
- `http://localhost:5051`

### 4. Instalar dependencias del frontend

```bash
cd zuli_frontend
npm install
```

### 5. Ejecutar frontend

```bash
npm run dev
```

La aplicación queda disponible en `http://localhost:5173`.

El frontend usa el proxy de Vite para enviar las solicitudes `/api` hacia `https://localhost:7034`, por lo que se recomienda ejecutar el backend principal con el perfil `https`.

### Compilar frontend para producción

```bash
cd zuli_frontend
npm run build
```

### Vista previa del build

```bash
cd zuli_frontend
npm run preview
```

## Contenedores y GitHub Actions

El proyecto está preparado para integrarse con GitHub Actions como parte del flujo de integración continua. Actualmente el repositorio incluye el workflow `.github/workflows/ci.yml`, el cual se ejecuta en cada pull request hacia la rama `develop`.

Este workflow realiza las siguientes tareas:

- Descarga el código del repositorio.
- Configura el SDK de .NET `10.0.x`.
- Restaura dependencias de las pruebas del backend principal.
- Restaura dependencias de las pruebas de la API externa.
- Ejecuta las pruebas automatizadas de `zuli_backend/Tests/Tests.csproj`.
- Ejecuta las pruebas automatizadas de `zuli_external_API/Tests/Tests.csproj`.

Los contenedores disponibles para el proyecto pueden ser utilizados desde GitHub Actions para estandarizar ambientes de ejecución, validación y despliegue. Esto permite que backend, API externa, frontend y servicios auxiliares se ejecuten en ambientes reproducibles, evitando diferencias entre máquinas locales y el entorno de CI/CD.

Componentes candidatos a contenedorización o ejecución mediante imágenes:

- Backend principal ASP.NET Core.
- API externa ASP.NET Core para integración con otras aerolíneas.
- Frontend Vue/Vite servido como aplicación web compilada.
- SQL Server para pruebas o ambientes de integración.

Flujo recomendado con contenedores en GitHub Actions:

- Construir las imágenes del backend, API externa y frontend.
- Ejecutar pruebas automatizadas antes de publicar imágenes.
- Publicar las imágenes aprobadas en el registro configurado para el repositorio, por ejemplo GitHub Container Registry.
- Usar las imágenes publicadas para despliegues o ambientes de prueba.

Nota: en esta copia del repositorio no se encontraron `Dockerfile`, `docker-compose.yml` ni workflows de publicación de imágenes. Estos estan en la rama de deploymente/v1

## Pruebas

Ejecutar pruebas del backend principal:

```bash
dotnet test zuli_backend/Tests/Tests.csproj
```

Ejecutar pruebas de la API externa:

```bash
dotnet test zuli_external_API/Tests/Tests.csproj
```

El repositorio incluye un flujo de CI en GitHub Actions que ejecuta estas pruebas automáticamente en pull requests hacia `develop`.

## Documentación API

En ambiente de desarrollo, las APIs exponen documentación mediante OpenAPI y Scalar.

Backend principal:

- `https://localhost:7034/openapi/v1.json`
- `https://localhost:7034/scalar/v1`

API externa:

- `https://localhost:7064/openapi/v1.json`
- `https://localhost:7064/scalar/v1`

## Uso general

Flujo público disponible desde el frontend:

- Buscar vuelos desde `/`.
- Ver resultados en `/buscar-vuelos`.
- Comprar boletos desde `/comprar-boleto`.
- Consultar reservas desde `/Mi-viaje`.
- Cancelar reservas desde `/cancelar-reserva`.
- Comprar equipaje adicional desde `/equipaje-adicional`.

Flujo administrativo:

- Iniciar sesión desde `/administrativo`.
- Acceder al panel en `/admin/`.
- Administrar usuarios, aeronaves, aeropuertos, rutas, vuelos y reportes.

## Contribuyentes

- M.Sc. Rebeca Obando Vásquez, profesora de Ingeniería de Software.
- M.Sc. Ricardo Sánchez Ramírez, profesor de Bases de Datos.

## Versión

`1.0.0.0`

## Autores

- Luis Arias Gómez | C10645
- Leonardo Calderón Rivera | C31452
- Geiner Montoya Barrientos | C25063
- Randy Rojas Pérez | C36937
- Jorge Salas Lau | C37130

## Soporte

Para dudas o consultas sobre el proyecto, contacte a los autores o abra un issue en el repositorio.

## Licencia

Este proyecto es de uso académico y no está autorizado para uso comercial o distribución sin permiso de los autores.

## Estado del proyecto

El proyecto se encuentra en fase de desarrollo. Actualmente cuenta con backend, frontend, base de datos, API externa, pruebas automatizadas y flujo de CI, pero la documentación funcional y técnica todavía está en proceso de ampliación.
