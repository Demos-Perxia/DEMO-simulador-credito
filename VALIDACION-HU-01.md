# Validación — Nuevo look and feel rojo del simulador de libranza

Fecha: 2026-09-04

## Conclusión de superficie

El repositorio sirve una interfaz web: `simulador-libranza-web/` es una SPA Angular y `simulador-libranza-api/` es una API ASP.NET Core. El grafo relaciona `App`, `CreditApiService`, `ApiEndpoints`, `SimulationEngine` y `SimulationAlternative`. Se validó la UI en vivo con `perxia_web` y se ejecutaron los runners disponibles.

## Inventario de 10 pruebas

| # | Capa | Escenario | Evidencia | Resultado |
|---|---|---|---|---|
| 1 | Estático | Análisis Angular | `npm run lint` | Aprobada |
| 2 | Estático | Tipado Angular | `npm run typecheck` | Aprobada |
| 3 | Build | Compilación de producción | `npm run build` | Aprobada con advertencia: `app.css` 7.98 kB, 1.98 kB sobre aviso de 6 kB y bajo máximo de 8 kB |
| 4 | Unitario UI | Suite de componente: catálogo, COP, modo, POST, estados y alertas | `npm run test -- --watch=false` | Bloqueada: compila, pero Karma no encuentra Chrome ni `CHROME_BIN` |
| 5 | Unitario backend | Cálculo francés y modos monto/cuota | `tests/CuotaClara.Domain.Tests/Program.cs` | Bloqueada: el comando `dotnet` no está disponible |
| 6 | Integración API | Catálogos, Problem Details y cinco alternativas | `tests/CuotaClara.Api.IntegrationTests/Program.cs` | Bloqueada: el comando `dotnet` no está disponible |
| 7 | E2E UI | Identidad Cuota Clara en escritorio 1440 px | `perxia_web`, snapshot a 1440 px | Aprobada: canvas claro, jerarquías rojas y tarjetas rojas con texto blanco visibles |
| 8 | E2E UI | Dependencias y alternativas antes del cálculo | `perxia_web`, snapshot a 1440 px | Aprobada: convenio deshabilitado con ayuda; cinco alternativas con atributo `disabled` |
| 9 | E2E UI | Error de formulario y reflow móvil 390 px | `perxia_web`, envío inválido y snapshot móvil | Aprobada: alerta visible con icono y acción; controles a 320 px internos, una columna y sin desborde observable |
| 10 | E2E UI | Cambio de modo nativo Monto → Cuota | `perxia_web`, click en radio `INSTALLMENT_CAPACITY` | Aprobada: las etiquetas cambian a `Cuota máxima` y `Monto estimado`; los plazos conservan estado deshabilitado |

## Trazabilidad de historias

| HU | Estado de validación |
|---|---|
| LF-HU-01 | Parcialmente validada: identidad visual y alerta inválida verificadas en UI; pruebas unitarias pendientes. |
| LF-HU-02 | Parcialmente validada: tarjetas rojas deshabilitadas verificadas; resultado exitoso y selección de 72 meses pendientes de API local. |
| LF-HU-03 | Parcialmente validada: reflow a 390 px, radios nativos y dependencia de convenio verificados; 320 px exactos y teclado real pendientes. |

## Evidencia

- UI viva: `http://localhost:4200` mediante `perxia_web`.
- Escritorio: `.perxia/screenshots/validacion-inicial-desktop-2026-09-04T20-06-31-608.jpg`.
- Error móvil: `.perxia/screenshots/validacion-error-mobile-2026-09-04T20-06-38-640.jpg`.
- Cambio de modo móvil: `.perxia/screenshots/validacion-mode-mobile-2026-09-04T20-06-49-156.jpg`.
- Cobertura declarada: `simulador-libranza-web/src/app/app.spec.ts` (10 casos), 4 pruebas de dominio y 3 de integración API.

## Gaps y acción requerida

1. Configurar un navegador existente mediante `CHROME_BIN` y repetir `npm run test -- --watch=false`; no se instaló navegador ni driver.
2. Proveer .NET SDK y ejecutar `dotnet restore CuotaClara.sln && dotnet test CuotaClara.sln --no-restore` desde `simulador-libranza-api/`.
3. Con API disponible, validar el flujo UI de actividad, convenio, simulación exitosa y selección de 72 meses; hoy la UI muestra el error de carga de actividades por falta de API.
4. Verificar viewport exacto de 320 px y navegación real con Tab, Enter y Espacio.
5. Reducir `src/app/app.css` por debajo del presupuesto de advertencia de 6 kB.
