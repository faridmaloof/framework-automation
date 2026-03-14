# Plantilla de Matriz de Trazabilidad — Framework de Automatización

> Esta plantilla establece cómo mantener trazabilidad entre **especificaciones** → **épicas** → **features** → **tasks** → **subtasks** (y, cuando aplique, hacia tests/código).

---

## 📖 Información del Documento

| Campo | Valor |
|------|-------|
| **Nombre** | Matriz de Trazabilidad |
| **Proyecto** | Framework de Automatización (.NET) |
| **Owner** | {Nombre/Equipo} |
| **Versión** | 1.0 |
| **Fecha de Creación** | {YYYY-MM-DD} |
| **Última Actualización** | {YYYY-MM-DD} |

---

## 🎯 Propósito de esta Matriz

- Asegurar que cada requerimiento implementable (Epic/Feature/Task/Subtask) proviene de una necesidad documentada.
- Habilitar **impact analysis** ante cambios en especificación/arquitectura.
- Evitar “backlog drift”: IDs, links y cobertura verificables.

---

## 📊 Resumen de Cobertura

| Nivel | Total | Completados | En Progreso | Pendientes | Cobertura |
|------|------:|------------:|------------:|-----------:|----------:|
| **Épicas** | {N} | {N} | {N} | {N} | {XX}% |
| **Features** | {N} | {N} | {N} | {N} | {XX}% |
| **Tasks** | {N} | {N} | {N} | {N} | {XX}% |
| **Subtasks** | {N} | {N} | {N} | {N} | {XX}% |

---

## 🔗 Matriz de Trazabilidad Especificaciones → Épicas

| Documento | Sección | Título Sección | Épica(s) Relacionada(s) | Tipo de Relación | Cobertura |
|----------|---------|----------------|-------------------------|------------------|-----------|
| `automation_framework.md` | 6 | Pilares arquitectónicos | EPIC-002, EPIC-003, EPIC-004, EPIC-005, EPIC-006, EPIC-007, EPIC-008, EPIC-009 | Implementa | ✅/⚠️/❌ |
| `automation_framework_architecture_option.md` | 5 | Arquitectura del repositorio | EPIC-001, EPIC-010 | Define | ✅/⚠️/❌ |
| {doc} | {n} | {título} | {EPIC-XXX} | Implementa/Habilita/Define | ✅/⚠️/❌ |

**Leyenda de Cobertura:**
- ✅ Completa: cobertura suficiente + links claros
- ⚠️ Parcial: falta desglose o hay gaps
- ❌ Sin Cubrir: no existe épica relacionada

---

## 🔗 Matriz de Trazabilidad Épicas → Features

### EPIC-XXX: {Título de la Épica}

| Feature ID | Título del Feature | Relación con la Épica | Prioridad | Estado |
|------------|-------------------|----------------------|-----------|--------|
| FEAT-XXX-001 | {Título} | Implementa/Habilita | {Crítica/Alta/Media/Baja} | {Pendiente/...} |
| FEAT-XXX-002 | {Título} | Implementa/Habilita | {Crítica/Alta/Media/Baja} | {Pendiente/...} |

*(Repetir por cada épica.)*

---

## 🔗 Matriz de Trazabilidad Features → Tasks

### FEAT-XXX-YYY: {Título del Feature}

| Task ID | Título de la Task | Relación con el Feature | Estimado | Estado | Owner |
|---------|------------------|------------------------|----------|--------|-------|
| TASK-XXX-YYY-001 | {Título} | Diseño/Contratos | {Nh} | {Pendiente/...} | {Nombre} |
| TASK-XXX-YYY-002 | {Título} | Implementación/Integración | {Nh} | {Pendiente/...} | {Nombre} |
| TASK-XXX-YYY-003 | {Título} | Tests/Docs | {Nh} | {Pendiente/...} | {Nombre} |

---

## 🔗 Matriz de Trazabilidad Tasks → Subtasks

### TASK-XXX-YYY-NNN: {Título de la Task}

| Subtask ID | Título de la Subtask | Resultado esperado | Estado |
|------------|----------------------|-------------------|--------|
| SUBTASK-XXX-YYY-NNN-001 | {Título} | {Outcome} | {Pendiente/...} |
| SUBTASK-XXX-YYY-NNN-002 | {Título} | {Outcome} | {Pendiente/...} |
| SUBTASK-XXX-YYY-NNN-003 | {Título} | {Outcome} | {Pendiente/...} |

---

## 🔗 Matriz de Trazabilidad Requisitos → Tests (opcional)

| Requisito | Tipo | Suite/Tags | Evidencia | Ubicación |
|-----------|------|------------|----------|-----------|
| FEAT-XXX-YYY | Funcional | `@smoke @ui` | Allure/ReportPortal | `{link}` |
| TASK-XXX-YYY-NNN | Técnico | `@framework` | Unit tests | `{link}` |

---

## 🔗 Matriz de Trazabilidad Requisitos → Arquitectura (opcional)

| Requisito | Artefacto de Arquitectura | Relación | Link |
|-----------|---------------------------|----------|------|
| EPIC-XXX | ADR-{NNN} | Decide | `{link}` |
| FEAT-XXX-YYY | Blueprint | Detalla | `{link}` |

---

## 📊 Análisis de Impacto

### Cambios en Especificaciones

- {Cambio} → {Epics/Features afectadas} → {Riesgos} → {Mitigación}

### Cambios en Arquitectura

- {Cambio} → {Contratos impactados} → {Compatibilidad hacia atrás} → {Plan de migración}

---

## 📝 Notas y Observaciones

### Requisitos Huérfanos

- {ID} — {Descripción} — {Acción recomendada}

### Gaps Identificados

- {Gap} — {Impacto} — {Propuesta}

---

## 📋 Historial de Revisiones

| Fecha | Autor | Cambio | Motivo |
|------|-------|--------|--------|
| {YYYY-MM-DD} | {Nombre} | {Resumen} | {Motivo} |

