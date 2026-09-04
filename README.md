# Demo · Simulador de Crédito de Libranza

Demo de **Cuota Clara** para simular un crédito de libranza con datos sintéticos. Ofrece los modos Monto y Cuota, calcula alternativas por plazo mediante una API ASP.NET Core y no guarda solicitudes ni datos personales.

## Estructura

- `simulador-libranza-web/`: Angular 20, TypeScript estricto, Reactive Forms y `HttpClient`.
- `simulador-libranza-api/`: ASP.NET Core Web API por capas (Domain, Application, Infrastructure y API).
- `design/`: referencia visual histórica; no participa en la aplicación productiva.

## Prerrequisitos

- Node.js 20 o superior y npm.
- .NET SDK 10.0 o superior para API y sus pruebas.

## Ejecutar localmente

### 1. Restaurar dependencias

Desde la raíz del repositorio, ejecuta una vez:

```powershell
cd simulador-libranza-api
dotnet restore CuotaClara.sln

cd ../simulador-libranza-web
npm ci
```

### 2. Iniciar la API

Abre una terminal en la raíz y ejecuta:

```powershell
cd simulador-libranza-api
dotnet run --project src/CuotaClara.Api --urls http://127.0.0.1:7040
```

La API queda disponible en `http://127.0.0.1:7040`. En desarrollo, OpenAPI/Swagger está disponible en `/swagger`.

### 3. Iniciar el frontend

Mantén la API activa, abre una segunda terminal desde la raíz y ejecuta:

```powershell
cd simulador-libranza-web
npm start -- --host 127.0.0.1
```

Abre `http://127.0.0.1:4200` en el navegador.

El frontend usa `apiBaseUrl: /api/v1` y el proxy de desarrollo reenvía `/api` a `http://127.0.0.1:7040`; por eso ambos servicios deben permanecer activos. Para detenerlos, usa `Ctrl+C` en cada terminal.

### 4. Verificar disponibilidad

```powershell
Invoke-WebRequest http://127.0.0.1:7040/api/v1/catalogs/activities
Invoke-WebRequest http://127.0.0.1:4200
```

La primera llamada debe devolver las actividades `Docente` y `Pensionado`; la segunda debe responder HTTP 200.

## Calidad

```powershell
cd simulador-libranza-web
npm run lint
npm run typecheck
npm run test -- --watch=false
npm run build

cd ../simulador-libranza-api
dotnet build CuotaClara.sln --no-restore
dotnet test CuotaClara.sln --no-restore
```

> `npm run test -- --watch=false` necesita Google Chrome o una variable `CHROME_BIN` configurada. El lint, typecheck, build y las pruebas .NET no dependen de Chrome.

## Alcance y limitaciones

- Tasa referencial mock: 18 % EA; plazos: 60, 72, 96, 108 y 120 meses.
- Monto permitido: 1.000.000 a 100.000.000 COP; capacidad máxima: 40 % del ingreso disponible.
- El resultado es informativo, no es una aprobación y no incluye seguro de vida.
- No hay autenticación, persistencia, centrales de riesgo, core bancario, desembolso ni seguros reales.
