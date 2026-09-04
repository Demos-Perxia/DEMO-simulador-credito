# Implementación — Nuevo look and feel rojo

## Alcance implementado

- Aplicado el sistema visual Cuota Clara a la SPA Angular: canvas blanco, jerarquías rojas y superficies destacadas rojas con contenido blanco.
- Reemplazado el selector de modo ARIA emulado por radios nativos ligados a `formControlName="mode"`.
- Reestructuradas las alternativas como `ul/li/button`; conservan `disabled`, `aria-pressed` y agregan una señal visible de selección.
- Ajustados foco, estados deshabilitados, alerta accesible, valores COP extensos y reflow móvil.
- No se cambiaron `app.ts`, API, DTOs, contratos ni reglas de cálculo.

## Archivos modificados

- `simulador-libranza-web/src/styles.css`
- `simulador-libranza-web/src/app/app.html`
- `simulador-libranza-web/src/app/app.css`
- `simulador-libranza-web/src/app/app.spec.ts`

## Verificación

- `npm run lint`: correcto.
- `npm run typecheck`: correcto.
- `npm run build`: correcto; conserva advertencia CSS de 1.98 kB sobre el umbral de aviso de 6 kB, sin superar el máximo de error de 8 kB.
- `npm run test -- --watch=false`: compiló las pruebas, pero no ejecutó por ausencia de Google Chrome configurado en `CHROME_BIN`.
- Validación manual en `http://localhost:4200`: confirmados canvas blanco, superficies rojas con texto blanco, alerta roja, convenio deshabilitado y reflow a 390 px. Capturas: `.perxia/screenshots/look-rojo-desktop-inicial-2026-09-04T20-02-58-248.jpg`, `.perxia/screenshots/look-rojo-mobile-inicial-2026-09-04T20-03-09-586.jpg` y `.perxia/screenshots/look-rojo-mobile-alerta-2026-09-04T20-03-31-102.jpg`.

## Pendiente

- Ejecutar pruebas Karma en un entorno con Chrome disponible; no fue posible validar resultado y selección de alternativas contra la API porque el SDK `dotnet` no está instalado en este entorno.
