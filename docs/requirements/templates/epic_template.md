# EPIC-{NN}: {Título de la Épica}

> **Descripción corta:** {Una línea describiendo el propósito de esta épica}

---

## 📋 Información General

| Campo | Valor |
|-------|-------|
| **ID** | EPIC-{NN} |
| **Estado** | 🟡 Planificada / 🟠 En Progreso / 🟢 Completada / 🔴 Bloqueada / ⚪ En Pausa |
| **Prioridad** | Crítica / Alta / Media / Baja |
| **Fase del Roadmap** | Fase {X} (Meses {X-Y}) |
| **Owner** | {Responsable - ej: Head of Engineering} |
| **Tech Lead** | {Líder técnico asignado} |
| **Fecha de Creación** | {YYYY-MM-DD} |
| **Última Actualización** | {YYYY-MM-DD} |
| **Sprint Objetivo** | Sprint {N} |

---

## 🎯 Objetivo de la Épica

{Descripción clara y concisa del QUÉ se quiere lograr y POR QUÉ es importante para el producto. Máximo 3-5 líneas. Enfocado en valor de negocio y técnico.}

### Problema que Resuelve

{¿Qué problema específico esta épica viene a solucionar? ¿Por qué es necesario?}

### Solución Propuesta

{Descripción de alto nivel de cómo se abordará el problema.}

---

## 📖 Contexto de Negocio y Técnico

### Contexto de Negocio

{¿Qué necesidad del negocio satisface? ¿Qué métrica de negocio impacta?}

### Contexto Técnico

{¿Qué componentes del sistema afecta? ¿Qué decisiones arquitectónicas clave hay?}

### Stack Tecnológico Asociado

| Capa | Tecnología | Versión | Justificación |
|------|------------|---------|---------------|
| Runtime | .NET | 10 | Base del framework, soporte enterprise |
| Lenguaje | C# | 13 | Modernidad del lenguaje y ecosistema .NET |
| BDD | Reqnroll | latest | Gherkin + hooks + integración con runners .NET |
| Runner | xUnit | v3 | Paralelismo avanzado + lifecycle moderno |
| UI Engine | Playwright | latest | Auto-waiting, context isolation, tracing, paralelismo |
| Mobile Engine | Appium | latest | Automatización Android/iOS (fase avanzada) |
| API Engine | HttpClient / RestSharp | latest | Pruebas API tipadas y reutilizables |
| Reporting | Allure / ReportPortal | latest | Reporting enterprise + evidencias |
| Observabilidad | OpenTelemetry / Prometheus / Grafana | latest | Métricas/trazas/logs para operación y mejora continua |

---

## 👥 Stakeholders y Roles

| Rol | Persona/Equipo | Responsabilidad | Expectativa |
|-----|----------------|-----------------|-------------|
| Sponsor (Quality) | {Nombre/Área} | Priorizar inversión en calidad | Reducción de riesgo y time-to-market |
| Product Owner (QA/Quality) | {Nombre} | Priorización del backlog | Valor y trazabilidad |
| Tech Lead | {Nombre} | Arquitectura del framework | Calidad técnica y consistencia |
| SDET Lead | {Nombre} | Estándares SDET + buenas prácticas | Framework escalable y usable |
| QA Automation | {Equipo} | Construcción de suites | Tests mantenibles y rápidos |
| DevOps / SRE | {Equipo} | CI/CD + observabilidad | Ejecución confiable y visible |
| Security / Architecture | {Equipo} | Lineamientos y auditoría | Cumplimiento y seguridad |

---

## 📊 Criterios de Éxito (Outcome-based)

| Métrica | Línea Base | Target | Cómo se mide | Frecuencia |
|---------|------------|--------|--------------|------------|
| {Métrica 1} | {valor actual} | {valor objetivo} | {método} | {diaria/semanal} |
| {Métrica 2} | {valor actual} | {valor objetivo} | {método} | {diaria/semanal} |
| {Métrica 3} | {valor actual} | {valor objetivo} | {método} | {diaria/semanal} |

---

## 🔗 Features Contenidas

**Total: {N} Features**

| Feature ID | Título | Prioridad | Estado | Sprint | Owner |
|------------|--------|-----------|--------|--------|-------|
| FEAT-{NN}-001 | {Título} | Crítica | Pendiente | {N} | {Nombre} |
| FEAT-{NN}-002 | {Título} | Alta | Pendiente | {N} | {Nombre} |
| FEAT-{NN}-003 | {Título} | Media | Pendiente | {N} | {Nombre} |
| FEAT-{NN}-004 | {Título} | Baja | Pendiente | {N} | {Nombre} |
| FEAT-{NN}-005 | {Título} | Media | Pendiente | {N} | {Nombre} |
| FEAT-{NN}-006 | {Título} | Baja | Pendiente | {N} | {Nombre} |

---

## 📝 Dependencias

### Dependencias Internas (entre Épicas)

| Épica | Tipo | Descripción | Estado |
|-------|------|-------------|--------|
| EPIC-{X} | Pre-requisito | {Qué debe estar listo} | Pendiente / Completado |
| EPIC-{Y} | Relacionado | {Qué puede hacerse en paralelo} | En Progreso |

### Dependencias Externas

| Dependencia | Tipo | Proveedor | Estado | Mitigación |
|-------------|------|-----------|--------|------------|
| {ej: MongoDB Atlas} | Infraestructura | MongoDB Inc. | Activo | Evaluar alternativas |
| {ej: Google Gemini API} | API Externa | Google | Activo | Fallback a OpenAI |

---

## ⚠️ Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación | Owner |
|--------|--------------|---------|------------|-------|
| {Riesgo técnico} | Baja/Media/Alta | Bajo/Medio/Alto | {Cómo mitigar} | {Nombre} |
| {Riesgo de dependencia} | Baja/Media/Alta | Bajo/Medio/Alto | {Cómo mitigar} | {Nombre} |

---

## 📈 Roadmap y Timeline

```
Fase {X} (Meses {X-Y})
├─ Sprint {N}: {Objetivo principal}
├─ Sprint {N+1}: {Objetivo principal}
└─ Sprint {N+2}: {Objetivo principal}
```

| Hito | Fecha Objetivo | Estado | Criterio de Completitud |
|------|----------------|--------|------------------------|
| Kickoff | {YYYY-MM-DD} | Pendiente | Equipo alineado, setup completo |
| Primer Feature | {YYYY-MM-DD} | Pendiente | FEAT-{NN}-001 en producción |
| Features Críticos | {YYYY-MM-DD} | Pendiente | 80% features completados |
| Épica Completada | {YYYY-MM-DD} | Pendiente | 100% features + QA sign-off |

---

## 🔗 Trazabilidad SRS

| Sección SRS | Descripción | Relación |
|-------------|-------------|----------|
| {Sección} | {Título de sección} | {Cómo se relaciona} |

---

## 📎 Anexos y Referencias

| Documento | Tipo | URL/Ubicación |
|-----------|------|---------------|
| Architecture Blueprint | Arquitectura | `/docs/architecture/Alvea_AI_Architecture_Blueprint.md` |
| SRS | Especificación | `/docs/especifications/Ampliación_SRS_Orchestrator_AI.md` |
| API Specs | Técnico | `/docs/api/` |

---

## 📋 Checklist de Definición de Done (Épica)

- [ ] Todos los Features completados y testeado
- [ ] Documentación técnica actualizada
- [ ] Tests de integración pasando
- [ ] Performance benchmarks cumplidos
- [ ] Security review completado
- [ ] QA sign-off obtenido
- [ ] Stakeholders aprobación

---

**Historial de Revisiones:**

| Versión | Fecha | Autor | Cambios | Aprobado Por |
|---------|-------|-------|---------|--------------|
| 1.0 | {YYYY-MM-DD} | {Autor} | Versión inicial | {Nombre} |
| 1.1 | {YYYY-MM-DD} | {Autor} | {Descripción de cambios} | {Nombre} |

---

## 💬 Notas de Reuniones y Decisiones

| Fecha | Tema | Decisión | Participantes |
|-------|------|----------|---------------|
| {YYYY-MM-DD} | {Tema discutido} | {Decisión tomada} | {Nombres} |

---

> **Última Actualización:** {YYYY-MM-DD}
> 
> **Próxima Revisión:** {YYYY-MM-DD}
