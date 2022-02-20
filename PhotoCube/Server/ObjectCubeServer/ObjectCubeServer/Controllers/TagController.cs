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
        // GET: api/tag?cubeObjectId=1
        /// <summary>
        /// Either returns all tags in the database: api/tag.
        /// Or returns all tags that cubeObject with cubeObjectId is tagged with.
        /// </summary>
        /// <param name="cubeObjectId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> Get(int? cubeObjectId)
        {
            if (cubeObjectId == null)
            {
                List<Tag>  allTags = await coContext.Tags.ToListAsync();
                
                return Ok(allTags);
            }

            List<string> tagsFound = await coContext.ObjectTagRelations
                .Where(otr => otr.ObjectId == cubeObjectId)
                .Select(otr => otr.Tag.GetTagName())
                .ToListAsync();

            if (tagsFound == null) return NotFound();
            
            return Ok(tagsFound);
        }

        // GET: api/Tag/5
        /// <summary>
        /// Returns single tag where Tag.Id == id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetTag")]
        public async Task<ActionResult<Tag>> Get(int id)
        {
            Tag tagFound = await coContext.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tagFound == null) return NotFound();
            
            return Ok(tagFound);
        }

        // GET: api/Tag/name=computer
        /// <summary>
        /// Returns single tag (of alphanumerical type) where Tag.name == name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("name={name}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagByName(string name)
        {
            List<Tag> tagsFound = await coContext.Tags
                    //* look at Nodecontroller line 80, same problem
                    .Where(t => ((AlphanumericalTag)t).Name.ToLower().StartsWith(name.ToLower()))
                    .ToListAsync();

            // List<Tag> tagsFound = coContext.Tags
            //         .AsEnumerable()
            //         .Where(t => t.GetTagName().ToLower().StartsWith(name.ToLower())) //* look at Nodecontroller line 80
            //         .ToList();
            
            if (tagsFound == null) return NotFound();
            var result = tagsFound.Select(tag => new PublicTag(tag.Id, tag.GetTagName(), tag.TagsetId)).ToList(); //*this should work for all tag types
            //var result = tagsFound.Select(tag => new PublicTag(tag.Id, ((AlphanumericalTag) tag).Name, tag.TagsetId)).ToList();
            //var result = tagsFound.Select(tag => new PublicTag(tag.Id, ((AlphanumericalTag) tag).Name)).ToList();
            
            return Ok(result);
        }
    }
}
