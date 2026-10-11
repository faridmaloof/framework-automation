# Enterprise Automation Framework - Guía de Usuario

**Versión:** 1.0  
**Fecha:** 2026-03-14  
**Estado:** ✅ **PRODUCCIÓN**

---

## 📋 ÍNDICE

1. [Primeros Pasos](#primeros-pasos)
2. [Instalación](#instalación)
3. [Ejecutar Tests](#ejecutar-tests)
4. [Crear Nuevos Tests](#crear-nuevos-tests)
5. [Configuración](#configuración)
6. [Reportes](#reportes)
7. [Solución de Problemas](#solución-de-problemas)

---

## PRIMEROS PASOS

### Requisitos Previos

- ✅ .NET 10 SDK
- ✅ Visual Studio 2022 / Rider / VS Code
- ✅ Git (opcional)

### NO se requiere

- ❌ Node.js (Playwright para .NET es nativo)
- ❌ npm / npx

---

## INSTALACIÓN

### 1. Clonar/Descargar

```bash
cd d:\framework-automation\src
```

### 2. Restaurar Paquetes

```bash
dotnet restore Framework.Automation.sln
```

### 3. Instalar Playwright

```bash
cd Scaffolding.Tests
pwsh bin/Release/net10.0/playwright.ps1 install
```

### 4. Verificar Instalación

```bash
dotnet build --configuration Release
```

Debe mostrar:
```
Compilación correcta.
0 Errores
```

---

## EJECUTAR TESTS

### Todos los Tests

```bash
cd d:\framework-automation\src
dotnet test
```

### Tests Específicos

```bash
# Solo tests web
dotnet test --filter "TestCategory=web"

# Solo tests API
dotnet test --filter "TestCategory=api"

# Solo smoke tests
dotnet test --filter "TestCategory=smoke"

# Por nombre
dotnet test --filter "FullyQualifiedName~GoogleSearch"
```

### Con Tags Múltiples

```bash
# Web + smoke
dotnet test --filter "TestCategory=web&TestCategory=smoke"

# Excluir flaky
dotnet test --filter "TestCategory!=flaky"
```

### En Paralelo

```bash
# Release configuration (paralelo por defecto)
dotnet test --configuration Release
```

---

## CREAR NUEVOS TESTS

### Paso 1: Crear Feature File

`BDD/Features/MyFeature.feature`:

```gherkin
@web @smoke
Feature: Mi Funcionalidad
  Como usuario
  Quiero hacer algo
  Para obtener un resultado

  Scenario: Mi escenario
    Given estoy en la página
    When hago algo
    Then veo un resultado
```

### Paso 2: Crear el PageObject (locators de la página)

`Pages/MyApp/LoginPage.cs`:

```csharp
using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Pages;

namespace Scaffolding.Tests.Pages.MyApp;

public sealed class LoginPage : PageObject
{
    public static readonly LoginPage Instance = new();

    public override string Name => "Login";
    public override string Url => "https://app.example.com/login";

    // Nombre humano SIEMPRE: los fallos muestran 'Usuario' (Login), no '#username'
    public Target UserField     => ByLabel("usuario", "Usuario", "Campo de usuario");
    public Target PasswordField => ByLabel("contraseña", "Contraseña");
    public Target SubmitButton  => ByTestId("botón ingresar", "login-submit");

    public override IEnumerable<ITarget> Targets => [UserField, PasswordField, SubmitButton];
}
```

### Paso 3: Crear el Task de la página

`Pages/MyApp/LoginTasks.cs`:

```csharp
using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay.Web.Interactions;

namespace Scaffolding.Tests.Pages.MyApp;

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
```

### Paso 4: Crear Steps

`BDD/Steps/MyFeatureSteps.cs`:

```csharp
using Framework.Core.BDD;
using Framework.Core.ScreenPlay.Interfaces;
using Reqnroll;
using Scaffolding.Tests.Pages.MyApp;

namespace Scaffolding.Tests.BDD.Steps;

[Binding]
public class MyFeatureSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IActor Actor => ScenarioActor.Get(_scenarioContext);

    [Given("estoy en la página de login")]
    public Task GivenOnLoginPage()
        => Actor.AttemptsToAsync(LoginPage.Instance.Open());

    [When("inicio sesión con {string} y {string}")]
    public Task WhenSignIn(string user, string password)
        => Actor.AttemptsToAsync(new SignIn(user, password));

    [Then("debo ver el botón de salir")]
    public Task ThenLogoutVisible()
        => Actor.AttemptsToAsync(
            EnsureVisible.Of(HomePage.Instance.LogoutButton, "el login debe llevar al home"));
}
```

### Paso 5: Ejecutar

```bash
dotnet test --filter "FullyQualifiedName~MyFeature"
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
    "AutoOpenReport": false,
    "ReportPort": 56789
  }
}
```

### Configuración por Entorno

**Desarrollo** (`appsettings.Development.json`):
```json
{
  "Framework": {
    "Headless": false,
    "Evidence": {
      "Screenshot": "Always",
      "Video": "Always"
    }
  }
}
```

**Producción** (`appsettings.Production.json`):
```json
{
  "Framework": {
    "Headless": true,
    "Evidence": {
      "Screenshot": "OnFailure",
      "Video": "OnFailure"
    },
    "Parallelism": {
      "MaxWorkers": 8
    }
  }
}
```

---

## REPORTES

### Allure Report

#### 1. Ejecutar Tests

```bash
dotnet test
```

#### 2. Abrir Reporte

```bash
# Opción A: Automático (si AutoOpenReport: true)
# Se abre automáticamente al finalizar

# Opción B: Manual
cd Scaffolding.Tests
pwsh bin/Release/net10.0/playwright.ps1 show-report allure-results
```

#### 3. Generar Reporte Estático

```bash
allure generate allure-results --clean -o allure-report
allure open allure-report
```

### Evidencias

Las evidencias se guardan en:

```
evidence/
├── screenshots/
├── videos/
└── traces/
```

---

## SOLUCIÓN DE PROBLEMAS

### Error: "No se encuentra Playwright"

**Solución:**
```bash
cd Scaffolding.Tests
pwsh bin/Release/net10.0/playwright.ps1 install
```

### Error: "Tests no se ejecutan"

**Verificar:**
1. Build exitoso: `dotnet build`
2. Tests descubiertos: `dotnet test --list-tests`
3. Tags correctos: `dotnet test --filter "TestCategory=web"`

### Error: "'usuario' no fue encontrado..." (fallo de elemento)

Los mensajes de fallo ya son humanos: dicen **qué actor, qué intención y
qué elemento con nombre + página** (ej. `'Carlos' could not fill in 'Usuario' (Login)`).
El selector crudo queda en el inner exception / trace.

**Solución:**
1. Verificar el locator en el `PageObject` de la página
2. Aumentar el timeout del target: `ByTestId("botón", "id", timeout: TimeSpan.FromSeconds(60))`
3. Si el elemento aparece tras una transición, añadir una espera explícita:
   `await Actor.AttemptsToAsync(WaitFor.Visible(Página.Instance.Elemento))`

### Error: "Flaky tests"

**Solución:**
1. Agregar tag `@flaky`
2. Configurar reintentos en configuración
3. Usar waits explícitos (`WaitFor.Visible/Hidden/Attached`) en vez de esperas fijas

### Error: "Allure no se abre"

**Solución:**
```bash
# Instalar Allure CLI
choco install allure

# O usar Playwright
pwsh bin/Release/net10.0/playwright.ps1 show-report allure-results
```

---

## PREGUNTAS FRECUENTES

### ¿Puedo usar este framework para múltiples aplicaciones?

**Sí.** Cada aplicación tiene su carpeta en `Pages/`:

```
Pages/
├── Application1/          # PageObject + Component + Tasks por página
├── Application2/
└── Application3/
```

### ¿Cómo agrego una nueva base de datos?

1. Heredar de `DatabaseConnection`
2. Implementar `CreateConnection()`
3. Usar en tus tests

```csharp
public class MyDbConnection : DatabaseConnection
{
    protected override IDbConnection CreateConnection()
    {
        return new MyDbConnection(ConnectionString);
    }
}
```

### ¿Cómo configuro proxies?

En `appsettings.json`:

```json
{
  "Device": {
    "Proxy": {
      "Server": "http://proxy.company.com:8080",
      "Username": "user",
      "Password": "pass"
    }
  }
}
```

### ¿Cómo ejecuto en CI/CD?

```yaml
# Azure DevOps
- task: DotNetCoreCLI@2
  displayName: 'Run Tests'
  inputs:
    command: 'test'
    arguments: '--configuration Release --logger "trx"'
```

---

## SOPORTE

- **Documentación**: `/docs/` folder
- **Arquitectura**: `01_ARCHITECTURE.md`
- **Issues**: GitHub Issues
- **Discussions**: GitHub Discussions

---

*Enterprise Automation Framework - Guía de Usuario*
