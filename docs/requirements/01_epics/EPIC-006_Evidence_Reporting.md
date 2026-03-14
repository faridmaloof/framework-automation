# EPIC-006: Evidencias + reporting adapters (Allure/ReportPortal)

> **Descripción corta:** Evidence Manager + estrategia por configuración y adapters de reporting desacoplados del proveedor.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-006 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Alta |
| **Fase del Roadmap** | Fase 3 (Meses 7-9) |
| **Owner** | QA Platform Owner |
| **Tech Lead** | Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Normalizar evidencias (screenshots/video/traces/logs) y reporting enterprise (Allure/ReportPortal) con contratos internos que permitan cambiar de proveedor sin reescribir tests.

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-006-001 | Evidence Manager core + EvidenceStrategy (config-driven) | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-006-002 | Layout de artefactos + retención + “latest” | Alta | Pendiente | TBD | DevOps/SRE |
| FEAT-006-003 | Contrato `ITestReporter` + event model + attachments | Crítica | Pendiente | TBD | Tech Lead |
| FEAT-006-004 | Adapters Allure y ReportPortal (pluggable) | Alta | Pendiente | TBD | QA Platform Owner |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-003 | Pre-requisito | Hooks disparan captura y eventos | Pendiente |
| EPIC-004 | Relacionado | Evidencias UI (video/trace/screenshot) | Pendiente |
| EPIC-005 | Relacionado | Evidencias API (req/resp sanitizada) | Pendiente |
| EPIC-008 | Pre-requisito | Masking y secrets | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 6.6 Reporting y evidencias
- `docs/especifications/automation_framework_architecture_option.md` → 11 Evidencias, 12 Artefactos, 13 Reporting

