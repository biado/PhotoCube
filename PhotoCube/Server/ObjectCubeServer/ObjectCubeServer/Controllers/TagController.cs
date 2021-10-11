using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TagController : ControllerBase
    {
        // GET: api/Tag
        // GET: api/tag?cubeObjectId=1
        /// <summary>
        /// Either returns all tags in the database: api/tag.
        /// Or returns all tags that cubeObject with cubeObjectId is tagged with.
        /// </summary>
        /// <param name="cubeObjectId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(int? cubeObjectId)
        {
            if (cubeObjectId == null)
            {
                List<Tag> allTags;
                await using (var context = new ObjectContext())
                {
                    allTags = context.Tags.ToList();
                }
                return Ok(allTags);
            }
            else
            {
                List<string> tagsFound;
                await using (var context = new ObjectContext())
                {
                    tagsFound = context.ObjectTagRelations
                        .Where(otr => otr.ObjectId == cubeObjectId)
                        .Select(otr => otr.Tag.GetTagName())
                        .ToList();
                }
                if (tagsFound != null)
                {
                    return Ok(tagsFound);
                }
                else return NotFound();
            }
        }

        // GET: api/Tag/5
        /// <summary>
        /// Returns single tag where Tag.Id == id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetTag")]
        public async Task<IActionResult> Get(int id)
        {
            Tag tagFound;
            await using (var context = new ObjectContext())
            {
                tagFound = context.Tags.FirstOrDefault(t => t.Id == id);
            }
            if (tagFound != null)
            {
                return Ok(tagFound);
            }
            else return NotFound();   
        }

        // GET: api/Tag/name=computer
        /// <summary>
        /// Returns single tag (of alphanumerical type) where Tag.name == name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("name={name}")]
        public async Task<IActionResult> GetTagByName(string name)
        {
            List<Tag> tagsFound;
            await using (var context = new ObjectContext())
            {
                tagsFound = context.Tags
                    .Where(t => ((AlphanumericalTag)t).Name.ToLower().StartsWith(name.ToLower()))
                    .ToList();
            }

            if (tagsFound != null)
            {
                var result = tagsFound.Select(tag => new PublicTag(tag.Id, ((AlphanumericalTag) tag).Name)).ToList();
                return Ok(result);
            }
            return NotFound();
        }
    }
}
