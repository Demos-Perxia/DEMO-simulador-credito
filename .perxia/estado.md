# Estado del proyecto

- Etapa actual: Implementación completada para HU-01; pendiente la validación integral de la etapa final.
- Decisión vigente: frontend Angular en `simulador-libranza-web/` y backend ASP.NET Core Web API por capas en `simulador-libranza-api/`; el look and feel se limita a la SPA para conservar contratos y cálculo.
- Sistema visual: Cuota Clara usa Manrope + DM Mono, fondo negro `#080808`, jerarquía roja `#FF334D`, verde `#62F58C`, borde `#26362C` y texto negro en cards verdes.
- Implementado: tokens globales, layout responsive, formulario y resultado oscuros, estados de carga/error, ayuda textual de convenio dependiente, `aria-live`, foco visible y selección de alternativas con `aria-pressed`.
- Verificado en implementación: `npm run lint`, `npm run typecheck` y `npm run build` pasan; el build conserva una advertencia de presupuesto CSS (7.16 kB frente a aviso de 6 kB, dentro del máximo de 8 kB).
- Verificación visual de implementación: inspección manual en 1440px y 390px confirma estructura responsive, estados inicial/error, ayuda de convenio y controles accesibles; evidencia en `.perxia/screenshots/`.
- Bloqueos de validación: `npm run test -- --watch=false` no puede iniciar sin Chrome/`CHROME_BIN`; el entorno actual no dispone de `dotnet`, por lo que no se repitió la regresión backend ni se levantó la API para E2E.
- Índice: `graphify-out/graph.json` presente. Arquitectura: `arquitectura-contexto-c4-v1.drawio` y `arquitectura-contenedores-c4-v1.drawio`.
- Siguiente etapa: Validación de HU-01, con pruebas Angular en Chrome, regresión .NET y recorridos visuales 1440px/390px/320px.
