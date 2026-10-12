# Enterprise Automation Framework — Estado Actual

**Fecha:** 2026-10-11
**Versión:** 2.0
**Estado:** ✅ **PRODUCCIÓN**

---

## 📊 RESUMEN EJECUTIVO

Refactorización completada: el framework ahora sigue el patrón **Screenplay
canónico**, el POM colapsó en **PageObjects** (catálogos de locators agrupados
por página) y los fallos del motor se traducen a **mensajes humanos**.

### ✅ Build Status
- **Framework.Core:** 0 warnings, 0 errores
- **Scaffolding.Tests:** 0 warnings, 0 errores

### ✅ Test Status
- **Framework.Core.Tests:** 25/25 (unitarias del núcleo — sujetos, extractor, run-history, concurrencia)
- **Scaffolding.Tests:** 5/5 (1 web Google + 3 API JSONPlaceholder + 1 multi-actor)
- **Total: 30/30 — 100% passing**

---

## 🏗️ ARQUITECTURA ACTUAL

### Framework.Core (Núcleo)

```
Framework.Core/
├── BDD/
│   ├── ActorSubjects.cs        # Registro/normalización/canonicalización de sujetos
│   ├── Extraction/
│   │   ├── ISubjectExtractor.cs       # Estrategia inyectable de extracción (§5.4)
│   │   └── DefaultSubjectExtractor.cs # Pronombres + roles + frase nominal (sin lista de verbos)
│   └── ScenarioActor.cs        # Actor por escenario (sin estado estático) + PersonaAsync
├── Configuration/              # Settings, ISettings, ApiSettings, enums, SettingsManager
├── Exceptions/                 # FrameworkException, AbilityNotFoundException,
│                                # TargetFailedException (fallos humanos)
├── Lifecycle/                  # Ciclo de vida REUTILIZABLE (antes vivía en el scaffolding)
│   ├── BrowserSessionManager.cs # Browser compartido por run (lazy + thread-safe)
│   ├── EvidenceCollector.cs     # Política de evidencia (screenshot/trace/video)
│   ├── WebLifecycleHooks.cs     # Hooks [Binding] para escenarios @web (contexto, personas)
│   └── RunHistoryHooks.cs       # Hooks [Binding] de reporte para TODOS los escenarios
├── ScreenPlay/
│   ├── Interfaces/             # IActor, IAbility, IAction, ITask, IInteraction, IQuestion
│   ├── Actor.cs / ActorBuilder.cs / ScreenAbstractions.cs / DisposableAbility.cs
│   ├── BrowseTheWeb.cs
│   ├── Locators/               # ITarget, Target (con Page/Parent), TargetExtensions
│   │                           # TargetGuard → timeouts del motor → fallos legibles
│   ├── Pages/                  # PageObject (locators por página) + Component (bloques UI)
│   ├── Web/                    # Interactions (Click, Enter...), Questions, Ensure*
│   ├── Api/                    # CallAnApi, GetResource/PostResource, Questions, Ensure*
│   └── Tasks/NavigateToUrl.cs
└── Tools/                      # Database (8 motores), RunHistory (append thread-safe),
                                # AllureReportHelper
```

> **Desacoplamiento Core ↔ Scaffolding:** el ciclo de vida (browser, contexto,
> evidencia, personas, run-history) vive en `Framework.Core/Lifecycle` como
> bindings `[Binding]`. `Scaffolding.Tests` los consume declarando el assembly
> en `reqnroll.json` (`bindingAssemblies`), **sin reimplementar nada**. El
> scaffolding ya no tiene hooks propios: solo Steps, Pages, Features y config.

### Scaffolding.Tests (Consumidor / Ejemplo)

```
Scaffolding.Tests/
├── reqnroll.json               # Declara Framework.Core como assembly de bindings
├── appsettings.json (+ .Production)  # Config por entorno (Headless, timeouts, Api...)
├── BDD/
│   ├── Features/               # GoogleSearch.feature, JsonPlaceholderApi.feature
│   └── Steps/                  # Steps delgados (solo Tasks/Ensure* al Actor)
└── Pages/Google/
    ├── SearchBox.cs            # Component reutilizable (input + botón)
    ├── GoogleHomePage.cs       # PageObject (locators con nombre humano)
    ├── GoogleSearchResultsPage.cs
    └── GoogleTasks.cs          # SearchFor (Task de negocio por página)
```

---

## 🎯 DECISIONES DE DISEÑO (v2.0)

| Problema (v1) | Solución (v2) |
|---|---|
| POM + Screenplay coexistían (duplicidad de conceptos) | `POM/` eliminado; `PageObject` = catálogo de locators por página, sin comportamiento |
| Selectores como `const string` sin contexto | `ITarget` tipado: nombre humano + selector + estrategia + página + scope |
| Selectores repetidos entre páginas | `Component` con root scoped; las páginas estampan `with { Page = Name }` |
| Errores crudos (`Timeout ... #usuario`) | `TargetGuard` + `TargetFailedException`: `'Carlos' could not fill in 'Usuario' (Login)`; selector solo en inner exception |
| `ScenarioActor` con cache estático entre escenarios | Sin estado estático: lectura directa del `ScenarioContext` |
| Steps con lógica de validación / clases Actions duplicadas | Steps delgados + Tasks por página + `Ensure*` reutilizables |
| Namespaces inconsistentes (`Screenplay` vs `ScreenPlay`) | Unificado a `Framework.Core.ScreenPlay` |
| Ciclo de vida (browser/evidencia/reportes) implementado en el **scaffolding** | Movido a `Framework.Core/Lifecycle` (bindings `[Binding]`); el scaffolding solo lo consume vía `reqnroll.json` |
| `_currentFeature` estático compartido entre features paralelas (run-history corrupto) | `FeatureContext` por escenario en `RunHistoryHooks`; sin estado estático |
| `lock` en cada lectura de `ActorSubjects`; escrituras de `RunHistory` sin sincronía | `ConcurrentDictionary` + `lock` solo en el append del historial |
| URL de API hard-codeada en los steps | Sección `Api` en configuración (`ApiSettings`), consumida por Core |
| Extractor de sujetos con **lista rígida de verbos + tope fijo de tokens** | `ISubjectExtractor` inyectable (`DefaultSubjectExtractor`: pronombres + roles + frase nominal acotada por auxiliares) |
| Todas las personas compartían la misma página (sin sesiones independientes) | `ScenarioActor.PersonaAsync` + factory async: cada persona secundaria tiene su propio `IBrowserContext` |

---

## 🔧 PATRONES EN USO

1. **Screenplay** — Actor, Abilities, Tasks, Interactions, Questions (puras).
2. **PageObject** — locators agrupados por página, con nombre humano y descripción.
3. **Component** — bloques UI reutilizables (header, modal, tabla, caja de búsqueda).
4. **Ensure\*** — validaciones reutilizables (Question + FluentAssertions).
5. **BDD (Reqnroll)** — steps delgados; el Actor se obtiene con `ScenarioActor`.
6. **E2E + API** — `BrowseTheWeb` (Playwright) y `CallAnApi` (RestSharp) sobre el mismo Actor/Task/Question.

---

## 📖 DOCUMENTACIÓN

- `docs/00_QUICKSTART.md` — Inicio rápido
- `docs/01_ARCHITECTURE.md` — Arquitectura (PageObject/Component incluidos)
- `docs/02_USER_GUIDE.md` — Crear tests con la estructura nueva
- `docs/03_BEST_PRACTICES.md` — Convenciones y anti-patrones
- `docs/04_AI_AGENT_SKILL.md` — Skill para agentes de IA (fuentes de verdad, plantillas, checklist)
- `docs/README.md` — Índice

---

## ✅ VALIDACIÓN

```bash
dotnet build src/Framework.Automation.sln --configuration Release   # 0 warnings, 0 errores
dotnet test  src/Framework.Automation.sln                          # 30/30 aprobados
#   ├─ Framework.Core.Tests : 25/25  (sujetos, extractor pluggable, run-history, concurrencia)
#   └─ Scaffolding.Tests    :  5/5   (1 web Google + 3 API + 1 multi-actor con sesiones aisladas)
```

> **Desacoplamiento:** `Scaffolding.Tests` es un consumidor puro —no tiene hooks
> ni lógica de ciclo de vida. Declara `Framework.Core` como assembly de bindings
> en `reqnroll.json`; todo el ciclo de vida vive en `Framework.Core/Lifecycle`.

### Allure report + histórico acumulativo (nuevo en v2.0)

- `AllureHistory = Cumulative` (predeterminado): cada ejecución se guarda en su
  propio subdirectorio `allure-results/<RunId>/` con sus resultados y evidencias;
  el HTML se genera siempre a partir de `allure-results/latest/` (el rollo acumulado
  de todos los runs retainidos).
- `KeepPastRuns = 20`: se conservan los últimos 20 ejecuciones; los demás se suelen
  borrar tras reconstruir el rollo `latest/`.
- `MergeResults` copia solo: cada resultado mantiene el suyo propio `uuid`; un
  `historyId` igual (`"Feature::Scenario"`) se acumula como reintento del mismo
  test en el History tab del reporte.
- `PruneOldRuns` elimina los folders del run más allá del límite de retención,
  siempre después de reconstruir `latest/`, para que la poda no afecte al histórico.
- `history.json` y `history/*.json` (tendencias duration/retry/categories) se copian
  al HTML para que el report renderice tendencias, retiros y resultados pasados.
- `TestHistory` (append-only, thread-safe) registra el resumen de ejecución: conteo
  por estado, duración promedio y número total de escenarios.

```bash
dotnet build src/Framework.Automation.sln --configuration Release   # 0 warnings, 0 errores
dotnet test  src/Framework.Automation.sln                          # 38/38 aprobados
#   ├─ Framework.Core.Tests : 33/33 (incluyendo 8 AllureReportTests)
#   └─ Scaffolding.Tests    :  5/5   (1 web Google + 3 API + 1 multi-actor)
```

---

