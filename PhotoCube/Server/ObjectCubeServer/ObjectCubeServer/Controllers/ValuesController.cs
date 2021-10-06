using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ObjectCubeServer.Controllers
{
    /// <summary>
    /// Not in use, but exists for illustrative purposes. 
    /// GET, POST, PUT and DELETE are available.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id:int}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id:int}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}
