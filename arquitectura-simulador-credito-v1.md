# Arquitectura de solución — Simulador Cuota Clara

## Resumen ejecutivo

Se mantiene un monolito modular compuesto por una SPA Angular y una API ASP.NET Core por capas. La HU-01 aplica exclusivamente el Design System al frontend; no modifica cálculos, contratos HTTP, persistencia, integraciones ni decisiones de crédito. Es la opción de menor riesgo para conservar el comportamiento validado mientras se entrega el nuevo look and feel.

## Alcance

Incluye el renderizado responsive y accesible de la experiencia, los estados de formulario y la integración existente con catálogos y simulación. Excluye autenticación, persistencia, aprobación crediticia, servicios externos y cambios de reglas.

## Requerimientos y escenarios de calidad

| Atributo ISO/IEC 25010 | Escenario medible |
|---|---|
| Adecuación funcional | Ante los mismos datos de entrada válidos, la API devuelve el mismo contrato, cálculo y validaciones que la versión base. |
| Capacidad de interacción | Ante navegación por teclado en desktop o móvil, cada control interactivo recibe foco visible, conserva etiqueta y se opera sin mouse. |
| Compatibilidad | Ante una respuesta HTTP 200 o `application/problem+json` existente, el SPA interpreta el contrato vigente sin adaptadores nuevos. |
| Mantenibilidad | Ante cambios visuales, estos permanecen en el contenedor Angular y no introducen dependencias desde Domain hacia API, Infrastructure o frontend. |
| Eficiencia de desempeño | Ante carga inicial normal, el rediseño no añade llamadas HTTP ni bloquea la interacción inicial. El umbral cuantitativo queda pendiente de negocio/QA. |
| Seguridad | Ante entradas inválidas, el servidor conserva validación y responde Problem Details sin exponer detalles internos. |

## Restricciones y supuestos

- Stack vigente: Angular 20, TypeScript, RxJS; ASP.NET Core Web API/.NET 10 y C#.
- El frontend conserva REST/HTTPS JSON bajo `/api/v1`.
- Catálogos y políticas están en infraestructura/configuración; no existe base de datos dentro del alcance actual.
- El Design System es la fuente única: negro `#080808`, rojo `#FF334D`, verde `#62F58C`; cards verdes con texto negro.
- La HU-01 no altera DTOs, endpoints, reglas, resultados ni mensajes funcionales.

## Vistas

- Contexto: `arquitectura-contexto-c4-v1.drawio`.
- Contenedores: `arquitectura-contenedores-c4-v1.drawio`.

## Contratos por capa

| Capa | Expone | No puede conocer |
|---|---|---|
| Domain | Reglas de simulación, `SimulationEngine`, modelos de política y validación | HTTP, DI, configuración, repositorios, Angular |
| Application | Casos de uso, puertos `ICatalogRepository` e `ICreditPolicyProvider` | Detalle de Minimal APIs, configuración concreta, UI |
| Infrastructure | Adaptadores de catálogos y política configurada | Componentes Angular y DTOs HTTP |
| API | Endpoints `/api/v1`, mapeo DTO, CORS y Problem Details | Reglas duplicadas de cálculo o acceso directo a UI |
| SPA | Formulario, estados y presentación; `CreditApiService` | Cálculo financiero y reglas server-side |

## Decisiones y trade-offs

### ADR-001: Mantener monolito modular por capas

- **Estado**: aceptado
- **Fecha**: 2026-09-04
- **Decisores**: Equipo Cuota Clara
- **Etiquetas**: arquitectura, mantenibilidad, simplicidad

#### Contexto

El sistema tiene una SPA y una API con dominio, aplicación, infraestructura y API. La HU-01 es un cambio visual y no demanda escalado independiente, persistencia distribuida ni autonomía por dominio.

#### Decisión

Usaremos el monolito modular vigente con dependencias hacia el dominio y contratos HTTP versionados existentes.

#### Alternativas consideradas

- Microservicios: permitirían despliegue independiente, pero introducen red, observabilidad distribuida y sobrecosto sin beneficio para el alcance.
- Mover la lógica al cliente: reduciría una llamada, pero debilita integridad y rompe la fuente única de reglas.

#### Consecuencias

- Positivas: menor riesgo de regresión y entrega rápida del rediseño.
- Negativas: API y dominio se despliegan juntos; se acepta mientras no haya necesidades de escalado o independencia demostrables.

#### Verificación

`dotnet test` conserva la cobertura de reglas de dominio y las pruebas Angular verifican que el mismo request llega a los mismos endpoints.

### ADR-002: El look and feel se implementa exclusivamente en la SPA

- **Estado**: aceptado
- **Fecha**: 2026-09-04
- **Decisores**: Equipo Cuota Clara
- **Etiquetas**: frontend, regresión, experiencia

#### Contexto

HU-01 exige una nueva presentación sin cambiar cálculos, endpoints, DTOs o datos.

#### Decisión

Los tokens, estilos, layout, responsive y estados visuales se implementarán en Angular; la API queda sin cambios funcionales.

#### Alternativas consideradas

- Modificar contrato para transportar metadatos visuales: acoplaría presentación y backend sin necesidad.
- Duplicar las reglas en Angular: crea riesgo de divergencia con el cálculo del dominio.

#### Consecuencias

- Positivas: aislamiento del cambio, menor superficie de regresión y contratos estables.
- Negativas: debe validarse que estilos no alteren accesibilidad ni flujos de formularios.

#### Verificación

Comparar requests/responses antes y después; ejecutar pruebas responsive, navegación por teclado y regresión de simulación.

## Riesgos y mitigación

| Riesgo | Mitigación |
|---|---|
| Regresión funcional al rediseñar formularios | Pruebas de componente y regresión sobre los tres endpoints existentes. |
| Contraste o foco insuficiente | Revisión visual desktop/móvil, prueba de teclado y validación AA. |
| Configuración de CORS restrictiva fuera de local | Definir explícitamente orígenes por ambiente y comprobar despliegue. |
| Catálogos en memoria limitan evolución | Mantener puertos; sustituir el adaptador de Infrastructure sin afectar Application/Domain cuando exista fuente real. |

## Fitness functions

1. `dotnet test` debe pasar para el dominio y pruebas de integración.
2. `npm run lint`, `npm run typecheck` y `npm run build` deben pasar.
3. Las pruebas de frontend deben verificar `GET /catalogs/activities`, `GET /catalogs/activities/{activityId}/agreements` y `POST /credit-simulations` sin cambios contractuales.
4. Validación visual en desktop y móvil: sin scroll horizontal involuntario, texto negro sobre cards verdes y foco visible.

## Plan de evolución

Si aparecen catálogos reales o auditoría regulatoria, implementar adaptadores nuevos detrás de los puertos existentes y añadir persistencia/auditoría sin contaminar Domain. Si el tráfico exige escalamiento, evaluar separar el catálogo primero; no dividir el motor de simulación antes de tener métricas y una frontera de dominio estable.
