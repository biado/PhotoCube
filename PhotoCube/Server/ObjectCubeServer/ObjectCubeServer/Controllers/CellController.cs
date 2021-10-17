using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.PublicClasses;
using ObjectCubeServer.Services;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CellController : ControllerBase
    {
        private readonly QueryGenerationService queryGenerationService = new();
        private readonly ObjectContext coContext;

        public CellController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SingleObjectCell>>> Get([FromQuery] CellRequest cq)
        {
            bool xDefined = cq.xAxis != null;
            bool yDefined = cq.yAxis != null;
            bool zDefined = cq.zAxis != null;
            bool filtersDefined = cq.filters != null;
            bool allDefined = cq.all != null;
            //Parsing:
            ParsedAxis axisX = xDefined ? cq.xAxis : new ParsedAxis { Type = "", Id = -1 };
            ParsedAxis axisY = yDefined ? cq.yAxis : new ParsedAxis { Type = "", Id = -1 };
            ParsedAxis axisZ = zDefined ? cq.zAxis : new ParsedAxis { Type = "", Id = -1 };
            List<ParsedFilter> filtersList =
                filtersDefined ? cq.filters : null;
            //Potential refactor: Parsed filter inheritance & make factory class to parse and instantiate filters without losing information

            if (allDefined)
            {
                List<PublicCubeObject> cubeobjects = 
                    await coContext.PublicCubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForObjects(filtersList)).ToListAsync();
                return Ok(cubeobjects);
            }

            //Creating Cells:
            List<PublicCell> result;

            //// If there are no axis or filters, it means it will call for the whole data set.
            //// We don't want to get all 190K cubeObjects in this case, but get only small number (1st page) and return fast.

            //// BÞJ: We can make a special case query for no axes / filters and get rid of this special case here
            ////      Currently, the count is missing, and this needs to be added for Aaron
            ////      The following query will yield the correct results fast, and is nearly the same as the current state query
            ////select X.idx, X.idy, X.idz,	O.file_uri, X.cnt-- , A1.name, A2.name
            ////from(
            ////  select 1 as idx, 1 as idy, 1 as idz, max(R1.id) as object_id, count(distinct R1.id) as cnt
            ////  from cubeobjects R1
            ////) X
            ////join cubeobjects O on X.object_id = O.id;
            //if (!xDefined && !yDefined && !zDefined && !filtersDefined)
            //{
            //    cells = new List<Cell>()
            //    {
            //        new Cell()
            //        {
            //            x = 1,
            //            y = 1,
            //            z = 1,
            //            CubeObjects = coContext.CubeObjects.Take(6).ToList()
            //        }
            //    };
            //    // Convert cells to publicCells
            //    result = cells.Select(c => c.GetPublicCell()).ToList();

            //    //Return OK with json result:
            //    return Ok(JsonConvert.SerializeObject(result,
            //        new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
            //}

            // BÞJ: I do not believe this is needed anymore... 
            //queryGenerationService.reset();

            //Filtering:
            //if (filtersDefined && filtersList.Count > 0)
            //{
            //    //Merge day of week filters, and initialize Ids List field in each filter
            //    filtersList = mergeDayOfWeekFilters(filtersList);
            //    findIdsFromDBAndInitializeIds(filtersList);

            //    //Generate query string for the filters part
            //    //queryGenerationService.generateFilterQuery(filtersList);
            //}

            axisX.initializeIds();
            axisY.initializeIds();
            axisZ.initializeIds();

            // Need to run the query, extract the results, and convert the coordinates
            //coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCells(axisX.AxisType, axisX.Id, axisY.AxisType, axisY.Id, axisZ.AxisType, axisZ.Id)).ToList();
            List<SingleObjectCell> singlecells = await
                coContext.SingleObjectCells.FromSqlRaw(queryGenerationService.generateSQLQueryForCells(axisX.Type, axisX.Id, axisY.Type, axisY.Id, axisZ.Type, axisZ.Id, filtersList)).ToListAsync();
            result = singlecells.Select(c =>
                new PublicCell(axisX.Ids[c.x], axisY.Ids[c.y], axisZ.Ids[c.z], c.count, c.id, c.fileURI)).ToList();

            //If cells have no cubeObjects, remove them:
            //cells.RemoveAll(c => !c.CubeObjects.Any());

            //Return OK with json result:
            return Ok(result);
        }
        
    }
}
