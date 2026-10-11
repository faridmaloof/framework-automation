# Enterprise Automation Framework - Arquitectura

**Versión:** 1.0  
**Fecha:** 2026-03-14  
**Estado:** ✅ **PRODUCCIÓN**

---

## 📋 ÍNDICE

1. [Visión General](#visión-general)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [Framework.Core](#frameworkcore)
4. [Scaffolding.Tests](#scaffoldingtests)
5. [ScreenPlay Pattern](#screenplay-pattern)
6. [Page Objects y Components](#page-objects-y-components)
7. [BDD con Reqnroll](#bdd-con-reqnroll)
8. [Configuración](#configuración)
9. [Base de Datos](#base-de-datos)
10. [Allure Reporting](#allure-reporting)

---

## VISIÓN GENERAL

Enterprise Automation Framework es un framework de automatización empresarial basado en:

- **.NET 10** - Runtime moderno
- **Playwright** - Browser automation
- **ScreenPlay Pattern** - Patrón de automatización mantenible
- **BDD (Reqnroll)** - Especificaciones ejecutables
- **Allure** - Reportes profesionales

### Principios de Diseño

1. **Mantenibilidad** - Código fácil de entender y modificar
2. **Reutilización** - Máximo aprovechamiento de código existente
3. **Extensibilidad** - Fácil agregar nuevas capacidades
4. **Claridad** - Estructura obvia y predecible
5. **Performance** - Ejecución rápida y eficiente

---

## ESTRUCTURA DEL PROYECTO

```
src/
├── Framework.Core/              # Núcleo del framework (REUTILIZABLE)
│   ├── ScreenPlay/              # Actor, Abilities, Tasks, Interactions, Questions
│   │   ├── Interfaces/          # Contratos (IActor, IAbility, ITask, IQuestion...)
│   │   ├── Locators/            # ITarget, Target, TargetGuard (fallos legibles)
│   │   ├── Pages/               # PageObject y Component (locators por página)
│   │   ├── Web/                 # Interacciones, Questions y Ensure* web
│   │   ├── Api/                 # Ability, Tasks, Questions y Ensure* de API
│   │   └── Tasks/               # NavigateToUrl
│   ├── Tools/                   # Database, AllureReportHelper
│   ├── Exceptions/              # FrameworkException, TargetFailedException
│   └── Configuration/           # Settings (ISettings, AllureSettings)
│
└── Scaffolding.Tests/           # Tests específicos (PERSONALIZABLE)
    ├── BDD/                     # Features, Steps, Hooks
    └── Pages/                   # Locators + Tasks agrupados por página
        └── Google/              # SearchBox (Component), GoogleHomePage, GoogleTasks
```

### Reglas de Oro

1. **Framework.Core** - Solo código REUTILIZABLE y ESTÁNDAR
2. **Scaffolding.Tests** - Código ESPECÍFICO de tu proyecto
3. **Nunca modificar Core** directamente - Extender desde Scaffolding
4. **Una clase por archivo** - Fácil de encontrar y mantener
5. **Un locator, un solo lugar** - Los selectores viven únicamente en el
   `PageObject`/`Component` de su página; nunca en Steps ni en Tests

---

## FRAMEWORK.CORE

### ScreenPlay/

Interfaces y clases base del ScreenPlay Pattern:

```
ScreenPlay/
├── Interfaces/                  # Contratos
│   ├── IActor.cs                # Contrato del Actor
│   ├── IAbility.cs              # Contrato de Habilidades
│   ├── IAction.cs               # Contrato de Acciones
│   ├── ITask.cs                 # Contrato de Tasks
│   ├── IInteraction.cs          # Contrato de Interacciones
│   └── IQuestion.cs             # Contrato de Questions
├── Actor.cs                     # Implementación del Actor
├── ActorBuilder.cs              # Builder fluente para crear Actores
├── ScreenAbstractions.cs        # ScreenTask / ScreenInteraction / ScreenQuestion
├── DisposableAbility.cs         # Base para habilidades con recursos
├── BrowseTheWeb.cs              # Habilidad estándar (Playwright)
├── Locators/
│   ├── ITarget.cs               # Locator tipado (nombre humano + selector)
│   ├── Target.cs                # Implementación inmutable (record)
│   ├── TargetExtensions.cs      # Resolución con scope de componentes
│   └── TargetGuard.cs           # Traduce timeouts del motor a fallos legibles
├── Pages/
│   ├── PageObject.cs            # Catálogo de locators agrupado por página
│   └── Component.cs             # Bloque UI reutilizable con root scoped
├── Web/
│   ├── Interactions/            # Click, Clear, Enter, PressKey, Hover, WaitFor
│   ├── Questions/               # CurrentUrl, CurrentTitle, IsVisible, Count, TextOf...
│   └── Tasks/                   # Ensure* (validaciones reutilizables)
├── Api/
│   ├── Abilities/               # CallAnApi (RestSharp)
│   ├── Tasks/                   # GetResource, PostResource, Ensure*
│   └── Questions/               # StatusCode, ResponseBody, BodyIsArray...
└── Tasks/
    └── NavigateToUrl.cs         # Navegación reutilizable
```

### Pages/

El "Page Object Model" colapsa aquí: un `PageObject` es un **catálogo de
locators agrupado por página** — identidad de página (nombre, URL) + los
`ITarget` que expone — **sin comportamiento**. El comportamiento vive en los
Tasks ScreenPlay, que consumen esos targets:

```csharp
// PageObject: SOLO locators, con nombre humano (nunca selectores sueltos)
public sealed class LoginPage : PageObject
{
    public static readonly LoginPage Instance = new();
    public override string Name => "Login";
    public override string Url => "https://app.example.com/login";

    public Target UserField     => ByLabel("usuario", "Usuario", "Credencial de acceso");
    public Target PasswordField => ByLabel("contraseña", "Contraseña");
    public Target SubmitButton  => ByTestId("botón ingresar", "login-submit");

    public override IEnumerable<ITarget> Targets => [UserField, PasswordField, SubmitButton];
}

// Component: bloque UI reutilizable con scope propio (modal, header, tabla)
public sealed class ConfirmationModal : Component
{
    public static readonly ConfirmationModal Instance =
        new(Target.Css("modal de confirmación", "[data-testid='confirm-modal']"));

    public Target AcceptButton => Css("botón aceptar", "[data-testid='accept']");
}
```

Los fallos referencian el nombre humano y la página — nunca el selector crudo:

```
'Carlos' could not click 'Botón ingresar' (Login): the element was not found or not ready before the timeout.
```

### Tools/

Utilidades comunes:

```
Tools/
├── Database/                    # 8 motores de base de datos
│   ├── DatabaseConnection.cs    # Base abstracta
│   ├── SqlServerConnection.cs
│   ├── PostgreSqlConnection.cs
│   ├── MySqlConnection.cs
│   ├── OracleConnection.cs
│   ├── MariaDbConnection.cs
│   ├── MongoDbConnection.cs     # NoSQL
│   └── RedisConnection.cs       # NoSQL
└── AllureReportHelper.cs        # Generar y abrir reportes
```

### Configuration/

Configuración tipada:

```csharp
// AllureSettings.cs
public class AllureSettings
{
    public string ResultsDirectory { get; set; } = "allure-results";
    public string ProjectName { get; set; } = "Enterprise Automation Framework";
    public bool AutoOpenReport { get; set; } = false;  // No abrir automáticamente
    public int ReportPort { get; set; } = 56789;
}
```

---

## SCAFFOLDING.TESTS

### BDD/

Tests en formato Gherkin:

```
BDD/
├── Features/                    # Archivos .feature
│   ├── GoogleSearch.feature
│   └── JsonPlaceholderApi.feature
├── Steps/                       # Step definitions (delgados)
│   ├── GoogleSearchSteps.cs
│   └── JsonPlaceholderApiSteps.cs
└── Hooks/                       # Hooks de ciclo de vida
    └── GlobalHooks.cs
```

### Pages/

Locators y Tasks específicos, agrupados por página:

```
Pages/Google/
├── SearchBox.cs                 # Component reutilizable (input + botón)
├── GoogleHomePage.cs            # PageObject: locators de la home
├── GoogleSearchResultsPage.cs   # PageObject: locators de resultados
└── GoogleTasks.cs               # Tasks del flujo de búsqueda (SearchFor)
```

---

## SCREENPLAY PATTERN

### ¿Qué es ScreenPlay?

ScreenPlay Pattern es un patrón de automatización que modela:

- **Actor** - Quién ejecuta las acciones
- **Ability** - Qué puede hacer el actor
- **Task** - Tareas de negocio que realiza
- **Interaction** - Interacciones técnicas
- **Question** - Consultas sobre el estado

### Ejemplo de Uso

```csharp
// Crear actor con habilidad
var actor = new ActorBuilder("Carlos")
    .WithAbility(BrowseTheWeb.With(page))
    .Build();

// Ejecutar: abrir la página y realizar la tarea de negocio SearchFor
await actor.AttemptsToAsync(
    GoogleHomePage.Instance.Open("networkidle"),
    SearchFor.Query("Playwright .NET"));

// Validar con un Ensure* task (la asertación vive ahí, no en el step)
await actor.AttemptsToAsync(
    EnsureUrlContains.Text("/search", "la búsqueda debe aterrizar en la URL de resultados"));

// O consultar estado puro con una Question y decidir en el step/test
var url = await actor.AsksForAsync(CurrentUrl.Value());
url.Should().Contain("/search");
```

### Jerarquía de Elementos

```
Actor (quién ejecuta)
├── Ability (lo que puede hacer)
│   ├── BrowseTheWeb
│   └── CallAnApi
├── Task (tarea de negocio, compuesta)
│   ├── SearchFor / NavigateToUrl
│   └── Ensure* (validaciones reutilizables)
├── Interaction (acción técnica atómica)
│   ├── Click / Clear / Enter / PressKey / Hover / WaitFor
│   └── GetResource / PostResource (dominio API)
└── Question (consulta pura, sin aserciones)
    ├── CurrentUrl / CurrentTitle / IsVisible / Count / TextOf
    └── StatusCode / ResponseBody / BodyIsArray
```

---

## PAGE OBJECTS Y COMPONENTS

### PageObject = catálogo de locators por página

El POM tradicional (con sus "Actions" y helpers de locator crudo) se
reemplaza por un único tipo base: `PageObject`. Reglas:

1. **PageObject** - SOLO locators (nombre humano + selector + estrategia) e identidad de página
2. **Tasks** - Composición de Interactions sobre los targets del PageObject
3. **Steps** - Entregan Tasks/Ensure* al Actor; nunca ven selectores

```csharp
// 1. PageObject: locators agrupados por página (nombre humano siempre)
public sealed class LoginPage : PageObject
{
    public static readonly LoginPage Instance = new();
    public override string Name => "Login";
    public override string Url => "https://app.example.com/login";

    public Target UserField     => ByLabel("usuario", "Usuario");
    public Target PasswordField => ByLabel("contraseña", "Contraseña");
    public Target SubmitButton  => ByTestId("botón ingresar", "login-submit");
}

// 2. Task de negocio agrupado por página (en Pages/Login/LoginTasks.cs)
public sealed class SignIn : ScreenTask
{
    private readonly string _user;
    private readonly string _password;

    public SignIn(string user, string password) { _user = user; _password = password; }

    public override string TaskName => $"Sign in as '{_user}'";

    public override Task PerformAsAsync(IActor actor) =>
        actor.AttemptsToAsync(
            Enter.TheText(_user).Into(LoginPage.Instance.UserField).Now(),
            Enter.TheText(_password).Into(LoginPage.Instance.PasswordField).Now(),
            Click.On(LoginPage.Instance.SubmitButton));
}

// 3. Step delgado: solo entrega la tarea al actor
[When("they sign in with {string} and {string}")]
public Task WhenSignIn(string user, string password)
    => Actor.AttemptsToAsync(new SignIn(user, password));
```

### Component = bloque UI reutilizable

Cuando un bloque (header, modal, tabla, caja de búsqueda) aparece en varias
páginas, se modela una sola vez como `Component` con su propio root; los
targets hijos se resuelven con scope y cada página estampa su nombre:

```csharp
public sealed class SearchBox : Component
{
    public static readonly SearchBox Instance =
        new(Target.Css("cuadro de búsqueda", "form[action='/search']"));

    public Target QueryField  => Css("campo de consulta", "[name='q']");
    public Target SubmitButton => Css("botón de búsqueda", "[type='submit']");
}

// Cada página expone el componente con su contexto:
public Target SearchQueryField => SearchBox.Instance.QueryField with { Page = Name };
```

---

## BDD CON REQNROLL

### Feature File

```gherkin
@web @smoke
Feature: Google Search
  As a user
  I want to search for concepts
  So that I can find information

  Scenario: Successful search
    Given the user is on the Google home page
    When they search for ".NET"
    Then the search should execute without errors
```

### Steps

Los steps son **delgados**: obtienen el Actor (helper `ScenarioActor`) y le
entregan Tasks/Ensure*. Cero selectores, cero lógica de validación:

```csharp
[Binding]
public class GoogleSearchSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IActor Actor => ScenarioActor.Get(_scenarioContext);

    [Given("the user is on the Google home page")]
    public Task GivenOnGooglePage()
        => Actor.AttemptsToAsync(GoogleHomePage.Instance.Open("networkidle"));

    [When("they search for {string}")]
    public Task WhenSearch(string query)
        => Actor.AttemptsToAsync(SearchFor.Query(query));

    [Then("the search should execute without errors")]
    public Task ThenSearchSucceeded()
        => Actor.AttemptsToAsync(
            EnsureUrlContains.Text("/search", "the search must land on a results URL"));
}
```

### Hooks

```csharp
[Binding]
public class GlobalHooks
{
    [BeforeScenario("web")]
    public async Task BeforeWebScenario()
    {
        // Setup browser, context, page, actor
    }
    
    [AfterScenario]
    public async Task AfterScenario()
    {
        // Cleanup resources
    }
}
```

---

## CONFIGURACIÓN

### appsettings.json

```json
{
  "Framework": {
    "Environment": "Development",
    "Headless": true,
    "Timeouts": {
      "GlobalTimeoutSeconds": 30
    },
    "Evidence": {
      "Screenshot": "PreAndPost",
      "Video": "OnFailure"
    }
  },
  "Device": {
    "Browser": "Chromium",
    "Viewport": [1920, 1080]
  },
  "Allure": {
    "ResultsDirectory": "allure-results",
    "ProjectName": "My Tests",
    "AutoOpenReport": false,
    "ReportPort": 56789
  }
}
```

### Configuración por Entorno

```
appsettings.json                 # Base
appsettings.Development.json     # Desarrollo (headless: false)
appsettings.Staging.json         # Staging
appsettings.Production.json      # Producción (headless: true)
```

---

## BASE DE DATOS

### Conexión SQL Server

```csharp
using Framework.Core.Tools.Database;

var db = new SqlServerConnection(connectionString);
var users = await db.QueryAsync<User>("SELECT * FROM Users");
var user = await db.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM Users WHERE Id = @Id", 
    new { Id = 123 }
);
```

### Conexión MongoDB

```csharp
using Framework.Core.Tools.Database;

var mongo = new MongoDbConnection(connectionString, "mydb");
var users = await mongo.FindAsync<User>("users");
await mongo.InsertOneAsync("users", newUser);
```

---

## ALLURE REPORTING

### Configuración

```json
{
  "Allure": {
    "ResultsDirectory": "allure-results",
    "AutoOpenReport": false,
    "ReportPort": 56789
  }
}
```

### Abrir Reporte Manualmente

```csharp
using Framework.Core.Tools;

// Después de ejecutar tests
AllureReportHelper.OpenReport("allure-results");
```

### Generar Reporte

```csharp
// Generar reporte estático
AllureReportHelper.GenerateReport("allure-results", "allure-report");
```

---

## MEJORES PRÁCTICAS

### ✅ HACER

1. Usar ScreenPlay Pattern para todas las automatizaciones
2. Declarar cada locator UNA sola vez, en el PageObject de su página
3. Una clase por archivo
4. Namespaces consistentes
5. Tests independientes entre sí
6. Usar tags para organizar tests (@smoke, @regression)

### ❌ NO HACER

1. Modificar Framework.Core directamente
2. Mezclar selectores con comportamiento
3. Múltiples clases en un archivo
4. Tests que dependen de otros tests
5. Hardcoded values (usar configuración)
6. Selectores frágiles (usar data-testid)

---

## SOPORTE

- **Documentación**: `/docs/` folder
- **Issues**: GitHub Issues
- **Discussions**: GitHub Discussions

---

*Enterprise Automation Framework - Documentación de Arquitectura*
