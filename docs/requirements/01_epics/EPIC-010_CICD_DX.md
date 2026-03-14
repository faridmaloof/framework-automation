# EPIC-010: CI/CD + Developer Experience (DX) + quality gates

> **Descripción corta:** Ejecución local/CI consistente, templates de pipelines, sharding, publicación de artefactos y una DX que reduzca fricción de adopción.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-010 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Alta |
| **Fase del Roadmap** | Fase 1–2 (Meses 1-6) |
| **Owner** | DevOps / SRE |
| **Tech Lead** | DevOps/SRE + SDET Lead |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Convertir el framework en un “producto interno” con un **golden path** claro: correr local, correr en CI, correr en paralelo y publicar evidencias/observabilidad sin fricción.

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-010-001 | DX de ejecución local (perfiles, tags, docs) | Alta | Pendiente | TBD | SDET Lead |
| FEAT-010-002 | Templates Azure DevOps (build/test/shard/artefactos) | Crítica | Pendiente | TBD | DevOps |
| FEAT-010-003 | Quality gates (analyzers/lint + validación docs) | Alta | Pendiente | TBD | Tech Lead |
| FEAT-010-004 | Scaffolding/templates (nuevo automation-project + skeletons) | Media | Pendiente | TBD | QA Platform Owner |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-003 | Pre-requisito | Tags/suites/sharding | Pendiente |
| EPIC-006 | Relacionado | Publicación de artefactos | Pendiente |
| EPIC-007 | Relacionado | Dashboards/metrics | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 7.4 DevOps/SRE, NFR performance/reliability
- `docs/especifications/automation_framework_architecture_option.md` → CI/CD Azure DevOps, estructura de artefactos

