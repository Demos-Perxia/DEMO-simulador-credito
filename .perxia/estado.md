# Estado del proyecto

- Etapa actual: Validación ejecutada; cierre condicionado por gaps de entorno.
- Decisión vigente: Cuota Clara usa canvas blanco, jerarquías rojas y tarjetas rojas con texto blanco; la selección se marca con rojo oscuro, borde interno blanco y texto `Seleccionado`.
- HUs: `LF-HU-01` a `LF-HU-03` parcialmente validadas; especificación en `docs/specs/nuevo-look-and-feel.md`.
- Alcance: interfaz Angular de escritorio y móvil; no cambió cálculo, catálogo ni contratos de simulación.
- Verificación: lint, typecheck y build pasan; build conserva advertencia CSS de 1.98 kB. UI visible aprobada en 1440 y 390 px para identidad, dependencia, alerta, reflow y cambio de modo.
- Gaps: Karma no ejecuta sin Chrome/`CHROME_BIN`; `dotnet` no está disponible para las pruebas backend ni la simulación UI completa; pendiente selección real de alternativa, 320 px y navegación de teclado.
- Evidencia: `VALIDACION-HU-01.md` y `.perxia/screenshots/validacion-*.jpg`.
