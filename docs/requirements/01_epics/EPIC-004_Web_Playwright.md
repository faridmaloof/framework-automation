# EPIC-004: Motor UI Web (Playwright) + POM segmentado

> **Descripción corta:** Proveer engine web (Playwright) como Ability Screenplay, con interacciones robustas, POM segmentado y soporte de evidencias.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-004 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Crítica |
| **Fase del Roadmap** | Fase 1 (Meses 1-3) |
| **Owner** | QA Platform Owner |
| **Tech Lead** | Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Habilitar automatización UI confiable y paralela con **Playwright .NET**, integrándola al core Screenplay (`BrowseWeb`) y al modelo de evidencias/reporting.

---

## 📊 Criterios de Éxito (Outcome-based)

| Métrica | Línea Base | Target | Cómo se mide | Frecuencia |
|---------|------------|--------|--------------|------------|
| Flakiness UI | {hoy} | ↓ 50% | retries + triage | Mensual |
| Tiempo UI smoke | {hoy} | < X min | CI | Por build |
| Reutilización interacciones web | {hoy} | ≥ 70% | Catálogo | Trimestral |

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-004-001 | Bootstrap Playwright + gestión de browser/context por escenario | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-004-002 | Ability `BrowseWeb` + navegación y acceso a páginas | Crítica | Pendiente | TBD | SDET Lead |
| FEAT-004-003 | Librería de interacciones web robustas (click/type/wait/assert) | Alta | Pendiente | TBD | QA Automation |
| FEAT-004-004 | POM segmentado (Locators/Actions) + convenciones y scaffolding | Alta | Pendiente | TBD | QA Automation |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-002 | Pre-requisito | BrowseWeb depende de Screenplay core | Pendiente |
| EPIC-003 | Pre-requisito | Hooks inicializan contextos y evidencias | Pendiente |
| EPIC-006 | Relacionado | Evidencias/reporting consumen artefactos UI | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 6.3 Motores, 6.6 Evidencias, 6.7 Paralelismo
- `docs/especifications/automation_framework_architecture_option.md` → 4 Stack (Playwright), 10 POM, 11 Evidencias

