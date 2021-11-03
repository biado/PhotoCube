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
        private readonly ObjectContext coContext;

        public ThumbnailController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/Thumbnail
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            List<string> allThumbnailURIs;

            allThumbnailURIs = await coContext.CubeObjects.Select(co => co.ThumbnailURI).ToListAsync();

            if (allThumbnailURIs == null)
                return NotFound();
            var data = new {thumbnailURIs = allThumbnailURIs};
            
            return Ok(data); //Does not return file!
        }

        // GET: api/Thumbnail/5
        [HttpGet("{id:int}", Name = "GetThumbnail")]
        public async Task<ActionResult<string>> Get(int id)
        {
            string thumbnailURI;
            CubeObject cubeObject = await coContext.CubeObjects.FirstOrDefaultAsync(co => co.Id == id);
            thumbnailURI = cubeObject.ThumbnailURI;
            
            if (thumbnailURI == null)
            {
                return NotFound();
            } 
            return Ok(thumbnailURI);
        }
    }
}
