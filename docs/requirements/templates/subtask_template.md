# SUBTASK-{EPIC}-{FEAT}-{TASK}-{NN}: {Título de la Sub-Task}

**Task Padre:** [TASK-{EPIC}-{FEAT}-{TASK}](../TASK-{EPIC}-{FEAT}-{TASK}_{slug}.md)

**Feature Anidado:** [FEAT-{EPIC}-{FEAT}](../../../../02_features/epic-{EPIC}/FEAT-{EPIC}-{FEAT}_<slug>.md)

> **Descripción corta:** {Una línea describiendo el paso técnico específico a implementar}

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | SUBTASK-{EPIC}-{FEAT}-{TASK}-{NN} |
| **Task Padre** | [TASK-{EPIC}-{FEAT}-{TASK}](../TASK-{EPIC}-{FEAT}-{TASK}_{slug}.md) |
| **Estado** | Pendiente / En Progreso / Completada |
| **Prioridad** | Alta / Media / Baja |
| **Tamaño Estimado** | <1h / 1-2h / 2-4h |
| **Estimación Horas** | {N} horas (máximo 4 horas por sub-task) |
| **Assignee** | {Developer responsable} |
| **Reviewer** | {Senior Dev / Tech Lead} |
| **Fecha Límite** | {YYYY-MM-DD} |

---

## 🎯 Objetivo de la Sub-Task

{Descripción ultra-específica del paso técnico a implementar. Debe ser tan concreta que cualquier developer pueda ejecutarla sin ambigüedad.}

### Criterio de Éxito

{¿Cómo sabremos que esta sub-task está completada? Una oración muy específica.}

---

## 📖 Instrucciones Técnicas Detalladas

### Pasos a Seguir

1. {Paso 1 específico y accionable}
2. {Paso 2 específico y accionable}
3. {Paso 3 específico y accionable}

### Código de Referencia

#### Antes (si aplica)

```csharp
// Estado actual del código (si es una modificación)
```

#### Después (esperado)

```csharp
// Código esperado después de completar la sub-task
namespace Alvea.{Domain}.{Type};

public class {ClassName}
{
    // Implementación esperada
}
```

### Archivos a Crear/Modificar

| Archivo | Acción | Líneas Estimadas |
|---------|--------|------------------|
| `/src/Alvea.{Module}/{Path}/{File}.cs` | Crear / Modificar | ~{N} líneas |

---

## ✅ Criterios de Aceptación

- [ ] Código implementado exactamente como se especifica
- [ ] Compila sin errores ni warnings
- [ ] Tests unitarios asociados pasan (si aplica)
- [ ] Code review aprobado por Senior Dev
- [ ] Integrado con rama develop

---

## 🔗 Dependencias

### Bloqueantes

| ID | Tipo | Título | Estado |
|----|------|--------|--------|
| SUBTASK-{X}-{Y}-{Z}-{N} | Sub-Task | {Título} | Pendiente / Completada |

### Relacionadas

| ID | Tipo | Título | Estado |
|----|------|--------|--------|
| SUBTASK-{X}-{Y}-{Z}-{N} | Sub-Task | {Título} | Pendiente |

---

## 📝 Notas de Implementación

> **Espacio para que el developer documente el trabajo realizado.**

### Decisiones Técnicas

| Fecha | Decisión | Racional |
|-------|----------|----------|
| {YYYY-MM-DD} | {Decisión} | {Por qué} |

---

## 🕒 Bitácora de Trabajo

| Fecha | Actividad | Horas | Developer | Notas |
|-------|-----------|-------|-----------|-------|
| {YYYY-MM-DD} | {Qué se hizo} | {N}h | {Nombre} | {Comentarios} |

**Total Horas:** {N} horas

---

## 📋 Checklist de Completitud

- [ ] Código implementado
- [ ] Compila sin errores
- [ ] Tests pasan (si aplica)
- [ ] Code review aprobado
- [ ] Sub-task marcada como completada

---

**Estado Actual:** {Pendiente / En Progreso / Completada}

**Última Actualización:** {YYYY-MM-DD}

---

> **NOTA:** Las sub-tasks son opcionales. Usar solo cuando una task requiere más de 8 horas y necesita dividirse en pasos técnicos más pequeños.
