namespace BuscadorTrabajo.WEB.Shared
{
    // Respuesta de los endpoints /paginated del API:
    // { page, pageSize, totalRecords, totalPages, data }
    public class PaginatedResponse<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public List<T> Data { get; set; } = new List<T>();
    }
}
