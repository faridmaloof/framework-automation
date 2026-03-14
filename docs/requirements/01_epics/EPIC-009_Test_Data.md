# EPIC-009: Test Data Management (builders/seeders/isolation)

> **Descripción corta:** Builders y seeders como “first-class citizens” con idempotencia, aislamiento para paralelismo y trazabilidad hacia reportes.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-009 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Alta |
| **Fase del Roadmap** | Fase 2 (Meses 4-6) |
| **Owner** | SDET Lead |
| **Tech Lead** | Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Habilitar creación y provisión de datos de prueba escalable, determinística y segura para suites paralelas (UI/API), evitando dependencias manuales.

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-009-001 | Builders base (Bogus) + convenciones + seeds determinísticos | Alta | Pendiente | TBD | QA Automation |
| FEAT-009-002 | Seeders/provisioning + idempotencia + cleanup hooks | Crítica | Pendiente | TBD | SDET Lead |
| FEAT-009-003 | Aislamiento de datos para paralelismo + trazabilidad | Alta | Pendiente | TBD | Tech Lead |
| FEAT-009-004 | Catálogo/gobernanza de TestData + shared-modules | Media | Pendiente | TBD | QA Platform Owner |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-005 | Relacionado | Seeders consumen ApiPools + auth | Pendiente |
| EPIC-003 | Relacionado | Paralelismo exige aislamiento | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 6.5 Data builders/seeders, 6.7 Paralelismo
- `docs/especifications/automation_framework_architecture_option.md` → Data/Provisioning en framework-core

