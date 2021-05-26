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
using ObjectCubeServer.Models.DomainClasses.TagTypes;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult Get(int? cubeObjectId)
        {
            if (cubeObjectId == null)
            {
                List<Tag> allTags;
                using (var context = new ObjectContext())
                {
                    allTags = context.Tags.ToList();
                }
                return Ok(JsonConvert.SerializeObject(allTags));
            }
            else
            {
                List<string> tagsFound;
                using (var context = new ObjectContext())
                {
                    tagsFound = context.ObjectTagRelations
                        .Where(otr => otr.ObjectId == cubeObjectId)
                        .Select(otr => otr.Tag.GetTagName())
                        .ToList();
                }
                if (tagsFound != null)
                {
                    return Ok(JsonConvert.SerializeObject(tagsFound));
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
        public IActionResult Get(int id)
        {
            Tag tagFound;
            using (var context = new ObjectContext())
            {
                tagFound = context.Tags.Where(t => t.Id == id).FirstOrDefault();
            }
            if (tagFound != null)
            {
                return Ok(JsonConvert.SerializeObject(tagFound));
            }
            else return NotFound();   
        }

        // GET: api/Tag/name=wood
        // Note: currently it is only for string tags
        /// <summary>
        /// Returns single tag (of alphanumerical type) where Tag.name == name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("name={name}")]
        public IActionResult GetTagByName(string name)
        {
            List<Tag> tagsFound;
            using (var context = new ObjectContext())
            {
                //tagsFound = context.Tags.AsEnumerable()
                //    .Where(t => t.GetTagName().ToLower().StartsWith(name.ToLower()))
                //    .ToList();
                tagsFound = context.Tags
                    .Where(t => ((AlphanumericalTag)t).Name.ToLower().StartsWith(name.ToLower()))
                    .ToList();
            }

            if (tagsFound != null)
            {
                var result = new List<PublicTag>();
                foreach (Tag tag in tagsFound)
                {
                    var publicTag = new PublicTag(tag.Id, ((AlphanumericalTag)tag).Name);
                    result.Add(publicTag);
                }
                return Ok(JsonConvert.SerializeObject(result));
            }
            return NotFound();
        }

        // GET: api/Tag/tagsetName=Year
        // Note: This currently only works with numerical tags.
        /// <summary>
        /// Returns all tags (of numerical type) in a tagset as a list, where Tagset.name == tagsetName.
        /// </summary>
        /// <param tagsetName="tagsetName"></param>
        /// <returns></returns>
        [HttpGet("tagsetName={tagsetName}")]
        public IActionResult GetAllTagsInNumericalTagsetByTagsetName(string tagsetName)
        {
            List<Tag> tagsFound;
            using (var context = new ObjectContext())
            {
                var Tagset = context.Tagsets
                    .Include(ts => ts.Tags)
                    .FirstOrDefault(ts => ts.Name.ToLower() == tagsetName.ToLower());
                tagsFound = Tagset.Tags.OrderBy(t => ((NumericalTag)t).Name).ToList();

            }

            if (tagsFound != null)
            {
                var result = new List<PublicTag>();
                foreach (Tag tag in tagsFound)
                {
                    var publicTag = new PublicTag(tag.Id, tag.GetTagName());
                    result.Add(publicTag);
                }
                return Ok(JsonConvert.SerializeObject(result));
            }
            return NotFound();
        }
    }
}
