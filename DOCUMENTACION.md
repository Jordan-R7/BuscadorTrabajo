# BuscadorTrabajo (SkillNet): documentación completa del proyecto

Este documento recoge **todo** lo que se ha hecho en el proyecto: las decisiones de diseño, el modelo de datos, las reglas de negocio, el API, el front, las pruebas y cada problema que apareció con su solución. Sirve para estudiar el proyecto, preparar el video y explicar cada parte.

---

## Índice

1. [Información general](#1-información-general)
2. [Cómo se definió el proyecto](#2-cómo-se-definió-el-proyecto)
3. [Tecnologías](#3-tecnologías)
4. [Estructura de la solución](#4-estructura-de-la-solución)
5. [Cómo ejecutar el proyecto](#5-cómo-ejecutar-el-proyecto)
6. [Modelo de datos](#6-modelo-de-datos)
7. [Configuración de la base de datos (DataContext)](#7-configuración-de-la-base-de-datos-datacontext)
8. [Migraciones](#8-migraciones)
9. [SeedDb: datos iniciales](#9-seeddb-datos-iniciales)
10. [Reglas de negocio](#10-reglas-de-negocio)
11. [WebAPI](#11-webapi)
12. [Front (Blazor WebAssembly)](#12-front-blazor-webassembly)
13. [Validaciones: cómo responde el sistema a los errores](#13-validaciones-cómo-responde-el-sistema-a-los-errores)
14. [Eliminación de desarrolladores (desactivación)](#14-eliminación-de-desarrolladores-desactivación)
15. [Pruebas realizadas](#15-pruebas-realizadas)
16. [Bitácora de problemas y soluciones](#16-bitácora-de-problemas-y-soluciones)
17. [Flujo de trabajo con Git y GitHub](#17-flujo-de-trabajo-con-git-y-github)
18. [Aportes por integrante](#18-aportes-por-integrante)
19. [Pendientes y trabajo futuro](#19-pendientes-y-trabajo-futuro)

---

## 1. Información general

| Dato | Valor |
|---|---|
| Asignatura | Programación de Software |
| Entrega | Práctica 3: WebAPI + base de datos + Blazor WebAssembly |
| Equipo | Error404 |
| Integrantes | Jordan del Aguila, Brayan Florez |
| Proyecto | Buscador de trabajo orientado al área de sistemas |
| Nombre visible de la aplicación | **SkillNet** ("Conecta. Desarrolla. Crece.") |
| Repositorio | `BuscadorTrabajo` en GitHub (`Jordan-R7/BuscadorTrabajo`), con el profesor (`orlapez`) invitado |

**Qué es.** Una plataforma que conecta a desarrolladores con empresas y clientes del área de sistemas. Hay dos tipos de publicación:

- **Empleo:** una oferta laboral publicada por una empresa (salario, tipo de contrato, nivel de experiencia).
- **Servicio:** una solicitud de un trabajo concreto (una página web, una app, una base de datos), publicada por una empresa o por un cliente particular (presupuesto y plazo en días).

Los desarrolladores registran su perfil profesional (experiencia, educación, certificaciones y habilidades) y **aplican** a las publicaciones.

**Requisitos de la práctica que cumple:**

- WebAPI en ASP.NET Core .NET 10 con controladores.
- Entity Framework Core con SQL Server y migraciones.
- Proyecto Shared con las entidades y sus validaciones.
- SeedDb con datos iniciales (más de 2 entidades), sin duplicados al reiniciar, y un administrador inicial.
- CRUD completo probado en Swagger y Postman.
- Front en Blazor WebAssembly que consume el API.

**Condiciones que puso el profesor:**

- **Sin claves, contraseñas ni login** en esta entrega.
- El repositorio se llama `BuscadorTrabajo` y el profesor está invitado.
- Cada integrante hace **sus propios commits** y debe poder explicar su parte en el video.

---

## 2. Cómo se definió el proyecto

Estas son las decisiones que se tomaron antes de escribir código, y por qué:

1. **Idea inicial:** un buscador de trabajo para sistemas, al estilo de Computrabajo o LinkedIn.
2. **Se pensó traer vacantes reales** desde LinkedIn u otras páginas. **Se descartó:** esas plataformas no ofrecen un API abierto para eso, y la práctica pide un API propio con base de datos propia. Las vacantes las publican los usuarios dentro de la plataforma.
3. **Se exploraron otras ideas** (un juez de código, un proyecto de mapas y otras). Se volvió a la idea original porque era la más clara y la que mejor encajaba con la práctica.
4. **Se definieron 9 entidades base** y luego se completaron hasta 14 (ver sección 6). Las entidades las armó el equipo en un `.zip` y se respetó su forma de nombrar: las llaves foráneas se llaman `IdXxx` (por ejemplo `IdEmpresa`, `IdCategoria`).
5. **Se descartó "Preguntas y Respuestas"** en las publicaciones, para mantener el alcance.
6. **Se agregó el Administrador**, porque la práctica pide un usuario administrador inicial. En esta entrega no tiene permisos especiales porque no hay login, pero queda creado por el SeedDb.
7. **Se unieron y corrigieron las reglas de negocio** que estaban repetidas, y quedaron 32 (RN01 a RN32). Luego se agregó la RN33 (desactivación de desarrolladores).

---

## 3. Tecnologías

| Tecnología | Uso |
|---|---|
| .NET 10 / ASP.NET Core | WebAPI con controladores |
| Entity Framework Core 10 | Acceso a datos, Fluent API, migraciones |
| SQL Server (+ SQL Server Management Studio 22) | Base de datos |
| Swashbuckle (Swagger) | Documentación y pruebas del API en `/swagger` |
| Postman | Pruebas del API |
| Blazor WebAssembly | Front |
| CurrieTechnologies.Razor.SweetAlert2 | Alertas y confirmaciones bonitas |
| Bootstrap (base) + CSS propio | Estilos |
| Visual Studio 2026 | Entorno de desarrollo |
| Git + GitHub | Control de versiones, ramas y Pull Requests |

---

## 4. Estructura de la solución

La solución (`BuscadorTrabajo.slnx`) tiene tres proyectos:

| Proyecto | Tipo | Qué contiene |
|---|---|---|
| `BuscadorTrabajo.Shared` | Biblioteca de clases | Las **entidades** con sus validaciones (DataAnnotations e `IValidatableObject`), los **DTOs** del registro de desarrolladores y el atributo `UrlOpcional`. Lo usan el API y el WEB, así las mismas reglas se validan en los dos lados. |
| `BuscadorTrabajo.API` | ASP.NET Core WebAPI | `DataContext`, `SeedDb`, migraciones, los 14 controladores y el helper `ErroresBaseDatos`. |
| `BuscadorTrabajo.WEB` | Blazor WebAssembly | Páginas, componentes compartidos, el repositorio HTTP, el layout y los estilos. |

**Árbol resumido:**

```
BuscadorTrabajo.API/
  Controllers/        14 controladores (uno por entidad)
  Data/               DataContext.cs, SeedDb.cs
  Helpers/            ErroresBaseDatos.cs
  Migrations/         Inicial, AgregarEstadoActivoDesarrollador
  Program.cs
BuscadorTrabajo.Shared/
  Entities/           14 entidades
  DTOs/               RegistroDesarrolladorDTO.cs
  Validaciones/       UrlOpcionalAttribute.cs
BuscadorTrabajo.WEB/
  Layout/             MainLayout, NavMenu (diseño SkillNet)
  Pages/              Home, Publicaciones, Aplicaciones, Empresas, Clientes, Habilidades,
                      Categorias, Desarrolladores, Administradores,
                      PerfilesHabilidades, PublicacionesHabilidades
  Repositories/       IRepository, Repository, HttpResponseWrapper
  Shared/             GenericList, Pagination, PaginatedResponse, PublicacionCard,
                      HabilidadesTabs, Formato
  wwwroot/css/        app.css (paleta y layout), modulos.css (módulos)
```

---

## 5. Cómo ejecutar el proyecto

### Requisitos

- Visual Studio 2026 con .NET 10 SDK.
- SQL Server (instancia local o SQL Express) y, de forma opcional, SQL Server Management Studio.

### Pasos

1. **Clonar** el repositorio y abrir `BuscadorTrabajo.slnx` con doble clic. Si el Explorador de soluciones muestra carpetas en lugar de proyectos, está en "Vista de carpetas": hay que abrir el `.slnx`.
2. **Cadena de conexión.** `appsettings.json` trae `Server=localhost\SQLEXPRESS`. Como cada integrante tiene un servidor distinto (unos usan `localhost`, otros `localhost\SQLEXPRESS`), cada uno la sobrescribe con **User Secrets**, sin tocar el archivo compartido:
   - Clic derecho en `BuscadorTrabajo.API` → **Manage User Secrets**.
   - Pegar:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=localhost;Database=BuscadorTrabajo;Trusted_Connection=True;TrustServerCertificate=True;"
       }
     }
     ```
3. **Base de datos.** No hace falta correr `Update-Database` a mano: al iniciar el API, el `SeedDb` llama a `MigrateAsync()`, que crea la base y aplica las migraciones pendientes. Después carga los datos iniciales.
4. **Inicio múltiple.** Clic derecho en la solución → **Configure Startup Projects** → **Multiple startup projects** → `BuscadorTrabajo.API` y `BuscadorTrabajo.WEB` en **Start**, ambos con el perfil **https**. Si se ejecuta `Shared` como proyecto de inicio sale "A project with an Output Type of Class Library cannot be started directly".
5. **Ejecutar (F5).**

| Proyecto | URL |
|---|---|
| API | `https://localhost:7000` (Swagger en `https://localhost:7000/swagger`) |
| WEB | `https://localhost:8000` |

La URL del API que usa el front está en `BuscadorTrabajo.WEB/Program.cs` (`BaseAddress = https://localhost:7000/`). Si cambia el puerto del API, se cambia ahí.

---

## 6. Modelo de datos

### 6.1 Entidades (14)

| Entidad | Para qué sirve | Campos principales |
|---|---|---|
| `Administrador` | Responsable de la plataforma. Lo crea el SeedDb. | Cédula, Nombre, Apellido, Celular, Correo |
| `Empresa` | Organización que publica empleos y servicios | NIT, Nombre, Descripción, País, Ciudad, Dirección, Celular, Correo, SitioWeb (opcional) |
| `Cliente` | Persona que publica solicitudes de servicio | Cédula, Nombre, Apellido, Celular, Correo |
| `Desarrollador` | Profesional que aplica a publicaciones | Cédula, Nombre, Apellido, Celular, Correo, FechaNacimiento, **Activo** |
| `PerfilProfesional` | Presentación profesional del desarrollador (máximo uno) | IdDesarrollador, TítuloProfesional, Descripción, País, Ciudad, Modalidad, GitHub, LinkedIn |
| `Experiencia` | Trabajos anteriores o actuales | IdPerfilProfesional, Empresa, Cargo, Descripción, FechaInicio, FechaFin, EstadoActual |
| `Educacion` | Estudios | IdPerfilProfesional, NivelEducativo, Institución, Título, FechaInicio, FechaFin, EnCurso |
| `Certificacion` | Certificados obtenidos | IdPerfilProfesional, Título, Institución, FechaEmisión, FechaVencimiento, UrlCertificado |
| `Categoria` | Área de sistemas (Backend, DevOps, IoT…) | Nombre (único), Descripción |
| `Habilidad` | Tecnología concreta (C#, Docker…) | Nombre, Descripción, IdCategoria |
| `Perfil_Habilidad` | Tabla intermedia: habilidades de un perfil | Id, IdPerfilProfesional, IdHabilidad |
| `Publicacion` | Oferta de empleo o solicitud de servicio | IdEmpresa **o** IdCliente, IdCategoria, Título, Descripción, Tipo, Estado, fechas, Salario/Presupuesto, Moneda, NivelExperiencia, TipoContrato, ModalidadTrabajo, País, Ciudad, Plazo |
| `Publicacion_Habilidad` | Tabla intermedia: habilidades que pide una publicación | Id, IdPublicacion, IdHabilidad |
| `Aplicacion` | Postulación de un desarrollador a una publicación | IdPublicacion, IdDesarrollador, FechaAplicación, Mensaje, Estado, PlazoEstimado, MontoPropuesto |

**Sobre "dueño y usuario".** El profesor comentó que cada "usuario" podía modelarse como dos entidades. Se decidió que **Empresa, Cliente, Desarrollador y Administrador** son entidades separadas, y que las tablas intermedias (`Perfil_Habilidad` y `Publicacion_Habilidad`) resuelven las relaciones muchos a muchos.

### 6.2 Relaciones

| Relación | Cardinalidad | Significado |
|---|---|---|
| Empresa → Publicación | 1:N | Una empresa hace muchas publicaciones |
| Cliente → Publicación | 1:N | Un cliente hace muchas publicaciones (solo de servicio) |
| Categoría → Publicación | 1:N | Una categoría clasifica muchas publicaciones |
| Categoría → Habilidad | 1:N | Una categoría agrupa muchas habilidades |
| Desarrollador → PerfilProfesional | 1:0..1 | Un desarrollador tiene **como máximo un** perfil |
| Desarrollador → Aplicación | 1:N | Un desarrollador presenta muchas aplicaciones |
| Publicación → Aplicación | 1:N | Una publicación recibe muchas aplicaciones |
| PerfilProfesional → Experiencia / Educación / Certificación | 1:N | Un perfil tiene muchos registros |
| PerfilProfesional ↔ Habilidad | N:M | Mediante `Perfil_Habilidad` |
| Publicación ↔ Habilidad | N:M | Mediante `Publicacion_Habilidad` |

`Administrador` no tiene relaciones en esta entrega.

### 6.3 Validaciones en las entidades (Shared)

Las validaciones viven en las entidades, así se aplican **igual** en el API y en los formularios de Blazor:

- `[Required]`, `[MaxLength]`, `[MinLength]` en los textos.
- `[RegularExpression]` para la cédula (solo números), el tipo (`EMPLEO|SERVICIO`), los estados, la moneda (`COP|USD`) y la modalidad (`REMOTO|HIBRIDO|PRESENCIAL`).
- `[EmailAddress]` y `[Phone]` en correos y celulares.
- `[Range(1, int.MaxValue)]` en las llaves foráneas, para que "0" (nada seleccionado) no sea válido.
- `[Range(typeof(decimal), "0", "999999999999.99", ParseLimitsInInvariantCulture = true)]` en salario, presupuesto y monto (ver el bug 16.6).
- `[UrlOpcional]` en sitio web, GitHub, LinkedIn y URL del certificado (ver el bug 16.11).
- `IValidatableObject` para reglas que comparan varios campos:
  - `Publicacion`: dueño único, un cliente solo publica servicios, campos obligatorios según el tipo y fechas.
  - `Experiencia` y `Educacion`: fechas coherentes, y sin fecha de fin si es el trabajo actual o el estudio está en curso.
  - `Certificacion`: el vencimiento no puede ser anterior a la emisión.
  - `Desarrollador`: fecha de nacimiento obligatoria y no futura.

---

## 7. Configuración de la base de datos (DataContext)

Archivo: `BuscadorTrabajo.API/Data/DataContext.cs`.

- **14 DbSet:** `Administradores`, `Aplicaciones`, `Categorias`, `Certificaciones`, `Clientes`, `Desarrolladores`, `Educaciones`, `Empresas`, `Experiencias`, `Habilidades`, `PerfilesProfesionales`, `Perfiles_Habilidades`, `Publicaciones`, `Publicaciones_Habilidades`.
- **Todas las llaves foráneas se declaran con Fluent API** (`HasOne`, `WithMany`/`WithOne`, `HasForeignKey`). Hace falta porque se llaman `IdXxx` y EF Core no las reconoce solo; sin esto crearía columnas "fantasma" (shadow FKs). Se revisó la migración y no quedó ninguna.
- **Relación 1 a 1** `Desarrollador` → `PerfilProfesional` con `WithOne()`, más un índice único en `IdDesarrollador`.
- **Comportamiento al borrar:**
  - **Cascade** (se borran solos): al borrar un perfil, se borran su experiencia, educación, certificaciones y `Perfil_Habilidad`. Al borrar una publicación, se borran sus `Publicacion_Habilidad`.
  - **Restrict** (no deja borrar si está en uso): todo lo demás. Por ejemplo, no se borra una categoría con habilidades ni una empresa con publicaciones.
- **Índices únicos (10):**
  - NIT de Empresa.
  - Cédula de Cliente, Desarrollador y Administrador.
  - Nombre de Categoría.
  - (IdCategoria, Nombre) en Habilidad.
  - Los pares de las dos tablas intermedias.
  - (IdPublicacion, IdDesarrollador) en Aplicación.
- **Precisión decimal(18,2)** para Salario, Presupuesto y MontoPropuesto.

---

## 8. Migraciones

| Migración | Qué hace |
|---|---|
| `20261029213233_Inicial` | Crea las 14 tablas, las llaves foráneas y los 10 índices únicos. Se verificó a mano que no hubiera llaves fantasma. |
| `AgregarEstadoActivoDesarrollador` | Agrega la columna `Activo` (bit) a `Desarrolladores`, con valor `true` para los registros existentes. Ver la sección 14. |

Comandos (Consola del Administrador de paquetes, con `BuscadorTrabajo.API` como proyecto predeterminado):

```
Add-Migration NombreDeLaMigracion
Update-Database
```

---

## 9. SeedDb: datos iniciales

Archivo: `BuscadorTrabajo.API/Data/SeedDb.cs`. Se ejecuta al iniciar el API desde `Program.cs`. Siguiendo la guía del profesor, se llama desde una función local con `.Wait()`.

**Cómo evita duplicados:**
1. Llama a `MigrateAsync()`, que crea la base y aplica las migraciones.
2. Tiene un método `Check...Async()` por entidad. Cada uno pregunta `AnyAsync()` y solo inserta si la tabla está vacía. Por eso, reiniciar el API no duplica nada.
3. Las relaciones se buscan por un dato natural (NIT, cédula, nombre, título) y no por un Id fijo. Así funciona aunque los Id cambien.

**Datos que carga:**

| Entidad | Cantidad | Detalle |
|---|---|---|
| Administrador | 1 | Cédula 1000000001, correo `admin@buscadortrabajo.example.com` (sin contraseña: no hay login) |
| Categorías | 10 | Desarrollo web, Desarrollo móvil, Backend, Frontend, Bases de datos, Ciencia de datos, Inteligencia artificial, Ciberseguridad, DevOps, IoT |
| Habilidades | 10 | Una por categoría: JavaScript, Flutter, C#, React, SQL Server, Python, TensorFlow, Pruebas de penetración, Docker, Arduino |
| Empresas | 10 | TecnoAndes, Nube Pacífico, DataCaribe, SeguraNet, AppsCafé, Inteligencia Andina, SensorAgro, WebCreativa, BaseSegura, CódigoNorte (NIT 900100001-1 a 900100010-0) |
| Clientes | 10 | Cédulas 1030000001 a 1030000010 |
| Desarrolladores | 10 | Cédulas 1020000001 a 1020000010 |
| Perfiles profesionales | 10 | Uno por desarrollador |
| Experiencias, Educaciones, Certificaciones | 10 cada una | |
| Perfiles_Habilidades | 10 | |
| Publicaciones | 10 | 6 empleos de empresas y 4 servicios (3 de clientes y 1 de empresa), todas PUBLICADA |
| Publicaciones_Habilidades | 10 | |
| Aplicaciones | 10 | |

---

## 10. Reglas de negocio

**Dónde se aplica cada regla:**
- **Shared:** en la entidad. Se valida en el API y en los formularios.
- **BD:** en la base de datos (índices únicos, llaves foráneas, comportamiento al borrar).
- **API:** en el controlador, consultando la base antes de guardar.

### A. Empresas, clientes, desarrolladores y administradores

| Código | Regla | Dónde |
|---|---|---|
| RN01 | Cada entidad tiene una clave primaria única. | BD |
| RN02 | El NIT de una empresa es obligatorio y no se repite. | Shared, BD |
| RN03 | La cédula de clientes, desarrolladores y administradores tiene solo números, entre 6 y 10 dígitos, y no se repite dentro de la misma entidad. | Shared, BD |
| RN04 | Los correos deben tener un formato válido. | Shared |
| RN05 | Los datos de contacto obligatorios (nombre, correo, celular) no pueden quedar vacíos. | Shared |
| RN06 | Existe al menos un administrador, creado por el SeedDb. | SeedDb |

### B. Perfil profesional

| Código | Regla | Dónde |
|---|---|---|
| RN07 | Todo perfil pertenece a un desarrollador existente, y un desarrollador tiene como máximo un perfil. | BD, API |
| RN08 | La modalidad solo puede ser REMOTO, HIBRIDO o PRESENCIAL. | Shared |
| RN09 | Experiencia, educación y certificaciones pertenecen a un perfil existente. Al borrar el perfil se borran también. | BD, API |
| RN10 | La fecha de inicio es obligatoria. En una experiencia no puede ser futura, y la fecha de fin no puede ser anterior a la de inicio. | Shared |
| RN11 | Si es el trabajo actual o el estudio está en curso, la fecha de fin queda vacía. | Shared |
| RN12 | El vencimiento de una certificación no puede ser anterior a su emisión. | Shared |

### C. Categorías y habilidades

| Código | Regla | Dónde |
|---|---|---|
| RN13 | El nombre de cada categoría es único. | BD |
| RN14 | Cada habilidad pertenece a una categoría existente, y su nombre no se repite dentro de esa categoría. | BD, API |
| RN15 | Un perfil o una publicación no pueden tener la misma habilidad dos veces. | BD |
| RN16 | Una categoría o habilidad en uso no se puede borrar. | BD, API |

### D. Publicaciones

| Código | Regla | Dónde |
|---|---|---|
| RN17 | Toda publicación tiene título, descripción, tipo, estado, moneda, país, ciudad y una categoría existente. | Shared, API |
| RN18 | El tipo es EMPLEO o SERVICIO. | Shared |
| RN19 | Un solo dueño: una empresa o un cliente. La empresa publica empleos y servicios; el cliente solo servicios. | Shared, API |
| RN20 | EMPLEO: salario, tipo de contrato y nivel obligatorios, sin presupuesto ni plazo. SERVICIO: presupuesto y plazo obligatorios, sin salario, contrato ni nivel. | Shared |
| RN21 | Salario y presupuesto mayores que cero; plazo entre 1 y 5000 días. | Shared |
| RN22 | La moneda es COP o USD. | Shared |
| RN23 | La fecha de vencimiento es posterior a la de publicación. | Shared, API |
| RN24 | Estados: BORRADOR, PUBLICADA, PAUSADA, CERRADA, CANCELADA, con transiciones controladas. Una publicación nueva solo puede ser BORRADOR o PUBLICADA. | Shared, API |
| RN25 | Una publicación con aplicaciones no se borra: se cierra o se cancela. Tampoco se le puede cambiar el tipo ni el dueño. | API |

### E. Aplicaciones

| Código | Regla | Dónde |
|---|---|---|
| RN26 | Solo se aplica a publicaciones PUBLICADA que no estén vencidas. | API |
| RN27 | La publicación y el desarrollador deben existir, y el desarrollador debe tener perfil profesional. | API |
| RN28 | Un desarrollador no aplica dos veces a la misma publicación. | BD, API |
| RN29 | Un desarrollador no aplica a una publicación de un cliente con su misma cédula. | API |
| RN30 | En servicios, el monto propuesto (mayor que cero) y el plazo son obligatorios; en empleos van vacíos. | API |
| RN31 | Estados: PENDIENTE, EN_REVISION, ACEPTADA, RECHAZADA, RETIRADA, con transiciones controladas. Toda aplicación nace PENDIENTE, y no se le puede cambiar la publicación ni el desarrollador. | Shared, API |

### F. Integridad

| Código | Regla | Dónde |
|---|---|---|
| RN32 | No se guardan llaves foráneas que apunten a registros inexistentes, y ningún borrado deja registros huérfanos. | BD, API |
| RN33 | Un desarrollador con aplicaciones no se borra: queda **inactivo** para conservar el historial. Uno inactivo no aparece en las listas ni puede aplicar. | API |

### Tabla de estados

| Entidad | Estado actual | Puede pasar a |
|---|---|---|
| Publicación | BORRADOR | PUBLICADA, CANCELADA |
| Publicación | PUBLICADA | PAUSADA, CERRADA, CANCELADA |
| Publicación | PAUSADA | PUBLICADA, CERRADA, CANCELADA |
| Publicación | CERRADA, CANCELADA | Ninguno (finales) |
| Aplicación | PENDIENTE | EN_REVISION, RECHAZADA, RETIRADA |
| Aplicación | EN_REVISION | ACEPTADA, RECHAZADA, RETIRADA |
| Aplicación | ACEPTADA, RECHAZADA, RETIRADA | Ninguno (finales) |

### Reglas para cuando haya login (no aplican en esta entrega)

| Código | Regla |
|---|---|
| RF01 | Un desarrollador solo modifica su propio perfil. |
| RF02 | Solo el dueño de una publicación la modifica o cambia su estado. |
| RF03 | Cada quien solo ve sus aplicaciones, o las de sus publicaciones. |
| RF04 | Las cédulas y los datos privados no se muestran en resultados públicos. |
| RF05 | Las operaciones administrativas quedan protegidas. |

---

## 11. WebAPI

### 11.1 Program.cs

- `AddDbContext<DataContext>(x => x.UseSqlServer("name=DefaultConnection"))`.
- `AddTransient<SeedDb>()`, que se ejecuta al arrancar.
- Swagger con Swashbuckle (`UseSwaggerUI` apuntando a `/openapi/v1.json`).
- **CORS** abierto para el front: `AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials()`, colocado después de `UseHttpsRedirection`.

### 11.2 Patrón común de los controladores

Los 14 controladores siguen el mismo patrón, para que sean fáciles de explicar:

| Acción | Ruta | Qué hace | Respuestas |
|---|---|---|---|
| Listar | `GET /api/x` | Todos, ordenados por Id (`AsNoTracking`) | 200 |
| Paginar | `GET /api/x/paginated?page=1&pageSize=5` | Una página. `page` mínimo 1, `pageSize` entre 1 y 50 | 200 |
| Uno | `GET /api/x/{id}` | Busca por Id | 200, 404 con mensaje |
| Crear | `POST /api/x` | Fuerza `Id = 0`, valida relaciones y reglas, guarda | 200, 400, 409 |
| Editar | `PUT /api/x` | Verifica que exista, valida, guarda | 200, 400, 404, 409 |
| Borrar | `DELETE /api/x/{id}` | Borra; si el registro está en uso, avisa | 204, 404, 409 |

**Formato de la paginación**, igual al de la guía del profesor:

```json
{ "page": 1, "pageSize": 5, "totalRecords": 10, "totalPages": 2, "data": [ ... ] }
```

**Manejo de errores de la base de datos** (`Helpers/ErroresBaseDatos.cs`):

```csharp
public static bool EsDuplicado(DbUpdateException ex) => ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
public static bool EsRelacion(DbUpdateException ex) => ex.InnerException is SqlException sql && sql.Number == 547;
```

- **2601 y 2627** = se violó un índice único → el API responde **409** con un mensaje como *"Ya existe una categoría con ese nombre."*
- **547** = se violó una llave foránea (el registro está en uso) → **409** con un mensaje como *"No se puede eliminar porque está en uso."*

### 11.3 Rutas

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
| Perfiles_Habilidades | `/api/perfiles_habilidades` |
| Publicaciones | `/api/publicaciones` |
| Publicaciones_Habilidades | `/api/publicaciones_habilidades` |

**Rutas especiales:**

| Ruta | Qué hace |
|---|---|
| `POST /api/desarrolladores/registro-completo` | Registra en **una sola transacción** el desarrollador, su perfil, su educación, experiencia, certificaciones y habilidades. Si algo falla, no se guarda nada. |
| `GET /api/desarrolladores/{id}/perfil-completo` | Devuelve todo el perfil en un solo objeto (`RegistroDesarrolladorDTO`). |
| `PUT /api/desarrolladores/{id}/perfil-completo` | Actualiza todo en una transacción. Crea los registros nuevos, edita los existentes y borra los que se quitaron, comprobando que cada uno pertenezca a ese perfil. |
| `DELETE /api/desarrolladores/{id}` y `?confirmarConservarHistorial=true` | Borrado o desactivación (sección 14). |

### 11.4 Lógica especial de Publicaciones (`PublicacionesController`)

- **POST:**
  - El estado inicial solo puede ser BORRADOR o PUBLICADA.
  - `FechaCreacion` se pone en hoy si no viene.
  - Si se crea PUBLICADA sin `FechaPublicacion`, se pone hoy.
  - Se revisa la RN23 después de asignar las fechas.
  - Deben existir la empresa o el cliente, y la categoría.
- **PUT:**
  - Valida la transición de estado (RN24).
  - Si la publicación ya tiene aplicaciones, no deja cambiar el tipo ni el dueño (400).
- **DELETE:** si tiene aplicaciones responde 409: *"ciérrela o cancélela"* (RN25).

### 11.5 Lógica especial de Aplicaciones (`AplicacionesController`)

- **POST:**
  - Fuerza `Estado = "PENDIENTE"` aunque se envíe otro, y pone la fecha de hoy.
  - Verifica que la publicación exista y que el desarrollador exista, esté activo y tenga perfil.
  - Exige que la publicación esté PUBLICADA y sin vencer.
  - Aplica la regla de la misma cédula (RN29).
  - Rechaza la aplicación duplicada (409).
  - Revisa monto y plazo según el tipo (RN30).
- **PUT:**
  - No deja cambiar la publicación ni el desarrollador.
  - Valida la transición de estado (RN31) y la propuesta.

---

## 12. Front (Blazor WebAssembly)

### 12.1 Arquitectura

| Pieza | Para qué sirve |
|---|---|
| `Repositories/IRepository` + `Repository` | Único lugar que habla con el API: `GetAsync`, `PostAsync`, `PutAsync`, `DeleteAsync`. Las páginas nunca usan `HttpClient` directamente. |
| `Repositories/HttpResponseWrapper` | Envuelve la respuesta: `Error`, `Response`, `HttpResponseMessage`. `GetErrorMessageAsync()` convierte los códigos 400, 404 y 409 en mensajes legibles; entiende texto simple, la lista `errors` de validación y el formato `{ codigo, mensaje }`. `GetCodigoErrorAsync()` lee el código. |
| `Shared/GenericList` | Muestra "Cargando…", "No hay registros" o la lista. |
| `Shared/Pagination` + `PaginatedResponse<T>` | Botones de página que leen el `/paginated` del API. |
| `Shared/PublicacionCard` | Tarjeta de publicación, reutilizada en el Inicio y en Publicaciones. |
| `Shared/HabilidadesTabs` | Pestañas Catálogo · En perfiles · En publicaciones. |
| `Shared/Formato` | `Dinero()` (3500000 → "$3.500.000 COP"), `Fecha()`, `Iniciales()` y `Texto()` (PUBLICADA → "Publicada"). |
| Formularios | `EditForm` + `DataAnnotationsValidator` + `ValidationSummary` + `ValidationMessage`. Un formulario reutilizable por módulo (`XxxForm.razor`), usado por Crear y Editar. |
| `NavigationLock` | Si el formulario tiene cambios sin guardar, pregunta antes de salir. |
| SweetAlert2 | Confirmaciones de borrado, errores del API y mensajes de éxito. |

### 12.2 Diseño

- **Layout SkillNet:** tema oscuro con barra superior, menú lateral desplegable y avatar.
- **Paleta** (`app.css`, variables `--sn-*`): fondo `#071018`, superficies `#0D1B25`, texto `#F2F7F6`, primario cian `#63D9F2`, violeta `#6C5C8D`, ámbar `#D7A45B`.
- **`modulos.css`:** estilos de los módulos con prefijo `bt-` (tarjetas, formularios, chips, insignias, paginación, botón flotante, stepper del registro). Se puso en un archivo aparte para no chocar con `app.css`.
- Insignias: **Empleo** en cian y **Servicio** en violeta. Cada estado lleva un punto de color.
- Funciona en celular: las grillas pasan a una columna por debajo de 700 px.

### 12.3 Páginas

| Ruta | Página | Qué permite |
|---|---|---|
| `/` | Inicio | Título, buscador (lleva a Publicaciones con el texto), 4 cifras (empleos, servicios, empresas, habilidades), las 4 publicaciones más recientes y el botón flotante "Crear publicación" |
| `/publicaciones` | Publicaciones | Buscador en vivo (título, descripción, ciudad, dueño, habilidades), filtros Empleo/Servicio y por estado, tarjetas con salario o presupuesto. Acepta `?buscar=texto`. |
| `/publicaciones/{id}` | Detalle | Toda la información, habilidades, **Aplicar**, **Ver aplicaciones**, Editar y Borrar |
| `/publicaciones/create`, `/publicaciones/edit/{id}` | Formulario | Elegir quién publica (empresa o cliente) y el tipo. Los campos cambian según el tipo y se limpian al cambiarlo. Solo muestra los estados permitidos y se marcan las habilidades requeridas. |
| `/aplicaciones` | Aplicaciones | Tarjetas con desarrollador, publicación, estado, mensaje y propuesta. Filtro por estado, buscador y paginación. Acepta `?publicacion=id`. |
| `/aplicaciones/create` | Aplicar | Solo publicaciones activas y desarrolladores activos con perfil. Pide monto y plazo solo si es un servicio. Acepta `?publicacion=id`. |
| `/aplicaciones/edit/{id}` | Editar aplicación | Mensaje, propuesta y estado (solo las transiciones permitidas) |
| `/empresas`, `/clientes`, `/administradores` | CRUD | Tarjetas con crear, editar, borrar y paginación del API |
| `/categorias` | CRUD | Tarjetas con el número de habilidades de cada categoría |
| `/habilidades` | CRUD | Tarjetas con su categoría y lista desplegable de categorías |
| `/perfiles-habilidades` | Tabla N:M | Asignar una habilidad a un perfil, cambiarla (✎) o quitarla (×), con buscador |
| `/publicaciones-habilidades` | Tabla N:M | Lo mismo, por publicación |
| `/desarrolladores` | Desarrolladores | Lista, ver perfil, editar datos, editar perfil y eliminar con doble confirmación |
| `/desarrolladores/registro` | Registro por pasos | Datos personales → perfil → educación → experiencia → certificaciones → habilidades (puede crear habilidades nuevas) → revisión. Se guarda todo con `registro-completo`. |
| `/desarrolladores/detalle/{id}` | Perfil | Toda la información del desarrollador |
| `/desarrolladores/edit/{id}`, `/desarrolladores/perfil/edit/{id}` | Edición | Datos personales, o el perfil completo |

### 12.4 Menú

- **Principal:** Inicio, Publicaciones, Aplicaciones, Registrarme como desarrollador.
- **Administración:** Categorías, Habilidades, Empresas, Clientes, Desarrolladores, Administradores.
- Todos los enlaces cierran el panel al hacer clic.

---

## 13. Validaciones: cómo responde el sistema a los errores

**Nivel 1: en el formulario, antes de enviar.** Si un dato está mal no se envía nada. El campo se marca en rojo con su mensaje y arriba aparece un resumen. Algunos ejemplos:
- *"El campo Nombre es obligatorio."*
- *"La cédula solo debe contener números."*
- *"Una oferta de empleo debe indicar un salario mayor que cero."*

Además, el formulario evita errores desde antes:
- Si publica un cliente, no deja elegir Empleo.
- Solo ofrece los estados permitidos.
- Limpia los campos que no aplican al tipo elegido.
- No ofrece habilidades ya asignadas.
- Solo lista las publicaciones activas al aplicar.

**Nivel 2: en el API, al guardar.** Revisa las reglas que necesitan consultar la base. El front muestra el mensaje del API en una alerta. Por ejemplo:
- *"Ya existe una categoría con ese nombre."* (409)
- *"La publicación tiene aplicaciones. En lugar de eliminarla, ciérrela o cancélela."* (409)
- *"Solo se puede aplicar a publicaciones en estado PUBLICADA y que no estén vencidas."* (400)

---

## 14. Eliminación de desarrolladores (desactivación)

**Problema.** El perfil profesional depende del desarrollador con `Restrict`, y con el registro por pasos todos los desarrolladores tienen perfil. Por eso el botón "Eliminar" siempre respondía *"No se puede eliminar…"*. Además, borrar a alguien con aplicaciones haría perder el historial de las publicaciones.

**Solución (plan acordado por el equipo):**

| Caso | Qué pasa |
|---|---|
| El desarrollador **no tiene aplicaciones** | Se borra de verdad, en una transacción: primero el perfil (la cascada borra educación, experiencia, certificaciones y habilidades) y luego el desarrollador. Responde **204**. |
| El desarrollador **tiene aplicaciones** | No se borra. La primera petición responde **409** con `{ "codigo": "REQUIERE_CONFIRMACION_HISTORIAL", "mensaje": "..." }`. Si se confirma con `?confirmarConservarHistorial=true`, queda `Activo = false` y responde **200**. Las aplicaciones se conservan. |
| No existe o ya está inactivo | **404** |

**Cambios:**
- `Desarrollador.Activo` (bool, por defecto `true`) y la migración `AgregarEstadoActivoDesarrollador`, que deja activos los registros existentes.
- `GET /api/desarrolladores` y `/paginated` filtran con `.Where(x => x.Activo)` **antes** de contar y paginar, así el total es correcto.
- `AplicacionesController`: un desarrollador inactivo no puede aplicar.
- **Front, en Desarrolladores:**
  1. Primera confirmación: *"¿Seguro que deseas eliminar a…?"* (Sí, continuar / Cancelar).
  2. Si el API pide confirmación, una segunda: *"El desarrollador tiene aplicaciones… quedará inactivo"* (Sí, desactivar / Cancelar).
  3. Cancelar en cualquiera de las dos no cambia nada.
- En Aplicaciones, el desarrollador desactivado aparece como "Desarrollador inactivo". En Habilidades por perfil ya no aparece su perfil.

**Pruebas a realizar:**

| Prueba | Resultado esperado |
|---|---|
| Borrar un Id inexistente | 404 |
| Borrar un desarrollador sin aplicaciones | Se borran el desarrollador y su perfil completo (204) |
| Borrar uno con aplicaciones, sin confirmar | 409 con el código |
| Confirmar la desactivación | `Activo = false` y las aplicaciones siguen existiendo |
| Cancelar cualquiera de las dos confirmaciones | Nada cambia |
| Listar después de desactivar | No aparece |

---

## 15. Pruebas realizadas

Se probó el API **en vivo** desde el navegador, con peticiones `fetch` al API corriendo en `https://localhost:7000`, intentando a propósito encontrar fallas. Al final se borraron todos los datos de prueba y cada tabla volvió a sus 10 registros.

| Ronda | Qué se probó | Casos | Resultado |
|---|---|---|---|
| 1 | Paginación; CRUD de categorías, clientes, desarrolladores, empresas, administradores, perfiles, experiencia, educación, certificaciones y habilidades; borrados en uso | 59 | 58 bien, 1 falla: los decimales (bug 16.6), corregida |
| 2 | Publicaciones: dueño único, cliente solo servicios, campos según el tipo, regex de tipo, moneda y modalidad, estados iniciales, RN23, transiciones, cambios bloqueados con aplicaciones, paginación | 28 | Todas bien |
| 3 | Aplicaciones: publicación pausada, cerrada o vencida, estado inicial forzado, duplicadas (409), monto y plazo, publicación propia (RN29), transiciones, cambiar de publicación, borrar una publicación con aplicaciones (409), cerrar y volver a aplicar, paginación, desarrollador sin perfil (RN27) | 28 | Todas bien |
| 4 | Eliminación y desactivación de desarrolladores (404, borrado en cascada, 409 con código, desactivación, lista y paginación sin inactivos, aplicación conservada, inactivo no aplica), enlaces vacíos guardados como `null`, URL inválida rechazada, empresa sin sitio web, CRUD de administradores | 30 | Todas bien |

**Total: 145 casos.** El único que falló antes de la corrección quedó resuelto.

Ejemplos de lo comprobado:
- Al crear una aplicación con `"estado": "ACEPTADA"`, queda en **PENDIENTE**.
- Una publicación cerrada no se reabre, no recibe aplicaciones y no se borra si tiene aplicaciones.
- Al borrar un perfil se borran su experiencia, educación y certificaciones.
- Cada error responde con su código (400, 404 o 409) y un mensaje claro.

---

## 16. Bitácora de problemas y soluciones

Todos los problemas que aparecieron durante el proyecto, en orden.

### 16.1 Los controladores no compilaban
- **Síntoma:** errores de compilación en los controladores generados.
- **Causas:** faltaba `using Microsoft.EntityFrameworkCore`, había nombres de tipo mal escritos (`Clientes` en lugar de `Cliente`, `PerfilesProfesional`, `Perfiles_Habilidad`, `Publicacion_habilidad`) y las tablas intermedias no tenían `Id`.
- **Solución:** se corrigieron los nombres, se agregaron los `using` y se puso `public int Id` en `Perfil_Habilidad` y `Publicacion_Habilidad`.
- **Error secundario:** un reemplazo automático también cambió el nombre de una variable dentro de `if (X == null)` en los Delete, lo que dio el error **CS0119**. Se revirtieron esas líneas.

### 16.2 La consola de paquetes se detenía con NU1902/NU1903
- **Síntoma:** `Add-Migration` fallaba por una advertencia de vulnerabilidad en `System.Security.Cryptography.Pkcs` 9.0.11.
- **Solución:** se instaló la versión más reciente del paquete desde el administrador de NuGet.

### 16.3 "A project with an Output Type of Class Library cannot be started"
- **Causa:** `Shared` estaba como proyecto de inicio.
- **Solución:** inicio múltiple con API y WEB, perfil https (sección 5). También funciona Depurar → Iniciar nueva instancia.

### 16.4 El SeedDb del compañero no conectaba a SQL Server
- **Causa:** unos usan `Server=localhost` y otros `localhost\SQLEXPRESS`, y la cadena estaba fija en `appsettings.json`.
- **Solución:** **User Secrets** por desarrollador (sección 5). El archivo compartido no se toca.

### 16.5 Postman: error de IDENTITY_INSERT
- **Causa:** se enviaba el `id` en el POST, y SQL Server no deja insertar un valor en una columna identidad.
- **Solución:** se explicó que el POST va sin `id`, y además todos los POST hacen `x.Id = 0`, así el API ignora el Id que llegue.

### 16.6 Error 500 con los decimales
- **Síntoma:** *"999999999999.99 is not a valid value for Decimal"* al validar salario o presupuesto.
- **Causa:** `[Range(typeof(decimal), "0", "999999999999.99")]` lee los límites con la cultura del equipo, y en español el punto no es el separador decimal.
- **Solución (commit de Jordan):** `ParseLimitsInInvariantCulture = true`.

### 16.7 La auditoría del compañero
Un compañero pasó una auditoría del API y se corrigió:
- **Paginación:** el profesor sí la pide. Se agregó `GET /paginated` a los 14 controladores con su formato.
- **Validaciones de varios campos** con `IValidatableObject` (publicación, experiencia, educación, certificación, desarrollador).
- **Duplicados → 409 y registro en uso → 409**, en lugar de un error 500, con `ErroresBaseDatos`.
- **Transiciones de estado** de publicaciones y aplicaciones.
- **Validación de relaciones** antes de guardar (400 si la llave foránea no existe).
- **Range(1, …)** en las llaves foráneas, para rechazar "0".
- **Se dejaron igual a propósito:**
  - El `.Wait()` del SeedDb, porque así está en la guía del profesor.
  - La autenticación, porque el profesor dijo que no va en esta entrega.
  - La búsqueda avanzada, que queda fuera del alcance.

### 16.8 "Hice merge y no se unió nada"
- **Causa:** se había hecho Pull de `main` pero no Merge hacia la rama propia.
- **Solución:** se revisó `.git/logs/HEAD` para confirmarlo y se explicó el flujo **Fetch → Manage Branches → Merge 'main' into 'Brayan'**. El merge se hizo sin conflictos.

### 16.9 Razor: `@foreach` e `@if` dentro de un bloque de código
- **Riesgo:** dentro de un bloque `@if { ... }` que ya es código C#, escribir `@foreach` o `@if` da el error **RZ1008**.
- **Solución:** se escribieron sin `@` en esos lugares (`foreach`, `if`). Se revisaron todas las páginas antes de entregarlas.

### 16.10 Conflicto en `index.html` al cambiar de rama
- **Síntoma:** al pasar de `main` a `Brayan` con cambios sin guardar, Visual Studio los guardó en un *stash*, cambió de rama y al reponerlos chocaron en `index.html`.
- **Causa:** la rama `Brayan` estaba atrasada, sin el diseño del compañero.
- **Solución:**
  1. **Keep Current** para resolver el conflicto.
  2. Como los archivos estaban en *Staged Changes*, primero **Unstage** y luego **Undo Changes** a todo.
  3. **Merge 'main' into 'Brayan'**, que fue *fast-forward*.
  4. Se volvieron a escribir los archivos del front encima de la versión correcta.

### 16.11 Los enlaces opcionales vacíos no se podían guardar
- **Síntoma:** si el usuario escribía un sitio web, GitHub, LinkedIn o URL de certificado y luego lo borraba, salía *"no es una URL válida"*.
- **Causa:** `[Url]` acepta `null` pero rechaza `""`, y los campos de texto de Blazor quedan en `""` al borrarlos.
- **Primera solución:** en el formulario de Empresas, el sitio web se guarda como `null` si queda vacío.
- **Solución definitiva:** se creó `[UrlOpcional]` en `Shared/Validaciones`. Acepta vacío y, si hay texto, exige una URL válida. Se usa en `Empresa`, `PerfilProfesional` y el DTO del registro. Además, el API guarda esos campos vacíos como `null`.

### 16.12 No se podía eliminar ningún desarrollador
- **Causa y solución:** ver la sección 14 (borrado físico o desactivación).

### 16.13 El menú tenía enlaces muertos y no se cerraba
- **Síntoma:** "Mi perfil", "Mis aplicaciones" y "Mis publicaciones" llevaban a páginas que no existen, y los enlaces de Administración no cerraban el panel.
- **Solución:** sin login no existe "mi" usuario, así que el menú quedó con Inicio, Publicaciones, Aplicaciones y "Registrarme como desarrollador", más la sección de Administración completa. Todos los enlaces llaman a `CloseMenu`.

### 16.14 Estilos compartidos sobrescritos
- **Situación:** al agregar sus estilos, el compañero cambió `.bt-card-foot` (los botones ahora ocupan dos columnas) y `.bt-detail-grid` (2 columnas fijas).
- **Decisión:** se dejaron así, porque se ven bien en todas las páginas. Lo importante es avisar cuando se toca un estilo compartido.

---

## 17. Flujo de trabajo con Git y GitHub

1. Cada integrante trabaja en **su propia rama** (por ejemplo `Brayan` o `Jordan_DelAguila`), así cada uno tiene sus commits.
2. Antes de empezar: **Pull de `main`** y **Merge 'main' into 'mi-rama'**, para trabajar sobre lo último.
3. Al terminar: **Commit** con un mensaje claro → **Push** → **Pull Request** de la rama a `main` en GitHub → **Merge**.
4. Avisar al equipo para que hagan Pull.
5. **Archivos compartidos** (`index.html`, `NavMenu.razor`, `Home.razor`, `modulos.css`, `_Imports.razor`): avisar antes de tocarlos, para evitar conflictos.
6. Nunca cambiar de rama con cambios sin guardar: primero commit o descartar.


## 18. Aportes por integrante

| Integrante | Aporte |
|---|---|
| **Brayan Florez** | Estructura de la solución. Entidad Administrador. DataContext con relaciones, índices y comportamiento al borrar. Migración inicial. SeedDb con datos de todas las entidades. Controladores con paginación, validaciones y manejo de errores. Reglas de publicaciones y aplicaciones. Front: Inicio, Publicaciones, Aplicaciones, Empresas, Clientes, Habilidades, Categorías, Administradores, las dos tablas N:M y el menú. Eliminación y desactivación de desarrolladores. Atributo `UrlOpcional`. Pruebas del API (115 casos). |
| **Jordan del Aguila** | Repositorio en GitHub e integración de los Pull Requests. Corrección de la cultura en `Range` (decimales). Módulo de Desarrolladores: registro por pasos, detalle, edición del perfil completo y endpoints transaccionales. _Confirmar:_ diseño base SkillNet (layout, menú, paleta), auditoría del API y plan de eliminación de desarrolladores. |

---

## 19. Pendientes y trabajo futuro

**Fuera del alcance (etapas siguientes):**
- Login, contraseñas y roles, con las reglas RF01 a RF05.
- Pantalla de desarrolladores inactivos, con la opción de reactivarlos.
- Preguntas y respuestas en las publicaciones.
- Vacantes externas de otras plataformas.
- Búsqueda avanzada, recomendaciones y alertas.
