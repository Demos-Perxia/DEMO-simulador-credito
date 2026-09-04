# Validación

## Ejecutado

- 2026-09-04: el repositorio se confirmó como SPA Angular con API ASP.NET Core; el grafo conecta `App`, `CreditApiService`, `ApiEndpoints`, `SimulationEngine` y `SimulationAlternative`.
- `npm run lint` y `npm run typecheck` aprobaron. `npm run build` aprobó con advertencia de `app.css` (7.98 kB, 1.98 kB sobre el aviso de 6 kB; bajo máximo de error de 8 kB).
- `npm run test -- --watch=false` compiló la suite, pero Karma no abrió Chrome porque falta el binario configurado en `CHROME_BIN`. No se instaló navegador ni driver.
- `dotnet` no está disponible: no se ejecutaron las 4 pruebas de dominio ni las 3 de integración API.
- Validación visible con `perxia_web`: a 1440 px se observó identidad rojo/blanco, convenio deshabilitado y cinco tarjetas iniciales deshabilitadas; a 390 px se verificó alerta de formulario inválido, reflow de columna única y selector nativo Cuota con etiquetas actualizadas.
- Informe con 10 pruebas: `VALIDACION-HU-01.md`.

## Hallazgos abiertos

- Ejecutar Karma con un Chrome existente configurado en `CHROME_BIN`.
- Proveer .NET SDK para ejecutar pruebas de dominio e integración API.
- Con API local activa, validar simulación exitosa y selección de 72 meses.
- Verificar 320 px exactos y navegación real de teclado.
- Reducir `app.css` bajo la advertencia de presupuesto de 6 kB.
