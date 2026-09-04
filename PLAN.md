# Plan: Nuevo look and feel — Cuota Clara

## 1. Objetivos

Implementar la HU-01 en la SPA Angular para que toda la experiencia del simulador adopte la identidad Cuota Clara, sin alterar el comportamiento actual.

Resultados observables:

- Fondo `#080808`, jerarquía roja `#FF334D`, acciones y acentos verdes `#62F58C`, bordes `#26362C`, Manrope para interfaz y DM Mono para metadatos.
- Encabezado, introducción, formulario progresivo, resultados, alternativas, avisos y estados inicial, carga, error y resultado visualmente consistentes.
- Las cards verdes mostrarán texto negro intenso, nunca rojo.
- La página será operable con teclado y adaptable a `1440px`, `390px` y `320px`, sin desbordamiento horizontal involuntario.
- Las peticiones, validaciones, cálculos, contratos HTTP y datos entregados por la API se conservarán intactos.

Fuera de alcance: cambios en backend, endpoints, DTOs, contratos, reglas, cálculos, tasas, rangos, persistencia, autenticación, copy legal, nuevas pantallas, nueva funcionalidad, activos o marcas de terceros.

## 2. Alcance por historia de usuario

### HU-01: Visualizar la página con el nuevo look and feel de Cuota Clara

Aplicar el Design System vigente a la única pantalla del simulador, cubriendo sus estados visuales y accesibilidad, mientras se preserva el flujo actual de actividad, convenio dependiente, ingresos, descuentos, monto/cuota, simulación y alternativas.

Dependencias:

- Fuente de alcance: `docs/specs/nuevo-look-and-feel.md`.
- Fuente visual: `.perxia/design-system.md` y los artboards de `design/sistema-simulador-credito/`.
- Restricción arquitectónica: `.perxia/arquitectura.md` y ADR-002 en `arquitectura-simulador-credito-v1.md`; la implementación queda aislada en Angular.
- Flujo y contratos existentes: componente `App` y `CreditApiService`.

## 3. Decisiones de arquitectura

| Decisión | Alternativa descartada | Razón |
|---|---|---|
| Implementar el rediseño solo en la SPA Angular. | Modificar API, DTOs o cálculo para transportar metadatos de presentación. | La HU es visual; modificar backend acoplaría presentación con reglas y aumentaría el riesgo de regresión. |
| Mantener el componente standalone `App` y sus bindings actuales. | Dividir el rediseño en nuevos componentes. | Es una sola pantalla y un cambio visual; conservar el contenedor reduce la superficie de cambio y protege el flujo reactivo existente. |
| Declarar tokens y base tipográfica en `styles.css`; mantener composición y variantes en `app.css`. | Estilos inline o una librería UI adicional. | Centraliza tokens, evita dependencias y ayuda a controlar el presupuesto de estilos de componente. |
| Servir Manrope y DM Mono mediante Google Fonts con fallbacks del sistema. | Autoalojar fuentes. | Decisión de producto para conservar el alcance actual sin añadir binarios ni gestión de licencias/activos. |
| Mantener el único estado y acción de reintento actuales. | Separar origen de error de catálogo y simulación en `app.ts`. | Se preserva estrictamente la lógica actual; la diferencia se comunicará con texto y presentación sin alterar el modelo de estado. |
| Usar HTML nativo, ARIA mínimo y CSS `:focus-visible`. | Widgets personalizados o navegación gestionada por JavaScript. | Menor complejidad y mejor semántica para formulario, tabs, mensajes y controles deshabilitados. |

No se modificarán `simulador-libranza-web/src/app/core/credit-api.service.ts`, `simulador-libranza-web/src/app/app.config.ts`, archivos de ambiente ni ningún archivo bajo `simulador-libranza-api/`.

## 4. Tareas

| # | Tarea | Depende de | HU | Archivos |
|---|---|---|---|---|
| 1 | Definir tokens globales de color, tipografía, espaciado, radios, fondo y foco visible. Reemplazar la base clara/violeta/rosa actual por la identidad Cuota Clara y conservar fallbacks tipográficos. | — | HU-01 | `simulador-libranza-web/src/styles.css` |
| 2 | Reordenar el template en bloques semánticos de encabezado, hero, formulario, resultado, alternativas y aviso; mantener sin cambios `formControlName`, interpolaciones, eventos, `@if`, `@for`, `ngSubmit` y métodos existentes. | 1 | HU-01 | `simulador-libranza-web/src/app/app.html` |
| 3 | Implementar las superficies, controles, tabs, botones, card de resultado y alternativas aplicando los tokens. La card verde debe usar texto negro; la selección debe tener señal adicional al color. | 1, 2 | HU-01 | `simulador-libranza-web/src/app/app.css` |
| 4 | Presentar los estados existentes: inicial con alternativas en cero, convenio dependiente deshabilitado, carga de convenios, carga de simulación, alerta de validación/error con el reintento actual y resultado con alternativa seleccionada. No modificar el modelo de estado ni `app.ts`. | 2, 3 | HU-01 | `simulador-libranza-web/src/app/app.html`, `simulador-libranza-web/src/app/app.css` |
| 5 | Incorporar accesibilidad de template y estilos: etiquetas persistentes, ayuda textual al convenio deshabilitado, atributos ARIA pertinentes, regiones `aria-live`, foco visible y orden lógico de navegación. | 2, 3 | HU-01 | `simulador-libranza-web/src/app/app.html`, `simulador-libranza-web/src/app/app.css` |
| 6 | Añadir responsive mobile-first: dos columnas cuando exista espacio, una columna en tablet/móvil y ajuste de cifras COP, selects, botones, alternativas y aviso sin cortes ni scroll horizontal. | 3, 4, 5 | HU-01 | `simulador-libranza-web/src/app/app.css`, ajuste puntual en `simulador-libranza-web/src/app/app.html` |
| 7 | Ajustar y ampliar pruebas de componente para proteger contratos y transiciones: convenio deshabilitado/habilitado, cargas, error, simulación, alternativas, selección y atributos semánticos críticos. Corregir la expectativa de `planValueLabel()` para que coincida con el comportamiento actual (`Monto estimado` en modo cuota), sin modificar el cálculo. | 2, 4, 5 | HU-01 | `simulador-libranza-web/src/app/app.spec.ts` |
| 8 | Ejecutar gates, regresión backend y recorrido manual integrado. Capturar evidencia en desktop, móvil y móvil estrecho para estados inicial, carga, error y resultado; comprobar teclado y ausencia de scroll horizontal. | 1–7 | HU-01 | Sin cambios de código |

Paralelización: después de la tarea 1 pueden avanzar en paralelo la estructura de la tarea 2 y el diseño de estados de la tarea 4. Tras estabilizar el template, las tareas 3 y 5 pueden implementarse en paralelo. Las tareas 6 y 7 requieren la semántica y los estados finales.

## 5. Archivos a cambiar

| Ruta | Operación | Cambio |
|---|---|---|
| `simulador-libranza-web/src/styles.css` | Modificar | Importación Google Fonts, tokens CSS, base oscura, tipografía, resets y foco global. |
| `simulador-libranza-web/src/app/app.html` | Modificar | Jerarquía semántica y presentación de encabezado, formulario, resultados, estados y avisos, conservando bindings y flujo. |
| `simulador-libranza-web/src/app/app.css` | Modificar | Layout, componentes visuales, estados, accesibilidad visual y breakpoints responsive. |
| `simulador-libranza-web/src/app/app.spec.ts` | Modificar | Regresión del flujo existente y verificaciones de estado/semántica que protegen la HU. |
| `simulador-libranza-web/src/app/app.ts` | Sin cambios previstos | La lógica de formulario, carga, errores, payload y resultados se conserva. Solo se reconsideraría si una deficiencia de accesibilidad no puede resolverse desde template/CSS. |
| `simulador-libranza-web/src/app/core/credit-api.service.ts` | Sin cambios | Contratos HTTP intactos. |
| `simulador-libranza-api/**` | Sin cambios | Backend y cálculo fuera de alcance. |

## 6. Riesgos

| Riesgo | Impacto | Señal temprana | Mitigación |
|---|---|---|---|
| Reestructurar HTML rompe bindings o eventos del formulario. | Alto: fallo de simulación o validaciones. | Requests ausentes, campos no actualizan o resultado no se invalida. | Mantener atributos y handlers existentes; ampliar pruebas de componente antes de cerrar la HU. |
| CSS del componente supera el presupuesto de 8 kB. | Alto: build de producción falla. | Advertencia o error `anyComponentStyle` en build. | Usar tokens globales, selectores reutilizables y eliminar reglas heredadas; medir con `npm run build`. |
| Cards verdes con contraste insuficiente. | Alto: incumple RN-02 y accesibilidad. | Texto rojo/claro o ilegible sobre `#62F58C`. | Fijar texto negro en estas variantes y revisarlo en estado inicial y resultado. |
| Layout móvil corta cifras, solapa controles o genera scroll horizontal. | Alto: incumple CA-02 y CA-09. | Overflow en `390px` o `320px`. | Diseñar mobile-first, permitir wrap y validar los tres viewports definidos. |
| Estados dependen solamente del color o carecen de foco visible. | Alto: incumple RN-05 y CA-10. | Teclado sin indicador, disabled ambiguo o alertas no anunciadas. | Añadir texto, borde/ícono, `:focus-visible`, labels y ARIA mínima; hacer recorrido por teclado. |
| Un único `error()` muestra reintento de actividades tras un fallo de simulación. | Medio: recuperación imprecisa heredada. | Mensaje de simulación acompañado de la acción actual de recarga. | Mantener la lógica por decisión de alcance y no representar una acción inexistente; documentar la limitación para evolución posterior. |
| Google Fonts no carga. | Medio: variación tipográfica. | Red lenta, bloqueo de red o fuentes no disponibles. | Declarar fallbacks del sistema y validar que el layout no dependa de una métrica exacta de fuente. |
| Karma no ejecuta por falta de Chrome/`CHROME_BIN`. | Medio: cobertura automatizada incompleta. | `npm run test -- --watch=false` no inicia. | Ejecutar gates estáticos/build; habilitar Chrome o `CHROME_BIN` antes de certificación y complementar con evidencia manual. |

## 7. Criterios de aceptación

| ID | Criterio verificable | Evidencia / verificación |
|---|---|---|
| CA-01 | En `1440px`, encabezado, introducción, formulario, tarjetas, resultado y avisos aplican el Design System sin overflow. | Captura desktop y recorrido contra API local. |
| CA-02 | En `390px`, todos los controles son operables y la página completa no tiene scroll horizontal ni solapamientos. | Captura y recorrido móvil. |
| CA-03 | Toda card verde muestra título, valor y metadato en negro intenso y no en rojo. | Revisión de card en estado inicial y con resultado. |
| CA-04 | Se distinguen visualmente los estados inicial, carga de convenio, carga de simulación, error y resultado, manteniendo contexto y la acción existente. | Pruebas de componente y evidencia manual por estado. |
| CA-05 | El mismo formulario válido conserva payload, endpoint, validaciones y respuesta funcional de la simulación. | `app.spec.ts`; comprobar `POST /api/v1/credit-simulations`; regresión integrada. |
| CA-06 | Antes de elegir actividad, el convenio está deshabilitado y comunica su dependencia de modo visible y textual. | Prueba de componente y revisión de UI. |
| CA-07 | Datos inválidos o incompletos muestran validación existente y no producen un resultado válido. | Prueba de componente y caso manual inválido. |
| CA-08 | Errores de catálogos o simulación conservan el formulario, no muestran datos ficticios y presentan el estado de error con la recuperación actual. | Pruebas HTTP y evidencia manual. |
| CA-09 | En `320px`, cards, selectores, botones y cifras largas se ajustan sin corte, superposición ni área inaccesible. | Captura de viewport estrecho. |
| CA-10 | Con teclado, links, tabs, selects, inputs, botón y alternativas siguen un orden lógico, tienen foco visible y se pueden operar sin mouse. | Recorrido manual de tabulación y revisión de `:focus-visible`. |
| Gate frontend | El frontend compila y pasa análisis estático. | Desde `simulador-libranza-web/`: `npm run lint`, `npm run typecheck`, `npm run build`. |
| Tests frontend | Las pruebas Angular pasan sin watch cuando Chrome/`CHROME_BIN` esté disponible. | Desde `simulador-libranza-web/`: `npm run test -- --watch=false`. |
| Regresión backend | No hay regresión en el motor ni contratos de API. | Desde `simulador-libranza-api/`: `dotnet restore CuotaClara.sln && dotnet build CuotaClara.sln --no-restore && dotnet test CuotaClara.sln --no-restore`. |
