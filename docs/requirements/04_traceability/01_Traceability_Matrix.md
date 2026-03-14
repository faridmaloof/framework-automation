# 🔗 Matriz de Trazabilidad — Framework de Automatización

> **Fecha:** 2026-03-14  
> **Fuente de verdad:** `docs/requirements/01_epics/*.md`

---

## 📖 Información del Documento

| Campo | Valor |
|------|-------|
| **Nombre** | Matriz de Trazabilidad |
| **Proyecto** | Framework de Automatización (.NET 10) |
| **Owner** | QA Platform Owner / SDET Lead |
| **Versión** | 1.0 |
| **Fecha de Creación** | 2026-03-14 |
| **Última Actualización** | 2026-03-14 |

---

## 🎯 Propósito

- Mapear **Especificaciones → Épicas → Features → Tasks** para evitar drift.
- Permitir **impact analysis** cuando cambien especificaciones o decisiones arquitectónicas.
- Mantener IDs y links verificables (ver `scripts/validate-requirements.ps1`).

---

## 📊 Resumen de Cobertura (documental)

| Nivel | Total | Estado |
|------|------:|--------|
| **Épicas** | 11 | Baseline generado |
| **Features** | 44 | Baseline generado |
| **Tasks** | 132 | Baseline generado |
| **Subtasks** | 396 | Baseline generado |

---

## 🔗 Especificaciones → Épicas

| Documento | Sección | Título/Concepto | Épica(s) | Relación |
|----------|---------|-----------------|----------|----------|
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.1 | BDD y especificaciones ejecutables | EPIC-003 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.2 | Screenplay Pattern | EPIC-002 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.3 | Motores (web/móvil/API) | EPIC-004, EPIC-005, EPIC-011 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.4 | Configuración centralizada (Options/appsettings) | EPIC-008 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.5 | Data builders y seeders | EPIC-009 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.6 | Reporting y evidencias | EPIC-006 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 6.7 | Paralelismo y escalabilidad | EPIC-003 | Implementa |
| [`automation_framework.md`](../../especifications/automation_framework.md) | 9.x | NFR (mantenibilidad, flakiness, observabilidad, seguridad) | EPIC-001, EPIC-003, EPIC-007, EPIC-008 | Define/Habilita |
| [`automation_framework.md`](../../especifications/automation_framework.md) | Adicional | Métricas (Direct/Queue/Off) | EPIC-007 | Implementa |
| [`automation_framework_architecture_option.md`](../../especifications/automation_framework_architecture_option.md) | 5 | Arquitectura del repositorio | EPIC-001, EPIC-010 | Define |
| [`automation_framework_architecture_option.md`](../../especifications/automation_framework_architecture_option.md) | 10 | POM segmentado (Locators/Actions) | EPIC-004 | Implementa |
| [`automation_framework_architecture_option.md`](../../especifications/automation_framework_architecture_option.md) | 11–13 | Evidencias + reporting flow | EPIC-006 | Implementa |
| [`automation_framework_architecture_option.md`](../../especifications/automation_framework_architecture_option.md) | 15–16 | Paralelismo + métricas | EPIC-003, EPIC-007 | Implementa |
| [`automation_framework_architecture_option.md`](../../especifications/automation_framework_architecture_option.md) | 17 | Seguridad | EPIC-008 | Implementa |

---

## 🔗 Épicas → Features (índices)

| Épica | Índice de Features | Notas |
|------|---------------------|-------|
| EPIC-001 | [`02_features/epic-001/README.md`](../02_features/epic-001/README.md) | Arquitectura base + gobernanza |
| EPIC-002 | [`02_features/epic-002/README.md`](../02_features/epic-002/README.md) | Screenplay core |
| EPIC-003 | [`02_features/epic-003/README.md`](../02_features/epic-003/README.md) | BDD/Execution |
| EPIC-004 | [`02_features/epic-004/README.md`](../02_features/epic-004/README.md) | UI Playwright |
| EPIC-005 | [`02_features/epic-005/README.md`](../02_features/epic-005/README.md) | API/Contract |
| EPIC-006 | [`02_features/epic-006/README.md`](../02_features/epic-006/README.md) | Evidence/Reporting |
| EPIC-007 | [`02_features/epic-007/README.md`](../02_features/epic-007/README.md) | Observability |
| EPIC-008 | [`02_features/epic-008/README.md`](../02_features/epic-008/README.md) | Config/Security |
| EPIC-009 | [`02_features/epic-009/README.md`](../02_features/epic-009/README.md) | Test Data |
| EPIC-010 | [`02_features/epic-010/README.md`](../02_features/epic-010/README.md) | CI/CD + DX |
| EPIC-011 | [`02_features/epic-011/README.md`](../02_features/epic-011/README.md) | Mobile |

---

## 🔗 Features → Tasks (baseline)

> Convención: cada Feature tiene 3 Tasks (`001` diseño, `002` implementación/integración, `003` tests+docs).

| Feature ID | Link Feature | Tasks Folder |
|-----------|--------------|--------------|
| FEAT-001-001 | [`FEAT-001-001`](../02_features/epic-001/) | [`03_tasks/epic-001/FEAT-001-001/`](../03_tasks/epic-001/FEAT-001-001/) |
| FEAT-001-002 | [`FEAT-001-002`](../02_features/epic-001/) | [`03_tasks/epic-001/FEAT-001-002/`](../03_tasks/epic-001/FEAT-001-002/) |
| FEAT-001-003 | [`FEAT-001-003`](../02_features/epic-001/) | [`03_tasks/epic-001/FEAT-001-003/`](../03_tasks/epic-001/FEAT-001-003/) |
| FEAT-001-004 | [`FEAT-001-004`](../02_features/epic-001/) | [`03_tasks/epic-001/FEAT-001-004/`](../03_tasks/epic-001/FEAT-001-004/) |
| FEAT-002-001 | [`FEAT-002-001`](../02_features/epic-002/) | [`03_tasks/epic-002/FEAT-002-001/`](../03_tasks/epic-002/FEAT-002-001/) |
| FEAT-002-002 | [`FEAT-002-002`](../02_features/epic-002/) | [`03_tasks/epic-002/FEAT-002-002/`](../03_tasks/epic-002/FEAT-002-002/) |
| FEAT-002-003 | [`FEAT-002-003`](../02_features/epic-002/) | [`03_tasks/epic-002/FEAT-002-003/`](../03_tasks/epic-002/FEAT-002-003/) |
| FEAT-002-004 | [`FEAT-002-004`](../02_features/epic-002/) | [`03_tasks/epic-002/FEAT-002-004/`](../03_tasks/epic-002/FEAT-002-004/) |
| FEAT-003-001 | [`FEAT-003-001`](../02_features/epic-003/) | [`03_tasks/epic-003/FEAT-003-001/`](../03_tasks/epic-003/FEAT-003-001/) |
| FEAT-003-002 | [`FEAT-003-002`](../02_features/epic-003/) | [`03_tasks/epic-003/FEAT-003-002/`](../03_tasks/epic-003/FEAT-003-002/) |
| FEAT-003-003 | [`FEAT-003-003`](../02_features/epic-003/) | [`03_tasks/epic-003/FEAT-003-003/`](../03_tasks/epic-003/FEAT-003-003/) |
| FEAT-003-004 | [`FEAT-003-004`](../02_features/epic-003/) | [`03_tasks/epic-003/FEAT-003-004/`](../03_tasks/epic-003/FEAT-003-004/) |
| FEAT-004-001 | [`FEAT-004-001`](../02_features/epic-004/) | [`03_tasks/epic-004/FEAT-004-001/`](../03_tasks/epic-004/FEAT-004-001/) |
| FEAT-004-002 | [`FEAT-004-002`](../02_features/epic-004/) | [`03_tasks/epic-004/FEAT-004-002/`](../03_tasks/epic-004/FEAT-004-002/) |
| FEAT-004-003 | [`FEAT-004-003`](../02_features/epic-004/) | [`03_tasks/epic-004/FEAT-004-003/`](../03_tasks/epic-004/FEAT-004-003/) |
| FEAT-004-004 | [`FEAT-004-004`](../02_features/epic-004/) | [`03_tasks/epic-004/FEAT-004-004/`](../03_tasks/epic-004/FEAT-004-004/) |
| FEAT-005-001 | [`FEAT-005-001`](../02_features/epic-005/) | [`03_tasks/epic-005/FEAT-005-001/`](../03_tasks/epic-005/FEAT-005-001/) |
| FEAT-005-002 | [`FEAT-005-002`](../02_features/epic-005/) | [`03_tasks/epic-005/FEAT-005-002/`](../03_tasks/epic-005/FEAT-005-002/) |
| FEAT-005-003 | [`FEAT-005-003`](../02_features/epic-005/) | [`03_tasks/epic-005/FEAT-005-003/`](../03_tasks/epic-005/FEAT-005-003/) |
| FEAT-005-004 | [`FEAT-005-004`](../02_features/epic-005/) | [`03_tasks/epic-005/FEAT-005-004/`](../03_tasks/epic-005/FEAT-005-004/) |
| FEAT-006-001 | [`FEAT-006-001`](../02_features/epic-006/) | [`03_tasks/epic-006/FEAT-006-001/`](../03_tasks/epic-006/FEAT-006-001/) |
| FEAT-006-002 | [`FEAT-006-002`](../02_features/epic-006/) | [`03_tasks/epic-006/FEAT-006-002/`](../03_tasks/epic-006/FEAT-006-002/) |
| FEAT-006-003 | [`FEAT-006-003`](../02_features/epic-006/) | [`03_tasks/epic-006/FEAT-006-003/`](../03_tasks/epic-006/FEAT-006-003/) |
| FEAT-006-004 | [`FEAT-006-004`](../02_features/epic-006/) | [`03_tasks/epic-006/FEAT-006-004/`](../03_tasks/epic-006/FEAT-006-004/) |
| FEAT-007-001 | [`FEAT-007-001`](../02_features/epic-007/) | [`03_tasks/epic-007/FEAT-007-001/`](../03_tasks/epic-007/FEAT-007-001/) |
| FEAT-007-002 | [`FEAT-007-002`](../02_features/epic-007/) | [`03_tasks/epic-007/FEAT-007-002/`](../03_tasks/epic-007/FEAT-007-002/) |
| FEAT-007-003 | [`FEAT-007-003`](../02_features/epic-007/) | [`03_tasks/epic-007/FEAT-007-003/`](../03_tasks/epic-007/FEAT-007-003/) |
| FEAT-007-004 | [`FEAT-007-004`](../02_features/epic-007/) | [`03_tasks/epic-007/FEAT-007-004/`](../03_tasks/epic-007/FEAT-007-004/) |
| FEAT-008-001 | [`FEAT-008-001`](../02_features/epic-008/) | [`03_tasks/epic-008/FEAT-008-001/`](../03_tasks/epic-008/FEAT-008-001/) |
| FEAT-008-002 | [`FEAT-008-002`](../02_features/epic-008/) | [`03_tasks/epic-008/FEAT-008-002/`](../03_tasks/epic-008/FEAT-008-002/) |
| FEAT-008-003 | [`FEAT-008-003`](../02_features/epic-008/) | [`03_tasks/epic-008/FEAT-008-003/`](../03_tasks/epic-008/FEAT-008-003/) |
| FEAT-008-004 | [`FEAT-008-004`](../02_features/epic-008/) | [`03_tasks/epic-008/FEAT-008-004/`](../03_tasks/epic-008/FEAT-008-004/) |
| FEAT-009-001 | [`FEAT-009-001`](../02_features/epic-009/) | [`03_tasks/epic-009/FEAT-009-001/`](../03_tasks/epic-009/FEAT-009-001/) |
| FEAT-009-002 | [`FEAT-009-002`](../02_features/epic-009/) | [`03_tasks/epic-009/FEAT-009-002/`](../03_tasks/epic-009/FEAT-009-002/) |
| FEAT-009-003 | [`FEAT-009-003`](../02_features/epic-009/) | [`03_tasks/epic-009/FEAT-009-003/`](../03_tasks/epic-009/FEAT-009-003/) |
| FEAT-009-004 | [`FEAT-009-004`](../02_features/epic-009/) | [`03_tasks/epic-009/FEAT-009-004/`](../03_tasks/epic-009/FEAT-009-004/) |
| FEAT-010-001 | [`FEAT-010-001`](../02_features/epic-010/) | [`03_tasks/epic-010/FEAT-010-001/`](../03_tasks/epic-010/FEAT-010-001/) |
| FEAT-010-002 | [`FEAT-010-002`](../02_features/epic-010/) | [`03_tasks/epic-010/FEAT-010-002/`](../03_tasks/epic-010/FEAT-010-002/) |
| FEAT-010-003 | [`FEAT-010-003`](../02_features/epic-010/) | [`03_tasks/epic-010/FEAT-010-003/`](../03_tasks/epic-010/FEAT-010-003/) |
| FEAT-010-004 | [`FEAT-010-004`](../02_features/epic-010/) | [`03_tasks/epic-010/FEAT-010-004/`](../03_tasks/epic-010/FEAT-010-004/) |
| FEAT-011-001 | [`FEAT-011-001`](../02_features/epic-011/) | [`03_tasks/epic-011/FEAT-011-001/`](../03_tasks/epic-011/FEAT-011-001/) |
| FEAT-011-002 | [`FEAT-011-002`](../02_features/epic-011/) | [`03_tasks/epic-011/FEAT-011-002/`](../03_tasks/epic-011/FEAT-011-002/) |
| FEAT-011-003 | [`FEAT-011-003`](../02_features/epic-011/) | [`03_tasks/epic-011/FEAT-011-003/`](../03_tasks/epic-011/FEAT-011-003/) |
| FEAT-011-004 | [`FEAT-011-004`](../02_features/epic-011/) | [`03_tasks/epic-011/FEAT-011-004/`](../03_tasks/epic-011/FEAT-011-004/) |

---

## 📝 Observaciones / Próximos pasos

- Esta matriz es un baseline. El refinamiento fino (mapeo a secciones exactas, ADRs y suites de pruebas) se completa durante implementación.
- Para estado por épica: ver [`03_Implementation_Status.md`](./03_Implementation_Status.md).

