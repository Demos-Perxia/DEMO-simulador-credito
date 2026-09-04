# Design System — Simulador de crédito

## Decisión

Se adopta una identidad propia para el demo llamada **Cuota Clara**, sin logos, nombre ni activos del referente bancario. La dirección visual combina sobriedad financiera, lectura clara de cifras y una progresión de formulario explícita.

## Fundamentos

- Tipografía: Manrope para interfaz, jerarquías y cifras; DM Mono para metadatos, pasos y valores de referencia.
- Paleta vigente: fondo negro `#080808`, rojo `#FF334D` para tipografía y jerarquía, verde `#62F58C` para acentos, iconos y acciones; bordes verde oscuro `#26362C`.
- Espaciado: escala 8 / 16 / 24 / 40 px.
- Superficies: tarjetas blancas con radio 18–22 px; controles con radio 10–14 px; acciones principales con radio 11–12 px.
- Accesibilidad: contraste AA, foco visible en rosa auxiliar, etiquetas persistentes, dependencias de formulario comunicadas en texto y color.

## Componentes definidos

Botones primario, secundario y deshabilitado; selector de perfil; campo monetario; selector dependiente; tarjeta de resultado; avisos de éxito y error; barra de progreso de tres pasos; panel informativo de condiciones.

## Referencia funcional aplicada

Se conserva la jerarquía introductoria, los indicadores de monto/cuota, el flujo de actividad → convenio → ingresos y descuentos → monto → tasa EA → plazo, la tarjeta de cuota, el detalle del crédito, las alternativas de 120/108/96/72/60 meses y las notas legales visibles. Se mantiene identidad, logo y copy propios del demo; no se usan activos ni marca del referente.

## Mockups y estados

El canvas cubre marca, fundamentos, componentes, simulador y estados inicial, cargando, error y resultado seleccionado. Los estados mantienen contexto y una acción clara; los resultados se comunican como estimados, sujetos a validación y sin incluir prima de seguro de vida.

## Entregable

Canvas: `design/sistema-simulador-credito/canvas.html`.
Artboards: Marca, Fundamentos, Componentes, Simulador y Estados.

## Pendientes

Validar con negocio el nombre final, el copy legal, las tasas y rangos configurables antes de implementación.