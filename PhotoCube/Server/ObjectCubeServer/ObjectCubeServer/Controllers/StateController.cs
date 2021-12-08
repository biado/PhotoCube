using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.PublicClasses;
using ObjectCubeServer.Services;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly QueryGenerationService queryGenerationService = new();
        private readonly ObjectContext coContext;

        public StateController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicCell>>> Get(string xAxis, string yAxis, string zAxis, string filters)
        {
            bool xDefined = xAxis != null;
            bool yDefined = yAxis != null;
            bool zDefined = zAxis != null;
            bool filtersDefined = filters != null;
            //Parsing:
            ParsedAxis axisX = xDefined ? JsonConvert.DeserializeObject<ParsedAxis>(xAxis) : new ParsedAxis { Type = "", Id = -1 };
            ParsedAxis axisY = yDefined ? JsonConvert.DeserializeObject<ParsedAxis>(yAxis) : new ParsedAxis { Type = "", Id = -1 };
            ParsedAxis axisZ = zDefined ? JsonConvert.DeserializeObject<ParsedAxis>(zAxis) : new ParsedAxis { Type = "", Id = -1 };
            List<ParsedFilter> filtersList =
                filtersDefined ? JsonConvert.DeserializeObject<List<ParsedFilter>>(filters) : null;

            //Creating Cells:
            axisX.initializeIds(coContext);
            axisY.initializeIds(coContext);
            axisZ.initializeIds(coContext);

            // Need to run the query, extract the results, and convert the coordinates
            List<SingleObjectCell> singlecells = await coContext.SingleObjectCells.FromSqlRaw(
                    queryGenerationService.generateSQLQueryForCells(axisX.Type, axisX.Id, axisY.Type, axisY.Id, axisZ.Type, axisZ.Id, filtersList)).
                ToListAsync();
            List<PublicCell> result = singlecells.Select(c =>
                new PublicCell(axisX.Ids[c.x], axisY.Ids[c.y], axisZ.Ids[c.z], c.count, c.id, c.fileURI)).ToList();

            //If cells have no cubeObjects, remove them:
            //cells.RemoveAll(c => !c.CubeObjects.Any());

            //Return OK with json result:
            return Ok(result);
        }
        
    }
}
