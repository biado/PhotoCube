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
    public class TagController : ControllerBase
    {
        private readonly ObjectContext coContext;

        public TagController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/Tag
        /// <summary>
        /// returns all tags in the database
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> Get()
        {
            return Ok(await coContext.Tags.ToListAsync());
        }

        // GET: api/Tag/5
        /// <summary>
        /// Returns single tag where Tag.Id == id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tag>> Get(int id)
        {
            Tag tagFound = await coContext.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tagFound == null) return NotFound();
            
            return Ok(tagFound);
        }

        // GET: api/Tag/computer
        /// <summary>
        /// Returns single tag (of alphanumerical type) where Tag.name == name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagByName(string name)
        {
            List<Tag> tagsFound = await coContext.Tags
                .Include(tag =>tag.TagType)
                .Where(t => ((AlphanumericalTag)t).Name.ToLower().StartsWith(name.ToLower()))
                .ToListAsync();
            
            if (tagsFound == null) return NotFound();
            var result = tagsFound.Select(tag => new PublicTag(tag.Id,  tag.GetTagName(),tag.TagsetId,tag.TagType.Description)).ToList();
            
            return Ok(result);
        }
    }
}
