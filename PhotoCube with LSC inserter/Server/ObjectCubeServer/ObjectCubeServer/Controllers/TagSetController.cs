using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ObjectCubeServer.Models.DataAccess;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsetController : ControllerBase
    {
        // GET: api/tagset
        [HttpGet]
        public IActionResult Get()
        {
            List<PublicTagset> allTagsets;
            using (var context = new ObjectContext())
            {
                allTagsets = context.Tagsets
                    .Select(t => new PublicTagset(t.Id, t.Name))
                    .ToList();
            }

            return Ok(JsonConvert.SerializeObject(allTagsets,
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })); //Ignore self referencing loops
        }

        // GET: api/tagset/5
        [HttpGet("{id}", Name = "GetTagset")]
        public IActionResult Get(int id)
        {
            Tagset tagsetWithId;
            using (var context = new ObjectContext())
            {
                tagsetWithId = context.Tagsets
                    .Where(ts => ts.Id == id)
                    .Include(ts => ts.Tags)
                    .Include(ts => ts.Hierarchies)
                    .FirstOrDefault();
            }
            return Ok(JsonConvert.SerializeObject(tagsetWithId, 
                new JsonSerializerSettings(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore})
            );
        }
    }
}
