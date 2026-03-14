# EPIC-002: Motor Screenplay (core) propio

> **Descripción corta:** Implementar un Screenplay Pattern propio (Actor/Ability/Task/Interaction/Question) con instrumentación transversal y compatibilidad con ejecución paralela.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-002 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Crítica |
| **Fase del Roadmap** | Fase 1 (Meses 1-3) |
| **Owner** | SDET Lead |
| **Tech Lead** | Principal Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Entregar el **núcleo Screenplay** como modelo mental unificado para UI/API/Mobile, con contratos estables y extensibles para 10+ años.

### Problema que Resuelve

- Sin un core Screenplay consistente, las suites se degradan (duplicación, acoplamiento a herramientas, baja reutilización).
- La ejecución paralela se vuelve frágil si el contexto se maneja por estado global.

### Solución Propuesta

- Definir contratos `Actor`, `Ability`, `Task`, `Interaction`, `Question` (async-first).
- Asegurar scope por escenario y ausencia de estado estático compartido.
- Instrumentación transversal (eventos) para logging/metrics/reporting sin acoplar a proveedores.

---

## 📖 Contexto de Negocio y Técnico

### Contexto de Negocio

- Aumenta velocidad de creación de tests y reduce costos de mantenimiento.
- Facilita onboarding: “hablar” en términos de negocio (Tasks) y no de herramientas.

### Contexto Técnico

- El core debe ser agnóstico de Playwright/Appium/HttpClient.
- Debe integrarse con BDD (Reqnroll) y ejecución paralela sin comprometer thread-safety.

---

## 📊 Criterios de Éxito (Outcome-based)

| Métrica | Línea Base | Target | Cómo se mide | Frecuencia |
|---------|------------|--------|--------------|------------|
| Reutilización de Tasks | {hoy} | ≥ 60% | Catálogo + review | Trimestral |
| Errores por contexto compartido | {hoy} | 0 críticos | Post-mortems | Mensual |
| Backwards compatibility | {hoy} | SemVer | Releases | Por release |

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-002-001 | Actor model (identidad + memory + abilities) | Crítica | Pendiente | TBD | SDET Lead |
| FEAT-002-002 | Lifecycle de Abilities (setup/teardown por escenario) | Alta | Pendiente | TBD | Tech Lead |
| FEAT-002-003 | Contratos Task/Interaction (composición + async) | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-002-004 | Questions/Assertions + eventos de instrumentación | Alta | Pendiente | TBD | SDET Lead |

---

## 📝 Dependencias

### Dependencias Internas (entre Épicas)

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-001 | Pre-requisito | Contratos base, DI y gobernanza | Pendiente |
| EPIC-003 | Relacionado | Hooks BDD crearán Actor y scope | Pendiente |

---

## ⚠️ Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación | Owner |
|--------|--------------|---------|------------|-------|
| Contratos demasiado verbosos | Media | Medio | DX + helpers + ejemplos | SDET Lead |
| Estado compartido rompe paralelismo | Media | Alto | Sin estáticos, scope por escenario | Tech Lead |

---

## 📈 Roadmap y Timeline

```
Fase 1 (Meses 1-3)
├─ Sprint 1: Actor + Ability + context
├─ Sprint 2: Task/Interaction + composición
└─ Sprint 3: Questions + instrumentación
```

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 6.2 Screenplay Pattern
- `docs/especifications/automation_framework_architecture_option.md` → 9 Implementación de Screenplay

