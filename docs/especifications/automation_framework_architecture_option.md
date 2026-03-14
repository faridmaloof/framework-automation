A continuación tienes **un informe completo, estructurado y profesional** del framework de automatización empresarial, consolidando todo lo que definiste durante la conversación:

* requerimientos
* arquitectura
* stack tecnológico
* reporting y evidencias
* seguridad
* paralelismo
* organización de repositorios
* gobernanza para múltiples equipos
* flujo de ejecución

El objetivo es que **pueda usarse como documento de arquitectura técnica o blueprint del framework**.

---

# Framework Empresarial de Automatización

## Plataforma de Testing Automatizado basada en .NET 10

---

# 1. Resumen ejecutivo

Este documento define la arquitectura, requerimientos y stack tecnológico de un **framework empresarial de automatización basado en .NET 10**, diseñado para soportar pruebas automatizadas de distintos niveles dentro de una organización.

El framework tiene como objetivo convertirse en una **plataforma unificada de automatización de calidad**, permitiendo ejecutar pruebas:

* End-to-End Web
* Mobile nativo
* APIs
* Contract testing
* pruebas unitarias del framework
* pruebas de componentes

El framework se basa en tecnologías modernas del ecosistema .NET y en patrones de diseño que priorizan:

* escalabilidad
* mantenibilidad
* paralelismo
* observabilidad
* extensibilidad
* gobernanza entre múltiples equipos

El diseño está pensado para soportar **evolución tecnológica durante al menos 10 años**, permitiendo reemplazar herramientas sin reescribir las pruebas.

---

# 2. Objetivos del framework

El framework debe proporcionar una plataforma que permita:

1. Definir **especificaciones ejecutables legibles para negocio** mediante BDD.
2. Ejecutar suites de pruebas de manera **altamente paralela**.
3. Reutilizar lógica de automatización mediante **Screenplay Pattern**.
4. Integrar distintos motores de pruebas (web, móvil, APIs).
5. Generar **reportes detallados con evidencias**.
6. Integrarse con pipelines CI/CD empresariales.
7. Emitir **métricas técnicas y observabilidad en tiempo real**.
8. Permitir colaboración de múltiples equipos sobre una plataforma común.

---

# 3. Principios arquitectónicos

El framework se diseñará siguiendo los siguientes principios.

---

## Desacoplamiento de herramientas

Las pruebas no deben depender directamente de herramientas específicas.

Por ejemplo:

```text
ITestReporter
   ├─ AllureReporter
   └─ ReportPortalReporter
```

Esto permite reemplazar la plataforma de reportes sin modificar las pruebas.

---

## Arquitectura por capas

El framework se organiza en capas funcionales separadas.

```
BDD Layer
Execution Layer
Automation Core
Drivers Layer
Infrastructure Layer
Observability Layer
```

Cada capa tiene responsabilidades claras.

---

## Configuración centralizada

Toda la configuración se gestiona mediante:

```
appsettings.json
appsettings.{environment}.json
```

con binding fuerte mediante:

```
IOptions<T>
```

---

## Observabilidad desde el diseño

El framework emitirá:

* logs estructurados
* métricas técnicas
* eventos de ejecución
* evidencias

---

# 4. Stack tecnológico

## Runtime

```
.NET 10
C# 13
```

---

## BDD

Framework BDD principal:

Reqnroll

Características:

* Gherkin
* hooks avanzados
* paralelismo
* integración con runners .NET

---

## Test runner

Se utilizará:

```
xUnit v3
```

Ventajas:

* paralelismo avanzado
* lifecycle moderno
* soporte async robusto

---

## Automatización Web

Motor principal:

Playwright

Capacidades utilizadas:

* auto waiting
* context isolation
* screenshots
* video recording
* tracing
* paralelismo

---

## Automatización móvil

Motor móvil:

Appium

---

## Automatización de APIs

Se utilizarán:

* HttpClient
* RestSharp

---

## Observabilidad

Stack de observabilidad:

* OpenTelemetry
* Prometheus
* Grafana

---

## Reporting

Plataformas soportadas:

### Opción principal

Allure Report

### Alternativa

ReportPortal

---

## CI/CD

Plataforma objetivo:

Azure DevOps

---

## Gestión de secretos

Desarrollo local:

```
dotnet user-secrets
```

CI/CD:

* Azure DevOps secret variables
* Azure Key Vault

---

# 5. Arquitectura del repositorio

Para soportar múltiples equipos se definen tres niveles.

```
automation-platform
│
├─ framework-core
│
├─ shared-modules
│
└─ automation-project
```

---

# 6. framework-core

Contiene la infraestructura del framework.

Los equipos no deben modificarlo frecuentemente.

```
framework-core
│
├─ BDD
│   ├─ Hooks
│   ├─ Context
│   └─ Execution
│
├─ Screenplay
│   ├─ Actors
│   ├─ Abilities
│   ├─ Tasks
│   ├─ Interactions
│   └─ Questions
│
├─ Engines
│   ├─ Web
│   ├─ Mobile
│   └─ Api
│
├─ Infrastructure
│   ├─ Configuration
│   ├─ Retry
│   ├─ Security
│   └─ Logging
│
├─ Data
│   ├─ Builders
│   ├─ Provisioning
│   └─ Contracts
│
├─ Observability
│   ├─ Metrics
│   ├─ Tracing
│   └─ Reporting
│
└─ Utilities
```

---

# 7. shared-modules

Contiene herramientas reutilizables entre equipos.

```
shared-modules
│
├─ ApiClients
├─ DomainBuilders
├─ DomainProvisioning
├─ Authentication
├─ MessagingTools
└─ UiComponents
```

Si algo se vuelve infraestructura general puede promoverse al core.

---

# 8. automation-project

Contiene las pruebas del sistema bajo prueba.

```
automation-project
│
├─ Features
│
├─ Steps
│
├─ Tasks
│
├─ Questions
│
├─ UI
│   ├─ Pages
│   │   ├─ Locators
│   │   └─ Actions
│
├─ Api
│
├─ TestData
│
└─ Config
```

---

# 9. Implementación de Screenplay

El framework implementará un **Screenplay Pattern propio**.

Componentes:

```
Actor
Ability
Task
Interaction
Question
```

---

## Actor

Representa un usuario del sistema.

Ejemplo:

```
Administrador
Cliente
Operador
```

---

## Abilities

Capacidades del actor.

Ejemplo:

```
BrowseWeb
CallApi
UseMobileDevice
```

---

## Tasks

Acciones de negocio.

Ejemplo:

```
CreateCustomer
AuthenticateUser
SubmitOrder
```

---

## Interactions

Acciones técnicas.

Ejemplo:

```
Click
EnterText
SendRequest
```

---

## Questions

Permiten consultar estado del sistema.

Ejemplo:

```
DisplayedMessage
OrderStatus
```

---

# 10. Page Object Model segmentado

Las páginas se dividen en dos partes.

```
LoginPage
 ├─ LoginLocators
 └─ LoginActions
```

---

## Locators

Solo contiene selectores.

---

## Actions

Contiene comportamiento.

Ventajas:

* evita clases gigantes
* evita herencia profunda
* mejora mantenibilidad

---

# 11. Estrategia de evidencias

Las evidencias generadas incluyen:

* capturas de pantalla
* videos
* logs
* traces

Configuración ejemplo:

```json
{
  "Evidence": {
    "Screenshots": "OnFailure",
    "StepScreenshots": "BeforeAndAfter",
    "Video": "OnFailure",
    "Trace": true
  }
}
```

---

## Estrategias de captura

Screenshots:

```
BeforeStep
AfterStep
BeforeAndAfter
OnFailure
Disabled
```

Video:

```
Always
OnFailure
Disabled
```

---

# 12. Estructura de artefactos

```
test-results
│
├─ runs
│   ├─ timestamp
│   │   ├─ screenshots
│   │   ├─ videos
│   │   ├─ traces
│   │   ├─ logs
│   │   └─ allure-results
│
└─ latest
```

---

# 13. Flujo de reporting

```
Test Execution
     ↓
Evidence Manager
     ↓
Reporter Adapter
     ↓
Allure / ReportPortal
```

Cada step puede contener:

* screenshot
* video
* logs
* trace

---

# 14. Flujo completo de ejecución

```
Feature (Gherkin)
     ↓
Reqnroll
     ↓
Step Definition
     ↓
Screenplay Actor
     ↓
Task
     ↓
Interaction
     ↓
Driver (Playwright / API / Mobile)
     ↓
Evidence Manager
     ↓
Reporter Adapter
     ↓
Allure / ReportPortal
```

---

# 15. Paralelismo

Se implementa paralelismo en tres niveles.

Nivel 1:

runner

```
xUnit parallel collections
```

Nivel 2:

BDD

```
parallel features
parallel scenarios
```

Nivel 3:

driver

Playwright workers.

---

# 16. Métricas y observabilidad

Arquitectura de métricas:

```
Test Execution
     ↓
MetricsEmitter
     ↓
OpenTelemetry
     ↓
Prometheus
     ↓
Grafana
```

Métricas principales:

```
tests_total
tests_passed
tests_failed
tests_retried
test_duration_seconds
suite_duration_seconds
```

---

# 17. Seguridad

Requisitos:

* no almacenar secretos en repositorios
* usar secret providers
* enmascarar datos sensibles en logs
* restringir acceso a APIs internas

---

# 18. Roadmap de implementación

Fase 1

```
Screenplay Core
Playwright
Reqnroll
xUnit
```

Fase 2

```
API layer
Builders
Provisioning
Retry policies
```

Fase 3

```
Reporting
Evidence manager
Metrics
Grafana dashboards
```

Fase 4

```
Mobile automation
Contract testing
Device farms
```

---

# 19. Beneficios del framework

El framework permitirá:

* reducir tiempos de ejecución de suites
* aumentar reutilización de código
* mejorar trazabilidad de calidad
* escalar automatización entre múltiples equipos
* mantener una plataforma estable a largo plazo

---

Si quieres, puedo también generarte **dos cosas que normalmente acompañan este documento en arquitectura enterprise**:

1. **El diagrama arquitectónico completo del framework (tipo C4)**
2. **El diseño del Screenplay engine interno (clases base y contratos)**

que son los artefactos que normalmente se entregan junto con este tipo de blueprint.
