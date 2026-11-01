# BuscadorTrabajo · SkillNet

Plataforma web que conecta a desarrolladores con empresas y clientes del área de sistemas. Construida con **ASP.NET Core .NET 10 (WebAPI)**, **Entity Framework Core + SQL Server** y **Blazor WebAssembly**.


---

## Integrantes · Equipo Error404

- Jordan del Aguila
- Brayan Andrés Florez

## Video demostrativo

 

---

## Descripción

**SkillNet** centraliza oportunidades del área de sistemas: desarrollo web y móvil, backend, frontend, bases de datos, ciencia de datos, inteligencia artificial, ciberseguridad, DevOps e IoT.

Hay dos tipos de publicación:

| Tipo | Quién publica | Qué incluye |
|---|---|---|
| **Empleo** | Empresas | Salario, tipo de contrato y nivel de experiencia |
| **Servicio** | Empresas o clientes particulares | Presupuesto y plazo de entrega en días |

Los **desarrolladores** registran su perfil profesional (educación, experiencia, certificaciones y habilidades) y **aplican** a las publicaciones. Cada aplicación avanza por estados (Pendiente → En revisión → Aceptada / Rechazada / Retirada).

**Objetivo general:** implementar una solución web completa (WebAPI + base de datos + Blazor WebAssembly) para publicar oportunidades tecnológicas, registrar perfiles profesionales y gestionar aplicaciones, con CRUD funcional sobre las entidades del negocio.

## Tecnologías

| Tecnología | Uso |
|---|---|
| ASP.NET Core .NET 10 | WebAPI con controladores |
| Entity Framework Core 10 | DbContext, Fluent API y migraciones |
| SQL Server | Base de datos |
| Swagger (Swashbuckle) y Postman | Pruebas del WebAPI |
| Blazor WebAssembly | Interfaz de usuario |
| SweetAlert2 | Confirmaciones y mensajes |
| Git y GitHub | Control de versiones (una rama por integrante y Pull Requests) |

## Estructura de la solución

| Proyecto | Responsabilidad |
|---|---|
| `BuscadorTrabajo.Shared` | Entidades con sus validaciones (DataAnnotations e `IValidatableObject`), DTOs del registro de desarrolladores y el atributo `UrlOpcional` |
| `BuscadorTrabajo.API` | `DataContext`, `SeedDb`, migraciones, 14 controladores y manejo de errores de base de datos |
| `BuscadorTrabajo.WEB` | Blazor WebAssembly: páginas, formularios, componentes compartidos y repositorio HTTP |

---

## Modelo de datos

**14 entidades:** `Administrador`, `Empresa`, `Cliente`, `Desarrollador`, `PerfilProfesional`, `Experiencia`, `Educacion`, `Certificacion`, `Categoria`, `Habilidad`, `Perfil_Habilidad`, `Publicacion`, `Publicacion_Habilidad` y `Aplicacion`.

| Relación | Cardinalidad |
|---|---|
| Empresa → Publicación | 1:N |
| Cliente → Publicación | 1:N (solo servicios) |
| Categoría → Publicación | 1:N |
| Categoría → Habilidad | 1:N |
| Desarrollador → PerfilProfesional | 1:0..1 |
| Desarrollador → Aplicación | 1:N |
| Publicación → Aplicación | 1:N |
| PerfilProfesional → Experiencia / Educación / Certificación | 1:N |
| PerfilProfesional ↔ Habilidad | N:M (`Perfil_Habilidad`) |
| Publicación ↔ Habilidad | N:M (`Publicacion_Habilidad`) |

**Restricciones en la base de datos:**
- Índices únicos en NIT, cédulas, nombre de categoría, habilidad por categoría, pares de las tablas intermedias y aplicación por publicación y desarrollador.
- Al borrar un perfil se borra en cascada su educación, experiencia, certificaciones y habilidades. Al borrar una publicación se borran sus habilidades.
- Todo lo demás está protegido (`Restrict`): no se puede borrar algo que está en uso.


---

## SeedDb implementado

`BuscadorTrabajo.API/Data/SeedDb.cs` se ejecuta al iniciar el API:

1. Aplica las migraciones pendientes.
2. Revisa cada tabla con `AnyAsync()` y solo inserta si está vacía. Así, **reiniciar no duplica datos**.
3. Busca las relaciones por datos naturales (NIT, cédula, nombre), no por Id fijo.

| Entidad | Registros |
|---|---|
| Administrador | 1 |
| Categorías | 10 (Desarrollo web, Desarrollo móvil, Backend, Frontend, Bases de datos, Ciencia de datos, Inteligencia artificial, Ciberseguridad, DevOps, IoT) |
| Habilidades | 10 (JavaScript, Flutter, C#, React, SQL Server, Python, TensorFlow, Pruebas de penetración, Docker, Arduino) |
| Empresas, Clientes, Desarrolladores | 10 cada una |
| Perfiles profesionales, Experiencias, Educaciones, Certificaciones | 10 cada una |
| Publicaciones | 10 (6 empleos y 4 servicios, todas publicadas) |
| Perfiles_Habilidades, Publicaciones_Habilidades, Aplicaciones | 10 cada una |

## Usuario administrador inicial

| Campo | Valor |
|---|---|
| Nombre | Administrador del Sistema |
| Celular | 3000000000 |
| Cédula | 1000000001 |
| Correo | admin@buscadortrabajo.example.com |

Por indicación del profesor, esta entrega **no maneja contraseñas, login ni roles**. El administrador queda creado para cuando se integre la autenticación.

---

## Endpoints del WebAPI

Cada recurso tiene estos endpoints:

| Método | Ruta | Respuestas |
|---|---|---|
| `GET` | `/api/{recurso}` | 200 |
| `GET` | `/api/{recurso}/paginated?page=1&pageSize=5` | 200 → `{ page, pageSize, totalRecords, totalPages, data }` |
| `GET` | `/api/{recurso}/{id}` | 200 · 404 |
| `POST` | `/api/{recurso}` | 200 · 400 (validación o relación inexistente) · 409 (duplicado) |
| `PUT` | `/api/{recurso}` | 200 · 400 · 404 · 409 |
| `DELETE` | `/api/{recurso}/{id}` | 204 · 404 · 409 (registro en uso) |

| Recurso | Ruta base |
|---|---|
| Administradores | `/api/administradores` |
| Aplicaciones | `/api/aplicaciones` |
| Categorías | `/api/categorias` |
| Certificaciones | `/api/certificaciones` |
| Clientes | `/api/clientes` |
| Desarrolladores | `/api/desarrolladores` |
| Educación | `/api/educacion` |
| Empresas | `/api/empresas` |
| Experiencias | `/api/experiencias` |
| Habilidades | `/api/habilidades` |
| Perfiles profesionales | `/api/perfilesprofesionales` |
| Habilidades de un perfil | `/api/perfiles_habilidades` |
| Publicaciones | `/api/publicaciones` |
| Habilidades de una publicación | `/api/publicaciones_habilidades` |

**Endpoints especiales de Desarrolladores:**

| Método | Ruta | Qué hace |
|---|---|---|
| `POST` | `/api/desarrolladores/registro-completo` | Registra en una sola transacción el desarrollador, su perfil, educación, experiencia, certificaciones y habilidades |
| `GET` | `/api/desarrolladores/{id}/perfil-completo` | Devuelve el perfil completo |
| `PUT` | `/api/desarrolladores/{id}/perfil-completo` | Actualiza todo el perfil en una transacción |
| `DELETE` | `/api/desarrolladores/{id}` | Si no tiene aplicaciones, lo borra (204). Si tiene, responde 409 con `REQUIERE_CONFIRMACION_HISTORIAL` |
| `DELETE` | `/api/desarrolladores/{id}?confirmarConservarHistorial=true` | Lo deja inactivo y conserva sus aplicaciones (200) |

---

## Interfaz Blazor (módulos)

| Ruta | Módulo |
|---|---|
| `/` | Inicio: buscador, cifras y publicaciones recientes |
| `/publicaciones` | Publicaciones: búsqueda, filtros, detalle, crear y editar (con sus habilidades) |
| `/aplicaciones` | Aplicaciones: aplicar, filtrar por estado y cambiar el estado |
| `/desarrolladores` | Desarrolladores: registro por pasos, perfil completo, edición y eliminación con doble confirmación |
| `/empresas`, `/clientes`, `/administradores` | CRUD con paginación |
| `/categorias`, `/habilidades` | Catálogos con paginación |
| `/perfiles-habilidades`, `/publicaciones-habilidades` | Tablas muchos a muchos: asignar, cambiar y quitar habilidades |

**Validaciones visibles al usuario:**
- **En el formulario:** campos en rojo con su mensaje, un resumen de errores y confirmación antes de salir con cambios sin guardar.
- **Respuestas del API:** los 400, 404 y 409 se muestran con su mensaje en una alerta.
- **API apagado:** si no se puede conectar, la aplicación muestra un aviso en lugar de caerse.

---

## Pruebas

- **Swagger y navegador:** 145 casos manuales sobre el WebAPI. Cubren el CRUD de las 14 entidades, la paginación y sus límites, los códigos 400, 404 y 409, las reglas de publicaciones y aplicaciones, las transiciones de estado, el borrado en cascada y la desactivación de desarrolladores. Todos pasaron, y los datos de prueba se limpiaron al terminar. Detalle en [BuscadorTrabajo_Postman.json]

## Resumen de funcionalidades implementadas

- [x] Proyecto Shared con entidades, validaciones y relaciones
- [x] WebAPI con EF Core, DbContext y base de datos SQL Server
- [x] Migraciones aplicadas (`Inicial`, `AgregarEstadoActivoDesarrollador`)
- [x] SeedDb con datos iniciales en las 14 entidades y usuario administrador
- [x] SeedDb sin duplicados al reiniciar
- [x] CRUD completo en el WebAPI (GET, GET por id, POST, PUT, DELETE) y paginación
- [x] Pruebas del WebAPI (Swagger / navegador)
- [x] Evidencia de pruebas en Postman
- [x] Blazor WebAssembly con servicios HTTP (patrón Repository)
- [x] Componentes Razor, formularios de creación y edición, y listados
- [x] Validaciones visuales y mensajes al usuario
- [x] Navegación entre módulos
- [x] Integración completa Blazor + WebAPI + base de datos
- [x] Video demostrativo

## Aportes por integrante

| Integrante | Aporte |
|---|---|
| Jordan del Aguila | Repositorio e integración de Pull Requests. Corrección de validaciones decimales. Controladores y entidades. Módulo de Desarrolladores (registro por pasos, perfil completo y endpoints transaccionales). documentación. |
| Brayan Andrés Florez | DataContext, migraciones y SeedDb. Controladores con paginación, validaciones y reglas de negocio. Front de Inicio, Publicaciones, Aplicaciones, Empresas, Clientes, Habilidades, Categorías, Administradores y tablas N:M. Eliminación y desactivación de desarrolladores. Pruebas |


## Fuera del alcance de esta entrega

Login, contraseñas y roles; preguntas y respuestas en publicaciones; vacantes externas de otras plataformas; recomendaciones y alertas.
