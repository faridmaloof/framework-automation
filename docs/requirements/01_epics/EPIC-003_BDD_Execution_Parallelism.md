# EPIC-003: BDD + ejecución + paralelismo

> **Descripción corta:** Integrar Reqnroll + xUnit v3 con hooks, selección de suites/tags y ejecución paralela aislada (runner/BDD/driver).

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-003 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Crítica |
| **Fase del Roadmap** | Fase 1 (Meses 1-3) |
| **Owner** | SDET Lead |
| **Tech Lead** | Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Estandarizar el runtime de ejecución: **BDD legible**, hooks robustos, y paralelismo configurable con aislamiento de contexto/datos/evidencias.

### Problema que Resuelve

- BDD sin convenciones termina en steps frágiles y acoplados.
- Paralelismo sin aislamiento produce intermitencias y datos cruzados (flakiness).

### Solución Propuesta

- Integración de Reqnroll con hooks para crear Actor(s), configurar abilities y capturar evidencias.
- Estrategia de suites por tags (smoke/regression/etc) y sharding en CI.
- Políticas base de retries/quarantine (config-driven) con visibilidad en reporting.

---

## 📊 Criterios de Éxito (Outcome-based)

| Métrica | Línea Base | Target | Cómo se mide | Frecuencia |
|---------|------------|--------|--------------|------------|
| Duración smoke suite | {hoy} | < X min | CI dashboards | Por build |
| Paralelismo efectivo | {hoy} | ≥ 4 workers | CI | Por build |
| Flakiness atribuible a aislamiento | {hoy} | ↓ 50% | tests_retried | Mensual |

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-003-001 | Integración base Reqnroll (estructura + convenciones) | Crítica | Pendiente | TBD | SDET Lead |
| FEAT-003-002 | Hooks y lifecycle (Actor, abilities, teardown) | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-003-003 | Paralelismo + aislamiento (xUnit/Reqnroll) + retry policy | Alta | Pendiente | TBD | Tech Lead |
| FEAT-003-004 | Selección de suites/tags + sharding para CI | Alta | Pendiente | TBD | DevOps/SRE |

---

## 📝 Dependencias

### Dependencias Internas (entre Épicas)

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-001 | Pre-requisito | Estructura base + DI | Pendiente |
| EPIC-002 | Pre-requisito | Actor/Abilities/Tasks | Pendiente |
| EPIC-008 | Pre-requisito | Config tipada + validación | Pendiente |

---

## ⚠️ Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación | Owner |
|--------|--------------|---------|------------|-------|
| Paralelismo BDD no estable | Media | Alto | Config + aislamiento + pilotos | Tech Lead |
| Steps poco mantenibles | Media | Medio | Guía estilo + code review | SDET Lead |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 6.1 BDD, 6.7 Paralelismo, 7.x casos de uso
- `docs/especifications/automation_framework_architecture_option.md` → 14 Flujo completo, 15 Paralelismo

