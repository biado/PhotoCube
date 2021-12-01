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
        private readonly ObjectContext coContext;

        public PhotoController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/Photo/5
        [HttpGet("{id:int}", Name = "GetPhoto")]
        public async Task<ActionResult<string>> Get(int id)
        {
            string fileURI;
            CubeObject cubeObject = await coContext.CubeObjects.FirstOrDefaultAsync(co => co.Id == id);
            fileURI = cubeObject?.FileURI;
            if(fileURI == null)
            {
                return NotFound();
            }
            
            return Ok(fileURI);
        }
    }
}
