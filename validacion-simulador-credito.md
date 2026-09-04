# Validación del simulador de crédito de libranza

**Fecha:** 2026-09-04  
**Etapa:** Validación (etapa 5/5 del ciclo Software)  
**Repositorio:** `D:/Proyectos/Perxia/DEMO-simulador-credito`  
**Estado de la UI:** Sirve web Angular (`simulador-libranza-web`); se abrió con `perxia_web` y se verificaron la carga inicial y el cambio de modo.

## Escenarios validados (2 pruebas explícitas)

### Prueba 1 – Backend (.NET 10)
- **Comando:** `dotnet test CuotaClara.sln --no-restore` (en `simulador-libranza-api/`)
- **Resultado:** 7 pruebas superadas (4 de dominio, 3 de integración API)
- **Cobertura:** Cálculo francés, política de capacidad, modo Monto/Cuota, plazos únicos (60/72/96/108/120), validaciones y respuestas `application/problem+json`.
- **Evidencia:** Salida completa de `dotnet test` (0 errores). Pruebas de regresión para monto creciente en modo Cuota.

### Prueba 2 – Frontend Angular + UI en vivo
- **Comando:** `npm run lint`, `npm run typecheck` y `npm run test -- --watch=false`
- **Resultado:** Lint y typecheck pasan. `npm test` falla por ausencia de Chrome (brecha conocida). UI validada manualmente con `perxia_web` (perfil `validation-simulator`).
- **Flujo verificado:** 
  - La carga inicial muestra actividades y las cinco alternativas: 60, 72, 96, 108 y 120 meses, en $0.
  - Cambio de tab Monto ↔ Cuota actualiza el campo a "Cuota máxima" y el resultado a "Monto estimado".
- **Evidencia:** Snapshot del navegador integrado con modo Cuota activo y cinco alternativas contenidas.

## Brechas y recomendaciones

- Karma necesita Chrome (`CHROME_BIN` o instalación). Se reporta en `.perxia/validacion.md` como gap.
- Falta E2E completo de selección de actividad/convenio, llenado y clic en "Calcular alternativas"; no fue automatizado en esta sesión.
- Presupuesto CSS sigue ligeramente excedido (≈300 bytes); no bloquea build.
- No se agregaron dependencias de testing visual (Playwright, Cypress, etc.) conforme a las reglas de esta etapa.

**Conclusión:** Backend aprobado (7/7). Frontend: lint y typecheck aprobados; suite Karma pendiente por Chrome. La validación UI parcial confirma carga y cambio de modo; queda E2E completo antes de un go/no-go.

**Archivo generado:** `validacion-simulador-credito.md` (en raíz).