# Enterprise Automation Framework - Documentación

**Versión:** 1.0  
**Fecha:** 2026-03-14  
**Estado:** ✅ **PRODUCCIÓN**

---

## 📚 DOCUMENTACIÓN DISPONIBLE

### Para Nuevos Usuarios

1. **[00_QUICKSTART.md](00_QUICKSTART.md)** - Inicio rápido (5 minutos)
   - Instalación básica
   - Primeros tests
   - Enlaces a documentación completa

2. **[01_ARCHITECTURE.md](01_ARCHITECTURE.md)** - Arquitectura Completa
   - Estructura del proyecto
   - Framework.Core vs Scaffolding.Tests
   - ScreenPlay Pattern
   - Page Object Model
   - BDD con Reqnroll
   - Configuración
   - Base de Datos
   - Allure Reporting

3. **[02_USER_GUIDE.md](02_USER_GUIDE.md)** - Guía de Usuario
   - Primeros pasos
   - Instalación
   - Ejecutar tests
   - Crear nuevos tests
   - Configuración
   - Reportes
   - Solución de problemas

4. **[03_BEST_PRACTICES.md](03_BEST_PRACTICES.md)** - Mejores Prácticas
   - ScreenPlay Pattern
   - Page Object Model
   - BDD
   - Configuración
   - Tests
   - Mantenimiento
   - Performance

---

## 📁 ESTRUCTURA DE DOCUMENTOS

```
docs/
├── 00_QUICKSTART.md           # Inicio rápido
├── 01_ARCHITECTURE.md         # Arquitectura completa
├── 02_USER_GUIDE.md           # Guía de usuario
├── 03_BEST_PRACTICES.md       # Mejores prácticas
└── requirements/              # Requisitos originales (referencia)
    └── (archivos de requisitos)
```

---

## 🎯 RUTA DE APRENDIZAJE RECOMENDADA

### Día 1 - Primeros Pasos
1. Leer [Quick Start](00_QUICKSTART.md)
2. Instalar framework
3. Ejecutar tests de ejemplo

### Día 2 - Entender Arquitectura
1. Leer [Arquitectura](01_ARCHITECTURE.md)
2. Entender ScreenPlay Pattern
3. Entender POM

### Día 3 - Crear Primeros Tests
1. Seguir [Guía de Usuario](02_USER_GUIDE.md)
2. Crear primer feature file
3. Ejecutar y ver reportes

### Día 4 - Mejores Prácticas
1. Leer [Mejores Prácticas](03_BEST_PRACTICES.md)
2. Revisar código existente
3. Aplicar convenciones

---

## 📖 DOCUMENTOS DE REFERENCIA

### Arquitectura
- **ScreenPlay Pattern**: Actor, Ability, Task, Interaction, Question
- **Page Object Model**: Locators separados de Actions
- **BDD**: Gherkin + Reqnroll

### Configuración
- **appsettings.json**: Configuración base
- **AllureSettings**: Reportes
- **Database**: 8 motores soportados

### Herramientas
- **PomHelpers**: Wait helpers, click helpers
- **AllureReportHelper**: Generar y abrir reportes
- **DatabaseConnection**: Base para conexiones

---

## ⚠️ LO QUE NO SE DEBE HACER

1. **NO modificar Framework.Core** - Solo extender desde Scaffolding
2. **NO mezclar selectores con comportamiento** - Usar POM
3. **NO hardcodear datos** - Usar configuración
4. **NO commitear sin build** - Siempre verificar build
5. **NO ignorar warnings** - Mantener código limpio

---

## 🆘 SOPORTE

### Problemas Comunes

| Problema | Documento |
|----------|-----------|
| Instalación | [02_USER_GUIDE.md](02_USER_GUIDE.md#instalación) |
| Crear tests | [02_USER_GUIDE.md](02_USER_GUIDE.md#crear-nuevos-tests) |
| Configuración | [01_ARCHITECTURE.md](01_ARCHITECTURE.md#configuración) |
| Mejores prácticas | [03_BEST_PRACTICES.md](03_BEST_PRACTICES.md) |

### Canales de Soporte

- **Documentación**: `/docs/` folder
- **Issues**: GitHub Issues
- **Discussions**: GitHub Discussions

---

*Enterprise Automation Framework - Documentación Principal*
