# Design System — Simulador de crédito

## Decisión

Se adopta una identidad propia para el demo llamada **Cuota Clara**, sin logos, nombre ni activos del referente bancario. La dirección visual combina sobriedad financiera, lectura clara de cifras y una progresión de formulario explícita.

## Fundamentos

- Tipografía: Manrope para interfaz, jerarquías y cifras; DM Mono para metadatos, pasos y valores de referencia.
- Paleta vigente: fondo blanco `#FFFFFF`, rojo principal `#FF334D` y rojo de texto `#D81F3A`; bordes rosados `#F1B7C1` y sombras rojas suaves para profundidad.
- Espaciado: escala 8 / 16 / 24 / 40 px.
- Superficies: blancas con radio 10–22 px; acciones principales rojas con texto blanco y elevación sutil.
- Accesibilidad: contraste AA, foco visible `#D81F3A`, etiquetas persistentes, dependencias de formulario comunicadas en texto y color.

## Componentes definidos

Botones primario, secundario y deshabilitado; selector de perfil; campo monetario; selector dependiente; tarjeta de resultado; avisos de éxito y error; barra de progreso de tres pasos; panel informativo de condiciones.

## Referencia funcional aplicada

Se conserva la jerarquía introductoria, los indicadores de monto/cuota, el flujo de actividad → convenio → ingresos y descuentos → monto → tasa EA → plazo, la tarjeta de cuota, el detalle del crédito, las alternativas de 120/108/96/72/60 meses y las notas legales visibles. Se mantiene identidad, logo y copy propios del demo; no se usan activos ni marca del referente.

## Mockups y estados

El canvas cubre marca, fundamentos, componentes, simulador y estados inicial, cargando, error y resultado seleccionado. Los estados mantienen contexto y una acción clara; los resultados se comunican como estimados, sujetos a validación y sin incluir prima de seguro de vida.

## Entregable

Canvas: `design/sistema-simulador-credito/canvas.html`.

Artboards: Marca, Fundamentos, Componentes, Simulador y Estados. No se detectaron logos ni imágenes del proyecto en `brand/`, `public/`, `assets/` o `src/assets/`; la identidad se expresa como monograma tipográfico `CC`, sin rutas de imagen externas.

## Validación de etapa

- Grafo reutilizado: `graphify-out/graph.json` existente; su comunidad UX relaciona el diseño con la SPA Angular, accesibilidad visual y contratos API sin alterar la arquitectura por capas.
- `canvas.html` quedó resincronizado con los cinco archivos reales de `artboards/`.
- La fuente de verdad de tokens es `simulador-libranza-web/src/styles.css`.

## Pendientes

Validar con negocio el nombre final, el copy legal, las tasas y rangos configurables antes de implementación.
