using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ObjectCubeServer.Models.DataAccess;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.TagTypes;
using ObjectCubeServer.Models.PublicClasses;
using ObjectCubeServer.Services;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CellController : ControllerBase
    {
        public QueryGenerationService queryGenerationService = new QueryGenerationService();
        /* EXAMPLES:
         * GET: /api/cell?xAxis={jsonObject}
         * GET: /api/cell?yAxis={jsonObject}
         * GET: /api/cell?zAxis={jsonObject}
         * GET: /api/cell?xAxis={jsonObject}&yAxis={jsonObject}
         * GET: /api/cell?xAxis={jsonObject}&zAxis={jsonObject}
         * GET: /api/cell?yAxis={jsonObject}&zAxis={jsonObject}
         * GET: /api/cell?xAxis={jsonObject}&yAxis={jsonObject}&zAxis={jsonObject}
         * 
         * Where an axis showing a Hierarchy could be:
         *  {"AxisType":"Hierarchy","Id":0} (Id is NodeId)
         * Or an axis showing a Tagset could be: 
         *  {"AxisType":"Tagset","Id":1} (Id is TagsetId)
         *  
         * The same way, filters can also be added:
         * Hierarchy filter:
         *     &filters=[{"type":"hierarchy","Id":0,"name":"Norway"}] (Id is NodeId)
         * Tag filter:
         *     &filters=[{"type":"tag","Id":42,"name":"Oslo"}] (Id is TagId)
         * Possible types are:
         *      "tag", "tagset", "hierarchy", "date", "time", and "day of week".
         *
         * Currently, each cell contains maximum { 6 * number of cells } cubeObjects.
        */
        public IActionResult Get(string xAxis, string yAxis, string zAxis, string filters)
        {
            using (var coContext = new ObjectContext())
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
                    //Potential refactor: Parsed filter inheritance & make factory class to parse and instantiate filters without losing information

                //Creating Cells:
                //List<Cell> cells = new List<Cell>();
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
                List<SingleObjectCell> singlecells =
                    coContext.SingleObjectCells.FromSqlRaw(queryGenerationService.generateSQLQueryForCells(axisX.Type, axisX.Id, axisY.Type, axisY.Id, axisZ.Type, axisZ.Id, filtersList)).ToList();
                result = new List<PublicCell>();
                foreach (var c in singlecells)
                {
                    //result.Add(new PublicCell(c.x, c.y, c.z, c.count, c.id, c.fileURI));
                    result.Add(new PublicCell(axisX.Ids[c.x], axisY.Ids[c.y], axisZ.Ids[c.z], c.count, c.id, c.fileURI));
                }

                //If cells have no cubeObjects, remove them:
                //cells.RemoveAll(c => !c.CubeObjects.Any());

                //Return OK with json result:
                return Ok(JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
            }
        }

        //private void findIdsFromDBAndInitializeIds(List<ParsedFilter> filtersList)
        //{
        //    foreach (var filter in filtersList)
        //    {
        //        filter.initializeIds();
        //    }
        //}

        //private List<ParsedFilter> mergeDayOfWeekFilters(List<ParsedFilter> filtersList)
        //{
        //    List<ParsedFilter> dayOfWeekFilters = filtersList.Where(f => f.type.Equals("day of week")).ToList();
        //    if (dayOfWeekFilters.Count < 2)
        //    {
        //        // No need to merge day of week filters
        //        return filtersList;
        //    }
        //    else
        //    {
        //        List<int> dayOfWeekIds = new List<int>();
        //        foreach (var dayOfWeekFilter in dayOfWeekFilters)
        //        {
        //            dayOfWeekIds.Add(dayOfWeekFilter.Id);
        //        }

        //        ParsedFilter firstDowFilter = dayOfWeekFilters[0];
        //        firstDowFilter.Ids = dayOfWeekIds;
        //        List<ParsedFilter> mergedList = filtersList.Where(f => !f.type.Equals("day of week")).ToList();
        //        mergedList.Add(firstDowFilter);
        //        return mergedList;
        //    }
        //}
    }
}
