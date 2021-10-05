using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObjectCubeServer.Models.DataAccess;
using ObjectCubeServer.Models.DomainClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThumbnailController : ControllerBase
    {
        // GET: api/Thumbnail
        [HttpGet]
        public IActionResult Get()
        {
            List<string> allThumbnailURIs;
            using (var context = new ObjectContext())
            {
                allThumbnailURIs = context.CubeObjects.Select(co => co.ThumbnailURI).ToList();
            }
            if (allThumbnailURIs != null)
            {
                var data = new { thumbnailURIs = allThumbnailURIs };
                return Ok(JsonConvert.SerializeObject(data)); //Does not return file!
            }
            else return NotFound();
        }

        // GET: api/Thumbnail/5
        [HttpGet("{id:int}", Name = "GetThumbnail")]
        public IActionResult Get(int id)
        {
            string thumbnailURI;
            using (var context = new ObjectContext())
            {
                CubeObject cubeObject = context.CubeObjects.FirstOrDefault(co => co.Id == id);
                thumbnailURI = cubeObject.ThumbnailURI;
            }
            if (thumbnailURI == null)
            {
                return NotFound();
            } 
            return Ok(thumbnailURI);
        }
    }
}
