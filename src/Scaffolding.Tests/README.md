# Scaffolding.Tests — Ejemplos del Enterprise Automation Framework

Proyecto plantilla que muestra **cómo escribir automatización correcta** con el
framework: Screenplay puro, locators agrupados por página y steps delgados.

## 📁 Estructura

```
Scaffolding.Tests/
├── reqnroll.json             # Declara Framework.Core como assembly de bindings
├── appsettings.json          # Config (Headless, timeouts, Api...) — la fuente es Core
├── BDD/
│   ├── Features/             # Gherkin (.feature) con tags @web/@api/@smoke/@multi-actor
│   └── Steps/                # Step definitions DELGADOS (solo entregan acciones al Actor)
│
└── Pages/                    # Locators + Tasks agrupados por página (aquí vive todo lo "de página")
    └── Google/
        ├── SearchBox.cs               # Component: bloque UI reutilizable con scope
        ├── GoogleHomePage.cs          # PageObject: catálogo de locators de la home
        ├── GoogleSearchResultsPage.cs # PageObject: catálogo de locators de resultados
        └── GoogleTasks.cs             # Task de negocio del flujo (SearchFor)
```

> **Sin hooks propios:** el ciclo de vida (browser, contexto, evidencia,
> personas, reportes) vive en `Framework.Core/Lifecycle` como bindings
> `[Binding]`. Este proyecto solo los **consume** declarando `Framework.Core`
> en `reqnroll.json`. Para multi-actor con sesiones aisladas, usa
> `ScenarioActor.PersonaAsync(...)` en los steps.

## 🎯 Patrones implementados

| Patrón | Dónde | Idea clave |
|--------|-------|------------|
| **Screenplay** | `Framework.Core` | Actor con Abilities ejecuta Tasks/Interactions y responde Questions |
| **PageObject (ex-POM)** | `Pages/<App>/<Pagina>.cs` | Solo locators con **nombre humano** + identidad de página. Sin comportamiento |
| **Component** | `Pages/<App>/<Bloque>.cs` | Bloque UI (header, modal, caja de búsqueda) compartido entre páginas, resuelto con scope |
| **BDD (Reqnroll)** | `BDD/` | Steps delgados: cero selectores, cero lógica de negocio |
| **Fallos humanos** | Core (`TargetGuard`) | `'Carlos' could not fill in 'Usuario' (Login)` — nunca `#usuario` |

## 🚀 Ejecutar

```bash
cd src
dotnet restore
dotnet build
pwsh Scaffolding.Tests/bin/Debug/net10.0/playwright.ps1 install   # solo la 1ª vez

dotnet test
dotnet test --filter "TestCategory=web"
dotnet test --filter "TestCategory=api"
```

## 📝 Cómo agregar automatización (resumen)

1. **Feature** en `BDD/Features/MiFlujo.feature` (tags `@web`/`@api`, `@smoke`...).
2. **PageObject** en `Pages/MiApp/MiPagina.cs` (hereda `PageObject`): declara
   cada target con nombre humano (`ByLabel("usuario", "Usuario", ...)`).
3. **Component** (opcional) si un bloque se repite en varias páginas.
4. **Tasks** en `Pages/MiApp/MiFlujoTasks.cs` (heredan `ScreenTask`): componen
   `Click`, `Enter`, `WaitFor`... sobre los targets del PageObject.
5. **Steps** delgados: `Actor.AttemptsToAsync(Tarea)` / `AsksForAsync(Question)`.
6. **Validaciones** con los `Ensure*` del Core (o Question pura + assert).

Plantillas completas y checklist: [docs/04_AI_AGENT_SKILL.md](../../docs/04_AI_AGENT_SKILL.md).

## 🏷️ Tags

- Tipo: `@web`, `@api`, `@mobile` (futuro)
- Criticidad: `@smoke`, `@regression`, `@sanity`
- Aplicación: `@app1`, `@app2`...

## 📖 Recursos

- [Framework README](../../README.md)
- [Arquitectura](../../docs/01_ARCHITECTURE.md)
- [AI Agent Skill](../../docs/04_AI_AGENT_SKILL.md)