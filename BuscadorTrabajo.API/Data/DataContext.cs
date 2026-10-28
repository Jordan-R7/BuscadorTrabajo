using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }



    }
}
