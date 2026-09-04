# Plan: Demo de Simulador de Crédito de Libranza

## 1. Objetivos

Construir un demo web responsive de **Cuota Clara** con Angular y ASP.NET Core Web API que permita simular crédito de libranza con datos sintéticos, bajo estas condiciones verificables:

- Actividad: `Docente` o `Pensionado`; convenio dependiente de la actividad.
- Dos modos de cálculo:
  - **Monto**: a partir del monto solicitado, calcula la cuota mensual por cada plazo.
  - **Cuota**: a partir de la capacidad máxima de pago, calcula el monto máximo financiable por cada plazo.
- Datos económicos: ingresos mensuales, descuentos de nómina y monto o cuota máxima según el modo.
- Tasa mock única de **18 % EA**, configurable por ambiente; conversión a tasa efectiva mensual.
- Amortización francesa de cuota fija para 60, 72, 96, 108 y 120 meses.
- Resultado informativo con tasa, monto, cuota, total estimado, interés estimado, desglose conceptual de capital/intereses, alternativas por plazo y aviso de seguro de vida no incluido.
- Validación de cliente y servidor, estados de carga/error, contratos OpenAPI y pruebas automatizadas.
- UI Angular accesible, responsive y fiel a los fundamentos visuales vigentes: Manrope, DM Mono, `#E71850`, `#7835C3`, tarjetas y foco visible.
- Separación física obligatoria: el frontend Angular residirá en `simulador-libranza-web/` y el backend ASP.NET Core en `simulador-libranza-api/`, sin mezclar código, dependencias, artefactos de compilación ni pruebas entre ambas carpetas.

Fuera del alcance: autenticación, aprobación real, solicitud persistida, score, centrales de riesgo, core bancario, nómina/convenios reales, desembolso, seguros reales, backoffice y Flutter.

## 2. Alcance por historia de usuario

### S3-HU1: Preparar la base técnica del demo

Como equipo de desarrollo, queremos contar con un monolito modular Angular/.NET reproducible para construir y validar el demo.

- Implica crear el workspace Angular estricto y la solución .NET por capas, scripts de calidad, configuración por ambiente, CORS local explícito, OpenAPI y convenciones de errores.
- Depende de ninguna funcionalidad previa.
- Excluye persistencia y secretos versionados.

### S3-HU2: Consultar actividad y convenio dependiente

Como usuario, quiero seleccionar mi actividad y un convenio compatible para iniciar la simulación.

- Actividades mock: Docente y Pensionado.
- Los convenios sintéticos se consultan por actividad; al cambiar actividad, el convenio y resultados previos se limpian.
- Depende de S3-HU1 y de catálogos mock en infraestructura.

### S3-HU3: Simular por monto solicitado

Como usuario, quiero ingresar un monto y obtener cuotas estimadas por plazo para comparar alternativas.

- Usa tasa única 18 % EA, moneda COP, amortización francesa y los cinco plazos aprobados.
- La cuota mensual no puede exceder el 40 % del ingreso disponible (`ingreso - descuentos`); monto permitido: 1.000.000–100.000.000 COP.
- Depende de S3-HU1 y S3-HU2.

### S3-HU4: Simular por cuota máxima

Como usuario, quiero indicar la cuota máxima que puedo pagar y conocer el monto máximo financiable por cada plazo.

- Usa la misma tasa, plazos y política de capacidad de S3-HU3.
- La cuota máxima debe ser positiva, no superar 40 % del ingreso disponible y derivar un monto dentro del rango mock configurado.
- Depende de S3-HU3, pues reutiliza las fórmulas, políticas, DTOs y presentación de alternativas.

### S3-HU5: Consultar resultados, alternativas y avisos

Como usuario, quiero entender el resultado estimado y elegir un plazo sin confundirlo con una aprobación real.

- Incluye selección de alternativa, resumen de monto/tasa/cuota/plazo/total/intereses, estado de simulación vigente y avisos informativos.
- La acción “Solicitar crédito” será demostrativa y no creará solicitudes ni persistirá datos.
- Depende de S3-HU3 y S3-HU4.

### S3-HU6: Garantizar calidad y experiencia

Como equipo, queremos validar cálculo, API, UI, accesibilidad y responsive antes de dar por terminado el demo.

- Depende de S3-HU1 a S3-HU5.

## 3. Decisiones de arquitectura

1. **Monolito modular Angular + ASP.NET Core.**
   - Decisión: un frontend Angular independiente y un backend .NET organizado en API, Application, Domain, Infrastructure y proyectos de prueba.
   - Alternativas descartadas: microservicios (sobredimensionados para el demo) y cálculo en Angular (duplicaría reglas y debilitaría la validación autoritativa).

2. **Reglas financieras puras en Domain.**
   - Decisión: encapsular dinero, tasa, plazo, política y calculadora francesa en clases/servicios sin dependencias de HTTP, UI o infraestructura.
   - Alternativa descartada: fórmulas en controladores, templates o servicios Angular.

3. **API versionada y contratos tipados.**
   - Decisión: exponer `/api/v1/catalogs/activities`, `/api/v1/catalogs/activities/{activityId}/agreements` y `/api/v1/credit-simulations`; documentarlos con OpenAPI en desarrollo.
   - Alternativa descartada: endpoints no versionados o entidades internas expuestas directamente.

4. **Política mock explícita y configurable.**
   - Decisión: `18 % EA`, COP, plazos `[60,72,96,108,120]`, monto entre `1.000.000` y `100.000.000` COP, y cuota/capacidad máxima igual a 40 % del ingreso disponible. Centralizarla en configuración de infraestructura y entregarla al dominio mediante una abstracción.
   - Alternativa descartada: hardcodear valores en componentes o crear tasas por convenio sin requerimiento aprobado.

5. **Dos operaciones o un request discriminado por modo.**
   - Decisión: `POST /api/v1/credit-simulations` recibe un DTO con `mode: AMOUNT | INSTALLMENT_CAPACITY` y exactamente un valor objetivo: `requestedAmountCop` o `maximumInstallmentCop`.
   - Alternativa descartada: dos endpoints con respuestas divergentes. Un contrato discriminado comparte validaciones y permite una UI uniforme.

6. **Cálculo con `decimal` y redondeo único.**
   - Decisión: usar `decimal` para dinero/tasas, convertir EA a mensual mediante `i=(1+EA)^(1/12)-1`, calcular francés y redondear valores COP a peso al construir la respuesta. Manejar explícitamente tasa cero.
   - Alternativa descartada: `double` para dinero o redondeos distribuidos que generen inconsistencias.

7. **Catálogos sintéticos sin persistencia.**
   - Decisión: infraestructura provee actividades y convenios mock en memoria; no se escribe información del usuario ni solicitudes.
   - Alternativa descartada: base de datos o convenios reales, fuera de alcance.

8. **Formulario reactivo y estado remoto en Angular.**
   - Decisión: Reactive Forms tipados, `HttpClient`, componentes presentacionales `OnPush`, flujos RxJS sin suscripciones anidadas y resultado invalidado ante cambios relevantes.
   - Alternativa descartada: formularios por template y cálculos financieros locales.

9. **Diseño propio Cuota Clara.**
   - Decisión: convertir el artboard `design/sistema-simulador-credito/artboards/Simulador.html` en componentes Angular, preservando tokens y jerarquía, sin reutilizar imagen de Unsplash, logos, copy o activos de terceros.
   - Alternativa descartada: incrustar o copiar directamente el mockup HTML y sus dependencias externas.

10. **Errores estándar y seguros.**
    - Decisión: devolver `application/problem+json` para validación y errores controlados; Angular los traduce a alertas accesibles, sin exponer trazas o valores sensibles.
    - Alternativa descartada: mensajes de excepción sin contrato.

## 4. Tareas

| # | Tarea | Depende de | HU | Archivos |
|---|---|---|---|---|
| 1 | Crear estructura `simulador-libranza-web/` Angular y `simulador-libranza-api/` .NET con SDK/versiones documentadas, configuración local, lockfile versionado y scripts reproducibles. Corregir `.gitignore` para no ignorar el lockfile elegido. | — | S3-HU1 | `simulador-libranza-web/*` (nuevo), `simulador-libranza-api/*` (nuevo), `.gitignore`, `README.md` |
| 2 | Configurar calidad base: TypeScript estricto, ESLint/formatter, pruebas Angular; analyzers .NET, cobertura y proyectos de prueba; pipeline local de build/test. | 1 | S3-HU1 | `simulador-libranza-web/angular.json`, `simulador-libranza-web/package.json`, `simulador-libranza-web/tsconfig*.json`, `simulador-libranza-api/*.sln`, `simulador-libranza-api/**/*.csproj`, configuración de pruebas |
| 3 | Definir contrato OpenAPI, versionado `/api/v1`, Problem Details, mapeo de errores, CORS por ambiente y Swagger exclusivo de desarrollo. | 1 | S3-HU1 | `simulador-libranza-api/src/CuotaClara.Api/Program.cs`, `simulador-libranza-api/src/CuotaClara.Api/Endpoints/*`, `simulador-libranza-api/src/CuotaClara.Api/Contracts/*`, `simulador-libranza-api/src/CuotaClara.Api/appsettings*.json` |
| 4 | Implementar valor de dinero, tasa EA, plazo, política mock, tipos de modo y calculadora francesa pura, incluido caso de tasa cero y redondeo COP. | 1 | S3-HU3 | `simulador-libranza-api/src/CuotaClara.Domain/Simulation/*`, `simulador-libranza-api/tests/CuotaClara.Domain.Tests/*` |
| 5 | Implementar casos de uso de catálogos y simulación; validar formato, actividad/convenio, rangos, capacidad, modo y plazos; construir alternativas. | 3, 4 | S3-HU2, S3-HU3, S3-HU4 | `simulador-libranza-api/src/CuotaClara.Application/Catalogs/*`, `simulador-libranza-api/src/CuotaClara.Application/Simulations/*`, `simulador-libranza-api/src/CuotaClara.Application/Abstractions/*` |
| 6 | Implementar proveedores mock de actividades/convenios y política configurable de 18 % EA, límites y plazos; registrar dependencias. | 4, 5 | S3-HU2, S3-HU3, S3-HU4 | `simulador-libranza-api/src/CuotaClara.Infrastructure/Catalogs/*`, `simulador-libranza-api/src/CuotaClara.Infrastructure/Configuration/*`, `simulador-libranza-api/src/CuotaClara.Api/appsettings*.json` |
| 7 | Exponer y probar endpoints de catálogos y simulación, incluyendo errores de validación, convenio incompatible, entradas manipuladas y serialización de COP. | 3, 5, 6 | S3-HU2, S3-HU3, S3-HU4 | `simulador-libranza-api/src/CuotaClara.Api/Endpoints/*`, `simulador-libranza-api/tests/CuotaClara.Api.IntegrationTests/*` |
| 8 | Implementar núcleo Angular: rutas, modelos API tipados, cliente HTTP, interceptación/mapeo de Problem Details, configuración por ambiente y caché de catálogos. | 1, 3 | S3-HU1, S3-HU2 | `simulador-libranza-web/src/app/core/*`, `simulador-libranza-web/src/app/shared/models/*`, `simulador-libranza-web/src/environments/*` |
| 9 | Construir formulario reactivo tipado: selector de modo, actividad, convenio dependiente, ingresos, descuentos y campo monetario contextual; aplicar validación de cliente e invalidación del resultado. | 7, 8 | S3-HU2, S3-HU3, S3-HU4 | `simulador-libranza-web/src/app/features/simulator/simulation-form/*`, `simulador-libranza-web/src/app/shared/ui/*` |
| 10 | Construir cliente y orquestación de simulación para ambos modos; manejar loading, error, reintento y estado desactualizado sin cálculos financieros en el navegador. | 7, 8, 9 | S3-HU3, S3-HU4 | `simulador-libranza-web/src/app/features/simulator/data-access/*`, `simulador-libranza-web/src/app/features/simulator/simulator-page/*` |
| 11 | Implementar tarjeta de resultado, detalle de crédito, selector de plazos y alternativas accesibles; seleccionar una alternativa actualiza el resumen de la respuesta actual. | 10 | S3-HU5 | `simulador-libranza-web/src/app/features/simulator/simulation-result/*`, `simulador-libranza-web/src/app/features/simulator/term-alternatives/*` |
| 12 | Aplicar el sistema visual Cuota Clara a layout, campos, botones, alertas, foco y breakpoints; sustituir dependencias visuales externas del mockup por recursos propios o CSS/SVG permitido. | 9, 11 | S3-HU5, S3-HU6 | `simulador-libranza-web/src/styles.*`, `simulador-libranza-web/src/app/shared/ui/*`, `simulador-libranza-web/src/app/features/simulator/*` |
| 13 | Añadir avisos: resultado informativo, sin aprobación, tasa mock de referencia y seguro de vida excluido; mantener “Solicitar crédito” como acción demostrativa no persistente. | 11 | S3-HU5 | `simulador-libranza-web/src/app/features/simulator/informational-disclaimer/*`, `simulador-libranza-web/src/app/features/simulator/simulator-page/*` |
| 14 | Crear pruebas unitarias Angular para validación, dependencia actividad-convenio, estados de API y selección de alternativas. | 9, 10, 11 | S3-HU6 | `simulador-libranza-web/src/app/**/*.spec.ts` |
| 15 | Ejecutar pruebas de dominio/API/UI, revisión manual responsive y teclado, y documentar instalación, variables no sensibles, comandos y limitaciones mock. | 2, 7, 12, 13, 14 | S3-HU6 | `README.md`, `simulador-libranza-web/*`, `simulador-libranza-api/*` |

Tareas paralelizables después de la tarea 1: las tareas 3 y 4; tras ellas, la tarea 8 puede iniciar mientras se completan 5–7. Las tareas 12 y 14 pueden ejecutarse en paralelo una vez estén los componentes pertinentes.

## 5. Archivos a cambiar

> El repositorio aún no tiene código Angular/.NET; todas las rutas de implementación siguientes son nuevas salvo donde se indique modificación.

| Ruta | Cambio |
|---|---|
| `.gitignore` | Modificar: conservar exclusión de cachés y secretos, pero permitir versionar el lockfile de dependencias seleccionado. |
| `README.md` | Modificar: prerrequisitos, arquitectura, ejecución local, variables no sensibles, comandos de calidad y limitaciones del demo. |
| `simulador-libranza-web/` | Nuevo: workspace Angular con configuración estricta, scripts, dependencias bloqueadas y entorno por ambiente. |
| `simulador-libranza-web/src/app/core/` | Nuevo: configuración, cliente HTTP, manejo de errores y servicios de API. |
| `simulador-libranza-web/src/app/shared/` | Nuevo: tipos, pipes COP/tasa, componentes reutilizables y utilidades de accesibilidad. |
| `simulador-libranza-web/src/app/features/simulator/` | Nuevo: página contenedora, formulario, datos, resultados, alternativas, avisos y pruebas. |
| `simulador-libranza-web/src/styles.*` | Nuevo: tokens Cuota Clara, tipografía local/permitida, foco, responsive global. |
| `simulador-libranza-api/CuotaClara.sln` | Nuevo: solución que agrupa API, Application, Domain, Infrastructure y pruebas. |
| `simulador-libranza-api/src/CuotaClara.Domain/` | Nuevo: modelos financieros, política y calculadora francesa pura. |
| `simulador-libranza-api/src/CuotaClara.Application/` | Nuevo: casos de uso, validadores, interfaces y modelos internos. |
| `simulador-libranza-api/src/CuotaClara.Infrastructure/` | Nuevo: catálogos mock, opciones por ambiente e implementaciones de interfaces. |
| `simulador-libranza-api/src/CuotaClara.Api/` | Nuevo: endpoints v1, DTOs, mapeos, Problem Details, OpenAPI, CORS y configuración. |
| `simulador-libranza-api/tests/CuotaClara.Domain.Tests/` | Nuevo: pruebas deterministas de cálculo y validaciones financieras. |
| `simulador-libranza-api/tests/CuotaClara.Api.IntegrationTests/` | Nuevo: pruebas de contratos HTTP, serialización y errores. |
| `design/sistema-simulador-credito/artboards/Simulador.html` | Sin modificar en implementación: se usa como referencia visual histórica; los componentes productivos viven en Angular. |

## 6. Riesgos

| Riesgo | Impacto | Señal temprana | Mitigación |
|---|---|---|---|
| Fórmula o redondeo financiero inconsistente | Alto: valores erróneos y pérdida de confianza | Diferencias entre plazos, totales no coherentes o pruebas frágiles | Dominio puro con `decimal`, casos conocidos, caso tasa cero y regla de redondeo única. |
| Ambigüedad entre modo Monto y Cuota | Alto: UI/API incoherente | DTO permite ambos valores o ningún valor | DTO discriminado, validación de exclusión mutua y pruebas por modo. |
| Capacidad máxima excede ingreso disponible | Alto: resultado no realista | Cuotas mayores a 40 % o monto resultante fuera de rango | Política centralizada, validación cliente/servidor y mensajes explícitos. |
| Catálogo de convenio no corresponde a actividad | Medio: flujo inválido | Convenio persiste al cambiar actividad | API filtra por actividad; formulario limpia, deshabilita y recarga; prueba de integración. |
| El mockup se copia con activos externos o identidad de terceros | Alto: incumplimiento de alcance | Dependencias a Unsplash, Google externos, logos/copy del referente | Implementar CSS/componentes propios y revisión final de activos/copy. |
| Lógica financiera duplicada en frontend | Alto: resultados divergentes | Fórmulas en componentes o tests con valores distintos | Frontend solo presenta respuesta API; backend es fuente de autoridad. |
| Dependencias no reproducibles | Medio: fallas de instalación/CI | Lockfile ignorado o scripts ausentes | Versionar lockfile, documentar SDKs y ejecutar instalación limpia en validación. |
| Errores API exponen detalles o rompen UI | Medio: riesgo de seguridad/UX | Stack traces, alertas genéricas o formulario bloqueado | Problem Details, mapeo de errores seguro, pruebas 400/500/reintento. |
| Ausencia de persistencia confundida con fallo | Medio: expectativas incorrectas | Acción final aparenta crear solicitud | Copy explícito, botón demostrativo y prueba de no persistencia. |
| Responsive y accesibilidad se atienden al final | Medio: retrabajo | Campos sin labels, foco invisible o desbordes | Componentes accesibles desde inicio, revisión por teclado y breakpoints antes de cierre. |

## 7. Criterios de aceptación

### S3-HU1

- El repositorio contiene un workspace Angular TypeScript estricto y una solución .NET ejecutables localmente mediante comandos documentados.
- Angular compila, tiene lint, typecheck, pruebas y build configurados; .NET restaura, compila y ejecuta pruebas.
- La API publica OpenAPI/Swagger solo en desarrollo, usa `/api/v1`, CORS de origen explícito y Problem Details para fallos controlados.
- Ningún secreto se versiona y el lockfile elegido se versiona.

### S3-HU2

- `GET /api/v1/catalogs/activities` devuelve Docente y Pensionado.
- `GET /api/v1/catalogs/activities/{activityId}/agreements` retorna exclusivamente convenios mock compatibles y maneja una actividad inexistente con error seguro.
- El convenio está deshabilitado hasta seleccionar actividad; al cambiarla se limpian convenio, datos dependientes y resultado previo.
- La UI comunica carga y error al consultar catálogos, permite reintento y conserva accesibilidad por teclado.

### S3-HU3

- En modo Monto, con datos válidos, `POST /api/v1/credit-simulations` devuelve cuota fija francesa, monto, tasa 18 % EA, tasa mensual, plazo, total, interés estimado y alternativas para 60/72/96/108/120 meses.
- El sistema rechaza monto menor de 1.000.000 o mayor de 100.000.000 COP, ingreso no positivo, descuentos negativos/superiores al ingreso, plazo no habilitado, convenio incompatible y cuota resultante superior al 40 % del ingreso disponible.
- La cuota y totales se calculan con precisión decimal, se redondean a COP una vez y pasan pruebas para un caso conocido y tasa cero.
- Para el mismo monto/tasa, un plazo mayor no incrementa la cuota mensual y no reduce el interés total estimado.

### S3-HU4

- En modo Cuota, la API acepta solo `maximumInstallmentCop`, valida que no supere 40 % del ingreso disponible y devuelve el monto máximo financiable por cada plazo.
- El request rechaza si contiene a la vez monto y cuota, si no contiene el valor requerido por su modo o si el resultado cae fuera del rango mock.
- Al cambiar de modo, la UI actualiza etiqueta y control (`Monto solicitado`/`Cuota máxima`), limpia el objetivo anterior, invalida resultado y no reutiliza datos obsoletos.
- Las alternativas muestran `Cuota máxima` en modo Monto y `Capacidad máxima` en modo Cuota de manera coherente con la respuesta de API.

### S3-HU5

- La interfaz muestra resultado informativo solo después de respuesta exitosa; durante la solicitud hay loading y ante error se presenta un mensaje accionable sin perder el formulario.
- Se puede seleccionar una alternativa y el resumen refleja plazo, monto, cuota, tasa, total e intereses de esa alternativa; no se recalcula localmente.
- Los avisos visibles indican carácter informativo, ausencia de aprobación, tasa mock referencial y exclusión de prima de seguro de vida.
- “Solicitar crédito” no guarda ni transmite solicitudes reales; su comportamiento demostrativo se comunica claramente.
- La UI respeta Cuota Clara y no incluye logos, nombre, textos propietarios o activos del referente; no reutiliza la imagen externa del artboard.

### S3-HU6

- Pasan pruebas unitarias del dominio para conversión EA, sistema francés, tasa cero, límites, ambos modos, todos los plazos y coherencia de alternativas.
- Pasan pruebas de integración para los tres endpoints, validación server-side, Problem Details y contratos OpenAPI.
- Pasan pruebas Angular para formulario, convenio dependiente, invalidación de resultado, loading/error y selección de alternativa.
- `npm run lint`, `npm run typecheck`, `npm test`, `npm run build`, `dotnet build` y `dotnet test` finalizan sin errores.
- Revisión manual confirma diseño responsive en escritorio/tablet/móvil, navegación completa por teclado, labels asociados, foco visible, contraste suficiente y mensajes no dependientes exclusivamente del color.

### Entregables y definición de terminado

- Código fuente Angular y .NET organizado por capas y funcionalidades, contratos OpenAPI, catálogos mock y políticas configurables.
- Suite de pruebas de dominio, API y frontend; documentación de ejecución local y limitaciones.
- Demo funcional de ambos modos de simulación contra API local, con datos sintéticos y sin persistencia.
- La entrega termina solo si se cumplen todos los criterios anteriores, los comandos de calidad pasan y se revisa la ausencia de identidad/activos de terceros.
