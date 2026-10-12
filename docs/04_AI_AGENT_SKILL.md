# 🤖 AI Agent Skill — Construir automatización con este framework

**Audiencia:** agentes de IA (y humanos) que generan o modifican código de
automatización en este repositorio.
**Objetivo:** producir automatización **consistente, ordenada y sin
antipatrones**, siguiendo el patrón Screenplay canónico del framework.

---

## 1. Modelo mental (5 reglas de oro)

1. **Screenplay manda.** Todo fluye por `Actor → Ability → Task/Interaction → Question`.
   El paso/step solo entrega acciones al Actor; nunca ejecuta Playwright directamente.
2. **El POM desapareció.** Lo que antes era "Page Object" es ahora un
   `PageObject`: un **catálogo de locators agrupado por página** (identidad +
   targets), **sin comportamiento**. El comportamiento vive en Tasks.
3. **Un locator, un solo lugar.** Todo selector existe una única vez, declarado
   en el `PageObject`/`Component` de su página, con **nombre humano**
   (ej. `ByLabel("usuario", "Usuario")`). Nunca selectores crudos en Steps,
   Tasks o Tests.
4. **Las validaciones son Tasks Ensure\* o Questions puras.** Las Questions
   **no** asientan; los `Ensure*` encapsulan Question + asertación y son
   reutilizables. Los Steps no contienen lógica de validación.
5. **Los fallos son humanos.** El framework traduce los timeouts del motor a
   mensajes como `'Carlos' could not fill in 'Usuario' (Login)`. Nunca se
   muestra el selector como referencia principal (solo queda en el inner
   exception/trace).

## 2. Vocabulario del framework

| Elemento | Tipo base | Dónde vive | Regla |
|----------|-----------|-----------|-------|
| Locator por página | `PageObject` (`ScreenPlay.Pages`) | `Pages/<App>/<Pagina>.cs` | Solo targets + `Name`/`Url`. Stateless, expone `Instance` |
| Bloque UI reutilizable | `Component` (`ScreenPlay.Pages`) | `Pages/<App>/<Bloque>.cs` | Root propio; hijos con scope. Las páginas estampan `with { Page = Name }` |
| Locator tipado | `ITarget` / `Target` (`ScreenPlay.Locators`) | dentro de PageObject/Component | Siempre nombre humano + descripción |
| Interacción atómica | `ScreenInteraction` | Core (`ScreenPlay/Web/Interactions`) | Reutilizar `Click`, `Clear`, `Enter`, `PressKey`, `Hover`, `WaitFor` |
| Tarea de negocio | `ScreenTask` | `Pages/<App>/<Flujo>Tasks.cs` | Compone Interactions; se agrupan por página/flujo |
| Consulta pura | `ScreenQuestion<T>` | Core (`ScreenPlay/Web/Questions` o `Api/Questions`) | Devuelve datos; nunca asienta |
| Validación reutilizable | `Ensure*` (`ScreenTask`) | Core (`ScreenPlay/Web/Tasks`, `Api/Tasks`) | Question + FluentAssertions |
| Habilidad | `IAbility` | Core (`BrowseTheWeb`, `CallAnApi`) | Se inyecta al Actor en los Hooks |
| Steps BDD | — | `BDD/Steps/` | Delgados: `Actor.AttemptsToAsync(...)` / `AsksForAsync(...)` |

## 3. Recetas (plantillas verbatim)

### 3.1 Nueva página → crear su PageObject

```csharp
using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Pages;

namespace Scaffolding.Tests.Pages.MyApp;

/// <summary>Catálogo de locators de la página de Login (sin comportamiento).</summary>
public sealed class LoginPage : PageObject {
    public static readonly LoginPage Instance = new();
    private LoginPage() { }

    public override string Name => "Login";
    public override string Url => "https://app.example.com/login";

    public Target UserField     => ByLabel("usuario", "Usuario", "Credencial de acceso");
    public Target PasswordField => ByLabel("contraseña", "Contraseña");
    public Target SubmitButton  => ByTestId("botón ingresar", "login-submit");

    public override IEnumerable<ITarget> Targets => [UserField, PasswordField, SubmitButton];
}
```

### 3.2 Bloque reutilizable en varias páginas → Component

```csharp
public sealed class SiteHeader : Component {
    public static readonly SiteHeader Instance =
        new(Target.Css("cabecera", "header[role='banner']"));

    public Target LogoutLink => ByText("enlace de salida", "Cerrar sesión");
}

// En cada PageObject que lo use, estampa su contexto de página:
public Target HeaderLogout => SiteHeader.Instance.LogoutLink with { Page = Name };
```

### 3.3 Flujo de negocio → Task agrupado por página

```csharp
public sealed class SignIn : ScreenTask {
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
```

### 3.4 Validación reutilizable (cuando el Ensure* de Core no alcanza)

```csharp
// Question pura (sin aserciones)...
public sealed class WelcomeMessage : ScreenQuestion<string> {
    public override string Description => "mensaje de bienvenida";
    public static WelcomeMessage Value() => new();

    public override Task<string> AnsweredByAsync(IActor actor)
        => actor.AsksForAsync(TextOf.On(HomePage.Instance.WelcomeBanner));
}

// ... + Ensure* que la compone con la asertación
public sealed class EnsureWelcomeMessage : ScreenTask {
    private readonly string _expected;
    public EnsureWelcomeMessage(string expected) => _expected = expected;

    public override string TaskName => $"Ensure welcome message is '{_expected}'";

    public override async Task PerformAsAsync(IActor actor)
    {
        var actual = await actor.AsksForAsync(WelcomeMessage.Value());
        actual.Should().Be(_expected, "the welcome banner must greet the signed-in user");
    }
}
```

### 3.5 Step delgado (BDD)

```csharp
[Binding]
public class LoginSteps(ScenarioContext scenarioContext) {
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private IActor Actor => ScenarioActor.Get(_scenarioContext);

    [Given("the user is on the login page")]
    public Task OnLoginPage() => Actor.AttemptsToAsync(LoginPage.Instance.Open());

    [When("they sign in as {string} with {string}")]
    public Task SignInAs(string user, string password)
        => Actor.AttemptsToAsync(new SignIn(user, password));

    [Then("they should see the welcome banner")]
    public Task WelcomeSeen()
        => Actor.AttemptsToAsync(EnsureVisible.Of(HomePage.Instance.WelcomeBanner));
}
```

### 3.6 API (e2e / integración)

```csharp
await actor.AttemptsToAsync(
    GetResource.From("/posts"),
    EnsureStatusSuccessful.Now("the endpoint must answer 2xx"),
    EnsureBodyIsArray.Now("a collection endpoint must return an array"));

var status = await actor.AsksForAsync(StatusCode.OfLastResponse());  // Question pura
```

## 4. Estructura de carpetas (convenir, no improvisar)

```
src/Scaffolding.Tests/
├── reqnroll.json                 # Declara Framework.Core como assembly de bindings
├── BDD/
│   ├── Features/<Flujo>.feature      # Gherkin, tags @web/@api/@smoke
│   └── Steps/<Flujo>Steps.cs         # Delgados (usa ScenarioActor / PersonaAsync)
└── Pages/<App>/
    ├── <Componente>.cs               # Component (si es reutilizable)
    ├── <Pagina>.cs                   # PageObject (locators agrupados por página)
    └── <Flujo>Tasks.cs               # Tasks de negocio del flujo
```

> No hay carpeta `Hooks/`: el ciclo de vida (browser, contexto, evidencia,
> personas, reportes) lo provee `Framework.Core/Lifecycle` vía `reqnroll.json`.
```

## 5. Anti-patrones prohibidos (el agente DEBE rechazarlos)

| Anti-patrón | Por qué | Qué hacer |
|-------------|---------|-----------|
| Selector crudo fuera de PageObject/Component (`"#usuario"`) | Rompe "un locator, un lugar" y los mensajes humanos | Declarar el target en su PageObject |
| Clase "Actions" estática con selectores (el viejo `POM/*Actions`) | Duplica el concepto Task y mezcla capas | `ScreenTask` por página |
| `Thread.Sleep` / esperas fijas | Flakiness | `WaitFor.Visible/Hidden/Attached` o auto-wait de Playwright |
| Aserciones dentro de una Question | Rompe composabilidad | Question pura + `Ensure*` o assert en el step |
| Lógica de negocio en Steps | Steps frágiles e ilegibles | Crear un `ScreenTask` |
| `actor.AbilityTo<BrowseTheWeb>().Page` en Steps/Tests | Fuga del patrón | Interactions/Questions del Core |
| Reinventar interacciones/questions | Duplicidad | Reusar las del Core (ver §7) antes de crear nuevas |

## 6. Checklist de verificación antes de entregar

- [ ] `dotnet build` → 0 errores, 0 warnings.
- [ ] `dotnet test` → los tests afectados pasan.
- [ ] Cada selector nuevo vive SOLO en un PageObject/Component, con nombre
      humano y descripción.
- [ ] Los steps solo llaman `AttemptsToAsync` / `AsksForAsync`; cero Playwright.
- [ ] Las validaciones usan `Ensure*` (o Question + assert en el step/test).
- [ ] XML docs breves en clases públicas; identificadores en inglés, nombres
      humanos de targets en el idioma del equipo.
- [ ] Un fallo provocado a propósito produce un mensaje legible (actor +
      intención + target con página).

## 7. Referencia rápida de API

```csharp
// Interacciones web (Core)
Click.On(target)                          Clear.Field(target)
Enter.TheText("x").Into(target).Now()     PressKey.EnterOn(target) / PressKey.Key(target, "Tab")
Hover.Over(target)                        WaitFor.Visible/Hidden/Attached(target)

// Questions web (Core)
CurrentUrl.Value()      CurrentTitle.Value()   IsVisible.Of(target)
Count.Of(target)        TextOf.On(target)      UrlContains.Text("...") / UrlEquals.Value("...")

// Ensure* web (Core)
EnsureVisible.Of(target, "porque...")     EnsureHidden.Of(target)
EnsureCountGreaterThan.Of(target, 5)      EnsureTextOf.Equals/Contains(target, "...")
EnsureUrlContains.Text("/login")

// API (Core)
GetResource.From("/posts")                PostResource.To("/posts", json)
StatusCode.OfLastResponse()               ResponseBody.OfLastResponse()
BodyIsValidJson/BodyIsArray/BodyIsObject/BodyContainsField/StatusIsSuccessful
EnsureStatusSuccessful/EnsureBodyIsArray/EnsureBodyIsObject/EnsureBodyContainsField/EnsureBodyIsValidJson
```

---

*Esta guía es la fuente de verdad de convenciones para agentes AI.
Si el código y esta guía divergen, primero corrige el código; solo actualiza
esta guía cuando el framework cambie deliberadamente.*
