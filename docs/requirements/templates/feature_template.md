# FEAT-{EPIC}-{NN}: {Título del Feature}

**Épica Padre:** [EPIC-{EPIC}](../../01_epics/EPIC-{EPIC}_<slug>.md)

> **Descripción corta:** {Una línea describiendo qué capacidad funcional entrega este feature}

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | FEAT-{EPIC}-{NN} |
| **Épica Padre** | [EPIC-{EPIC}](../../01_epics/EPIC-{EPIC}_<slug>.md) |
| **Estado** | Pendiente / En Progreso / Completado / En Pausa / Bloqueado |
| **Prioridad** | Crítica / Alta / Media / Baja |
| **Fase del Roadmap** | Fase {X} (Meses {X-Y}) |
| **Sprint Asignado** | Sprint {N} |
| **Owner (Producto)** | {Product Owner} |
| **Tech Lead** | {Líder técnico} |
| **Developers** | {Nombres} |
| **QA Owner** | {Nombre} |
| **Fecha de Creación** | {YYYY-MM-DD} |
| **Fecha Límite** | {YYYY-MM-DD} |
| **Story Points** | {3/5/8/13} |
| **Estimación Horas** | {N} horas (suma de tasks) |

---

## 🎯 Objetivo del Feature

{Descripción clara de QUÉ capacidad funcional se está entregando y POR QUÉ es necesaria para cumplir la épica padre. Máximo 2-3 líneas.}

### Valor de Negocio

{¿Qué valor concreto entrega este feature al negocio o usuario final?}

### Alcance

#### ✅ Qué INCLUYE este Feature

- ✅ {Capacidad 1}
- ✅ {Capacidad 2}
- ✅ {Capacidad 3}

#### ❌ Qué NO INCLUYE este Feature

- ❌ {Fuera de alcance 1}
- ❌ {Fuera de alcance 2}

---

## 📖 Descripción Funcional

### Historia de Usuario

```
COMO:      {rol de usuario o sistema}
QUIERO:    {qué quiero hacer o qué capacidad necesito}
PARA:      {qué valor o beneficio obtengo}
```

### Historias de Usuario Detalladas

| ID | Historia | Prioridad |
|----|----------|-----------|
| US-{N} | **Como** {rol}<br>**Quiero** {acción}<br>**Para** {beneficio} | Alta |

### Criterios de Aceptación (Functional)

#### ✅ Criterio 1: {Nombre del criterio}

```gherkin
Dado que {contexto o condición inicial}
Cuando {acción o evento}
Entonces {resultado funcional esperado observable}
```

**Evidencia Requerida:**
- [ ] Test automatizado pasando
- [ ] Screenshot / Video
- [ ] Log de ejecución

#### ✅ Criterio 2: {Nombre del criterio}

```gherkin
Dado que {contexto o condición inicial}
Cuando {acción o evento}
Entonces {resultado funcional esperado observable}
Y {condición adicional}
```

**Evidencia Requerida:**
- [ ] Test automatizado pasando
- [ ] Screenshot / Video

---

## 🎨 Comportamiento Esperado

### Flujo Principal

```
┌─────────────┐      ┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│   Inicio    │─────►│   Paso 1    │─────►│   Paso 2    │─────►│   Fin       │
└─────────────┘      └─────────────┘      └─────────────┘      └─────────────┘
```

**Descripción del Flujo:**

1. **Paso 1:** {Qué sucede, qué se espera, quién actúa}
2. **Paso 2:** {Qué sucede, qué se espera, quién actúa}
3. **Paso 3:** {Qué sucede, qué se espera, quién actúa}

### Flujos Alternativos

| ID | Escenario | Condición | Comportamiento | Resultado |
|----|-----------|-----------|----------------|-----------|
| ALT-01 | {Nombre} | {Cuándo aplica} | {Qué hace el sistema} | {Resultado} |
| ALT-02 | {Nombre} | {Cuándo aplica} | {Qué hace el sistema} | {Resultado} |

### Casos Borde

| ID | Caso | Descripción | Comportamiento Esperado |
|----|------|-------------|------------------------|
| EDGE-01 | {Nombre} | {Situación límite} | {Cómo debe comportarse} |
| EDGE-02 | {Nombre} | {Situación límite} | {Cómo debe comportarse} |

### Reglas de Negocio

| ID | Regla | Prioridad | Validación |
|----|-------|-----------|------------|
| RN-01 | {Descripción de la regla} | Crítica | {Cómo se valida} |
| RN-02 | {Descripción de la regla} | Alta | {Cómo se valida} |

---

## 📥 Inputs y 📤 Outputs

### Entradas (Inputs)

| Dato/Parámetro | Tipo | Obligatorio | Validación | Descripción | Fuente |
|----------------|------|-------------|------------|-------------|--------|
| {Parámetro} | {String/Int/Bool/JSON} | Sí/No | {Reglas de validación} | {Qué es} | {De dónde viene} |

### Salidas (Outputs)

| Dato/Resultado | Tipo | Descripción | Destino | Formato |
|----------------|------|-------------|---------|---------|
| {Resultado} | {Tipo} | {Qué es} | {A dónde va} | {JSON/XML/etc} |

### Esquemas de Datos

#### Request Example

```json
{
  "campo1": "valor1",
  "campo2": 123,
  "campo3": true
}
```

#### Response Example

```json
{
  "success": true,
  "data": {
    "resultado": "valor"
  },
  "metadata": {
    "timestamp": "2026-03-12T10:00:00Z"
  }
}
```

---

## 🏗️ Diseño Técnico

### Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                    Componentes Involucrados                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────┐      ┌─────────────┐      ┌─────────────┐     │
│  │  Componente │─────►│  Componente │─────►│  Componente │     │
│  │     A       │      │     B       │      │     C       │     │
│  └─────────────┘      └─────────────┘      └─────────────┘     │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Componentes por Implementar

| Componente | Tipo | Lenguaje | Responsabilidad | Owner |
|------------|------|----------|-----------------|-------|
| {Nombre} | {Library/Adapter/Package/Script} | {.NET/Docs} | {Qué hace} | {Developer/SDET} |

### Interfaces / Contratos Públicos (si aplica)

| Tipo | Contrato | Descripción | Estabilidad |
|------|----------|-------------|------------|
| Interface | `{IInterfaceName}` | {Contrato público del framework} | Estable / Evolutivo |
| Config Key | `{Section:Key}` | {Entrada de configuración} | Estable / Evolutivo |
| CLI/Script | `{command}` | {Comando para DX/CI} | Estable / Evolutivo |

### Persistencia / Artefactos (si aplica)

| Artefacto | Formato | Ubicación | Retención | Notas |
|----------|---------|----------|----------|------|
| Evidencias (UI/Mobile) | png/mp4/zip | `test-results/runs/<runId>/...` | {días} | Según EvidenceStrategy |
| Resultados reporter | allure-results / rp | `test-results/runs/<runId>/reporter/` | {días} | Provider-dependent |
| Logs | txt/json | `test-results/runs/<runId>/logs/` | {días} | Masking aplicado |

### Eventos / Telemetría (si aplica)

| Evento | Emisor | Consumidor | Propósito | Campos clave |
|-------|--------|-----------|-----------|-------------|
| `TestRunStarted` | Execution | Reporter/Metrics | Inicio de run | runId, env, tags |
| `TestCaseFinished` | Execution | Reporter/Metrics | Resultado | status, duration, retries |

---

## 🔗 Dependencias con otros Features

| Feature ID | Tipo de Dependencia | Descripción | Estado |
|------------|---------------------|-------------|--------|
| FEAT-{X}-{Y} | Pre-requisito | {Por qué depende} | Pendiente / Completado |
| FEAT-{X}-{Y} | Bloqueante | {Qué bloquea} | En Progreso |
| FEAT-{X}-{Y} | Relacionado | {Relación} | N/A |

---

## 📊 Métricas de Éxito del Feature

| Métrica | Línea Base | Target | Cómo se mide | Frecuencia |
|---------|------------|--------|--------------|------------|
| {Métrica} | {valor} | {valor} | {método} | {diaria/semanal} |

---

## 📝 Tasks Desglosadas

**Total: {N} Tasks** | **Estimado Total: {N} horas**

| Task ID | Título | Owner | Estado | Estimado | Real | Sprint |
|---------|--------|-------|--------|----------|------|--------|
| TASK-{EPIC}-{FEAT}-001 | {Título} | {Nombre} | Pendiente | 4h | - | {N} |
| TASK-{EPIC}-{FEAT}-002 | {Título} | {Nombre} | Pendiente | 8h | - | {N} |
| TASK-{EPIC}-{FEAT}-003 | {Título} | {Nombre} | Pendiente | 6h | - | {N} |
| TASK-{EPIC}-{FEAT}-004 | {Título} | {Nombre} | Pendiente | 8h | - | {N} |
| TASK-{EPIC}-{FEAT}-005 | {Título} | {Nombre} | Pendiente | 4h | - | {N} |

> **Nota:** Cada task no debe exceder 8 horas. Si una task requiere más tiempo, debe atomizarse en múltiples tasks.

---

## 🧪 Plan de Pruebas

### Tests Unitarios

| Componente | Test | Cobertura Esperada | Estado |
|------------|------|-------------------|--------|
| {Clase/Método} | {Qué prueba} | {>%} | Pendiente |

### Tests de Integración

| Escenario | Componentes | Datos de Prueba | Estado |
|-----------|-------------|-----------------|--------|
| {Escenario} | {Qué integra} | {Descripción} | Pendiente |

### Tests E2E

| Flujo | Pasos | Resultado Esperado | Estado |
|-------|-------|-------------------|--------|
| {Flujo E2E} | {Pasos} | {Resultado} | Pendiente |

### Criterios de Aceptación de QA

- [ ] Todos los tests unitarios pasando (>80% cobertura)
- [ ] Todos los tests de integración pasando
- [ ] Tests E2E completados exitosamente
- [ ] Performance benchmarks cumplidos
- [ ] Security scan sin vulnerabilidades críticas
- [ ] Documentación actualizada

---

## 🔗 Trazabilidad SRS

| Sección SRS | Descripción | Relación |
|-------------|-------------|----------|
| {Sección} | {Título} | {Cómo se relaciona} |

---

## 🔗 Trazabilidad con Arquitectura

| Documento | Sección | Relación |
|-----------|---------|----------|
| Architecture Blueprint | {Sección} | {Cómo se relaciona} |

---

## 📎 Anexos

| Tipo | Título | Ubicación |
|------|--------|-----------|
| Mockup | {Nombre} | `/docs/design/` |
| API Spec | {Nombre} | `/docs/api/` |
| Diagrama | {Nombre} | `/docs/architecture/` |

---

## 💬 Notas y Decisiones de Diseño

| Fecha | Decisión | Racional | Participantes |
|-------|----------|----------|---------------|
| {YYYY-MM-DD} | {Decisión} | {Por qué} | {Nombres} |

---

## 📋 Definición de Done (Feature)

- [ ] Todas las tasks completadas
- [ ] Criterios de aceptación verificados
- [ ] Tests unitarios pasando (>80% cobertura)
- [ ] Tests de integración pasando
- [ ] Code review aprobado (2 aprobaciones mín.)
- [ ] Documentación técnica actualizada
- [ ] QA sign-off obtenido
- [ ] Deploy a staging exitoso
- [ ] Aprobación del Product Owner

---

**Historial de Revisiones:**

| Versión | Fecha | Autor | Cambios | Aprobado Por |
|---------|-------|-------|---------|--------------|
| 1.0 | {YYYY-MM-DD} | {Autor} | Versión inicial | {Nombre} |

---

## 🕒 Bitácora de Trabajo

| Fecha | Task | Actividad | Developer | Horas | Notas |
|-------|------|-----------|-----------|-------|-------|
| {YYYY-MM-DD} | TASK-{X} | {Qué se hizo} | {Nombre} | {N}h | {Comentarios} |

---

> **Estado Actual:** {Pendiente / En Progreso / Completado}
> 
> **Última Actualización:** {YYYY-MM-DD}
> 
> **Próximo Hito:** {Hito y fecha}
