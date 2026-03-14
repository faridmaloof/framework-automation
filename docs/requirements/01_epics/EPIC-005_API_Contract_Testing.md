# EPIC-005: Motor API + contract testing

> **Descripción corta:** Proveer ability `CallApi` con clientes tipados, pools por entorno/dominio, aserciones reutilizables y estrategia de contract testing.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-005 |
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

Estandarizar automatización de APIs bajo Screenplay, con trazabilidad (request/response sanitizada), autenticación pluggable y opción de contract testing (Pact/OpenAPI).

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-005-001 | Ability `CallApi` + HttpClientFactory + captura request/response | Crítica | Pendiente | TBD | SDET Lead |
| FEAT-005-002 | ApiPools + auth strategies (OAuth/token) por configuración | Alta | Pendiente | TBD | Tech Lead |
| FEAT-005-003 | Interactions/Questions + aserciones + validación de esquema | Alta | Pendiente | TBD | QA Automation |
| FEAT-005-004 | Contract testing (Pact/OpenAPI) + CI gating | Media | Pendiente | TBD | Architecture |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-002 | Pre-requisito | CallApi se modela como Ability | Pendiente |
| EPIC-008 | Pre-requisito | ApiPools/auth via Options + secrets | Pendiente |
| EPIC-007 | Relacionado | Métricas de estabilidad y latencia API | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 4.1 API tests, 6.3 Motores, 17 Seguridad
- `docs/especifications/automation_framework_architecture_option.md` → 4 Stack (API), 6 Engines.Api

