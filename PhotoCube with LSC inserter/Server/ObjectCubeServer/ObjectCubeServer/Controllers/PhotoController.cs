using System;
using System.Collections.Generic;
using System.IO;
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
    public class PhotoController : ControllerBase
    {
        // GET: api/Photo/5
        [HttpGet("{id}", Name = "GetPhoto")]
        public IActionResult Get(int id)
        {
            string fileURI;
            using (var context = new ObjectContext())
            {
                CubeObject cubeObject = context.CubeObjects.Where(co => co.Id == id).FirstOrDefault();
                fileURI = cubeObject.FileURI;
                if(fileURI == null)
                {
                    return NotFound();
                }
            }
            return Ok(fileURI);
        }
    }
}
