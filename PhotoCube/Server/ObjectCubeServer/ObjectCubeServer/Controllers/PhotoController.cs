using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Get(int id)
        {
            string fileURI;
            using (var context = new ObjectContext())
            {
                CubeObject cubeObject = context.CubeObjects.FirstOrDefault(co => co.Id == id);
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
