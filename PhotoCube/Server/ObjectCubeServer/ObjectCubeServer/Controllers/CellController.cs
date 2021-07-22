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
                ParsedAxis axisX = xDefined ? JsonConvert.DeserializeObject<ParsedAxis>(xAxis) : null;
                ParsedAxis axisY = yDefined ? JsonConvert.DeserializeObject<ParsedAxis>(yAxis) : null;
                ParsedAxis axisZ = zDefined ? JsonConvert.DeserializeObject<ParsedAxis>(zAxis) : null;
                List<ParsedFilter> filtersList =
                    filtersDefined ? JsonConvert.DeserializeObject<List<ParsedFilter>>(filters) : null;
                    //Potential refactor: Parsed filter inheritance & make factory class to parse and instantiate filters without losing information

                //Creating Cells:
                List<Cell> cells = new List<Cell>();
                List<PublicCell> result;

                // If there are no axis or filters, it means it will call for the whole data set.
                // We don't want to get all 190K cubeObjects in this case, but get only small number (1st page) and return fast.
                if (!xDefined && !yDefined && !zDefined && !filtersDefined)
                {
                    cells = new List<Cell>()
                    {
                        new Cell()
                        {
                            x = 1,
                            y = 1,
                            z = 1,
                            CubeObjects = coContext.CubeObjects.Take(6).ToList()
                        }
                    };
                    // Convert cells to publicCells
                    result = cells.Select(c => c.GetPublicCell()).ToList();

                    //Return OK with json result:
                    return Ok(JsonConvert.SerializeObject(result,
                        new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
                }

                queryGenerationService.reset();

                //Filtering:
                if (filtersDefined && filtersList.Count > 0)
                {
                    //Merge day of week filters, and initialize Ids List field in each filter
                    filtersList = mergeDayOfWeekFilters(filtersList);
                    findIdsFromDBAndInitializeIds(filtersList);

                    //Generate query string for the filters part
                    queryGenerationService.generateFilterQuery(filtersList);
                }

                //Build cells by generating and executing SQL query for each cell (max 6 images per cell)
                if (xDefined && yDefined && zDefined) //XYZ
                {
                    axisX.initializeIds();
                    axisY.initializeIds();
                    axisZ.initializeIds();
                    cells =
                        axisX.Ids.SelectMany((v1, index1) =>
                        axisY.Ids.SelectMany((v2, index2) =>
                            axisZ.Ids.Select((v3, index3) => new Cell()
                                {
                                    x = index1 + 1,
                                    y = index2 + 1,
                                    z = index3 + 1,
                                    CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell(axisX.AxisType, v1, axisY.AxisType, v2, axisZ.AxisType, v3)).ToList()
                                }))).ToList();
                }
                else if (xDefined && yDefined) //XY
                {
                    axisX.initializeIds();
                    axisY.initializeIds();
                    cells =
                        axisX.Ids.SelectMany((v1, index1) =>
                            axisY.Ids.Select((v2, index2) =>
                                new Cell()
                                {
                                    x = index1 + 1,
                                    y = index2 + 1,
                                    z = 0,
                                    CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell(axisX.AxisType, v1, axisY.AxisType, v2, "", -1)).ToList() //Where co is in colist2 as well
                                })).ToList();
                }
                else if (xDefined && zDefined) //XZ
                {
                    axisX.initializeIds();
                    axisZ.initializeIds();
                    cells =
                        axisX.Ids.SelectMany((v1, index1) =>
                            axisZ.Ids.Select((v2, index2) =>
                                new Cell()
                                {
                                    x = index1 + 1,
                                    y = 0,
                                    z = index2 + 1,
                                    CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell(axisX.AxisType, v1, "", -1, axisZ.AxisType, v2)).ToList() //Where co is in colist2 as well
                                })).ToList();
                }
                else if (yDefined && zDefined) //YZ
                {
                    axisY.initializeIds();
                    axisZ.initializeIds();
                    cells =
                        axisY.Ids.SelectMany((v1, index1) =>
                            axisZ.Ids.Select((v2, index2) =>
                                new Cell()
                                {
                                    x = 0,
                                    y = index1 + 1,
                                    z = index2 + 1,
                                    CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell("", -1, axisY.AxisType, v1, axisZ.AxisType, v2)).ToList()
                                })).ToList();
                }
                else if (xDefined) //X
                {
                    axisX.initializeIds();
                    cells =
                        axisX.Ids.Select((v1, index1) =>
                            new Cell()
                            {
                                x = index1 + 1,
                                y = 1,
                                z = 0,
                                CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell(axisX.AxisType, v1, "", -1, "", -1)).ToList()
                            }).ToList();
                }
                else if (yDefined) //Y
                {
                    axisY.initializeIds();
                    cells =
                        axisY.Ids.Select((v1, index1) =>
                            new Cell()
                            {
                                x = 1,
                                y = index1 + 1,
                                z = 0,
                                CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell("", -1, axisY.AxisType, v1, "", -1)).ToList()
                            }).ToList();
                }
                else if (zDefined) //Z
                {
                    axisZ.initializeIds();
                    cells =
                        axisZ.Ids.Select((v1, index1) =>
                            new Cell()
                            {
                                x = 0,
                                y = 1,
                                z = index1 + 1,
                                CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell("", -1, "", -1, axisZ.AxisType, v1)).ToList()
                            }).ToList();
                }
                else if (!xDefined && !yDefined && !zDefined) //If X Y and Z are not defined, show all:
                {
                    cells = new List<Cell>()
                    {
                        new Cell()
                        {
                            x = 1,
                            y = 1,
                            z = 1,
                            CubeObjects = coContext.CubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForCell("", -1, "", -1, "", -1)).ToList()
                        }
                    };
                }

                //If cells have no cubeObjects, remove them:
                cells.RemoveAll(c => !c.CubeObjects.Any());

                // Convert cells to publicCells
                result = cells.Select(c => c.GetPublicCell()).ToList();

                //Return OK with json result:
                return Ok(JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
            }
        }

        private void findIdsFromDBAndInitializeIds(List<ParsedFilter> filtersList)
        {
            foreach (var filter in filtersList)
            {
                filter.initializeIds();
            }
        }

        private List<ParsedFilter> mergeDayOfWeekFilters(List<ParsedFilter> filtersList)
        {
            List<ParsedFilter> dayOfWeekFilters = filtersList.Where(f => f.type.Equals("day of week")).ToList();
            if (dayOfWeekFilters.Count < 2)
            {
                // No need to merge day of week filters
                return filtersList;
            }
            else
            {
                List<int> dayOfWeekIds = new List<int>();
                foreach (var dayOfWeekFilter in dayOfWeekFilters)
                {
                    dayOfWeekIds.Add(dayOfWeekFilter.Id);
                }

                ParsedFilter firstDowFilter = dayOfWeekFilters[0];
                firstDowFilter.Ids = dayOfWeekIds;
                List<ParsedFilter> mergedList = filtersList.Where(f => !f.type.Equals("day of week")).ToList();
                mergedList.Add(firstDowFilter);
                return mergedList;
            }
        }
    }
}
