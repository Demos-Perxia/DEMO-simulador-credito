# Estado del proyecto

- Etapa actual: validación en curso; backend aprobado en .NET 10.
- Decisión vigente: frontend Angular en `simulador-libranza-web/` y backend ASP.NET Core Web API por capas en `simulador-libranza-api/`, ambos compatibles con runtime .NET 10 para la API.
- Implementado: catálogos mock, simulación por monto/cuota, política 18 % EA, amortización francesa decimal, Problem Details, CORS, OpenAPI y formulario Angular alineado con el artboard de Simulador, con formato de miles en montos y alternativas iniciales en cero. En modo Cuota se muestra el monto financiable por plazo, que aumenta al aumentar los meses; modo Monto conserva las cuotas por plazo.
- Verificado: `dotnet restore`, `dotnet build` y `dotnet test` (7 pruebas) pasan en .NET 10; `npm run lint`, `npm run typecheck` y `npm run build` pasan. La simulación retorna 5 plazos únicos: 60, 72, 96, 108 y 120 meses. UI en vivo confirma carga de actividades/alternativas y cambio de modo Cuota (etiquetas correctas).
- Documentado: README con preparación, arranque de API en `127.0.0.1:7040`, frontend en `127.0.0.1:4200`, proxy y verificación. Pendiente: Chrome/CHROME_BIN para Karma y recorrido E2E visual contra API local.
