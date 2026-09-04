# Design System — Simulador de crédito

## Decisión

Se adopta una identidad propia para el demo llamada **Cuota Clara**, sin logos, nombre ni activos del referente bancario. La dirección visual combina sobriedad financiera, lectura clara de cifras y una progresión de formulario explícita.

## Fundamentos

- Tipografía: Manrope para interfaz, jerarquías y cifras; DM Mono para metadatos, pasos y valores de referencia.
- Paleta: Medianoche `#172033`, auxiliar `#E71850` para títulos, botones y header, púrpura `#7835C3` para iconos, Rosa suave `#FFF0F4`, Niebla `#F7F8FA`, Alerta `#C94855`.
- Espaciado: escala 8 / 16 / 24 / 40 px.
- Superficies: tarjetas blancas con radio 18–22 px; controles con radio 10–14 px; acciones principales con radio 11–12 px.
- Accesibilidad: contraste AA, foco visible en rosa auxiliar, etiquetas persistentes, dependencias de formulario comunicadas en texto y color.

## Componentes definidos

Botones primario, secundario y deshabilitado; selector de perfil; campo monetario; selector dependiente; tarjeta de resultado; avisos de éxito y error; barra de progreso de tres pasos; panel informativo de condiciones.

## Referencia funcional aplicada

Se tomó como referencia el simulador de crédito de libranza proporcionado por el usuario. Se conservaron la jerarquía introductoria, los indicadores de monto/cuota, el flujo de actividad → convenio → ingresos y descuentos → monto → tasa EA → plazo, la tarjeta de cuota, el detalle del crédito, las alternativas de 120/108/96/72/60 meses y las notas legales visibles. Se mantuvo identidad, logo, nombre y copy propios del demo; no se copiaron activos ni marca de Banco Finandina.

## Mockup principal

El flujo inicia con actividad, continúa con convenio e ingresos/descuentos, monto, tasa y plazo; comunica que el resultado es estimado, no una aprobación. El mockup recalcula la cuota estimada con cuota fija para 120, 108, 96, 72 y 60 meses, y expone ingreso mensual, monto solicitado, plazo elegido y cuota mensual. Las cifras de ejemplo son datos sintéticos y no condiciones comerciales.

## Entregable

Canvas: `design/sistema-simulador-credito/canvas.html`.
Artboards: Marca, Fundamentos, Componentes y Mockup del Simulador.

## Pendientes

Validar con negocio el nombre final, el copy legal, las tasas y rangos configurables antes de implementación.
