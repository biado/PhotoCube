using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly ObjectContext coContext;

        public TagsetController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/tagset
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicTagset>>> Get()
        {
            List<PublicTagset> allTagsets = await coContext.Tagsets
                .Select(t => new PublicTagset(t.Id, t.Name))
                .ToListAsync();

            return Ok(allTagsets); //implicit Ignore self referencing loops
        }

        // GET: api/tagset/5
        [HttpGet("{id:int}", Name = "GetTagset")]
        public async Task<ActionResult<Tagset>> Get(int id)
        {
            Tagset tagsetWithId = await coContext.Tagsets
                .Where(ts => ts.Id == id)
                .Include(ts => ts.Tags)
                .Include(ts => ts.Hierarchies)
                .FirstOrDefaultAsync();
            
            return Ok(tagsetWithId);
        }

        // GET: api/tagset/Year
        /// <summary>
        /// Returns all tags in a tagset as a list, where Tagset.name == tagsetName.
        /// </summary>
        /// <param tagsetName="tagsetName"></param>
        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<PublicTag>>> GetAllTagsByTagsetName(string name)
        {
            var tagset = await coContext.Tagsets
                .Include(ts => ts.Tags)
                    .ThenInclude(tag => tag.TagType)
                .FirstOrDefaultAsync(ts => ts.Name.ToLower() == name.ToLower());
            List<Tag>  tagsFound = tagset?.Tags;

            if (tagsFound == null) return NotFound();
            
            var result = tagsFound.Select(tag => new PublicTag(tag.Id, tag.GetTagName(),tag.TagsetId,tag.TagType.Description)).ToList();
            
            return Ok(result);
        }
    }
}
