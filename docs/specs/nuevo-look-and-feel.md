# FEATURE: Nuevo look and feel rojo del simulador de libranza

## Nomenclatura
Prefijo de HU: LF-HU

## CONTEXTO / CONVERSACIÓN
El visitante debe percibir el nuevo sistema visual Cuota Clara durante toda la simulación de libranza. La decisión aprobada aplica fondo blanco, identidad roja visible y tarjetas rojas con letra blanca en todo el simulador. Las tarjetas disponibles y seleccionadas se diferencian por tonos: la alternativa seleccionada usa un rojo más oscuro. Los mensajes dentro de tarjetas rojas usan texto blanco e icono; el foco de teclado se indica con borde blanco y los controles deshabilitados conservan una apariencia atenuada y legible. El alcance incluye escritorio y móvil, sin modificar reglas de cálculo, catálogos ni contratos existentes.

Evidencia reutilizada: el flujo actual presenta alternativas de 120, 108, 96, 72 y 60 meses y conserva sus estados mediante el componente `App` y sus contratos `SimulationResponse`/`SimulationAlternative`; el sistema visual vigente está documentado en `.perxia/design-system.md`.

## HISTORIAS DE USUARIO

### LF-HU-01 Reconocer la identidad visual del simulador
*PARA: * reconocer claramente el simulador y completar sus datos con confianza visual
*YO COMO:* visitante del simulador de crédito
*QUIERO:* visualizar el encabezado, formulario, controles, avisos y resultado con el nuevo lenguaje visual rojo sobre fondo blanco

**Criterios de aceptación**
- Cuando abro el simulador en escritorio o móvil, visualizo fondo blanco, texto rojo de jerarquía y superficies rojas con texto blanco en los componentes destacados.
- Cuando visualizo un control, aviso o resultado, la tipografía y los colores respetan la jerarquía de Cuota Clara sin ocultar su etiqueta, valor o acción.
- Cuando aparece un mensaje de ayuda, validación o error dentro de una superficie roja, visualizo texto blanco acompañado por un icono que comunica el tipo de mensaje.

**Escenarios de prueba (Gherkin)**
```gherkin
Feature: Identidad roja del simulador
  Scenario: Visualizar el simulador con la nueva identidad
    Given que el visitante abre el simulador de crédito
    When la pantalla termina de cargar
    Then visualiza fondo blanco, jerarquías rojas y componentes destacados rojos con texto blanco legible

  Scenario: Mostrar una validación sin datos obligatorios
    Given que el visitante deja un dato obligatorio sin diligenciar
    When intenta calcular las alternativas
    Then visualiza el mensaje de validación con texto blanco e icono si se presenta dentro de una superficie roja

  Scenario: Leer un contenido extenso en una superficie destacada
    Given que una ayuda o aviso destacado contiene un texto largo
    When el visitante lo visualiza en el simulador
    Then el texto se envuelve dentro de la superficie sin quedar cortado ni perder contraste
```

### LF-HU-02 Comparar y seleccionar alternativas de plazo
*PARA: * identificar con claridad el plazo que mejor se ajusta a mi capacidad de pago
*YO COMO:* visitante que consulta una simulación de crédito
*QUIERO:* visualizar las tarjetas de alternativas y el detalle estimado con fondo rojo y letra blanca, diferenciando el plazo seleccionado con un rojo más oscuro

**Criterios de aceptación**
- Cuando el simulador muestra alternativas válidas, cada tarjeta de 60, 72, 96, 108 o 120 meses presenta plazo, valor y etiqueta con texto blanco sobre un tono rojo.
- Cuando selecciono una alternativa disponible, su tarjeta cambia a rojo más oscuro y comunica la selección mediante una señal adicional al color.
- Cuando aún no existe un resultado de simulación, las tarjetas conservan texto legible, tono rojo atenuado y no permiten seleccionar un plazo.
- Cuando visualizo monto, cuota, total estimado o intereses de la alternativa elegida, sus valores permanecen legibles y corresponden a la alternativa seleccionada.

**Escenarios de prueba (Gherkin)**
```gherkin
Feature: Tarjetas rojas de alternativas de plazo
  Scenario: Seleccionar una alternativa disponible
    Given que el visitante recibe alternativas válidas de simulación
    When selecciona la tarjeta de 72 meses
    Then la tarjeta de 72 meses se muestra en rojo más oscuro con texto blanco y el detalle presenta los valores de 72 meses

  Scenario: Intentar seleccionar un plazo antes de simular
    Given que el visitante todavía no ha obtenido un resultado
    When intenta seleccionar una tarjeta de plazo
    Then la tarjeta permanece deshabilitada y no cambia el detalle de la simulación

  Scenario: Visualizar un valor monetario largo
    Given que una alternativa muestra un valor monetario de ocho o más dígitos
    When el visitante visualiza la tarjeta en móvil
    Then el valor se ajusta dentro de la tarjeta roja sin superponerse al plazo ni perder legibilidad
```

### LF-HU-03 Operar el look and feel en móvil, escritorio y teclado
*PARA: * completar la simulación sin barreras de dispositivo o navegación
*YO COMO:* visitante del simulador de crédito
*QUIERO:* usar los controles y tarjetas rojas con una presentación adaptable y foco visible

**Criterios de aceptación**
- Cuando consulto el simulador en escritorio o móvil, el formulario, resultado, tarjetas y avisos se reorganizan sin scroll horizontal no intencional.
- Cuando navego por teclado hacia un enlace, campo, selector, botón o tarjeta disponible, visualizo un borde blanco de foco sin perder el texto blanco sobre rojo.
- Cuando un control depende de una selección previa o está en carga, visualizo su estado atenuado y no puedo ejecutar una acción inválida.
- Cuando cambio de modo entre monto y cuota, el control seleccionado conserva una señal visible adicional al color y el campo correspondiente permanece operable.

**Escenarios de prueba (Gherkin)**
```gherkin
Feature: Operación accesible del look and feel
  Scenario: Navegar por teclado hasta una alternativa habilitada
    Given que el visitante tiene un resultado de simulación disponible
    When navega con la tecla Tab hasta una tarjeta de plazo
    Then visualiza un borde blanco de foco y puede seleccionar la alternativa usando el teclado

  Scenario: Visualizar un convenio dependiente sin actividad
    Given que el visitante no ha seleccionado una actividad
    When visualiza el campo de convenio
    Then el campo se muestra atenuado, explica su dependencia y no permite elegir un convenio

  Scenario: Consultar el simulador en un móvil estrecho
    Given que el visitante abre el simulador en una pantalla móvil de 320 píxeles de ancho
    When recorre el formulario y las tarjetas de plazo
    Then el contenido se distribuye en una columna sin scroll horizontal, controles inaccesibles ni texto cortado
```

## DOD GENERAL DE LA FEATURE / PRODUCTO / PROYECTO
- Las tres historias son revisadas en escritorio y móvil, con capturas de los estados inicial, deshabilitado, error y resultado.
- Se verifica que el flujo existente de actividad, convenio, cálculo y selección de alternativas conserva sus resultados funcionales.
- Se verifica contraste visual y navegación por teclado en controles interactivos y tarjetas habilitadas.
- No se incorporan logos, copy ni activos de la entidad bancaria de referencia.

## PENDIENTES POR DEFINIR
- No hay pendientes funcionales para el corte actual.
