# Validación

## Ejecutado

- `dotnet restore CuotaClara.sln`: correcto con runtime .NET 10.
- `dotnet build CuotaClara.sln --no-restore`: correcto, 0 advertencias y 0 errores.
- `dotnet test CuotaClara.sln --no-restore`: correcto; 7 pruebas superadas (4 de dominio y 3 de integración API).
- `npm run lint`, `npm run typecheck` y `npm run build` en `simulador-libranza-web/`: correctos.
- Servicios locales: API responde en `http://127.0.0.1:7040`; frontend responde en `http://127.0.0.1:4200`.
- Verificación navegador: el frontend carga actividades desde la API correctamente a través del proxy de desarrollo `/api`.
- Corrección de plazos: la API ya devuelve exactamente 5 alternativas únicas (60, 72, 96, 108 y 120 meses); cubierto por prueba de integración.
- `dotnet test CuotaClara.sln --no-restore`: correcto; 7 pruebas superadas, incluida la regresión de capacidad por cuota que asegura un monto estrictamente creciente por plazo.
- `npm run lint`, `npm run typecheck` y `npm run build`: correctos tras compactar las alternativas en ambos modos; persiste la advertencia de presupuesto CSS (306 bytes).
- Verificación visual en navegador: el modo Cuota conserva la rejilla y compacta sus valores para mantenerlos contenidos en la tarjeta.
- 2026-09-04: `dotnet test CuotaClara.sln --no-restore` aprobado (7/7); `npm run lint` y `npm run typecheck` aprobados. `npm run test -- --watch=false` no inicia por falta de Chrome.
- 2026-09-04: UI abierta con `perxia_web`: carga actividades y las cinco alternativas iniciales; Monto → Cuota actualiza etiquetas a "Cuota máxima" y "Monto estimado".

## Hallazgos abiertos

- `npm test -- --watch=false --browsers=ChromeHeadless` no inicia porque no existe un binario Chrome; instalar Chrome o configurar `CHROME_BIN` permitirá ejecutar la suite Angular.
- Falta recorrido E2E completo de selección de convenio y simulación en el navegador.
