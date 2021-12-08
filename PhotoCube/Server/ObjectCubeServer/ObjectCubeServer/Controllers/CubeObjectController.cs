using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubeObjectController : ControllerBase
    {
        private readonly ObjectContext coContext;

        public CubeObjectController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/CubeObject (currently not in use)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CubeObject>>> GetCubeObjects()
        {
            var allCubeObjects = await coContext.CubeObjects
                .Include(co => co.ObjectTagRelations)
                .ToListAsync();

            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/5 (currently not in use)
        [HttpGet("{id:int}", Name = "GetCubeObject")]
        public async Task<ActionResult<CubeObject>> Get(int id)
        {
            CubeObject cubeObjectFound = await coContext.CubeObjects.FirstOrDefaultAsync(co => co.Id == id);
            
            if (cubeObjectFound == null)
            {
                return NotFound();
            }
            return Ok(cubeObjectFound);
        }
        // GET : api/CubeObject/5/tags
        [HttpGet("{id:int}/tags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags(int id)
        {
            var cubeObjectFound = await coContext.CubeObjects.FirstOrDefaultAsync(co => co.Id == id);

            if (cubeObjectFound == null)
            {
                return NotFound();
            }
            
            var tags = await coContext.ObjectTagRelations
                .Include(otr => otr.Tag.TagType)
                .Where(otr => otr.ObjectId == id)
                .Select(otr => new PublicTag(otr.TagId,  otr.Tag.GetTagName(),otr.Tag.TagsetId,otr.Tag.TagType.Description))
                .ToListAsync();

            return Ok(tags);
        }
    }
}
