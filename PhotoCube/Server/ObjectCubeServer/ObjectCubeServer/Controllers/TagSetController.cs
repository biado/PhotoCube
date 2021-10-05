using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ObjectCubeServer.Models.Contexts;
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

        // GET: api/tagset/name=Year
        /// <summary>
        /// Returns all tags in a tagset as a list, where Tagset.name == tagsetName.
        /// </summary>
        /// <param tagsetName="tagsetName"></param>
        [HttpGet("name={name}")]
        public IActionResult GetAllTagsByTagsetName(string name)
        {
            List<Tag> tagsFound;
            using (var context = new ObjectContext())
            {
                var Tagset = context.Tagsets
                    .Include(ts => ts.Tags)
                    .FirstOrDefault(ts => ts.Name.ToLower() == name.ToLower());
                tagsFound = Tagset?.Tags;
            }

            if (tagsFound != null)
            {
                var result = tagsFound.Select(tag => new PublicTag(tag.Id, tag.GetTagName())).ToList();
                return Ok(JsonConvert.SerializeObject(result));
            }
            return NotFound();
        }
    }
}
