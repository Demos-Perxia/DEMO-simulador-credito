# Arquitectura — Simulador Cuota Clara

## Decisión

Se conserva el monolito modular: SPA Angular 20 y API ASP.NET Core/.NET 10 por capas Domain, Application, Infrastructure y API. La HU-01 se implementa exclusivamente en la SPA para preservar reglas, contratos y cálculos existentes.

## Artefactos

- Contexto C4: `arquitectura-contexto-c4-v1.drawio`.
- Contenedores C4: `arquitectura-contenedores-c4-v1.drawio`.
- Documento y ADRs: `arquitectura-simulador-credito-v1.md`.

## Contratos vigentes

- `GET /api/v1/catalogs/activities`.
- `GET /api/v1/catalogs/activities/{activityId}/agreements`.
- `POST /api/v1/credit-simulations`.
- Errores como `application/problem+json`; validaciones de dominio y aplicación continúan server-side.

## Riesgos y siguientes pasos

Validar regresión visual y funcional en desktop/móvil, teclado y estados de error; no introducir cambios de backend para el look and feel. Si aparecen catálogos reales, sustituir el adaptador en Infrastructure detrás de los puertos actuales.
