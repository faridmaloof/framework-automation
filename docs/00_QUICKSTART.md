# Enterprise Automation Framework - Quick Start

**Versión:** 1.0  
**Fecha:** 2026-03-14  

---

## 🚀 INICIO RÁPIDO (5 minutos)

### 1. Instalar

```bash
cd d:\framework-automation\src
dotnet restore
```

### 2. Instalar Playwright

```bash
cd Scaffolding.Tests
pwsh bin/Release/net10.0/playwright.ps1 install
```

### 3. Ejecutar Tests

```bash
cd ..
dotnet test
```

### 4. Ver Reporte

```bash
cd Scaffolding.Tests
pwsh bin/Release/net10.0/playwright.ps1 show-report allure-results
```

---

## 📖 DOCUMENTACIÓN

| Documento | Descripción |
|-----------|-------------|
| [01_ARCHITECTURE.md](01_ARCHITECTURE.md) | Arquitectura completa del framework |
| [02_USER_GUIDE.md](02_USER_GUIDE.md) | Guía de usuario detallada |
| [03_BEST_PRACTICES.md](03_BEST_PRACTICES.md) | Mejores prácticas y convenciones |

---

## 🎯 PRÓXIMOS PASOS

1. Leer [Arquitectura](01_ARCHITECTURE.md) para entender la estructura
2. Seguir [Guía de Usuario](02_USER_GUIDE.md) para crear tests
3. Revisar [Mejores Prácticas](03_BEST_PRACTICES.md) para mantener calidad

---

*Enterprise Automation Framework - Inicio Rápido*
