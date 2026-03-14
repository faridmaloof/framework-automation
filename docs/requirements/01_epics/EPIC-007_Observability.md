# EPIC-007: Observabilidad (logs, métricas, tracing)

> **Descripción corta:** Instrumentación end-to-end (logging estructurado, métricas y trazas) integrable con OpenTelemetry/Prometheus/Grafana.

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-007 |
| **Estado** | 🟡 Planificada |
| **Prioridad** | Alta |
| **Fase del Roadmap** | Fase 3 (Meses 7-9) |
| **Owner** | DevOps / SRE |
| **Tech Lead** | Platform/SRE + Test Automation Architect |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |
| **Sprint Objetivo** | Sprint TBD |

---

## 🎯 Objetivo de la Épica

Habilitar observabilidad operacional del framework: métricas de ejecución, tracing y logs correlacionados para detectar flakiness, cuellos de botella y degradación.

---

## 🔗 Features Contenidas

**Total: 4 Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-007-001 | Logging estructurado + correlación (RunId/Scenario/Actor) | Alta | Pendiente | TBD | SRE |
| FEAT-007-002 | Contrato `MetricsEmitter` + set de métricas base | Crítica | Pendiente | TBD | SRE |
| FEAT-007-003 | OpenTelemetry (metrics/tracing) + exporters (Direct/OTLP) | Alta | Pendiente | TBD | SRE |
| FEAT-007-004 | Dashboards Grafana + guías (cardinalidad/flakiness) | Media | Pendiente | TBD | SRE |

---

## 📝 Dependencias

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-003 | Pre-requisito | Eventos/hook points para instrumentación | Pendiente |
| EPIC-006 | Relacionado | Reporting consume correlación | Pendiente |

---

## 🔗 Trazabilidad SRS

- `docs/especifications/automation_framework.md` → 9.4 Observabilidad + sección adicional de métricas (Direct/Queue/Off)
- `docs/especifications/automation_framework_architecture_option.md` → 16 Métricas y observabilidad

