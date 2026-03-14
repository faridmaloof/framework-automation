# TASK-{EPIC}-{FEAT}-{NN}: {Título de la Task}

**Feature Padre:** [FEAT-{EPIC}-{FEAT}](../../../02_features/epic-{EPIC}/FEAT-{EPIC}-{FEAT}_<slug>.md)

**Épica Anidada:** [EPIC-{EPIC}](../../../01_epics/EPIC-{EPIC}_<slug>.md)

> **Descripción corta:** {Una línea describiendo qué trabajo específico debe hacerse}

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | TASK-{EPIC}-{FEAT}-{NN} |
| **Feature Padre** | [FEAT-{EPIC}-{FEAT}](../../../02_features/epic-{EPIC}/FEAT-{EPIC}-{FEAT}_<slug>.md) |
| **Épica Anidada** | [EPIC-{EPIC}](../../../01_epics/EPIC-{EPIC}_<slug>.md) |
| **Estado** | Pendiente / En Progreso / En Revisión / Completada / Bloqueada |
| **Prioridad** | Crítica / Alta / Media / Baja |
| **Tamaño Estimado** | XS (<2h) / S (2-4h) / M (4-6h) / L (6-8h) |
| **Estimación Horas** | {N} horas (máximo 8 horas) |
| **Sprint Asignado** | Sprint {N} |
| **Assignee** | {Developer responsable} |
| **Reviewer** | {Tech Lead / Peer reviewer} |
| **QA Reviewer** | {QA responsable} |
| **Fecha de Creación** | {YYYY-MM-DD} |
| **Fecha Límite** | {YYYY-MM-DD} |
| **Fecha de Inicio** | {YYYY-MM-DD} |
| **Fecha de Completitud** | {YYYY-MM-DD} |

---

## 🎯 Objetivo de la Task

{Descripción clara de QUÉ se va a hacer y POR QUÉ es necesaria para cumplir el feature padre. Enfocado en el resultado funcional, no en la implementación técnica. Máximo 2-3 líneas.}

### Criterio de Éxito

{¿Cómo sabremos que esta task está completada exitosamente? Una oración clara.}

---

## 📖 Descripción Detallada

### ¿Qué se debe hacer?

{Descripción narrativa detallada de lo que se espera que esta task entregue. Enfocado en el QUÉ, no en el CÓMO.}

### ¿Por qué es necesario?

{Contexto sobre por qué esta task es importante para el feature y la épica.}

### Entregables Esperados

- ✅ {Entregable concreto 1}
- ✅ {Entregable concreto 2}
- ✅ {Entregable concreto 3}
- ✅ {Documentación actualizada (si aplica)}
- ✅ {Tests automatizados (si aplica)}

---

## ✅ Criterios de Aceptación

| # | Criterio | Tipo | Verificación | Evidencia Requerida |
|---|----------|------|--------------|---------------------|
| 1 | {Criterio de aceptación específico y medible} | Funcional | {Cómo se verifica} | {Test/Screenshot/Log} |
| 2 | {Criterio de aceptación específico y medible} | Técnico | {Cómo se verifica} | {Test/Code review} |
| 3 | {Criterio de aceptación específico y medible} | Documentación | {Cómo se verifica} | {Doc link} |

### Definición de Done (Task)

- [ ] Código implementado según especificaciones
- [ ] Tests unitarios escritos y pasando
- [ ] Code review completado (1 aprobación mín.)
- [ ] Integrado con rama principal (main/develop)
- [ ] Documentación actualizada (si aplica)
- [ ] QA notificado (si aplica)

---

## 🎨 Especificaciones Técnicas

> **NOTA:** Esta sección puede ser completada inicialmente por el Tech Lead o durante la implementación por el developer assignado.

### Enfoque de Implementación Sugerido

{Descripción de alto nivel de cómo podría implementarse. El developer puede proponer alternativas.}

### Componentes Involucrados

| Componente | Tipo | Lenguaje | Archivo/Clase | Acción |
|------------|------|----------|---------------|--------|
| {Nombre} | {Library/Adapter/Script/Docs} | {.NET/Docs} | `{Ruta/Archivo.cs}` | {Crear/Modificar} |

### Código de Referencia

#### Ejemplo de Estructura (si aplica)

```csharp
// .NET - Ejemplo de estructura
namespace AutomationPlatform.Framework.{Domain};

public interface I{ServiceName}
{
    Task<{ReturnType}> {MethodName}Async({Parameters}, CancellationToken ct = default);
}

public class {ServiceName} : I{ServiceName}
{
    private readonly ILogger<{ServiceName}> _logger;
    
    public {ServiceName}(ILogger<{ServiceName}> logger)
    {
        _logger = logger;
    }
    
    public async Task<{ReturnType}> {MethodName}Async({Parameters}, CancellationToken ct = default)
    {
        // TODO: Implementar lógica
        throw new NotImplementedException();
    }
}
```

```go
// Go - Ejemplo de estructura
package workers

type {ServiceName} interface {
    {MethodName}(ctx context.Context, params {Params}) ({ReturnType}, error)
}

type {serviceName} struct {
    logger *zap.Logger
}

func New{ServiceName}(logger *zap.Logger) {ServiceName} {
    return &{serviceName}{logger: logger}
}

func (s *{serviceName}) {MethodName}(ctx context.Context, params {Params}) ({ReturnType}, error) {
    // TODO: Implementar lógica
    return nil, errors.New("not implemented")
}
```

### Esquemas de Datos

#### Modelo de Datos (si aplica)

```csharp
// .NET Model
namespace Alvea.{Domain}.Models;

public class {EntityName}
{
    public ObjectId Id { get; set; }
    public string Nombre { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

```json
// MongoDB Document Example
{
  "_id": "ObjectId",
  "nombre": "String",
  "created_at": "ISODate",
  "updated_at": "ISODate"
}
```

### APIs / Endpoints (si aplica)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| {POST/GET/PUT/DELETE} | `/api/v1/{recurso}` | {Descripción} |

### Mensajería (si aplica)

| Topic/Queue | Tipo | Payload |
|-------------|------|---------|
| `alvea.{dominio}.{evento}` | {Pub/Sub/Queue} | `{Schema}` |

---

## 📥 Dependencias

### Bloqueantes (Debe estar listo antes de empezar)

| ID | Tipo | Título | Estado | Link |
|----|------|--------|--------|------|
| TASK-{X}-{Y}-{Z} | Tarea | {Título} | Pendiente / Completado | [Link](./TASK-{X}-{Y}-{Z}_{slug}.md) |
| FEAT-{X}-{Y} | Feature | {Título} | Pendiente / Completado | [Link](../../../02_features/epic-{X}/FEAT-{X}-{Y}_<slug>.md) |

### Relacionadas (Pueden desarrollarse en paralelo)

| ID | Tipo | Título | Estado | Link |
|----|------|--------|--------|------|
| TASK-{X}-{Y}-{Z} | Tarea | {Título} | Pendiente | [Link](./TASK-{X}-{Y}-{Z}_{slug}.md) |

### Recursos Requeridos

| Recurso | Tipo | Estado | Owner |
|---------|------|--------|-------|
| {ej: MongoDB Atlas cluster} | Infraestructura | Activo | {Nombre} |
| {ej: API Key de Gemini} | Credencial | Activo | {Nombre} |

---

## 🧪 Plan de Pruebas

### Tests Unitarios a Implementar

| Test Name | Qué Prueba | Input Esperado | Output Esperado |
|-----------|------------|----------------|-----------------|
| `{MethodName}_When_{Condition}_Should_{Result}` | {Descripción} | {Input} | {Output} |

### Tests de Integración (si aplica)

| Escenario | Componentes | Datos de Prueba |
|-----------|-------------|-----------------|
| {Escenario} | {Qué integra} | {Descripción} |

### Criterios de Aceptación de QA

- [ ] Tests unitarios pasando
- [ ] Código revisado por peer
- [ ] Integración continua pasando
- [ ] Sin degradación de performance

---

## ⚠️ Riesgos y Consideraciones

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|--------------|---------|------------|
| {Riesgo técnico} | Baja/Media/Alta | Bajo/Medio/Alto | {Cómo mitigar} |

### Notas de Seguridad

- [ ] ¿Se manejan datos sensibles? {Sí/No}
- [ ] ¿Requiere encriptación? {Sí/No}
- [ ] ¿Requiere validación de input? {Sí/No}
- [ ] ¿Requiere logging de auditoría? {Sí/No}

---

## 📝 Notas de Implementación

> **Espacio para que el developer documente decisiones técnicas, aprendizajes y problemas encontrados durante la implementación.**

### Decisiones Técnicas

| Fecha | Decisión | Racional |
|-------|----------|----------|
| {YYYY-MM-DD} | {Decisión} | {Por qué se decidió así} |

### Problemas Encontrados y Soluciones

| Fecha | Problema | Solución |
|-------|----------|----------|
| {YYYY-MM-DD} | {Problema} | {Solución aplicada} |

### Deuda Técnica Generada (si aplica)

- [ ] {Ítem de deuda técnica, si hay}

---

## 🔗 Trazabilidad

### Trazabilidad con Feature

| Feature | Relación |
|---------|----------|
| [FEAT-{EPIC}-{FEAT}](../../../02_features/epic-{EPIC}/FEAT-{EPIC}-{FEAT}_<slug>.md) | {Cómo esta task contribuye al feature} |

### Trazabilidad con SRS

| Sección SRS | Relación |
|-------------|----------|
| {Sección} | {Cómo se relaciona} |

### Trazabilidad con Código

| Repositorio | Ruta | Pull Request |
|-------------|------|--------------|
| `Alvea-AI/backend` | `/src/{ruta}/` | #{N} |
| `Alvea-AI/workers` | `/pkg/{ruta}/` | #{N} |

---

## 📎 Anexos

| Tipo | Título | Ubicación |
|------|--------|-----------|
| {Documento} | {Nombre} | {Link} |

---

## 🕒 Bitácora de Trabajo

| Fecha | Actividad | Horas | Developer | Notas |
|-------|-----------|-------|-----------|-------|
| {YYYY-MM-DD} | {Qué se hizo} | {N}h | {Nombre} | {Comentarios} |
| {YYYY-MM-DD} | {Qué se hizo} | {N}h | {Nombre} | {Comentarios} |

**Total Horas:** {N} horas

---

## 📋 Checklist de Completitud

### Pre-Desarrollo

- [ ] Criterios de aceptación claros
- [ ] Dependencias resueltas
- [ ] Recursos disponibles
- [ ] Estimación aprobada

### Durante Desarrollo

- [ ] Código implementado
- [ ] Tests unitarios escritos
- [ ] Documentación actualizada
- [ ] Code review solicitado

### Post-Desarrollo

- [ ] Code review aprobado
- [ ] Integrado a rama principal
- [ ] CI/CD pasando
- [ ] QA notificado
- [ ] Task marcada como completada

---

## 💬 Comentarios y Discusiones

| Fecha | Usuario | Comentario |
|-------|---------|------------|
| {YYYY-MM-DD} | {Nombre} | {Comentario} |
| {YYYY-MM-DD} | {Nombre} | {Comentario} |

---

**Estado Actual:** {Pendiente / En Progreso / En Revisión / Completada / Bloqueada}

**Última Actualización:** {YYYY-MM-DD}

**Próximo Paso:** {Qué sigue para esta task}

---

> **IMPORTANTE:** 
> - Cada task no debe exceder 8 horas de trabajo.
> - Si una task requiere más tiempo, debe dividirse en múltiples tasks.
> - Mantener esta documentación actualizada durante todo el ciclo de vida de la task.
