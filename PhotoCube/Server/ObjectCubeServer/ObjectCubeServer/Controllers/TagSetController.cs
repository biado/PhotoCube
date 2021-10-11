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
    public class TagsetController : ControllerBase
    {
        // GET: api/tagset
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<PublicTagset> allTagsets;
            await using (var context = new ObjectContext())
            {
                allTagsets = await context.Tagsets
                    .Select(t => new PublicTagset(t.Id, t.Name))
                    .ToListAsync();
            }

            return Ok(allTagsets); //Ignore self referencing loops
        }

        // GET: api/tagset/5
        [HttpGet("{id}", Name = "GetTagset")]
        public async Task<IActionResult> Get(int id)
        {
            Tagset tagsetWithId;
            await using (var context = new ObjectContext())
            {
                tagsetWithId = context.Tagsets
                    .Where(ts => ts.Id == id)
                    .Include(ts => ts.Tags)
                    .Include(ts => ts.Hierarchies)
                    .FirstOrDefault();
            }
            return Ok(tagsetWithId);
        }

        // GET: api/tagset/name=Year
        /// <summary>
        /// Returns all tags in a tagset as a list, where Tagset.name == tagsetName.
        /// </summary>
        /// <param tagsetName="tagsetName"></param>
        [HttpGet("name={name}")]
        public async Task<IActionResult> GetAllTagsByTagsetName(string name)
        {
            List<Tag> tagsFound;
            await using (var context = new ObjectContext())
            {
                var Tagset = context.Tagsets
                    .Include(ts => ts.Tags)
                    .FirstOrDefault(ts => ts.Name.ToLower() == name.ToLower());
                tagsFound = Tagset?.Tags;
            }

            if (tagsFound != null)
            {
                var result = tagsFound.Select(tag => new PublicTag(tag.Id, tag.GetTagName())).ToList();
                return Ok(result);
            }
            return NotFound();
        }
    }
}
