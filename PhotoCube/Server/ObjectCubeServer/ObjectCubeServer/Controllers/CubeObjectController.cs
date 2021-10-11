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
    [Produces("application/json")]
    public class CubeObjectController : ControllerBase
    {
        // GET: api/CubeObject (currently not in use)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<CubeObject> allCubeObjects;
            await using (var context = new ObjectContext())
            {
                allCubeObjects = await context.CubeObjects
                    .Include(co => co.ObjectTagRelations)
                    .ToListAsync();
            }
            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/5 (currently not in use)
        [HttpGet("{id:int}", Name = "GetCubeObject")]
        public async Task<IActionResult> Get(int id)
        {
            CubeObject cubeObjectFound;
            await using (var context = new ObjectContext())
            {
                cubeObjectFound = await context.CubeObjects.FirstOrDefaultAsync(co => co.Id == id);
            }
            if (cubeObjectFound != null)
            {
                return Ok(cubeObjectFound);
            }
            else return NotFound();
        }

        // GET: api/CubeObject/fromTagId/1 (currently not in use)
        [HttpGet("[action]/{tagId}")]
        public async Task<IActionResult> FromTagId(int tagId)
        {
            List<CubeObject> allCubeObjects;
            await using (var context = new ObjectContext())
            {
                allCubeObjects = await context.CubeObjects
                    //.Include(co => co.ObjectTagRelations)
                    .Where(co => co.ObjectTagRelations.Any(otr => otr.TagId == tagId)) //Is tagged with tagId at least once
                    .ToListAsync();
            }

            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/from2TagIds/1/2 (currently not in use)
        [HttpGet("[action]/{tagId1:int}/{tagId2:int}")]
        public async Task<IActionResult> From2TagIds(int tagId1, int tagId2)
        {
            List<CubeObject> allCubeObjects;
            await using (var context = new ObjectContext())
            {
                allCubeObjects = await context.CubeObjects
                    .Where(co => 
                            co.ObjectTagRelations.Any(otr => otr.TagId == tagId1) &&  //Is tagged with tag1
                            co.ObjectTagRelations.Any(otr => otr.TagId == tagId2)     //Is tagged with tag2
                    ).ToListAsync();
            }

            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/from3TagIds/1/2/3 (currently not in use)
        [HttpGet("[action]/{tagId1:int}/{tagId2:int}/{tagId3:int}")]
        public async Task<IActionResult> From3TagIds(int tagId1, int tagId2, int tagId3)
        {
            List<CubeObject> allCubeObjects;
            await using (var context = new ObjectContext())
            {
                allCubeObjects = await context.CubeObjects
                    .Where(co =>
                            co.ObjectTagRelations.Any(otr => otr.TagId == tagId1) &&  //Is tagged with tag1
                            co.ObjectTagRelations.Any(otr => otr.TagId == tagId2) &&  //Is tagged with tag2
                            co.ObjectTagRelations.Any(otr => otr.TagId == tagId3)     //Is tagged with tag3
                    ).ToListAsync();
            }

            return Ok(allCubeObjects);

        }

        // GET: api/CubeObject/fromTagIdWithOTR/1 (currently not in use)
        [HttpGet("[action]/{tagId}")]
        public async Task<IActionResult> FromTagIdWithOTR(int tagId) //OTR is ObjectTagRelations
        {
            List<CubeObject> allCubeObjects;
            await using (var context = new ObjectContext())
            {
                allCubeObjects = await context.CubeObjects
                    .Include(co => co.ObjectTagRelations)
                    .Where(co => co.ObjectTagRelations.Any(otr => otr.TagId == tagId)) //Is tagged with tagId at least once
                    .ToListAsync();
            }

            return Ok(allCubeObjects);
        }
    }
}
