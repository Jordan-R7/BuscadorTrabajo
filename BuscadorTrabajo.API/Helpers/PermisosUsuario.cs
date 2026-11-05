using System.Security.Claims;
using BuscadorTrabajo.Shared.Entities;

namespace BuscadorTrabajo.API.Helpers
{
    // Reglas de "quién puede ver o cambiar qué", a partir de los datos del token.
    // El token trae el rol del usuario y el Id de su registro (claim "IdRegistro"):
    // el Id del desarrollador, de la empresa, del cliente o del administrador.
    public static class PermisosUsuario
    {
        public static int IdRegistro(this ClaimsPrincipal usuario)
        {
            return int.TryParse(usuario.FindFirst("IdRegistro")?.Value, out var id) ? id : 0;
        }

        public static bool EsAdmin(this ClaimsPrincipal usuario) => usuario.IsInRole("Admin");

        // El dueño de una publicación es la empresa o el cliente que la publicó (o un administrador)
        public static bool EsDuenoPublicacion(this ClaimsPrincipal usuario, Publicacion publicacion)
        {
            if (usuario.EsAdmin())
            {
                return true;
            }

            var id = usuario.IdRegistro();
            return id > 0
                && ((usuario.IsInRole("Empresa") && publicacion.IdEmpresa == id)
                    || (usuario.IsInRole("Cliente") && publicacion.IdCliente == id));
        }

        // Aplicaciones que cada usuario puede ver:
        // el administrador todas, el desarrollador las suyas,
        // la empresa y el cliente las que llegaron a sus publicaciones.
        public static IQueryable<Aplicacion> VisiblesPara(this IQueryable<Aplicacion> consulta, ClaimsPrincipal usuario)
        {
            if (usuario.EsAdmin())
            {
                return consulta;
            }

            var id = usuario.IdRegistro();
            if (usuario.IsInRole("Desarrollador"))
            {
                return consulta.Where(x => x.IdDesarrollador == id);
            }

            if (usuario.IsInRole("Empresa"))
            {
                return consulta.Where(x => x.Publicacion.IdEmpresa == id);
            }

            if (usuario.IsInRole("Cliente"))
            {
                return consulta.Where(x => x.Publicacion.IdCliente == id);
            }

            return consulta.Where(x => false);
        }
    }
}
