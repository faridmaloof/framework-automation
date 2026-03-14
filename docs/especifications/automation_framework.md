# Super requerimiento de framework de automatización empresarial .NET 10 con BDD moderno y paralelismo

## 1. Resumen ejecutivo

Este documento define el super requerimiento para un framework de automatización empresarial en .NET 10 orientado a pruebas E2E web, móvil, API, contratos y unitarias, soportando BDD moderno, paralelismo masivo y una arquitectura extensible y gobernable a 10 años.
Se apoya en tecnologías vigentes y activas en el ecosistema .NET: Playwright para UI web responsiva con soporte de ejecución paralela, Appium para móvil nativo, frameworks BDD como Reqnroll (sucesor abierto de SpecFlow) y compatibilidad opcional con Gauge, y la implementación Screenplay de Boa Constrictor para encapsular interacciones reutilizables.[1][2][3][4]

El enfoque prioriza legibilidad para negocio (BDD), mantenibilidad a nivel de código (Screenplay, POM desacoplado, i18n), configurabilidad centralizada (Options + appsettings.{env}.json), ejecución altamente paralela (Playwright, runners .NET, Gauge) y observabilidad avanzada (reporting enterprise, métricas y trazas), manteniendo una superficie clara para evolucionar herramientas sin reescribir las pruebas.[5][3][6]

## 2. Declaración de propósito

El propósito del framework es proporcionar una plataforma unificada de automatización de calidad que permita a equipos cross‑funcionales (QA manual y automatización, SDET, desarrollo, DevOps, producto, arquitectura y seguridad) validar el comportamiento de los productos de la organización de forma confiable, rápida y trazable.
Debe permitir escribir especificaciones ejecutables y suites automatizadas que evolucionen al ritmo del negocio, evitando que el framework se convierta en un cuello de botella o en deuda técnica.

En particular, el framework debe:
- Servir como "sistema operativo de pruebas" sobre el que se puedan enchufar motores (Playwright, Appium, APIs, colas, bases de datos) sin cambiar el modelo mental de los usuarios.
- Hacer trivial ejecutar las mismas pruebas en distintos entornos, idiomas, dispositivos y configuraciones, mediante parámetros y configuración, no mediante forks de código.

## 3. Propuesta de valor única

La propuesta de valor única del framework es combinar en un solo ecosistema:
- BDD moderno, compatible con Reqnroll (y opcionalmente Gauge) para exponer escenarios legibles a negocio y mantener requisitos vivos.[2][3][1]
- Un modelo Screenplay consolidado (Boa Constrictor + extensiones propias) que abstrae interacciones y permite reutilizar tareas, preguntas y habilidades en cualquier tipo de prueba (E2E, API, componentes, contratos).[7][8][4]
- Soporte de paralelismo nativo a varios niveles: runners .NET (xUnit, NUnit, MSTest), Playwright Test para .NET y el propio BDD/runner, reduciendo el tiempo total de ejecución de suites masivas.[9][6][5]
- Configuración, seguridad y observabilidad tratadas como primeras ciudadanas (Options, API Pools, EvidenceStrategy, reporting enterprise, métricas), aptas para auditorías de arquitectura y seguridad.

Esto permite que equipos de distintos niveles (junior, mid, senior, SDET) contribuyan sin necesidad de conocer todos los detalles de la infraestructura, concentrándose en el comportamiento del sistema bajo prueba.

## 4. Alcance del producto

### 4.1 Tipos de pruebas soportados

El framework debe soportar, como mínimo:
- Pruebas E2E web (desktop y móvil responsivo) sobre navegadores modernos utilizando Playwright para .NET, con auto‑waiting, contextos aislados y ejecución paralela configurable.[5][9]
- Pruebas E2E móviles nativas sobre Android/iOS mediante Appium, integradas como una habilidad más del modelo Screenplay.[10][11]
- Pruebas de API (REST/GraphQL, SOAP si aplica) utilizando clientes HTTP tipados y/o librerías como RestSharp, encapsulados como tareas y preguntas Screenplay.[12][4]
- Pruebas de contratos (p.ej. consumer‑driven contract tests) integrables como un tipo especializado de pruebas de API.
- Pruebas unitarias y de componentes del propio framework (Tools, Builders, Seeders, helpers, i18n), usando xUnit/NUnit/MSTest, con soporte de paralelismo y mocks para aislar unidades.[6]

### 4.2 Entornos y plataformas

- Múltiples entornos: Dev, QA, UAT/Preprod, Prod‑Like, con capacidad de añadir entornos adicionales sin recompilar.
- Drivers web: Chromium/Chrome, Firefox, WebKit (según soporte de Playwright) y navegadores en modo headless/headed.[5]
- Móvil: dispositivos físicos y emuladores, con integración futura posible con device farms.
- Ejecución local (developer workstation), on‑prem y cloud (agentes de CI, contenedores, Kubernetes, etc.).

### 4.3 Público objetivo interno

- Ingenieros de Automatización QA (junior, mid, senior).
- SDET e ingenieros de calidad con foco en diseño de frameworks y herramientas.
- Desarrolladores backend/frontend.
- Ingenieros DevOps / SRE.
- Arquitectos de software y seguridad.
- QA manual / analistas de negocio que leen y, en algunos casos, escriben escenarios BDD.

## 5. Visión estratégica a gran escala (10 años)

### 5.1 Horizonte tecnológico

Aunque .NET 10 es la base inicial, el framework debe diseñarse para migrar con un esfuerzo acotado a futuras LTS (.NET 12, .NET 14), evitando el uso de APIs obsoletas o específicas de versión y encapsulando todo acceso a runtime dentro de capas propias.

Playwright y Appium se consideran apuestas de medio plazo en automatización web y móvil, con estrategias de evolución claras y comunidades activas. Reqnroll parte del código abierto de SpecFlow y ha sido creado precisamente para continuar el soporte BDD en .NET después del fin de vida de SpecFlow, lo que lo hace una elección racional para un roadmap a largo plazo.[13][11][1][2][10][5]

Boa Constrictor, como implementación Screenplay en .NET, sigue activo, ha separado sus paquetes por conjunto de interacciones (Selenium, RestSharp, core Screenplay) y recientemente añadió un paquete Playwright con habilidades e interacciones, además de actualizar sus propios tests internos a .NET 8.[4][7][12]

### 5.2 Principios de diseño a largo plazo

- **Herramienta‑agnóstico por capa**: BDD runner, motor web, motor móvil, reporter, data seeding y i18n deben estar detrás de interfaces internas para permitir reemplazos futuros.
- **Configuración como contrato**: toda configuración crítica se modela en clases tipadas (Options) y se valida al arranque, permitiendo versionado y migraciones de esquema sin reescribir código de pruebas.[14][15]
- **Observabilidad y métricas**: el framework emite métricas de ejecución (tiempos, flakiness, retries, consumo de evidencias) desde el día 1 para poder tomar decisiones de optimización y capacidad.

## 6. Pilares arquitectónicos

### 6.1 BDD y especificaciones ejecutables

- BDD estándar Gherkin usando Reqnroll, con soporte completo para features, scenarios, hooks, tags y data tables.[1][2]
- Compatibilidad o exploración futura con Gauge para equipos que requieran especificaciones en Markdown y un plugin system muy modular (C#, JavaScript, Java, etc.), manteniendo la lógica de automación en C#.[16][3]
- Guía de estilo interna para escenarios Gherkin (lenguaje de negocio, evitar detalles técnicos, reutilización de steps) y/o especificaciones Gauge.

### 6.2 Screenplay Pattern

- Adopción de Screenplay como modelo central: Actores, Habilidades (Abilities), Tareas/Interacciones (Tasks/Interactions), Preguntas (Questions) y eventualmente Objetos de Dominio.[8][7]
- Uso de Boa Constrictor como base Screenplay, añadiendo interacciones propias para Playwright .NET, Appium y capa API donde no existan aún.
- Actores dinámicos y semánticos ("El Administrador", "El Cliente", "El SRE") para enriquecer logs y reportes.

### 6.3 Motores de ejecución (web, móvil, API)

- Web: Playwright .NET como motor principal para UI, con soporte para:
  - Auto‑waiting.
  - Contextos aislados por test.
  - Emulación de dispositivos/resoluciones.
  - Ejecución paralela a nivel de archivo y/o de test según configuración.[9][5]
- Móvil nativo: Appium como ability en Screenplay, reutilizable entre actores.
- API: clientes HTTP tipados (HttpClient, RestSharp u otros) encapsulados en Tasks/Questions Screenplay.[12][4]

### 6.4 Configuración centralizada (Options + appsettings)

- Uso sistemático de `appsettings.json` y `appsettings.{Environment}.json` para almacenar:
  - FrameworkSettings (nombre del framework, culture, estrategias globales de timeout/retry, EvidenceStrategy, flags de paralelismo por tipo de prueba).
  - DeviceSettings (browser, resolución, device profiles, target: WebDesktop, MobileWebEmulation, AndroidNative, iOSNative, etc.).
  - ApiPools (URLs base clasificadas por entorno y dominio funcional).
  - Integraciones (Allure, ReportPortal, otros reportes, logging, métricas).
- Binding fuerte de estos archivos a clases de Options (`IOptions<T>`, `IOptionsSnapshot<T>`) y validación en arranque (`ValidateOnStart`) con data annotations para fallar rápido en caso de configuraciones inválidas.[15][14]

### 6.5 Data builders y seeders

- Uso de Bogus u otra librería de generación de datos para crear Builders por entidad de dominio (UserBuilder, InvoiceBuilder, etc.).
- Separación clara entre:
  - Builders: definen reglas de generación de instancias (Faker<T>) sin efectos de red.
  - Seeders: usan Builders y ApiPools para realizar llamadas HTTP async y preparar estado en sistemas downstream.
- Trazabilidad de datos seeded (IDs clave) hacia los reportes para análisis forense.

### 6.6 Reporting enterprise y evidencias

- Integración con al menos una plataforma de reporting enterprise (Allure TestOps, ReportPortal), aprovechando su capacidad de dashboards, histórico, agrupación de fallos, adjuntos de evidencias y analítica.[17][18][19]
- Definición declarativa de estrategia de evidencias en configuración (EvidenceStrategy): screenshots antes/después de cada step, vídeos on‑failure, retención diferencial por entorno.[18][17]
- Interfaz interna de reporter (ITestReporter) para desacoplar escenarios y steps de una herramienta concreta.

### 6.7 Paralelismo y escalabilidad

- Uso de las capacidades de paralelismo de los test runners .NET (xUnit, NUnit, MSTest) y Playwright:
  - NUnit/xUnit/MSTest pueden ejecutar clases/colecciones en paralelo por defecto.[6]
  - Playwright Test para .NET soporta workers paralelos, fullyParallel por proyecto y paralelismo configurable por archivo/spec.[9][5]
- BDD runner (Reqnroll) configurado para permitir ejecución en paralelo de escenarios y features, gestionando aislamiento de datos y entornos.[2][1]
- Exploración de capacidades de Gauge para ejecución paralela y multi‑proceso cuando se utilice como BDD alternativo.[3][20][21]

## 7. Casos de uso representativos

### 7.1 QA Automation (junior/mid/senior)

- Crear un nuevo escenario BDD para validar un flujo de alta de cliente en web, reutilizando tareas Screenplay existentes ("Autenticar", "Navegar a módulo de clientes", "Crear cliente básico").
- Ejecutar el mismo escenario en distintos idiomas (es‑ES, en‑US, pt‑BR) sin cambiar el código del escenario, solo cambiando la `Culture` en appsettings.
- Lanzar el set de regresión crítico en paralelo contra 4 workers para reducir el tiempo de 40 a 10 minutos.

### 7.2 SDET / desarrollador de framework

- Crear un nuevo Builder y Seeder para una entidad "Tarifa" y exponerlo como utility accesible desde steps BDD.
- Añadir un nuevo motor de autenticación API (p.ej. OAuth) sin tocar escenarios, solo extendiendo Core.Tools y configuración.
- Diseñar una nueva habilidad Screenplay para interactuar con un microservicio de colas (Kafka, RabbitMQ) como parte de la preparación de datos.

### 7.3 Desarrolladores backend/frontend

- Ejecutar suites de smoke E2E desde su IDE apuntando a entornos de desarrollo locales.
- Revisar reportes BDD y logs Screenplay para investigar un fallo de regresión.
- Añadir aserciones contractuales en pruebas de API o contract tests para asegurar compatibilidad entre servicios.

### 7.4 DevOps / SRE

- Configurar pipelines declarativos (Jenkins, Azure DevOps, GitHub Actions) que levanten el framework, inyecten configuración vía parámetros seguros y ejecuten suites en paralelo con sharding.
- Publicar artefactos de ejecución (traces, logs, evidencias) como parte de los jobs de CI.
- Monitorear métricas de tiempo total de suite, flakiness y consumo de recursos.

### 7.5 Arquitectura y seguridad

- Auditar cómo se manejan secretos y endpoints sensibles (API Pools, tokens) y validar que no se exponen en código ni en repositorios.
- Verificar que todos los accesos a sistemas internos pasan por capas controladas y observables (Tools, Core.Tools).
- Asegurar que el framework puede seguir funcionando tras migraciones de infraestructura (por ejemplo, cambio de gateway de APIs, modificación de dominios, cambios en política de certificados).

### 7.6 QA manual / analistas de negocio

- Redactar o revisar escenarios BDD legibles que describan reglas de negocio clave.
- Consultar reportes para verificar que los criterios de aceptación de un feature se han cubierto en automatización.
- Lanzar subconjuntos de pruebas (por tags o suites) desde una interfaz amigable o pipelines manuales.

## 8. Información específica por área

### 8.1 Para Producto / Gestión de producto

- El framework debe ofrecer trazabilidad clara de requisitos → escenarios BDD → ejecuciones → resultados.
- Debe permitir filtrar reportes por feature, release, tag, entorno y responsable.
- Debe integrarse con herramientas de gestión de backlog (Jira, Azure Boards) mediante tags o IDs de historias/épicas.

### 8.2 Para desarrolladores de framework y SDET

- Definir estándares de codificación (naming, estructura de carpetas, convenciones de Screenplay, POM y Builders).
- Mantener una librería central de Tasks/Interactions reusables por dominio.
- Definir plantillas de proyectos y snippets para crear rápidamente nuevas features, steps y tests unitarios.

### 8.3 Para Arquitectura

- El framework debe alinearse con los lineamientos de arquitectura corporativa (stack .NET, mecanismos de configuración, logging, seguridad).
- Debe permitir despliegues como parte del ecosistema de CI/CD existente (agentes, contenedores, orquestadores).
- Debe publicar métricas y logs de manera compatible con la plataforma de observabilidad existente.

### 8.4 Para Seguridad

- Ningún secreto (tokens, passwords, client secrets) debe residir en appsettings sin protección: se deben usar providers seguros (KeyVault, secretos de CI, etc.).[22][14]
- El framework debe minimizar datos sensibles en logs y evidencias (enmascarar PII, tarjetas, etc.).
- Los accesos a APIs internas deben atravesar mecanismos de autenticación/autorización corporativos, no bypasses.

## 9. Requisitos no funcionales clave

### 9.1 Mantenibilidad y extensibilidad

- Código modular y limpio, siguiendo principios SOLID.
- Capas claramente separadas: BDD, Screenplay, Drivers, Tools, Configuración, Reporting.
- Interfaces públicas estables y versionadas para extensiones (por ejemplo, nuevos motores de UI, nuevas plataformas de reporte).

### 9.2 Rendimiento y paralelismo

- Capacidad de ejecutar suites de cientos o miles de tests en paralelo, utilizando workers configurables (por ejemplo, `n` workers Playwright, `m` agentes de CI).[6][5][9]
- Tiempo objetivo de smoke suite < X minutos y de regresión completa dentro de ventanas de ejecución aceptables para releases.
- Optimización del uso de recursos (CPU, memoria, almacenamiento de evidencias) mediante configuración (EvidenceStrategy, tamaño de pools, etc.).[17][18]

### 9.3 Confiabilidad y flakiness

- Estrategias de retries configurables por tipo de prueba y tipo de fallo (infraestructura vs. lógica).
- Auto‑waiting y aserciones robustas para reducir flakiness (Playwright ayuda con waits implícitos y contextos aislados).[5]
- Análisis periódico de causas raíz de fallos intermitentes y acciones de hardening.

### 9.4 Observabilidad

- Logs estructurados con correlación por test, actor, entorno, idioma y datos seeded.
- Métricas estándares: tiempo por escenario, tasa de fallos, flakes, cantidad de retries, volumen de evidencias.
- Integración con sistemas de logging/metrics existentes (Elastic, Prometheus, etc.).

## 10. Consideraciones de implementación inicial (no exhaustivas)

Aunque este documento no baja a épicas/historias, sí marca algunos hitos técnicos iniciales:

- Implementar el núcleo del framework en .NET 10 con DI y Options para configuración.[14][15]
- Integrar Playwright .NET con un primer set de páginas/modelos y tareas Screenplay.
- Integrar Reqnroll como BDD, con hooks iniciales para configuración, creación de actores y validación de entorno.[1][2]
- Crear Core.Tools con clientes HTTP, Builders/Seeders base y utilidades de configuración/seguridad.
- Integrar un primer reporter enterprise (Allure o ReportPortal) a través de una interfaz interna de reporter.[19][18][17]
- Habilitar ejecución en paralelo de un conjunto controlado de pruebas para validar la estrategia de aislamiento de datos y entornos.[9][6][5]

Con estos elementos, el framework puede evolucionar hacia un producto interno estratégico que soporte el crecimiento de la organización en los próximos años.


adicionales

Sí, tiene todo el sentido integrar el framework con Grafana/Prometheus (u otra stack de observabilidad) mediante métricas asíncronas, y encaja bien con el diseño que ya tienes. Se puede y se debe considerar como un pilar adicional de “observabilidad ligera” complementaria a Allure/ReportPortal, sin impactar tiempos de ejecución ni consumo de recursos. [youtube](https://www.youtube.com/watch?v=WCPAgxp2KQY)

Te propongo cómo incorporarlo al super‑requerimiento (puedes añadirlo como sección nueva o extender la de observabilidad/telemetría):

## 1. Nuevo objetivo específico

“El framework debe exponer métricas técnicas y de negocio de las ejecuciones de prueba en un formato compatible con Prometheus/Grafana (u otra plataforma equivalente), priorizando bajo overhead y recolección asíncrona, para disponer de dashboards en tiempo (casi) real con estadísticas de estabilidad, volumen y performance de las suites, sin competir con el reporter funcional detallado (Allure/ReportPortal).” [dev](https://dev.to/cyberscoper/building-enterprise-level-monitoring-from-prometheus-to-grafana-dashboards-3pge)

## 2. Diseño de alto nivel de métricas

### 2.1 Qué métricas exponer

Mínimo:

- Conteos: número de tests ejecutados, pasados, fallados, skippeados, retries.
- Duraciones: histograma de duración de tests (p50, p90, p95), duración de suites, tiempo medio por tipo de test.
- Flakiness: tasa de retries, ratio de fallos intermitentes por suite/tag.
- Recursos opcionales: tiempo medio de seeding, tiempo medio de step “lento”, etc.

Todo esto como métricas técnicas, no como evidencias detalladas (que siguen yendo a Allure/ReportPortal). [grafana](https://grafana.com/grafana/dashboards/19268-prometheus/)

### 2.2 Cómo exponerlas

Dos opciones (no excluyentes):

1. **Export directo desde .NET con OpenTelemetry + Prometheus**  
   - El framework (o un pequeño host .NET companion) instrumenta métricas vía OpenTelemetry .NET Metrics y expone `/metrics` en formato Prometheus o exporta vía OTLP a un collector. [opentelemetry](https://opentelemetry.io/docs/languages/dotnet/metrics/getting-started-prometheus-grafana/)
   - Prometheus scrapea ese endpoint y Grafana se conecta a Prometheus para visualizar dashboards. [prometheus](https://prometheus.io/docs/visualization/grafana/?stream=top)

2. **Vía cola + worker externo (como comentas, en Go u otro lenguaje)**  
   - Los tests publican eventos/metricas agregadas en una cola (Kafka, RabbitMQ, Redis Streams, etc.) de forma rápida y fire‑and‑forget.  
   - Un **worker service** (puede ser Go, .NET, Python, lo que mejor encaje) consume estos eventos y:
     - Normaliza y agrega datos.
     - Expone su propio `/metrics` Prometheus.
     - O escribe directamente en una TSDB/Elastic, según la plataforma elegida. [last9](https://last9.io/blog/what-is-asynchronous-job-monitoring/)
   - Este patrón desacopla completamente la ejecución de pruebas del almacenamiento de métricas y es ideal para que no afecte tiempos ni recursos de los jobs de CI.

Dentro del requerimiento, puedes describirlo explícitamente como:

> “El framework no realizará escrituras síncronas a bases de datos de métricas durante la ejecución de tests. Toda métrica se emitirá de forma asíncrona (eventos o export OTLP) para que la persistencia y agregación la realice un componente externo (Prometheus/worker service), garantizando mínimo impacto en tiempos de respuesta y consumo de recursos de los jobs de testing.”

## 3. Requisitos concretos a añadir

Te dejo bullets que puedes pegar en el documento:

### 3.1 Para Observabilidad / DevOps

- Exponer métricas en formato compatible con Prometheus (ya sea directamente desde el host .NET o desde un worker externo), siguiendo buenas prácticas de cardinalidad y agregación. [grafana](https://grafana.com/grafana/dashboards/19341-prometheus-metrics-management/)
- Proveer dashboards de referencia en Grafana que muestren:
  - Número de ejecuciones por pipeline/branch.
  - Tiempos medios y percentiles por suite/escenario/tag.
  - Tasa de fallos y flakiness histórica.
  - Volumen de evidencias generadas (si se quiere monitorizar almacenamiento). [grafana](https://grafana.com/docs/grafana/latest/fundamentals/getting-started/first-dashboards/get-started-grafana-prometheus/)
- Hacer opcional la activación de estas métricas via configuración (`Metrics.Enabled`, `Metrics.Mode = Direct | Queue | Off`), para no obligar a todos los entornos a tener la misma infraestructura.

### 3.2 Para Arquitectura

- Definir un **contrato interno de eventos de métricas** (ej. `TestRunStarted`, `TestRunFinished`, `TestCaseResult`, `RetryOccurred`) que se emitan desde el framework.
- Asegurar que estos eventos no contienen PII ni datos sensibles; solo IDs técnicos, etiquetas y tiempos.
- Documentar que la integración con Grafana/Prometheus u otras herramientas se hace vía este contrato, nunca acoplando el core a una plataforma concreta. [opentelemetry](https://opentelemetry.io/docs/languages/dotnet/metrics/)

### 3.3 Para Seguridad

- Validar que los endpoints de métricas no exponen datos sensibles ni secretos (son solo contadores y tiempos).
- Si se usa worker externo, situarlo en la misma zona de confianza que el sistema de observabilidad y restringir acceso según políticas corporativas.

***

Con esto cubres justo lo que faltaba: un **canal ligero de métricas en tiempo real**, orientado a Grafana u otra herramienta de observabilidad, asíncrono y desacoplado, que convive con el reporting funcional rico de Allure/ReportPortal sin interferir en tiempos de ejecución.
