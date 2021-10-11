using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        // GET: api/Photo/5
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> Get(int id)
        {
            string fileURI;
            await using (var context = new ObjectContext())
            {
                CubeObject cubeObject = await context.CubeObjects.FirstOrDefaultAsync(co => co.Id == id);
                fileURI = cubeObject?.FileURI;
                if(fileURI == null)
                {
                    return NotFound();
                }
            }
            return Ok(fileURI);
        }
    }
}
