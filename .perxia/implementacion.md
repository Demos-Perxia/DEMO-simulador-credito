# Implementación

- S3-HU1 a S3-HU4: creada la API .NET 8 en `simulador-libranza-api/`, con capas Domain, Application, Infrastructure y API.
- Incluye catálogos sintéticos, política configurable de 18 % EA, cálculo francés decimal, endpoints v1, Problem Details, CORS local y contrato OpenAPI de desarrollo.
- S3-HU2 a S3-HU5: creado `simulador-libranza-web/` con Angular 20, TypeScript estricto, standalone components, Reactive Forms y `HttpClient` contra los endpoints v1.
- El frontend implementa actividad/convenio dependiente, modos Monto y Cuota, validación cliente, carga/error, invalidación de resultado, selección de alternativas y desglose de la respuesta sin cálculos financieros locales.
- El cliente API usa URL de ambiente y caché de actividades; se añadieron pruebas Angular para dependencia del convenio, cambio de modo, simulación, selección de alternativa y error Problem Details.
- La interfaz aplica fundamentos Cuota Clara con recursos propios: no carga fuentes, imágenes, logos ni activos de terceros; incluye foco visible y diseño responsive.
- Verificado: `npm run lint`, `npm run typecheck` y `npm run build` finalizan correctamente.
- Se corrigió el enlace de la configuración de política para evitar duplicados al cargar plazos; la API entrega 60, 72, 96, 108 y 120 meses una sola vez.
- La interfaz muestra desde el inicio los cinco plazos con valores en $0; al calcular, los reemplaza por las alternativas retornadas por la API.
- Los campos monetarios del formulario normalizan la entrada a dígitos y muestran separadores de miles colombianos, sin alterar los valores numéricos enviados a la API.
- Se alineó la interfaz Angular con el artboard `design/sistema-simulador-credito/artboards/Simulador.html`: tipografías Manrope/DM Mono, escala, espaciado, tarjetas, encabezado, campos con iconografía Material, rejilla de plazos y superficie visual.
- Las tarjetas de formulario y resultado usan columnas iguales y altura uniforme en escritorio; sus acciones principales quedan alineadas al borde inferior.
- En modo Cuota, las tarjetas muestran el monto financiable calculado para cada plazo; a cuota máxima fija, el monto aumenta al aumentar los meses. Ambos modos conservan la composición original en rejilla, con valores compactos y alineados hacia la izquierda para evitar desbordes.
- Se añadió una prueba de dominio que asegura que el monto financiable crece estrictamente con el plazo y que la cuota se mantiene fija en modo Cuota.
- HU-01 implementada en la SPA: `styles.css` define los tokens negro/rojo/verde y foco visible; `app.html` comunica estados, dependencia del convenio, nombres accesibles para campos monetarios y selección accesible; `app.css` implementa superficies, cards verdes con texto negro y responsive 320px; `app.spec.ts` protege la ayuda del convenio, etiqueta de modo y `aria-pressed` de alternativas. No se modificaron `app.ts`, contratos HTTP ni backend.
- Pendiente: la ejecución de `npm test` requiere un binario ChromeHeadless y la regresión backend requiere SDK `dotnet` disponible; falta recorrido E2E visual completo.
