# Onboarding — Demo simulador de crédito de libranza

## Objetivo

Construir un demo web que reproduzca la experiencia funcional de un simulador de crédito de libranza, usando Angular en el frontend y .NET en el backend. El simulador debe permitir ingresar información básica del solicitante, seleccionar condiciones del crédito y visualizar una propuesta informativa de cuota, tasa y plazos.

El producto será una demostración técnica y funcional. No representa una oferta financiera real, no consulta centrales de riesgo y no ejecuta una aprobación crediticia real.

## Referente funcional y visual

La estructura funcional, las opciones principales y la composición general deben tomar como referencia el simulador público de crédito de libranza revisado durante el onboarding:

- Encabezado y navegación institucional.
- Sección introductoria del producto.
- Formulario progresivo de simulación.
- Selección de actividad del solicitante.
- Selección de convenio condicionada por la actividad.
- Ingresos mensuales, descuentos de nómina y monto solicitado.
- Tasa efectiva anual de referencia, plazo y cuota mensual estimada.
- Detalle resumido y alternativas de plazo.
- Acción de continuación, avisos legales y aclaraciones informativas.

La estética debe tomar la jerarquía funcional como referencia, pero usar una identidad propia: no logos, imágenes, nombre comercial ni activos del banco de referencia.

## Stack y estructura verificados

- `simulador-libranza-web/`: Angular 20, TypeScript estricto, Reactive Forms, `HttpClient`, Karma/Jasmine.
- `simulador-libranza-api/`: ASP.NET Core Web API por capas `Domain`, `Application`, `Infrastructure` y `Api`; pruebas unitarias e integración.
- API versionada: `GET /api/v1/catalogs/activities`, `GET /api/v1/catalogs/activities/{activityId}/agreements` y `POST /api/v1/credit-simulations`.
- El frontend usa `/api/v1` y un proxy de desarrollo hacia la API local.
- El dominio es la autoridad de reglas y validaciones; no hay persistencia ni servicios externos.

## Flujo funcional

1. El usuario selecciona actividad y convenio dependiente.
2. Ingresa ingresos, descuentos y monto o capacidad de cuota.
3. El frontend valida formato, rangos y obligatoriedad.
4. La API vuelve a validar y calcula alternativas para 60, 72, 96, 108 y 120 meses.
5. La SPA presenta tasa, cuota, plazo, detalle y alternativas como estimación informativa.

## Reglas de dominio vigentes

- Moneda COP, tasa mock de 18 % EA y cuota fija (sistema francés).
- Monto entre 1.000.000 y 100.000.000 COP; capacidad máxima de 40 % del ingreso disponible.
- No se incluyen seguro de vida, aprobación real, centrales de riesgo, autenticación, desembolso ni persistencia de solicitudes.
- Errores controlados usan `application/problem+json`.

## Convenciones y calidad

- Angular usa TypeScript estricto y scripts `lint`, `typecheck`, `test` y `build`.
- Backend usa `dotnet build CuotaClara.sln --no-restore` y `dotnet test CuotaClara.sln --no-restore`.
- Mantener DTOs versionados, validación server-side, configuración por ambiente y secretos fuera del repositorio.
- El sistema visual vigente es Cuota Clara: Manrope + DM Mono, negro `#080808`, rojo `#FF334D`, verde `#62F58C` y borde `#26362C`.

## Índice de conocimiento

- Graphify reindexado el 2026-09-04 sobre 208 archivos (~119.510 palabras): 401 nodos, 469 aristas y 77 comunidades.
- Comunidades principales: backend/API de simulación, lógica Angular, reglas de dominio, contratos/mapeos API, abstracciones de catálogos, pruebas de integración, diseño y UX.
- Nodos de mayor centralidad: `App`, `SimulationEngine`, `ICatalogRepository`, `InMemoryCatalogRepository`, `CreditPolicyOptions` y `SimulationInput`.
- Ambigüedad técnica: el índice incorporó artefactos generados `obj/` y 184 aristas AST con destinos externos/no resueltos; úselos como contexto, no como evidencia de dependencia interna. El reindexado no modificó código.
- Salidas locales: `graphify-out/graph.json`, `graphify-out/GRAPH_REPORT.md` y `graphify-out/graph.html`; permanecen ignoradas por Git.
