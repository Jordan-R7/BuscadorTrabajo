using BuscadorTrabajo.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Data
{
    // Alimentador de la base de datos (SeedDb).
    // Carga datos de ejemplo la primera vez que arranca el WebAPI.
    // Cada método revisa si su tabla ya tiene datos y, si es así, no inserta nada:
    // por eso no se duplican registros al reiniciar la aplicación.
    // Todos los datos son ficticios.
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedDbAsync()
        {
            // Aplica las migraciones pendientes (si la base no existe, la crea).
            await _context.Database.MigrateAsync();

            // El orden importa: primero las tablas "padre" y luego las que dependen de ellas.
            await CheckAdministradoresAsync();
            await CheckCategoriasAsync();
            await CheckHabilidadesAsync();
            await CheckEmpresasAsync();
            await CheckClientesAsync();
            await CheckDesarrolladoresAsync();
            await CheckPerfilesProfesionalesAsync();
            await CheckExperienciasAsync();
            await CheckEducacionesAsync();
            await CheckCertificacionesAsync();
            await CheckPerfilesHabilidadesAsync();
            await CheckPublicacionesAsync();
            await CheckPublicacionesHabilidadesAsync();
            await CheckAplicacionesAsync();
        }

        // ---------- Administrador inicial (RN06) ----------
        private async Task CheckAdministradoresAsync()
        {
            if (!await _context.Administradores.AnyAsync())
            {
                _context.Administradores.Add(new Administrador
                {
                    Cedula = "1000000001",
                    Nombre = "Administrador",
                    Apellido = "del Sistema",
                    Celular = "3000000000",
                    Correo = "admin@buscadortrabajo.example.com"
                });
                await _context.SaveChangesAsync();
            }
        }

        // ---------- Categorías ----------
        private async Task CheckCategoriasAsync()
        {
            if (!await _context.Categorias.AnyAsync())
            {
                _context.Categorias.AddRange(
                    new Categoria { Nombre = "Desarrollo web", Descripcion = "Sitios y aplicaciones que funcionan en el navegador." },
                    new Categoria { Nombre = "Desarrollo móvil", Descripcion = "Aplicaciones para Android e iOS." },
                    new Categoria { Nombre = "Backend", Descripcion = "Lógica del servidor, APIs y servicios." },
                    new Categoria { Nombre = "Frontend", Descripcion = "Interfaces de usuario y experiencia visual." },
                    new Categoria { Nombre = "Bases de datos", Descripcion = "Diseño, administración y optimización de datos." },
                    new Categoria { Nombre = "Ciencia de datos", Descripcion = "Análisis, estadística y visualización de datos." },
                    new Categoria { Nombre = "Inteligencia artificial", Descripcion = "Modelos de aprendizaje automático y visión." },
                    new Categoria { Nombre = "Ciberseguridad", Descripcion = "Protección de sistemas, redes e información." },
                    new Categoria { Nombre = "DevOps", Descripcion = "Integración, despliegue continuo e infraestructura." },
                    new Categoria { Nombre = "IoT", Descripcion = "Dispositivos y sensores conectados a internet." });
                await _context.SaveChangesAsync();
            }
        }

        // ---------- Habilidades (una por categoría) ----------
        private async Task CheckHabilidadesAsync()
        {
            if (!await _context.Habilidades.AnyAsync())
            {
                _context.Habilidades.AddRange(
                    new Habilidad { Nombre = "JavaScript", Descripcion = "Lenguaje de programación para la web.", IdCategoria = await IdCategoriaAsync("Desarrollo web") },
                    new Habilidad { Nombre = "Flutter", Descripcion = "Framework para apps móviles multiplataforma.", IdCategoria = await IdCategoriaAsync("Desarrollo móvil") },
                    new Habilidad { Nombre = "C#", Descripcion = "Lenguaje de programación de .NET.", IdCategoria = await IdCategoriaAsync("Backend") },
                    new Habilidad { Nombre = "React", Descripcion = "Librería para construir interfaces web.", IdCategoria = await IdCategoriaAsync("Frontend") },
                    new Habilidad { Nombre = "SQL Server", Descripcion = "Motor de bases de datos relacionales.", IdCategoria = await IdCategoriaAsync("Bases de datos") },
                    new Habilidad { Nombre = "Python", Descripcion = "Lenguaje usado en análisis de datos.", IdCategoria = await IdCategoriaAsync("Ciencia de datos") },
                    new Habilidad { Nombre = "TensorFlow", Descripcion = "Librería para aprendizaje automático.", IdCategoria = await IdCategoriaAsync("Inteligencia artificial") },
                    new Habilidad { Nombre = "Pruebas de penetración", Descripcion = "Búsqueda controlada de vulnerabilidades.", IdCategoria = await IdCategoriaAsync("Ciberseguridad") },
                    new Habilidad { Nombre = "Docker", Descripcion = "Contenedores para empaquetar aplicaciones.", IdCategoria = await IdCategoriaAsync("DevOps") },
                    new Habilidad { Nombre = "Arduino", Descripcion = "Placas para prototipos electrónicos.", IdCategoria = await IdCategoriaAsync("IoT") });
                await _context.SaveChangesAsync();
            }
        }

        // ---------- Empresas ----------
        private async Task CheckEmpresasAsync()
        {
            if (!await _context.Empresas.AnyAsync())
            {
                _context.Empresas.AddRange(
                    NuevaEmpresa("900100001-1", "TecnoAndes S.A.S.", "Desarrollo de software a la medida.", "Medellín", "Calle 10 # 43-25", "6044000001", "tecnoandes"),
                    NuevaEmpresa("900100002-2", "Nube Pacífico S.A.S.", "Servicios en la nube y DevOps.", "Cali", "Avenida 6 # 25-40", "6024000002", "nubepacifico"),
                    NuevaEmpresa("900100003-3", "DataCaribe S.A.S.", "Analítica de datos para empresas.", "Barranquilla", "Carrera 53 # 80-10", "6054000003", "datacaribe"),
                    NuevaEmpresa("900100004-4", "SeguraNet Ltda.", "Ciberseguridad para pequeñas y medianas empresas.", "Bogotá", "Calle 93 # 15-20", "6014000004", "seguranet"),
                    NuevaEmpresa("900100005-5", "AppsCafé S.A.S.", "Aplicaciones móviles a la medida.", "Manizales", "Carrera 23 # 60-15", "6064000005", "appscafe"),
                    NuevaEmpresa("900100006-6", "Inteligencia Andina S.A.S.", "Soluciones de inteligencia artificial.", "Bogotá", "Carrera 7 # 71-21", "6014000006", "inteligenciaandina"),
                    NuevaEmpresa("900100007-7", "SensorAgro S.A.S.", "Internet de las cosas para el campo.", "Pereira", "Avenida 30 de Agosto # 40-12", "6064000007", "sensoragro"),
                    NuevaEmpresa("900100008-8", "WebCreativa S.A.S.", "Diseño y desarrollo de sitios web.", "Bucaramanga", "Calle 36 # 27-50", "6074000008", "webcreativa"),
                    NuevaEmpresa("900100009-9", "BaseSegura S.A.S.", "Administración de bases de datos.", "Medellín", "Carrera 43A # 1-50", "6044000009", "basesegura"),
                    NuevaEmpresa("900100010-0", "CódigoNorte S.A.S.", "Consultoría en desarrollo backend.", "Cúcuta", "Avenida 0 # 12-30", "6074000010", "codigonorte"));
                await _context.SaveChangesAsync();
            }
        }

        private static Empresa NuevaEmpresa(string nit, string nombre, string descripcion, string ciudad,
            string direccion, string celular, string dominio) => new Empresa
            {
                NIT = nit,
                Nombre = nombre,
                Descripcion = descripcion,
                Pais = "Colombia",
                Ciudad = ciudad,
                Direccion = direccion,
                Celular = celular,
                Correo = $"contacto@{dominio}.example.com",
                SitioWeb = $"https://{dominio}.example.com"
            };

        // ---------- Clientes ----------
        private async Task CheckClientesAsync()
        {
            if (!await _context.Clientes.AnyAsync())
            {
                _context.Clientes.AddRange(
                    NuevoCliente("1030000001", "Carlos", "Pérez", "3201000001", "carlos.perez"),
                    NuevoCliente("1030000002", "Ana", "Rodríguez", "3201000002", "ana.rodriguez"),
                    NuevoCliente("1030000003", "Luis", "Hernández", "3201000003", "luis.hernandez"),
                    NuevoCliente("1030000004", "Paula", "Jiménez", "3201000004", "paula.jimenez"),
                    NuevoCliente("1030000005", "Jorge", "Morales", "3201000005", "jorge.morales"),
                    NuevoCliente("1030000006", "Natalia", "Rojas", "3201000006", "natalia.rojas"),
                    NuevoCliente("1030000007", "Felipe", "Mejía", "3201000007", "felipe.mejia"),
                    NuevoCliente("1030000008", "Carolina", "Suárez", "3201000008", "carolina.suarez"),
                    NuevoCliente("1030000009", "Ricardo", "Duque", "3201000009", "ricardo.duque"),
                    NuevoCliente("1030000010", "Isabel", "Arango", "3201000010", "isabel.arango"));
                await _context.SaveChangesAsync();
            }
        }

        private static Cliente NuevoCliente(string cedula, string nombre, string apellido, string celular, string usuario) => new Cliente
        {
            Cedula = cedula,
            Nombre = nombre,
            Apellido = apellido,
            Celular = celular,
            Correo = $"{usuario}@example.com"
        };

        // ---------- Desarrolladores ----------
        private async Task CheckDesarrolladoresAsync()
        {
            if (!await _context.Desarrolladores.AnyAsync())
            {
                _context.Desarrolladores.AddRange(
                    NuevoDesarrollador("1020000001", "Andrés", "Gómez", "3101000001", "andres.gomez", new DateOnly(1998, 3, 14)),
                    NuevoDesarrollador("1020000002", "Laura", "Martínez", "3101000002", "laura.martinez", new DateOnly(2000, 7, 22)),
                    NuevoDesarrollador("1020000003", "Santiago", "Restrepo", "3101000003", "santiago.restrepo", new DateOnly(1996, 11, 5)),
                    NuevoDesarrollador("1020000004", "Valentina", "Ospina", "3101000004", "valentina.ospina", new DateOnly(2001, 1, 30)),
                    NuevoDesarrollador("1020000005", "Camilo", "Herrera", "3101000005", "camilo.herrera", new DateOnly(1999, 9, 12)),
                    NuevoDesarrollador("1020000006", "Daniela", "Castro", "3101000006", "daniela.castro", new DateOnly(2002, 4, 18)),
                    NuevoDesarrollador("1020000007", "Mateo", "Vargas", "3101000007", "mateo.vargas", new DateOnly(1997, 6, 9)),
                    NuevoDesarrollador("1020000008", "Sofía", "Ramírez", "3101000008", "sofia.ramirez", new DateOnly(2003, 2, 25)),
                    NuevoDesarrollador("1020000009", "Juan Pablo", "Torres", "3101000009", "juanpablo.torres", new DateOnly(1995, 12, 1)),
                    NuevoDesarrollador("1020000010", "Mariana", "López", "3101000010", "mariana.lopez", new DateOnly(2000, 10, 17)));
                await _context.SaveChangesAsync();
            }
        }

        private static Desarrollador NuevoDesarrollador(string cedula, string nombre, string apellido, string celular,
            string usuario, DateOnly nacimiento) => new Desarrollador
            {
                Cedula = cedula,
                Nombre = nombre,
                Apellido = apellido,
                Celular = celular,
                Correo = $"{usuario}@example.com",
                FechaNacimiento = nacimiento
            };

        // ---------- Perfiles profesionales (uno por desarrollador) ----------
        private async Task CheckPerfilesProfesionalesAsync()
        {
            if (!await _context.PerfilesProfesionales.AnyAsync())
            {
                _context.PerfilesProfesionales.AddRange(
                    await NuevoPerfilAsync("1020000001", "Desarrollador backend .NET", "Desarrollo de APIs REST con C# y SQL Server.", "Medellín", "HIBRIDO"),
                    await NuevoPerfilAsync("1020000002", "Desarrolladora frontend", "Interfaces web con React y diseño adaptable.", "Bogotá", "REMOTO"),
                    await NuevoPerfilAsync("1020000003", "Desarrollador full stack", "Aplicaciones web completas con JavaScript y C#.", "Cali", "PRESENCIAL"),
                    await NuevoPerfilAsync("1020000004", "Desarrolladora móvil", "Apps móviles multiplataforma con Flutter.", "Medellín", "REMOTO"),
                    await NuevoPerfilAsync("1020000005", "Analista de datos", "Análisis y visualización de datos con Python.", "Barranquilla", "HIBRIDO"),
                    await NuevoPerfilAsync("1020000006", "Ingeniera de inteligencia artificial", "Modelos de aprendizaje automático con TensorFlow.", "Bogotá", "REMOTO"),
                    await NuevoPerfilAsync("1020000007", "Analista de ciberseguridad", "Pruebas de penetración y aseguramiento de servidores.", "Bucaramanga", "PRESENCIAL"),
                    await NuevoPerfilAsync("1020000008", "Ingeniera DevOps", "Contenedores, integración y despliegue continuo.", "Pereira", "REMOTO"),
                    await NuevoPerfilAsync("1020000009", "Administrador de bases de datos", "Diseño y optimización de bases de datos SQL Server.", "Manizales", "HIBRIDO"),
                    await NuevoPerfilAsync("1020000010", "Desarrolladora IoT", "Prototipos con Arduino y sensores conectados.", "Medellín", "PRESENCIAL"));
                await _context.SaveChangesAsync();
            }
        }

        private async Task<PerfilProfesional> NuevoPerfilAsync(string cedula, string titulo, string descripcion,
            string ciudad, string modalidad) => new PerfilProfesional
            {
                IdDesarrollador = await IdDesarrolladorAsync(cedula),
                TituloProfesional = titulo,
                Descripcion = descripcion,
                Pais = "Colombia",
                Ciudad = ciudad,
                Modalidad = modalidad
            };

        // ---------- Experiencias ----------
        private async Task CheckExperienciasAsync()
        {
            if (!await _context.Experiencias.AnyAsync())
            {
                _context.Experiencias.AddRange(
                    await NuevaExperienciaAsync("1020000001", "Soluciones Ágiles S.A.S.", "Desarrollador junior .NET", "Mantenimiento de APIs internas.", new DateOnly(2022, 2, 1), null),
                    await NuevaExperienciaAsync("1020000002", "Estudio Creativo Web", "Desarrolladora frontend", "Maquetación de sitios con React.", new DateOnly(2021, 6, 1), new DateOnly(2024, 3, 31)),
                    await NuevaExperienciaAsync("1020000003", "Agencia Digital Pacífico", "Desarrollador web", "Sitios web para pequeños negocios.", new DateOnly(2020, 1, 15), new DateOnly(2023, 12, 15)),
                    await NuevaExperienciaAsync("1020000004", "Apps del Sur S.A.S.", "Desarrolladora Flutter", "Apps de domicilios y reservas.", new DateOnly(2023, 3, 1), null),
                    await NuevaExperienciaAsync("1020000005", "Retail Analítica S.A.S.", "Analista de datos junior", "Reportes de ventas con Python.", new DateOnly(2022, 8, 1), null),
                    await NuevaExperienciaAsync("1020000006", "Laboratorio de IA Universitario", "Asistente de investigación", "Entrenamiento de modelos de visión.", new DateOnly(2023, 1, 10), new DateOnly(2024, 12, 20)),
                    await NuevaExperienciaAsync("1020000007", "Seguridad Total Ltda.", "Analista de monitoreo", "Monitoreo de eventos de seguridad.", new DateOnly(2021, 9, 1), null),
                    await NuevaExperienciaAsync("1020000008", "Nube Andina S.A.S.", "Ingeniera DevOps junior", "Integración continua con Docker.", new DateOnly(2023, 5, 2), null),
                    await NuevaExperienciaAsync("1020000009", "Clínica Central", "Administrador de bases de datos", "Respaldos y afinamiento de SQL Server.", new DateOnly(2019, 4, 1), new DateOnly(2024, 6, 30)),
                    await NuevaExperienciaAsync("1020000010", "AgroTec Sensores", "Desarrolladora IoT", "Sensores de humedad para cultivos.", new DateOnly(2024, 1, 15), null));
                await _context.SaveChangesAsync();
            }
        }

        // Si no tiene fecha de fin, es el trabajo actual (RN11).
        private async Task<Experiencia> NuevaExperienciaAsync(string cedula, string empresa, string cargo, string descripcion,
            DateOnly inicio, DateOnly? fin) => new Experiencia
            {
                IdPerfilProfesional = await IdPerfilAsync(cedula),
                Empresa = empresa,
                Cargo = cargo,
                Descripcion = descripcion,
                FechaInicio = inicio,
                FechaFin = fin,
                EstadoActual = fin == null
            };

        // ---------- Educación ----------
        private async Task CheckEducacionesAsync()
        {
            if (!await _context.Educaciones.AnyAsync())
            {
                _context.Educaciones.AddRange(
                    await NuevaEducacionAsync("1020000001", "Universitario", "Universidad Andina de Tecnología", "Ingeniería de Sistemas", new DateOnly(2016, 2, 1), new DateOnly(2021, 12, 10)),
                    await NuevaEducacionAsync("1020000002", "Tecnologo", "Instituto Tecnológico del Valle", "Tecnología en Desarrollo de Software", new DateOnly(2018, 2, 1), new DateOnly(2020, 11, 30)),
                    await NuevaEducacionAsync("1020000003", "Universitario", "Universidad del Pacífico Digital", "Ingeniería de Software", new DateOnly(2015, 8, 1), new DateOnly(2020, 6, 30)),
                    await NuevaEducacionAsync("1020000004", "Tecnologo", "Politécnico Nacional de Software", "Tecnología en Sistemas", new DateOnly(2019, 2, 1), new DateOnly(2021, 12, 1)),
                    await NuevaEducacionAsync("1020000005", "Universitario", "Universidad Andina de Tecnología", "Estadística", new DateOnly(2017, 2, 1), new DateOnly(2022, 6, 30)),
                    await NuevaEducacionAsync("1020000006", "Postgrado", "Universidad del Pacífico Digital", "Maestría en Inteligencia Artificial", new DateOnly(2025, 2, 1), null),
                    await NuevaEducacionAsync("1020000007", "Universitario", "Universidad Tecnológica de Santander", "Ingeniería de Telecomunicaciones", new DateOnly(2015, 2, 1), new DateOnly(2020, 12, 1)),
                    await NuevaEducacionAsync("1020000008", "Tecnico", "Instituto Técnico de la Nube", "Técnico en Redes y Servidores", new DateOnly(2021, 2, 1), new DateOnly(2022, 6, 30)),
                    await NuevaEducacionAsync("1020000009", "Universitario", "Universidad Cafetera de Ingeniería", "Ingeniería de Sistemas", new DateOnly(2013, 2, 1), new DateOnly(2018, 12, 1)),
                    await NuevaEducacionAsync("1020000010", "Universitario", "Universidad Andina de Tecnología", "Ingeniería Electrónica", new DateOnly(2022, 2, 1), null));
                await _context.SaveChangesAsync();
            }
        }

        // Si no tiene fecha de fin, está en curso (RN11).
        private async Task<Educacion> NuevaEducacionAsync(string cedula, string nivel, string institucion, string titulo,
            DateOnly inicio, DateOnly? fin) => new Educacion
            {
                IdPerfilProfesional = await IdPerfilAsync(cedula),
                NivelEducativo = nivel,
                Institucion = institucion,
                Titulo = titulo,
                FechaInicio = inicio,
                FechaFin = fin,
                EnCurso = fin == null
            };

        // ---------- Certificaciones ----------
        private async Task CheckCertificacionesAsync()
        {
            if (!await _context.Certificaciones.AnyAsync())
            {
                _context.Certificaciones.AddRange(
                    await NuevaCertificacionAsync("1020000001", "Desarrollo de APIs con ASP.NET Core", "Academia Digital .NET", new DateOnly(2024, 5, 10), null),
                    await NuevaCertificacionAsync("1020000002", "React avanzado", "Escuela Frontend Online", new DateOnly(2023, 9, 1), null),
                    await NuevaCertificacionAsync("1020000003", "Fundamentos de JavaScript", "Academia Web Abierta", new DateOnly(2022, 3, 15), null),
                    await NuevaCertificacionAsync("1020000004", "Desarrollo de apps con Flutter", "Escuela Móvil Online", new DateOnly(2024, 2, 20), null),
                    await NuevaCertificacionAsync("1020000005", "Análisis de datos con Python", "Academia de Datos", new DateOnly(2023, 11, 11), new DateOnly(2026, 11, 11)),
                    await NuevaCertificacionAsync("1020000006", "Aprendizaje profundo", "Academia de Inteligencia Artificial", new DateOnly(2024, 8, 30), null),
                    await NuevaCertificacionAsync("1020000007", "Pruebas de penetración", "Centro de Formación en Ciberseguridad", new DateOnly(2023, 6, 1), new DateOnly(2026, 6, 1)),
                    await NuevaCertificacionAsync("1020000008", "Contenedores con Docker", "Academia DevOps", new DateOnly(2024, 4, 4), new DateOnly(2027, 4, 4)),
                    await NuevaCertificacionAsync("1020000009", "Administración de SQL Server", "Academia de Bases de Datos", new DateOnly(2022, 10, 10), new DateOnly(2025, 10, 10)),
                    await NuevaCertificacionAsync("1020000010", "Sistemas embebidos con Arduino", "Laboratorio Maker", new DateOnly(2024, 7, 7), null));
                await _context.SaveChangesAsync();
            }
        }

        private async Task<Certificacion> NuevaCertificacionAsync(string cedula, string titulo, string institucion,
            DateOnly emision, DateOnly? vencimiento) => new Certificacion
            {
                IdPerfilProfesional = await IdPerfilAsync(cedula),
                Titulo = titulo,
                Institucion = institucion,
                FechaEmision = emision,
                FechaVencimiento = vencimiento
            };

        // ---------- Habilidades de cada perfil ----------
        private async Task CheckPerfilesHabilidadesAsync()
        {
            if (!await _context.Perfiles_Habilidades.AnyAsync())
            {
                var asignaciones = new (string Cedula, string Habilidad)[]
                {
                    ("1020000001", "C#"),
                    ("1020000002", "React"),
                    ("1020000003", "JavaScript"),
                    ("1020000004", "Flutter"),
                    ("1020000005", "Python"),
                    ("1020000006", "TensorFlow"),
                    ("1020000007", "Pruebas de penetración"),
                    ("1020000008", "Docker"),
                    ("1020000009", "SQL Server"),
                    ("1020000010", "Arduino")
                };

                foreach (var (cedula, habilidad) in asignaciones)
                {
                    _context.Perfiles_Habilidades.Add(new Perfil_Habilidad
                    {
                        IdPerfilProfesional = await IdPerfilAsync(cedula),
                        IdHabilidad = await IdHabilidadAsync(habilidad)
                    });
                }
                await _context.SaveChangesAsync();
            }
        }

        // ---------- Publicaciones: 6 empleos y 4 servicios ----------
        private async Task CheckPublicacionesAsync()
        {
            if (!await _context.Publicaciones.AnyAsync())
            {
                _context.Publicaciones.AddRange(
                    // Ofertas de empleo publicadas por empresas (RN20: salario, contrato y nivel)
                    await NuevoEmpleoAsync("900100001-1", "Backend", "Desarrollador backend .NET junior",
                        "Desarrollo y mantenimiento de APIs REST con C# y SQL Server.", 3500000, "Indefinido", "Junior", "HIBRIDO", "Medellín"),
                    await NuevoEmpleoAsync("900100008-8", "Frontend", "Desarrollador frontend React",
                        "Construcción de interfaces web adaptables con React.", 4000000, "Término fijo", "Semisenior", "REMOTO", "Bucaramanga"),
                    await NuevoEmpleoAsync("900100003-3", "Ciencia de datos", "Analista de datos",
                        "Limpieza, análisis y visualización de datos de ventas.", 3800000, "Indefinido", "Junior", "PRESENCIAL", "Barranquilla"),
                    await NuevoEmpleoAsync("900100004-4", "Ciberseguridad", "Analista de ciberseguridad",
                        "Pruebas de penetración y respuesta a incidentes.", 5000000, "Indefinido", "Semisenior", "HIBRIDO", "Bogotá"),
                    await NuevoEmpleoAsync("900100002-2", "DevOps", "Ingeniero DevOps",
                        "Automatización de despliegues con contenedores.", 6500000, "Indefinido", "Senior", "REMOTO", "Cali"),
                    await NuevoEmpleoAsync("900100006-6", "Inteligencia artificial", "Ingeniero de aprendizaje automático",
                        "Entrenamiento y puesta en producción de modelos.", 7000000, "Indefinido", "Senior", "HIBRIDO", "Bogotá"),

                    // Solicitudes de servicio (RN20: presupuesto y plazo)
                    await NuevoServicioAsync(null, "1030000001", "Desarrollo web", "Página web para tienda de ropa",
                        "Sitio con catálogo de productos y formulario de contacto.", 2500000, 30, "REMOTO", "Medellín"),
                    await NuevoServicioAsync(null, "1030000002", "Desarrollo móvil", "App de reservas para peluquería",
                        "Aplicación móvil para agendar citas y recibir recordatorios.", 4500000, 60, "REMOTO", "Bogotá"),
                    await NuevoServicioAsync(null, "1030000003", "Bases de datos", "Base de datos para inventario",
                        "Diseño de una base de datos para controlar el inventario de una ferretería.", 1800000, 20, "PRESENCIAL", "Cali"),
                    // Una empresa también puede publicar servicios (RN19)
                    await NuevoServicioAsync("900100007-7", null, "IoT", "Prototipo de sensores para cultivo",
                        "Prototipo con sensores de humedad y temperatura conectados a internet.", 6000000, 90, "HIBRIDO", "Pereira"));
                await _context.SaveChangesAsync();
            }
        }

        private async Task<Publicacion> NuevoEmpleoAsync(string nitEmpresa, string categoria, string titulo, string descripcion,
            decimal salario, string tipoContrato, string nivel, string modalidad, string ciudad) => new Publicacion
            {
                IdEmpresa = await IdEmpresaAsync(nitEmpresa),
                IdCategoria = await IdCategoriaAsync(categoria),
                Titulo = titulo,
                Descripcion = descripcion,
                Tipo = "EMPLEO",
                Estado = "PUBLICADA",
                FechaCreacion = new DateOnly(2026, 10, 20),
                FechaPublicacion = new DateOnly(2026, 10, 21),
                FechaVencimiento = new DateOnly(2026, 12, 31),
                Salario = salario,
                Moneda = "COP",
                TipoContrato = tipoContrato,
                NivelExperiencia = nivel,
                ModalidadTrabajo = modalidad,
                Pais = "Colombia",
                Ciudad = ciudad
            };

        private async Task<Publicacion> NuevoServicioAsync(string nitEmpresa, string cedulaCliente, string categoria,
            string titulo, string descripcion, decimal presupuesto, int plazo, string modalidad, string ciudad) => new Publicacion
            {
                IdEmpresa = nitEmpresa == null ? null : await IdEmpresaAsync(nitEmpresa),
                IdCliente = cedulaCliente == null ? null : await IdClienteAsync(cedulaCliente),
                IdCategoria = await IdCategoriaAsync(categoria),
                Titulo = titulo,
                Descripcion = descripcion,
                Tipo = "SERVICIO",
                Estado = "PUBLICADA",
                FechaCreacion = new DateOnly(2026, 10, 22),
                FechaPublicacion = new DateOnly(2026, 10, 23),
                FechaVencimiento = new DateOnly(2026, 12, 15),
                Presupuesto = presupuesto,
                Plazo = plazo,
                Moneda = "COP",
                ModalidadTrabajo = modalidad,
                Pais = "Colombia",
                Ciudad = ciudad
            };

        // ---------- Habilidades que pide cada publicación ----------
        private async Task CheckPublicacionesHabilidadesAsync()
        {
            if (!await _context.Publicaciones_Habilidades.AnyAsync())
            {
                var asignaciones = new (string Titulo, string Habilidad)[]
                {
                    ("Desarrollador backend .NET junior", "C#"),
                    ("Desarrollador frontend React", "React"),
                    ("Analista de datos", "Python"),
                    ("Analista de ciberseguridad", "Pruebas de penetración"),
                    ("Ingeniero DevOps", "Docker"),
                    ("Ingeniero de aprendizaje automático", "TensorFlow"),
                    ("Página web para tienda de ropa", "JavaScript"),
                    ("App de reservas para peluquería", "Flutter"),
                    ("Base de datos para inventario", "SQL Server"),
                    ("Prototipo de sensores para cultivo", "Arduino")
                };

                foreach (var (titulo, habilidad) in asignaciones)
                {
                    _context.Publicaciones_Habilidades.Add(new Publicacion_Habilidad
                    {
                        IdPublicacion = await IdPublicacionAsync(titulo),
                        IdHabilidad = await IdHabilidadAsync(habilidad)
                    });
                }
                await _context.SaveChangesAsync();
            }
        }

        // ---------- Aplicaciones ----------
        private async Task CheckAplicacionesAsync()
        {
            if (!await _context.Aplicaciones.AnyAsync())
            {
                _context.Aplicaciones.AddRange(
                    // A empleos: sin monto ni plazo (RN30)
                    await NuevaAplicacionAsync("Desarrollador backend .NET junior", "1020000001", 22, "Tengo dos años de experiencia con C#.", "PENDIENTE", null, null),
                    await NuevaAplicacionAsync("Desarrollador backend .NET junior", "1020000003", 23, "Trabajo como full stack y quiero enfocarme en backend.", "EN_REVISION", null, null),
                    await NuevaAplicacionAsync("Desarrollador frontend React", "1020000002", 22, "Tengo experiencia maquetando sitios con React.", "ACEPTADA", null, null),
                    await NuevaAplicacionAsync("Analista de datos", "1020000005", 24, "Realizo reportes de ventas con Python.", "PENDIENTE", null, null),
                    await NuevaAplicacionAsync("Analista de ciberseguridad", "1020000007", 24, "Cuento con certificación en pruebas de penetración.", "EN_REVISION", null, null),
                    await NuevaAplicacionAsync("Ingeniero DevOps", "1020000008", 25, "Manejo Docker y pipelines de integración continua.", "PENDIENTE", null, null),
                    await NuevaAplicacionAsync("Ingeniero de aprendizaje automático", "1020000006", 25, "Estoy cursando una maestría en inteligencia artificial.", "RECHAZADA", null, null),

                    // A servicios: con monto y plazo propuestos (RN30)
                    await NuevaAplicacionAsync("Página web para tienda de ropa", "1020000003", 26, "Puedo entregar el sitio con catálogo y formulario.", "PENDIENTE", 25, 2300000),
                    await NuevaAplicacionAsync("App de reservas para peluquería", "1020000004", 27, "He desarrollado apps de reservas con Flutter.", "EN_REVISION", 55, 4200000),
                    await NuevaAplicacionAsync("Prototipo de sensores para cultivo", "1020000010", 28, "Trabajo con sensores de humedad para cultivos.", "PENDIENTE", 85, 5800000));
                await _context.SaveChangesAsync();
            }
        }

        private async Task<Aplicacion> NuevaAplicacionAsync(string tituloPublicacion, string cedula, int diaOctubre,
            string mensaje, string estado, int? plazo, decimal? monto) => new Aplicacion
            {
                IdPublicacion = await IdPublicacionAsync(tituloPublicacion),
                IdDesarrollador = await IdDesarrolladorAsync(cedula),
                FechaAplicacion = new DateOnly(2026, 10, diaOctubre),
                Mensaje = mensaje,
                Estado = estado,
                PlazoEstimado = plazo,
                MontoPropuesto = monto
            };

        // ---------- Búsquedas de Id para armar las relaciones ----------
        private Task<int> IdCategoriaAsync(string nombre) =>
            _context.Categorias.Where(x => x.Nombre == nombre).Select(x => x.Id).FirstAsync();

        private Task<int> IdHabilidadAsync(string nombre) =>
            _context.Habilidades.Where(x => x.Nombre == nombre).Select(x => x.Id).FirstAsync();

        private Task<int> IdEmpresaAsync(string nit) =>
            _context.Empresas.Where(x => x.NIT == nit).Select(x => x.Id).FirstAsync();

        private Task<int> IdClienteAsync(string cedula) =>
            _context.Clientes.Where(x => x.Cedula == cedula).Select(x => x.Id).FirstAsync();

        private Task<int> IdDesarrolladorAsync(string cedula) =>
            _context.Desarrolladores.Where(x => x.Cedula == cedula).Select(x => x.Id).FirstAsync();

        private Task<int> IdPerfilAsync(string cedulaDesarrollador) =>
            _context.PerfilesProfesionales.Where(x => x.Desarrollador.Cedula == cedulaDesarrollador).Select(x => x.Id).FirstAsync();

        private Task<int> IdPublicacionAsync(string titulo) =>
            _context.Publicaciones.Where(x => x.Titulo == titulo).Select(x => x.Id).FirstAsync();
    }
}
