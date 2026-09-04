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
- Ingresos mensuales.
- Descuentos de nómina.
- Monto solicitado.
- Tasa de interés efectiva anual de referencia.
- Plazo a financiar.
- Cuota mensual estimada.
- Detalle resumido del crédito.
- Alternativas de plazo con su cuota correspondiente.
- Acción para solicitar o continuar el proceso.
- Avisos legales y aclaraciones sobre carácter informativo, seguro de vida y aprobación.

La estética debe ser similar en jerarquía visual, distribución, ritmo de formularios, uso de tarjetas, colores sobrios, llamados a la acción y presentación de resultados. Se debe crear una identidad visual propia para el demo: no usar logos, imágenes de marca, textos, nombre comercial ni elementos que hagan pasar la aplicación por el banco de referencia.

El enlace de referencia funcional y visual es el proporcionado por el usuario en la solicitud original. Debe usarse únicamente como referencia de análisis y no como dependencia técnica del aplicativo.

## Stack obligatorio

### Frontend

- Angular con TypeScript estricto.
- Angular CLI para crear, desarrollar, probar y validar la aplicación.
- Componentes pequeños y reutilizables.
- Formularios reactivos para validación y estados de habilitación.
- Servicios Angular para acceso a la API y reglas de presentación.
- HttpClient para comunicación con el backend.
- Detección de cambios `OnPush` cuando sea compatible con el componente.
- Diseño responsive para escritorio, tablet y móvil.

### Backend

- .NET con ASP.NET Core Web API.
- API versionada desde el inicio, preferiblemente bajo `/api/v1`.
- Separación entre contratos HTTP, casos de uso, dominio y persistencia.
- DTOs para entrada y salida; no exponer entidades de persistencia directamente.
- Validación server-side como autoridad final de las reglas.
- Documentación OpenAPI/Swagger en ambiente de desarrollo.
- Configuración por ambiente y secretos fuera del repositorio.

### Arquitectura inicial

Usar un monolito modular simple, evitando microservicios para este demo. La separación lógica recomendada es:

- `frontend`: presentación, estado de la simulación, formularios y consumo de API.
- `backend`: API, aplicación, dominio, infraestructura y pruebas.
- `domain`: reglas puras de cálculo y validaciones del crédito.
- `application`: casos de uso como simular crédito y consultar catálogos.
- `infrastructure`: persistencia o fuentes mock, configuración y adaptadores.
- `api`: endpoints, DTOs, mapeo, errores y documentación.

Las dependencias deben apuntar hacia el dominio. El dominio no debe conocer Angular, ASP.NET Core, Entity Framework ni detalles de infraestructura.

## Flujo funcional esperado

1. El usuario visualiza la propuesta del simulador.
2. Selecciona su actividad, inicialmente `Docente` o `Pensionado`.
3. El sistema habilita y carga los convenios disponibles para esa actividad.
4. El usuario selecciona un convenio.
5. El sistema habilita ingresos mensuales, descuentos de nómina y monto solicitado.
6. El frontend valida formato, rangos y obligatoriedad.
7. El usuario solicita la simulación.
8. El backend valida nuevamente los datos y calcula la propuesta.
9. La interfaz muestra tasa de referencia, cuota mensual, plazo y detalle del crédito.
10. Se muestran alternativas de plazo, inicialmente 60, 72, 96, 108 y 120 meses.
11. La acción de solicitud permanece deshabilitada hasta que la simulación sea válida.
12. Las condiciones y resultados se presentan como informativos y sujetos a políticas de crédito.

## Reglas iniciales del dominio

- Moneda de presentación: COP, configurable por ambiente.
- Tasa expresada como efectiva anual y convertida internamente a la periodicidad de pago definida.
- Sistema de amortización configurable; el demo debe iniciar con cuota fija, sistema francés.
- La cuota debe separar conceptualmente capital e intereses.
- El seguro de vida debe aparecer como exclusión o valor separado, no mezclarse silenciosamente con la cuota base.
- El resultado debe incluir monto, plazo, tasa, cuota y total estimado pagado.
- Los rangos de monto, ingresos, descuentos y plazos deben ser configurables, no valores dispersos en componentes.
- El backend debe rechazar montos negativos, ingresos no válidos, descuentos superiores a los ingresos y plazos no habilitados.
- No implementar aprobación real, consulta de score, desembolso, cobranza, firma digital ni integración bancaria dentro del alcance inicial.

## Buenas prácticas obligatorias de Angular

Se deben aplicar las prácticas descritas en la referencia proporcionada por el usuario: [Angular Best Practices and Security](https://www-tatvasoft-com.translate.goog/blog/angular-optimization-and-best-practices/?_x_tr_sl=en&_x_tr_tl=es&_x_tr_hl=es&_x_tr_pto=tc).

Lineamientos aplicables al proyecto:

- Usar Angular CLI y scripts reproducibles.
- Mantener una estructura de carpetas por funcionalidad.
- Evitar `any`; definir interfaces, tipos unión y contratos explícitos.
- Usar `const` cuando una referencia no cambie y `let` solo cuando sea necesario.
- Evitar lógica de negocio compleja dentro de las plantillas.
- Dividir componentes grandes en componentes presentacionales y contenedores.
- Aplicar responsabilidad única a componentes, servicios y clases.
- Usar nombres consistentes, descriptivos y convenciones de Angular.
- Usar `trackBy` en listas dinámicas cuando aplique.
- Preferir carga diferida para funcionalidades que no sean necesarias en el arranque.
- Evitar suscripciones anidadas; componer observables con operadores RxJS.
- Usar `AsyncPipe` o mecanismos de destrucción automática de suscripciones.
- Evitar fugas de memoria y limpiar recursos asociados al ciclo de vida.
- Cachear catálogos que no cambian frecuentemente cuando sea apropiado.
- Usar inmutabilidad en actualizaciones de estado.
- Aplicar `OnPush` y evitar cálculos costosos repetidos durante la detección de cambios.
- Mantener archivos y funciones pequeños, legibles y fáciles de probar.
- Usar lint, formateo y tipado estricto como puertas de calidad.
- Mantener configuración por ambiente para URLs y parámetros no sensibles.
- No concatenar plantillas HTML con entradas del usuario.
- No usar `innerHTML` salvo necesidad justificada y contenido sanitizado.
- Mantener Angular y sus dependencias actualizados dentro de una política controlada.

## Seguridad

- Validar toda entrada en frontend y backend; la validación del backend es obligatoria.
- No incluir secretos, llaves, tokens ni credenciales en el código o archivos versionados.
- Configurar CORS con orígenes explícitos por ambiente.
- Usar HTTPS fuera de desarrollo local.
- Evitar información sensible en logs y mensajes de error.
- Devolver errores consistentes con un formato común y mensajes seguros para el usuario.
- Proteger la API contra inyección mediante APIs tipadas y consultas parametrizadas.
- Aplicar encabezados de seguridad y una política CSP cuando el despliegue lo permita.
- No tratar el simulador como un sistema de decisión crediticia real.
- No almacenar datos personales reales en el demo; usar datos sintéticos.

## Diseño y experiencia

- Crear una identidad de demo propia, sin logos ni nombre del banco de referencia.
- Mantener una estética bancaria moderna, clara y sobria, con alto contraste y jerarquía visual consistente.
- Conservar la estructura de navegación, formulario progresivo, tarjetas de resultado, selector de plazos, llamados a la acción y notas informativas del referente.
- Usar etiquetas claras, estados de campo visibles, mensajes de error accionables y estados de carga.
- Mantener los campos dependientes deshabilitados hasta completar la selección previa.
- Garantizar navegación por teclado, foco visible, etiquetas asociadas y contraste suficiente.
- Diseñar primero para el flujo principal y después para estados vacíos, error, carga y datos inválidos.
- No copiar activos de marca, logos, textos propietarios ni elementos que impliquen afiliación.

## Fuera del alcance inicial

- Migración a Flutter.
- Aplicación móvil nativa.
- Acoplamiento con Flutter o desarrollo multiplataforma.
- Integración con core bancario, nómina, convenios reales, centrales de riesgo o proveedores de seguros.
- Autenticación de clientes reales.
- Aprobación, desembolso o contratación del crédito.
- Persistencia de solicitudes reales.
- Administración de tasas y políticas mediante portal backoffice.

Flutter se considera una línea de evolución futura. Para facilitarla sin incorporarla al alcance inicial, mantener contratos API independientes de Angular, DTOs versionados, reglas de cálculo en el backend y respuestas estables.

## Criterios de onboarding técnico

Antes de iniciar funcionalidades, el equipo debe dejar disponible:

- Aplicación Angular ejecutable localmente mediante scripts documentados.
- API .NET ejecutable localmente con configuración por ambiente.
- Endpoint para catálogos de actividades y convenios.
- Endpoint para simulación de crédito.
- Contratos DTO y ejemplos de solicitudes/respuestas.
- Cálculo de amortización cubierto por pruebas unitarias.
- Manejo común de errores y validaciones.
- Estructura de carpetas estable y convenciones documentadas.
- Lint, typecheck y pruebas ejecutables desde la línea de comandos.
- Datos mock sintéticos para el flujo principal.

## Definición técnica de terminado

Una entrega se considera terminada cuando cumple criterios funcionales, pruebas unitarias y de integración relevantes, lint y typecheck sin errores, validación responsive básica, accesibilidad esencial, manejo de estados de carga/error y revisión de que no contiene logos, nombre ni activos del banco de referencia.

## Índice de conocimiento

- Graphify reindexado el 2026-09-03 sobre 22 archivos (~41.891 palabras): 30 nodos, 16 aristas y 17 comunidades.
- Núcleos: flujo funcional del simulador, arquitectura y alcance, fundamentos/componentes, marca/canvas y evolución de los detalles visuales.
- El repositorio contiene documentación, canvas HTML y capturas de iteración; no hay código Angular/.NET implementable indexado todavía.
- Ambigüedad: 12 comunidades son delgadas o aisladas, principalmente capturas históricas; no deben interpretarse como decisiones vigentes sin contrastarlas con `Simulador.html` y `.perxia/estado.md`.
- Salidas generadas localmente en `graphify-out/`, excluidas de control de versiones.
