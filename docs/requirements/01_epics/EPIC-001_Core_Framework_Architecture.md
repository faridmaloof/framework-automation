# EPIC-001: Arquitectura base y gobernanza del framework

> **Descripción corta:** Estructura de repositorio, contratos públicos, modularidad y reglas de evolución para un framework mantenible 10+ años.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-001 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Crítica |
| **Fase del Roadmap** | Fase 1 (Meses 1-3) |
| **Owner** | QA Platform Owner / SDET Lead |
| **Tech Lead** | Principal Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Definir e implementar la **arquitectura base** del framework (módulos, contratos, empaquetado y gobernanza) para que múltiples equipos puedan construir automatización sin acoplamiento y sin deuda estructural.

### Problema que Resuelve

- Backlogs de automatización tienden a degradarse cuando no existen contratos estables, estructura clara y reglas de evolución.
- Sin gobernanza, cada equipo crea “frameworks paralelos”, reduciendo reutilización y aumentando flakiness.

### Solución Propuesta

- Establecer una arquitectura por capas con **módulos claros** (`framework-core`, `shared-modules`, `automation-project`).
- Definir **contratos públicos versionados** y una estrategia de extensibilidad (plugins/adapters).
- Definir estándares (naming, estructura, DoD) y proceso de release (SemVer + deprecaciones).

---

## 📖 Contexto de Negocio y Técnico

### Contexto de Negocio

- Reduce lead time de automatización y mejora la confiabilidad de releases.
- Permite escalado multi-equipo con reutilización real y trazabilidad.

### Contexto Técnico

- Define límites de módulos, dependencias permitidas, estabilidad de APIs y estrategia de empaquetado.
- Habilita que UI/API/Mobile/Reporting/Observability se desarrollen como adapters sin romper tests existentes.

### Stack Tecnológico Asociado

| Capa | Tecnología | Versión | Justificación |
|------|------------|---------|---------------|
| Runtime | .NET | 10 | Base del framework, soporte enterprise |
| Lenguaje | C# | 13 | Ecosistema moderno |
| BDD | Reqnroll | latest | Estándar BDD en .NET |
| Runner | xUnit | v3 | Paralelismo + lifecycle |
| UI Engine | Playwright | latest | Motor UI principal |
| Observabilidad | OpenTelemetry/Prometheus/Grafana | latest | Métricas/trazas |

---

## 👥 Stakeholders y Roles

| Rol | Persona/Equipo | Responsabilidad | Expectativa |
|-----|----------------|-----------------|-------------|
| Sponsor (Quality) | {Área} | Priorizar inversión | Reducción riesgo/tiempo |
| SDET Lead | {Nombre} | Estándares del framework | Consistencia + escalabilidad |
| QA Automation | {Equipo} | Consumidores primarios | “Golden path” claro |
| Dev Teams | {Equipos} | Consumidores secundarios | Ejecución local/CI fácil |
| DevOps/SRE | {Equipo} | CI/CD + Observabilidad | Runs estables + visibles |
| Security/Architecture | {Equipo} | Lineamientos | Cumplimiento |

---

## 📊 Criterios de Éxito (Outcome-based)

| Métrica | Línea Base | Target | Cómo se mide | Frecuencia |
|---------|------------|--------|--------------|------------|
| Tiempo onboarding nuevo SDET | {hoy} | ≤ 2h | Guía + checklist | Mensual |
| Reutilización (Tasks/Interactions) | {hoy} | ≥ 60% | Catálogo + review | Trimestral |
| Cambios breaking del framework | {hoy} | 0 por release | SemVer + CI gates | Por release |

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-001-001 | Estructura de repositorio y solución (core/shared/project) | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-001-002 | DI + registro de módulos (ServiceCollection extensions) | Alta | Pendiente | TBD | Tech Lead |
| FEAT-001-003 | Contratos públicos + modelo de plugins/adapters | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-001-004 | Packaging, SemVer, releases y política de deprecación | Alta | Pendiente | TBD | QA Platform Owner |

---

## 📝 Dependencias

### Dependencias Internas (entre Épicas)

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-002 | Relacionado | Screenplay consume contratos y módulos base | Pendiente |
| EPIC-003 | Relacionado | BDD/Execution requiere estructura y DI base | Pendiente |

### Dependencias Externas

| Dependencia | Tipo | Proveedor | Estado | Mitigación |
|-------------|------|-----------|--------|------------|
| .NET SDK 10 | Toolchain | Microsoft | TBD | Pin de versiones + CI |
| Feed NuGet interno | Infraestructura | Org | TBD | Alternativa: local packages |

---

## ⚠️ Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación | Owner |
|--------|--------------|---------|------------|-------|
| Diseño demasiado rígido | Media | Alto | Extensión por adapters + DI | Tech Lead |
| Falta de disciplina multi-equipo | Alta | Alto | DoD + PR gates + ownership | SDET Lead |
| Breaking changes por refactors | Media | Alto | SemVer + deprecations + CI | QA Platform Owner |

---

## 📈 Roadmap y Timeline

```
Fase 1 (Meses 1-3)
├─ Sprint 1: Estructura base + contratos mínimos
├─ Sprint 2: DI + packaging inicial
└─ Sprint 3: Gobernanza (DoD, deprecaciones, guías)
```

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` (Visión 10 años, pilares 6.x, NFR 9.x)
- `docs/especifications/automation_framework_architecture_option.md` (Arquitectura del repo, capas, desacoplamiento)

---

## 📎 Anexos y Referencias

- `docs/requirements/00_Architecture_Understanding_Summary.md`
- `docs/requirements/00_Ideal_Framework_Design.md`

---

## 📋 Checklist de Definición de Done (Épica)

- [ ] Contratos y límites de módulos definidos y documentados
- [ ] Estrategia de versionado + deprecación publicada
- [ ] “Golden path” documentado (crear proyecto, correr local/CI)
- [ ] CI con quality gates mínimos

---

## 💬 Notas de Reuniones y Decisiones

- (TBD) Registrar ADRs clave: límites de módulos, plugin model, SemVer.

