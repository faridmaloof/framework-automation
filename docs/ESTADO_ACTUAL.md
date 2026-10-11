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
- **Total:** 4 (1 web + 3 API) — **100% passing**

---

## 🏗️ ARQUITECTURA ACTUAL

### Framework.Core (Núcleo)

```
Framework.Core/
├── BDD/ScenarioActor.cs        # Actor por escenario (sin estado estático)
├── Configuration/              # Settings, enums, SettingsManager
├── Exceptions/                 # FrameworkException, AbilityNotFoundException,
│                                # TargetFailedException (fallos humanos)
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
└── Tools/                      # Database (8 motores), AllureReportHelper
```

### Scaffolding.Tests (Ejemplos)

```
Scaffolding.Tests/
├── BDD/
│   ├── Features/               # GoogleSearch.feature, JsonPlaceholderApi.feature
│   ├── Steps/                  # Steps delgados (solo Tasks/Ensure* al Actor)
│   └── Hooks/GlobalHooks.cs    # Actor + BrowseTheWeb por escenario
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
dotnet test  src/Scaffolding.Tests/Scaffolding.Tests.csproj         # 4/4 aprobados
```

---

*Enterprise Automation Framework — Estado actual y validado* 🚀