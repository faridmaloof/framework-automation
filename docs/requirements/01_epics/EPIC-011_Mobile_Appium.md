# EPIC-011: Mobile automation (Appium) + device farms

> **Descripción corta:** Integrar Appium como engine/ability Screenplay, con interacciones móviles, configuración de capabilities y soporte futuro de device farms.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-011 |
| **Estado** | ⚪ En Pausa |
| **Prioridad** | Media |
| **Fase del Roadmap** | Fase 4 (Meses 10-12) |
| **Owner** | QA Platform Owner |
| **Tech Lead** | Mobile SDET Lead |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Habilitar automatización móvil nativa (Android/iOS) integrando Appium al modelo Screenplay, manteniendo consistencia de evidencias/reporting/observabilidad.

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-011-001 | Bootstrap Appium + Ability `UseMobileDevice` | Alta | Pendiente | TBD | Mobile SDET |
| FEAT-011-002 | Librería de interacciones móviles (tap/swipe/wait) | Alta | Pendiente | TBD | Mobile SDET |
| FEAT-011-003 | Capabilities por config + adapter para device farms | Media | Pendiente | TBD | DevOps |
| FEAT-011-004 | Evidencias móviles + reporting integration | Alta | Pendiente | TBD | QA Platform Owner |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-002 | Pre-requisito | Ability/Task/Interaction contracts | Pendiente |
| EPIC-006 | Relacionado | Evidencias y reporting | Pendiente |
| EPIC-008 | Pre-requisito | Config + secrets | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 4.1 Mobile, roadmap fase 4
- `docs/especifications/automation_framework_architecture_option.md` → 4 Stack (Appium), 18 Roadmap

