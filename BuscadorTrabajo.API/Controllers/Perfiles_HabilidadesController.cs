using BuscadorTrabajo.API.Data;
using Microsoft.AspNetCore.Mvc;
using BuscadorTrabajo.Shared.Entities;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/perfiles_habilidades")]
    public class Perfiles_HabilidadesController : ControllerBase
    {
     
        private readonly DataContext  _context;

        public Perfiles_HabilidadesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {

            return Ok(await _context.Perfiles_Habilidades.ToListAsync());


        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var perfiles_habilidad = await _context.Perfiles_Habilidades.FirstOrDefaultAsync(x => x.Id == id);
            if (perfiles_habilidad == null)
            {
                return NotFound(); //404
            }
            return Ok(perfiles_habilidad);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Perfiles_Habilidad perfiles_habilidad)
        {
            _context.Add(perfiles_habilidad);
            await _context.SaveChangesAsync();
            return Ok(perfiles_habilidad);
        }

       
        public async Task<ActionResult> Put(Perfiles_Habilidad perfiles_habilidad)
        {
            _context.Update(perfiles_habilidad);
            await _context.SaveChangesAsync();
            return Ok(perfiles_habilidad);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var Perfiles_Habilidad = await _context.Perfiles_Habilidades.FirstOrDefaultAsync(x => x.Id == id);
            if (Perfiles_Habilidad == null)
            {
                return NotFound(); //404
            }
            _context.Remove(Perfiles_Habilidad);
            await _context.SaveChangesAsync();
            return NoContent(); //204
        }

        /*

        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {


            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;
            var query = _context.Publicaciones
            .AsNoTracking()//AsNoTracking() is used to improve performance when you don't need to track changes to the entities
            .OrderBy(x => x.Id);

            var totalRecords = await query.CountAsync(); //CountAsync() is used to get the total number of records in the query

            var data = await query
                .Skip((page - 1) * pageSize) //Skip() is used to skip the records that are not needed for the current page
                .Take(pageSize)//Take() is used to take the records that are needed for the current page
                .ToListAsync();//ToListAsync() is used to execute the query and get the results as a list1

            var totalPages =
            (int)Math.Ceiling(totalRecords / (double)pageSize); //Math.Ceiling() is used to round up the total number of pages  


            return Ok(new
            {
                TotalRecords = totalRecords, //TotalRecords is used to get the total number of records in the query
                TotalPages = totalPages, //TotalPages is used to get the total number of pages
                Page = page, //Page is used to get the current page
                PageSize = pageSize, //PageSize is used to get the number of records per page
                Data = data //Data is used to get the records for the current page
            });


        }


        [HttpGet("filter")]
        public async Task<ActionResult> GetFiltered(string text)
        {

            var query = _context.Publicaciones

            .AsNoTracking()//AsNoTracking() is used to improve performance when you don't need to track changes to the entities 
            .AsQueryable(); //AsQueryable() is used to create a query that can be filtered


            if (!string.IsNullOrWhiteSpace(text)) //Check if the text is not null or empty
            {
                text = text.Trim().ToLower(); //Trim() is used to remove the white spaces from the text and ToLower() is used to convert the text to lower case
                query = query.Where(x => //select * from Owners where FirstName like '%text%' or LastName like '%text%' or Document like '%text%'
              x.FirstName.ToLower().Contains(text) ||//Where() is used to filter the query and Contains() is used to check if the text is contained in the first name
              x.LastName.ToLower().Contains(text) ||//Where() is used to filter the query and Contains()
              x.Document.ToLower().Contains(text));  //Where() is used to filter the query and Contains()



            }

            var data = await query
                .OrderBy(x => x.FirstName) //OrderBy() is used to order the query by first name
                .ThenBy(x => x.LastName) //ThenBy() is used to order the query by last name
                .ToListAsync(); //ToListAsync() is used to execute the query and get the results as a list

            return Ok(data);
        }

        [HttpGet("{id:int}/hateoas")]
        public async Task<ActionResult> GetwithLinks(int id)
        {
            var publicacion = await _context.Publicaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (publicacion == null)
            {
                return NotFound(); //404
            }
            var response = new
            {
                publicacion,
                links = new List<object>
                {
                    new { rel = "self", href = Url.Action("Get", new { id = publicacion.Id }) },
                    new { rel = "update", href = Url.Action("Put") },
                    new { rel = "delete", href = Url.Action("Delete", new { id = publicacion.Id }) }
                }
            };
            return Ok(response);
        }

        */

    }
}
