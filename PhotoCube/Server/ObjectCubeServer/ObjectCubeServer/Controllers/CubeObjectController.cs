using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Get()
        {
            List<CubeObject> allCubeObjects;
            using (var context = new ObjectContext())
            {
                allCubeObjects = context.CubeObjects
                    .Include(co => co.ObjectTagRelations)
                    .ToList();
            }
            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/5 (currently not in use)
        [HttpGet("{id:int}", Name = "GetCubeObject")]
        public IActionResult Get(int id)
        {
            CubeObject cubeObjectFound;
            using (var context = new ObjectContext())
            {
                cubeObjectFound = context.CubeObjects.Where(co => co.Id == id).FirstOrDefault();
            }
            if (cubeObjectFound != null)
            {
                return Ok(cubeObjectFound);
            }
            else return NotFound();
        }

        // GET: api/CubeObject/fromTagId/1 (currently not in use)
        [HttpGet("[action]/{tagId}")]
        public IActionResult FromTagId(int tagId)
        {
            List<CubeObject> allCubeObjects;
            using (var context = new ObjectContext())
            {
                allCubeObjects = context.CubeObjects
                    //.Include(co => co.ObjectTagRelations)
                    .Where(co => co.ObjectTagRelations.Where(otr => otr.TagId == tagId).Count() > 0) //Is tagged with tagId at least once
                    .ToList();
            }

            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/from2TagIds/1/2 (currently not in use)
        [HttpGet("[action]/{tagId1:int}/{tagId2:int}")]
        public IActionResult From2TagIds(int tagId1, int tagId2)
        {
            List<CubeObject> allCubeObjects;
            using (var context = new ObjectContext())
            {
                allCubeObjects = context.CubeObjects
                    .Where(co => 
                        co.ObjectTagRelations.Where(otr => otr.TagId == tagId1).Count() > 0 &&  //Is tagged with tag1
                        co.ObjectTagRelations.Where(otr => otr.TagId == tagId2).Count() > 0     //Is tagged with tag2
                    ).ToList();
            }

            return Ok(allCubeObjects);
        }

        // GET: api/CubeObject/from3TagIds/1/2/3 (currently not in use)
        [HttpGet("[action]/{tagId1:int}/{tagId2:int}/{tagId3:int}")]
        public IActionResult From3TagIds(int tagId1, int tagId2, int tagId3)
        {
            List<CubeObject> allCubeObjects;
            using (var context = new ObjectContext())
            {
                allCubeObjects = context.CubeObjects
                    .Where(co =>
                        co.ObjectTagRelations.Where(otr => otr.TagId == tagId1).Count() > 0 &&  //Is tagged with tag1
                        co.ObjectTagRelations.Where(otr => otr.TagId == tagId2).Count() > 0 &&  //Is tagged with tag2
                        co.ObjectTagRelations.Where(otr => otr.TagId == tagId3).Count() > 0     //Is tagged with tag3
                    ).ToList();
            }

            return Ok(allCubeObjects);

        }

        // GET: api/CubeObject/fromTagIdWithOTR/1 (currently not in use)
        [HttpGet("[action]/{tagId}")]
        public IActionResult FromTagIdWithOTR(int tagId) //OTR is ObjectTagRelations
        {
            List<CubeObject> allCubeObjects;
            using (var context = new ObjectContext())
            {
                allCubeObjects = context.CubeObjects
                    .Include(co => co.ObjectTagRelations)
                    .Where(co => co.ObjectTagRelations.Where(otr => otr.TagId == tagId).Count() > 0) //Is tagged with tagId at least once
                    .ToList();
            }

            return Ok(allCubeObjects);
        }
    }
}
