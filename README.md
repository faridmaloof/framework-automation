# Enterprise Automation Framework

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Playwright](https://img.shields.io/badge/Playwright-1.58-2EAD33?logo=playwright)](https://playwright.dev/dotnet/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Framework de automatización **empresarial, profesional y personalizable** con **2 proyectos** basado en **.NET 10**, **Playwright** y **Screenplay Pattern**.

## 🎯 Características Principales

### Testing Multi-Nivel
- ✅ **Web Testing** (Playwright) - 10+ interacciones reutilizables
- ✅ **API Testing** (RestSharp) - REST API con Screenplay
- ✅ **Performance Testing** - Load, stress, endurance
- ✅ **Unit Testing** - Tests del framework

### Screenplay Pattern
- ✅ Actor, Abilities, Tasks, Interactions, Questions
- ✅ Actor Builder (API fluente)
- ✅ Actor Memory (estado por escenario)

### POM Segmentado (Locators + Actions)
- ✅ **Locators** - Solo selectores (fáciles de mantener)
- ✅ **Actions** - Solo comportamiento (reutilizable)
- ✅ **Tasks** - Composición de acciones (nivel negocio)

### Seeds Configurables por Equipo
- ✅ **Seeds Base** - Framework.Core (estándar)
- ✅ **Seeds por Equipo** - Scaffolding.Tests (específicos)
- ✅ **Seeds por Aplicación** - Extensibles (App1, App2, App3)

### Reporting & Evidence
- ✅ **Allure Reporter** completo
- ✅ **Screenshots** Pre-Step, Post-Step, OnFailure
- ✅ **Video** recording
- ✅ **Iteraciones con reintentos**

### CI/CD Tags
```gherkin
@web @api @performance @load @stress
@smoke @regression @critical @sanity
@app1 @app2 @app3  # Por aplicación
@flaky @retry @evidence @slow
```

---

## 📦 Estructura de 2 Proyectos

```
src/
├── Framework.Core/              # ESTÁNDAR + TOOLS + SEEDS BASE
│   ├── Core/                    # Contratos (Screenplay, Exceptions)
│   ├── Configuration/           # Settings (Framework, Device, Allure)
│   ├── Engines/                 # Implementaciones (Web, API)
│   ├── Tools/                   # Utilidades (TestData, Seeds)
│   ├── Execution/               # Hooks, Evidence, Iterations
│   ├── Reporting/               # Allure, Console
│   ├── Observability/           # Metrics, Correlation, PII
│   └── Policies/                # Retry, Timeout
│
└── Scaffolding.Tests/           # PERSONALIZABLE POR EQUIPO
    ├── Configuration/           # appsettings por entorno
    ├── Pages/                   # POMs segmentados (Locators + Actions)
    │   ├── Application1/
    │   │   ├── Locators/
    │   │   └── Actions/
    │   ├── Application2/
    │   └── Application3/
    ├── Seeds/                   # Seeds específicos por equipo
    │   ├── UserSeed.cs
    │   ├── ProductSeed.cs
    │   └── CustomSeed.cs
    ├── Tasks/                   # Tasks reutilizables
    │   ├── LoginTasks.cs
    │   └── SearchTasks.cs
    ├── Features/                # Tests con tags
    │   ├── Web/
    │   │   ├── Application1/
    │   │   ├── Application2/
    │   │   └── Application3/
    │   ├── API/
    │   └── Performance/
    ├── Steps/                   # Step definitions
    └── Hooks/                   # Before/After hooks
```

---

## 🚀 Quick Start

### 1. Restaurar
```bash
cd d:\framework-automation\src
dotnet restore Framework.Automation.slnx
```

### 2. Instalar Playwright
```bash
cd Scaffolding.Tests
dotnet exec playwright.ps1 install
```

### 3. Ejecutar Tests
```bash
# Todos los tests
dotnet test Framework.Automation.slnx

# Por aplicación
dotnet test --filter "TestCategory=app1"
dotnet test --filter "TestCategory=app2"
dotnet test --filter "TestCategory=app3"

# Por tipo
dotnet test --filter "TestCategory=web"
dotnet test --filter "TestCategory=api"
dotnet test --filter "TestCategory=performance"

# Por criticidad
dotnet test --filter "TestCategory=smoke"
dotnet test --filter "TestCategory=regression"

# Combinar
dotnet test --filter "TestCategory=web&TestCategory=app1&TestCategory=smoke"
```

### 4. Ver Reporte Allure
```bash
cd Scaffolding.Tests/allure-results
allure serve
```

---

## 🎭 Screenplay Pattern - Ejemplo

### Actor
```csharp
var actor = new ActorBuilder("User")
    .WithAbility(BrowseTheWeb.With(page))
    .Build();
```

### POM Segmentado (Locators + Actions)
```csharp
// Locators (solo selectores)
public static class LoginPageLocators
{
    public const string UsernameInput = "[name='username']";
    public const string PasswordInput = "[name='password']";
    public const string SubmitButton = "[type='submit']";
}

// Actions (solo comportamiento)
public static class LoginPageActions
{
    public static IInteraction EnterUsername(string username) =>
        Enter.TheText(username).Into(LoginPageLocators.UsernameInput);
    
    public static IInteraction EnterPassword(string password) =>
        Enter.TheText(password).Into(LoginPageLocators.PasswordInput);
    
    public static IInteraction ClickSubmit() =>
        Click.On(LoginPageLocators.SubmitButton);
}
```

### Tasks (Composición de acciones)
```csharp
await actor.AttemptsToAsync(
    new LoginToApplication("user", "password")
);
```

### Seeds (Datos de test)
```csharp
// Seed base (Framework.Core)
var user = UserBuilder.Valid()
    .WithUsername("test")
    .Build();

// Seed específico (Scaffolding.Tests)
var seeder = new MyAppUserSeed("api-key");
await seeder.CreateUserWithDepartmentAsync("user", "IT");
```

---

## 🏷️ Tags para CI/CD

### Por Tipo de Test
| Tag | Descripción | Ejemplo |
|-----|-------------|---------|
| `@web` | Tests de UI Web | `@web @app1` |
| `@api` | Tests de API | `@api @smoke` |
| `@performance` | Tests de Performance | `@performance @load` |

### Por Aplicación
| Tag | Descripción | Ejemplo |
|-----|-------------|---------|
| `@app1` | Application 1 | `@app1 @regression` |
| `@app2` | Application 2 | `@app2 @smoke` |
| `@app3` | Application 3 | `@app3 @web` |

### Por Criticidad
| Tag | Descripción | Tiempo |
|-----|-------------|--------|
| `@smoke` | Critical path | < 5 min |
| `@critical` | Tests críticos | < 10 min |
| `@regression` | Suite completa | < 1 hora |

### Ejecución en CI/CD
```yaml
# Azure DevOps / GitHub Actions

# Por aplicación
dotnet test --filter "TestCategory=app1"
dotnet test --filter "TestCategory=app2"
dotnet test --filter "TestCategory=app3"

# Combinar
dotnet test --filter "TestCategory=web&TestCategory=app1"
dotnet test --filter "TestCategory=smoke&TestCategory=app1"

# Todas las aplicaciones (menos performance)
dotnet test --filter "TestCategory!=performance"
```

---

## ⚙️ Configuración

### appsettings.json
```json
{
  "Framework": {
    "Environment": "Development",
    "Headless": true,
    "Evidence": {
      "Screenshot": "PreAndPost",
      "Video": "OnFailure",
      "Trace": true
    },
    "Iterations": {
      "Enabled": true,
      "MaxRetries": 1
    },
    "Parallelism": {
      "MaxWorkers": 4
    }
  },
  "Device": {
    "Browser": "Chromium",
    "Viewport": [1920, 1080]
  },
  "Allure": {
    "ResultsDirectory": "allure-results",
    "Screenshots": {
      "Mode": "PreAndPost",
      "Quality": 90
    }
  },
  "Applications": {
    "App1": {
      "BaseUrl": "https://app1.example.com",
      "ApiKey": "xxx"
    },
    "App2": {
      "BaseUrl": "https://app2.example.com",
      "ApiKey": "yyy"
    },
    "App3": {
      "BaseUrl": "https://app3.example.com",
      "ApiKey": "zzz"
    }
  }
}
```

---

## 📖 Documentación

| Documento | Descripción |
|-----------|-------------|
| [Architecture 2 Projects](docs/ARCHITECTURE_2_PROJECTS.md) | Arquitectura detallada de 2 proyectos |
| [Allure Guide](docs/ALLURE_GUIDE.md) | Guía completa de Allure |
| [Structure 3 Projects](docs/STRUCTURE_3_PROJECTS.md) | Estructura anterior (referencia) |
| [Quick Start](docs/QUICKSTART.md) | Inicio rápido |

---

## 🔗 Dependencias

El framework incluye:
- **Playwright** (1.58.0) - Browser automation nativo .NET
- **Reqnroll** (3.3.3) - BDD
- **Allure.XUnit** (2.14.0) - Reporting profesional
- **Polly** (8.5.0) - Retry/Timeout policies
- **OpenTelemetry** (1.15.0) - Observabilidad
- **Bogus** (35.6.5) - Test data builders
- **RestSharp** (114.0.0) - API client
- **xUnit** (2.9.3) - Test runner
- **FluentAssertions** (6.12.0) - Assertions
- **FluentValidation** (11.9.0) - Validación

---

## 🤝 Contribuir

1. Fork del repositorio
2. Crear feature branch
3. Hacer cambios
4. Ejecutar tests
5. Submit PR

## 📄 Licencia

MIT License - ver [LICENSE](LICENSE)

---

*Construido con ❤️ para automatización empresarial mantenible - 2 Proyectos* 🚀
