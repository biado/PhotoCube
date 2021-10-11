using System.Collections.Generic;
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
    public class ThumbnailController : ControllerBase
    {
        // GET: api/Thumbnail
        [Produces("application/json")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<string> allThumbnailURIs;
            await using (var context = new ObjectContext())
            {
                allThumbnailURIs = await context.CubeObjects.Select(co => co.ThumbnailURI).ToListAsync();
            }
            if (allThumbnailURIs != null)
            {
                var data = new { thumbnailURIs = allThumbnailURIs };
                return Ok(data); //Does not return file!
            }
            else return NotFound();
        }

        // GET: api/Thumbnail/5
        [HttpGet("{id:int}", Name = "GetThumbnail")]
        public async Task<IActionResult> Get(int id)
        {
            string thumbnailURI;
            await using (var context = new ObjectContext())
            {
                CubeObject cubeObject = context.CubeObjects.FirstOrDefault(co => co.Id == id);
                thumbnailURI = cubeObject.ThumbnailURI;
            }
            if (thumbnailURI == null)
            {
                return NotFound();
            } 
            return Ok(thumbnailURI);
        }
    }
}
