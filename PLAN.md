# Plan: Nuevo look and feel rojo del simulador de libranza

## 1. Objetivos

Implementar el sistema visual **Cuota Clara** exclusivamente en la SPA Angular: fondo blanco, jerarquías rojas, superficies destacadas rojas con texto blanco y una alternativa seleccionada en rojo más oscuro. La experiencia debe conservarse operable en escritorio, móvil de 320 px y teclado, sin alterar cálculos, catálogos, endpoints, DTOs ni reglas del backend.

Fuera de alcance:

- Cambios en `simulador-libranza-api/**`, políticas de crédito, catálogos o contratos HTTP.
- Incorporar una librería UI, activos o logos bancarios, fuentes nuevas, autenticación o persistencia.
- Corregir comportamientos funcionales no solicitados, como la acción de reintento existente para errores de simulación.

## 2. Alcance por historia de usuario

### LF-HU-01 — Reconocer la identidad visual del simulador

Aplicar tokens de marca claros/rojos al encabezado, hero, formulario, controles, avisos, resumen y resultado. Las superficies rojas deben presentar texto e iconos blancos, permitir envoltura de mensajes largos y mantener etiquetas, valores y acciones legibles.

Depende de centralizar la paleta y foco en `src/styles.css`, y de aplicar las variantes en el template y CSS del componente `App`.

### LF-HU-02 — Comparar y seleccionar alternativas de plazo

Mostrar los plazos disponibles con tarjetas rojas, texto blanco y valores que se adapten a cifras COP extensas. Con resultado, la alternativa activa debe usar rojo más oscuro y una señal adicional al color; antes del cálculo, las alternativas se conservan visibles, atenuadas, legibles y realmente deshabilitadas.

Depende del estado actual de `result`, `selectedTerm`, `displayedAlternatives` y `selectedAlternative` de `App`; no requiere modificar su lógica ni `SimulationResponse`/`SimulationAlternative`.

### LF-HU-03 — Operar el look and feel en móvil, escritorio y teclado

Asegurar reflow sin scroll horizontal a 320 px, foco visible y contrastado para controles nativos, estados dependientes o de carga legibles, y selector de modo con semántica de selección exclusiva y señal adicional al color.

Depende de sustituir roles ARIA incompletos por semántica HTML nativa en el template y de revisar reglas responsivas del CSS del componente.

## 3. Decisiones de arquitectura

1. **Implementación aislada en la SPA Angular.** Se respeta ADR-002 y la arquitectura C4 vigente: el cambio toca tokens, template, estilos y pruebas de `App`; API, dominio, aplicación e infraestructura quedan sin cambios. Se descarta transportar metadatos visuales en el contrato porque acoplaría presentación y backend sin aportar valor.
2. **Tokens semánticos globales y composición local.** `src/styles.css` definirá canvas, tinta, rojo principal, rojo seleccionado, contenido sobre rojo, borde, disabled y foco; `app.css` consumirá esos tokens para las variantes. Se descartan colores repetidos o estilos inline porque dificultan mantener contraste y marca.
3. **HTML nativo antes que ARIA emulada.** El selector de monto/cuota se convertirá a elección exclusiva nativa (radios asociados al control reactivo `mode`) para soportar teclado y lector de pantalla sin código de eventos. Las alternativas seguirán como botones nativos dentro de una lista `ul/li`, usando `aria-pressed` y una señal visual adicional; se descartan roles `tab` sin paneles y `button role=listitem`.
4. **Selección y foco no dependientes solo del color.** La alternativa elegida combinará rojo oscuro, borde/inset blanco y check/texto de selección. El foco será blanco sobre superficies rojas y se complementará con contraste rojo oscuro sobre superficies blancas para que no desaparezca visualmente.
5. **No cambiar estado ni contratos funcionales.** Se preservan formularios reactivos, signals, carga de catálogos, `POST /credit-simulations`, invalidación de resultados y valores calculados. `app.ts` no requiere cambios previstos.

## 4. Tareas

| # | Tarea | Depende de | HU | Archivos |
|---|---|---|---|---|
| 1 | Sustituir el esquema global oscuro/verde por tokens semánticos blancos y rojos; mantener tipografías, caja universal y base de 320 px; definir foco contrastado para canvas claro y superficies rojas sin ocultar overflow real. | — | LF-HU-01, LF-HU-03 | `simulador-libranza-web/src/styles.css` |
| 2 | Reestructurar el selector de modo como control nativo de elección exclusiva ligado a `formControlName="mode"`; conservar textos, iconos, valores y el flujo del formulario; añadir estado seleccionado que no dependa solo del color. | 1 | LF-HU-01, LF-HU-03 | `simulador-libranza-web/src/app/app.html` |
| 3 | Corregir la semántica de alternativas a `ul/li` con botones nativos; preservar los bindings de alternativa, `disabled`, `aria-pressed` y selección actual; añadir señal visible/accesible de selección y una explicación breve para el estado previo al cálculo cuando sea viable sin duplicar contenido. | 1 | LF-HU-02, LF-HU-03 | `simulador-libranza-web/src/app/app.html` |
| 4 | Mantener el aviso con `role="alert"` y mensaje vivo; asegurar icono oculto para lectores de pantalla, copy y acción de reintento que soporten envoltura. Revisar iconos decorativos para que no se anuncien como texto. | 1 | LF-HU-01, LF-HU-03 | `simulador-libranza-web/src/app/app.html` |
| 5 | Reestilizar encabezado, hero, tarjetas, formulario, controles, modo, alerta, resultado, resumen, alternativas, breakdown y aviso usando tokens aprobados. Implementar rojo/texto blanco en destacados, rojo oscuro para selección, disabled legible y cifras COP que envuelvan sin superponerse. Consolidar selectores para controlar el presupuesto CSS. | 1, 2, 3, 4 | LF-HU-01, LF-HU-02, LF-HU-03 | `simulador-libranza-web/src/app/app.css` |
| 6 | Ajustar el layout responsive: eliminar mínimos rígidos fuera del escritorio, asegurar una columna en 320 px para campos, resumen, plazos y breakdown; conservar controles táctiles de al menos 44 px y padding que no cause desborde. | 5 | LF-HU-03 | `simulador-libranza-web/src/app/app.css` |
| 7 | Extender pruebas de componente para selector de modo, alternativas deshabilitadas iniciales, alternativa seleccionada (clase, `aria-pressed`, señal adicional y detalle), alerta de formulario inválido y dependencia de convenio; mantener las pruebas de catálogo y payload de simulación. | 2, 3, 4, 5 | LF-HU-01, LF-HU-02, LF-HU-03 | `simulador-libranza-web/src/app/app.spec.ts` |
| 8 | Ejecutar los gates y revisión manual de estados inicial, disabled, error y resultado en desktop, 768/390/320 px; hacer una pasada completa de teclado y capturar evidencia. | 6, 7 | LF-HU-01, LF-HU-02, LF-HU-03 | Sin cambios de archivo |

Las tareas 2, 3 y 4 pueden ejecutarse en paralelo tras la tarea 1; las tareas 5 y 6 se mantienen secuenciales para validar el reflow sobre el sistema visual final.

## 5. Archivos a cambiar

| Ruta | Operación | Cambio previsto |
|---|---|---|
| `simulador-libranza-web/src/styles.css` | Modificar | Reemplazar tokens dark/verde por canvas blanco, jerarquías rojas, superficies rojas, texto sobre rojo, disabled y foco visible en contextos claros/oscuros. |
| `simulador-libranza-web/src/app/app.html` | Modificar | Ajustar semántica nativa del selector de modo y lista de plazos; añadir señal no cromática de selección, accesibilidad de iconos y alerta roja accesible, sin alterar bindings ni contrato del formulario. |
| `simulador-libranza-web/src/app/app.css` | Modificar | Aplicar look and feel rojo/blanco, variantes de selección/disabled/foco, wrapping de cifras y reflow responsive hasta 320 px. |
| `simulador-libranza-web/src/app/app.spec.ts` | Modificar | Añadir cobertura de estados y semántica del nuevo diseño, preservando regresión de HTTP y simulación. |
| `simulador-libranza-web/src/app/app.ts` | Sin cambios previstos | Signals, validación, catálogo, payload y selección existente son suficientes para el alcance visual. |
| `simulador-libranza-web/src/app/core/credit-api.service.ts` | No modificar | Contratos de API fuera de alcance. |
| `simulador-libranza-api/**` | No modificar | Dominio, cálculo, catálogos y endpoints deben permanecer estables. |

## 6. Riesgos

| Riesgo | Impacto | Señal temprana | Mitigación |
|---|---|---|---|
| El rediseño de markup rompe bindings reactivos o el payload. | Alto | Fallan pruebas de catálogo/simulación o cambia el request POST. | Conservar nombres de controles, handlers y signals; mantener y ampliar pruebas con `HttpTestingController`. |
| Blanco sobre blanco no hace visible el foco. | Alto | El anillo de foco desaparece en inputs o links del canvas. | Combinar foco blanco con offset/contorno rojo oscuro en superficies claras; conservar anillo blanco puro sobre rojo. |
| La selección se entiende únicamente por rojo oscuro. | Alto | Sin color, no se distingue el plazo elegido. | Mantener `aria-pressed`, borde/inset blanco y señal visible con check/texto. |
| Un disabled basado solo en opacidad pierde contraste. | Alto | Plazos o convenio no se leen antes de simular. | Usar tokens específicos de disabled y contraste verificable; el atributo `disabled` conserva la no interacción. |
| Valores COP largos rompen tarjetas a 320 px. | Alto | Scroll horizontal, solapamiento de plazo/valor o corte de cifra. | `min-width: 0`, columna única en móvil y `overflow-wrap` limitado al valor. |
| El CSS supera el límite de 8 kB por componente. | Medio/alto | `ng build` falla por el presupuesto `anyComponentStyle`. | Reutilizar tokens/selectores y ejecutar build de producción antes de cierre. |
| Roles ARIA contradictorios degradan navegación asistiva. | Medio | Tabs sin panel asociado o botón anunciado como list item. | Usar radios y `ul/li/button` nativos, con ARIA solo para estado complementario. |
| Karma no inicia por ausencia de Chrome. | Medio | `npm run test -- --watch=false` compila pero no abre navegador. | Configurar `CHROME_BIN` si es necesario; registrar la limitación y ejecutar los demás gates. |
| Las pruebas unitarias no detectan contraste, foco o reflow. | Medio | Gates verdes pero defectos visuales en móvil/teclado. | Validación manual a 1440, 768, 390 y 320 px, más recorrido con Tab/Enter/Espacio y capturas requeridas por DoD. |

## 7. Criterios de aceptación

### LF-HU-01

- [ ] En 1440 px y móvil, el simulador usa canvas blanco, títulos/jerarquías rojas y superficies destacadas rojas con texto blanco legible.
- [ ] Formulario, controles, resumen, resultado, aviso y acciones mantienen etiquetas, valores y acciones sin ocultarse ni perder contraste.
- [ ] Una validación por formulario inválido presenta `[role="alert"]`, icono de error y texto blanco sobre una superficie roja; un mensaje extenso envuelve sin corte ni desbordamiento.

### LF-HU-02

- [ ] Con una respuesta válida, los plazos 60, 72, 96, 108 y 120 se presentan como tarjetas rojas con plazo, etiqueta y valor blancos.
- [ ] Al elegir 72 meses, se actualizan `selectedTerm`, `aria-pressed`, señal adicional de selección y el detalle de monto, cuota, total e intereses de esa alternativa.
- [ ] Antes de obtener resultado, cada alternativa está visiblemente atenuada pero legible, tiene atributo `disabled` y no modifica el detalle al activarla.
- [ ] En 320 px, un valor de ocho o más dígitos se acomoda dentro de su tarjeta sin tapar plazo, etiqueta ni señal de selección.

### LF-HU-03

- [ ] A 320 px, 390 px, 768 px y 1440 px no hay scroll horizontal involuntario; campos, resumen, plazos y breakdown se reorganizan adecuadamente.
- [ ] Con teclado, Tab llega a enlaces, selector de modo, campos, botón y alternativas habilitadas; Enter/Espacio activa los controles nativos correspondientes.
- [ ] El foco es visible sobre canvas blanco y superficies rojas; el modo activo y la alternativa activa tienen señal adicional al color.
- [ ] El convenio sin actividad se muestra atenuado, conserva texto de dependencia y no permite selección; los controles de carga o dependencia no permiten acciones inválidas.

### Gates de verificación

Ejecutar desde `simulador-libranza-web/`:

```bash
npm run lint
npm run typecheck
npm run build
npm run test -- --watch=false
```

El build debe respetar el límite de error de 8 kB para `anyComponentStyle`. La verificación manual debe cubrir los estados inicial, disabled, error y resultado, con capturas en escritorio y móvil, y una pasada de navegación por teclado.