using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
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

        // GET: api/CubeObject/fromTagId/1 (currently not in use)
        [HttpGet("[action]/{tagId:int}")]
        public async Task<ActionResult<IEnumerable<CubeObject>>> FromTagId(int tagId)
        {
            List<CubeObject> allCubeObjects = await coContext.CubeObjects
                //.Include(co => co.ObjectTagRelations)
                .Where(co => co.ObjectTagRelations.Any(otr => otr.TagId == tagId)) //Is tagged with tagId at least once
                .ToListAsync();

            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/from2TagIds/1/2 (currently not in use)
        [HttpGet("[action]/{tagId1:int}/{tagId2:int}")]
        public async Task<ActionResult<IEnumerable<CubeObject>>> From2TagIds(int tagId1, int tagId2)
        {

            List<CubeObject> allCubeObjects = await coContext.CubeObjects
                .Where(co => 
                        co.ObjectTagRelations.Any(otr => otr.TagId == tagId1) &&  //Is tagged with tag1
                        co.ObjectTagRelations.Any(otr => otr.TagId == tagId2)     //Is tagged with tag2
                        ).ToListAsync();
            
        return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/from3TagIds/1/2/3 (currently not in use)
        [HttpGet("[action]/{tagId1:int}/{tagId2:int}/{tagId3:int}")]
        public async Task<ActionResult<IEnumerable<CubeObject>>> From3TagIds(int tagId1, int tagId2, int tagId3)
        {
            List<CubeObject> allCubeObjects = await coContext.CubeObjects
                .Where(co =>
                        co.ObjectTagRelations.Any(otr => otr.TagId == tagId1) &&  //Is tagged with tag1
                        co.ObjectTagRelations.Any(otr => otr.TagId == tagId2) &&  //Is tagged with tag2
                        co.ObjectTagRelations.Any(otr => otr.TagId == tagId3)     //Is tagged with tag3
                ).ToListAsync();

            return Ok(allCubeObjects);

        }

        // GET: api/CubeObject/fromTagIdWithOTR/1 (currently not in use)
        [HttpGet("[action]/{tagId}")]
        public async Task<ActionResult<IEnumerable<CubeObject>>> FromTagIdWithOTR(int tagId) //OTR is ObjectTagRelations
        {
            List<CubeObject> allCubeObjects = await coContext.CubeObjects
                .Include(co => co.ObjectTagRelations)
                .Where(co => co.ObjectTagRelations.Any(otr => otr.TagId == tagId)) //Is tagged with tagId at least once
                .ToListAsync();

            return Ok(allCubeObjects);
        }
    }
}
