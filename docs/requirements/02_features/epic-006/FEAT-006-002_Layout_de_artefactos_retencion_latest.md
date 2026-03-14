<!-- AUTO-GENERATED: scripts/generate-requirements.ps1 -->
# FEAT-006-002: Layout de artefactos + retención + “latest”

**Épica Padre:** [EPIC-006: Evidencias + reporting adapters (Allure/ReportPortal)](../../01_epics/EPIC-006_Evidence_Reporting.md)

> **Descripción corta:** Entregar **Layout de artefactos + retención + “latest”** como capacidad reutilizable del framework.
>
> **Plantilla:** [feature_template.md](../../templates/feature_template.md)

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | FEAT-006-002 |
| **Épica Padre** | [EPIC-006](../../01_epics/EPIC-006_Evidence_Reporting.md) |
| **Estado** | Pendiente |
| **Prioridad** | Alta |
| **Fase del Roadmap** | Fase 3 (Meses 7-9) |
| **Sprint Asignado** | TBD |
| **Tasks** | 3 |
| **Estimación Total (Tasks)** | 14h |

---

## 🎯 Objetivo del Feature

Entregar la capacidad **FEAT-006-002** (*Layout de artefactos + retención + “latest”*) dentro de la épica **EPIC-006**, con contratos estables, configuración tipada (si aplica) y soporte para ejecución paralela.

---

## 📖 Descripción Funcional (framework)

### Historia de Usuario (SDET)

```text
COMO:   SDET / QA Automation
QUIERO: Layout de artefactos + retención + “latest”
PARA:   escribir pruebas mantenibles, paralelas y observables sin acoplarme a herramientas específicas
```

### Criterios de Aceptación (mínimo)

- [ ] Existe un contrato público (interfaces/config) para esta capacidad.
- [ ] La implementación es extensible (adapters/plugins) sin romper tests existentes.
- [ ] Hay ejemplos de uso y tests mínimos del framework (unit/integración según aplique).

---

## 🏗️ Diseño Técnico (resumen)

- **Contratos/Interfaces:** TBD (definir en Task 001).
- **Configuración:** TBD (Options + validación si aplica).
- **Observabilidad:** emitir logs/métricas/trazas relevantes sin overhead excesivo.

---

## 📝 Tasks (≤8h c/u)

| Task ID | Título | Estimado | Link |
|---------|--------|----------|------|
| TASK-006-002-001 | Diseño, contratos y configuración para Layout de artefactos + retención + “latest” | 4h | [Link](../../03_tasks/epic-006/FEAT-006-002/TASK-006-002-001_Diseno_contratos_y_configuracion_para_Layout_de_artefactos_retencion_latest.md) |
| TASK-006-002-002 | Implementación e integración (DI/lifecycle/observability) de Layout de artefactos + retención + “latest” | 6h | [Link](../../03_tasks/epic-006/FEAT-006-002/TASK-006-002-002_Implementacion_e_integracion_DI_lifecycle_observability_de_Layout_de_artefactos_retencion_latest.md) |
| TASK-006-002-003 | Tests mínimos + documentación + ejemplo de uso para Layout de artefactos + retención + “latest” | 4h | [Link](../../03_tasks/epic-006/FEAT-006-002/TASK-006-002-003_Tests_minimos_documentacion_ejemplo_de_uso_para_Layout_de_artefactos_retencion_latest.md) |

---

## 🧪 Plan de Pruebas (mínimo)

- Unit tests del core del feature.
- Smoke/integration test demostrando uso (cuando aplique) sin dependencias externas no determinísticas.

---

## 🔗 Trazabilidad

- **Especificación:** `docs/especifications/automation_framework.md`
- **Blueprint:** `docs/especifications/automation_framework_architecture_option.md`