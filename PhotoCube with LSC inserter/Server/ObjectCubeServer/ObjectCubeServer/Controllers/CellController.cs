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
    public class CellController : ControllerBase
    {
        public static int pageSize = 10;
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
         *     &filters=[{"type":"hierarchy","tagId":0,"nodeId":116}]
         * Tag filter:
         *     &filters=[{"type":"tag","tagId":42,"nodeId":0}]
         *
         * The call may specify which page of the result it wants.
         * For example,
         * GET: /api/cell?xAxis={jsonObject}&currentPage=3
         * If not specified, the default is currentPage=1.
         *
         * A page of result contains maximum { PageSize (currently set to 10) * number of cells } cubeObjects.
        */
        public IActionResult Get(string xAxis, string yAxis, string zAxis, string filters, string currentPage)
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
                int currentPageNumber = parseCurrentPage(currentPage);
                var skip = (currentPageNumber - 1) * pageSize;

                //Creating Cells:
                List<Cell> cells = new List<Cell>();

                PublicPage result = instantiatePageResult(currentPageNumber);
                // If there are no axis or filters, it means it will call for the whole data set.
                // We don't want to get all 190K cubeObjects in this case, but get only small number (1st page) and return fast.
                if (!xDefined && !yDefined && !zDefined && !filtersDefined)
                {
                    result.TotalFileCount = coContext.CubeObjects.Count();
                    var totalPages = (double) result.TotalFileCount / pageSize;
                    result.PageCount = (int) Math.Ceiling(totalPages);

                    cells = new List<Cell>()
                    {
                        new Cell()
                        {
                            x = 1,
                            y = 1,
                            z = 1,
                            CubeObjects = coContext.CubeObjects.Skip(skip).Take(pageSize).ToList()
                        }
                    };
                    // Convert cells to publicCells
                    result.Results = cells.Select(c => c.GetPublicCell()).ToList();

                    //Return OK with json result:
                    return Ok(JsonConvert.SerializeObject(result,
                        new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
                }

                IEnumerable<CubeObject> filteredCubeObjects = coContext.CubeObjects.Include(co => co.ObjectTagRelations);
                //Filtering:
                if (filtersDefined && filtersList.Count > 0)
                {
                    //Divide filters:
                    List<ParsedFilter> dayOfWeekFilers = filtersList.Where(f => f.type.Equals("day of week")).ToList();
                    List<ParsedFilter> timeFilters = filtersList.Where(f => f.type.Equals("time")).ToList();
                    List<ParsedFilter> tagFilters = filtersList.Where(f => f.type.Equals("tag") || f.type.Equals("date")).ToList();
                    List<ParsedFilter> hierarchyFilters = filtersList.Where(f => f.type.Equals("hierarchy")).ToList();
                    List<ParsedFilter> tagsetFilters = filtersList.Where(f => f.type.Equals("tagset")).ToList();

                    //Apply filters:
                    // As default the cubeObject that has all these filters (AND) will remain in the result.
                    // Exception: Day of week filters have OR logic. For example, Monday filter and Sunday filter gives cubeObjects with either one of those.
                    if (dayOfWeekFilers.Count > 0)
                    {
                        filteredCubeObjects =
                            filterCubeObjectsWithDayOfWeekFilters(filteredCubeObjects, dayOfWeekFilers); // OR logic, if there are multiple "day of week" filters
                    }
                    if (timeFilters.Count > 0)
                    {
                        filteredCubeObjects = filterCubeObjectsWithTimeFilters(filteredCubeObjects, timeFilters); // range filter
                    } 
                    if (tagFilters.Count > 0)
                    {
                        filteredCubeObjects = filterCubeObjectsWithTagFilters(filteredCubeObjects, tagFilters);
                    }

                    if (hierarchyFilters.Count > 0)
                    {
                        filteredCubeObjects =
                            filterCubeObjectsWithHierarchyFilters(filteredCubeObjects, hierarchyFilters);
                    }

                    if (tagsetFilters.Count > 0)
                    {
                        filteredCubeObjects = filterCubeObjectsWithTagsetFilters(filteredCubeObjects, tagsetFilters);
                    }
                }

                //Extracting cubeObjects:
                List<List<CubeObject>> xAxisCubeObjects = getAllCubeObjectsFromAxis(xDefined, axisX, filteredCubeObjects);
                List<List<CubeObject>> yAxisCubeObjects = getAllCubeObjectsFromAxis(yDefined, axisY, filteredCubeObjects);
                List<List<CubeObject>> zAxisCubeObjects = getAllCubeObjectsFromAxis(zDefined, axisZ, filteredCubeObjects);

                if (xDefined && yDefined && zDefined) //XYZ
                {
                    cells =
                        xAxisCubeObjects.SelectMany((colist1, index1) =>
                            yAxisCubeObjects.SelectMany((colist2, index2) =>
                                zAxisCubeObjects.Select((colist3, index3) => new Cell()
                                {
                                    x = index1 + 1,
                                    y = index2 + 1,
                                    z = index3 + 1,
                                    CubeObjects = colist1.Intersect(colist2).Intersect(colist3).ToList()
                                }))).ToList();
                }
                else if (xDefined && yDefined) //XY
                {
                    cells =
                        xAxisCubeObjects.SelectMany((colist1, index1) =>
                            yAxisCubeObjects.Select((colist2, index2) =>
                                new Cell()
                                {
                                    x = index1 + 1,
                                    y = index2 + 1,
                                    z = 0,
                                    CubeObjects = colist1.Intersect(colist2).ToList() //Where co is in colist2 as well
                                })).ToList();
                }
                else if (xDefined && zDefined) //XZ
                {
                    cells =
                        xAxisCubeObjects.SelectMany((colist1, index1) =>
                            zAxisCubeObjects.Select((colist2, index2) =>
                                new Cell()
                                {
                                    x = index1 + 1,
                                    y = 0,
                                    z = index2 + 1,
                                    CubeObjects = colist1.Intersect(colist2).ToList() //Where co is in colist2 as well
                                })).ToList();
                }
                else if (yDefined && zDefined) //YZ
                {
                    cells =
                        yAxisCubeObjects.SelectMany((colist1, index1) =>
                            zAxisCubeObjects.Select((colist2, index2) =>
                                new Cell()
                                {
                                    x = 0,
                                    y = index1 + 1,
                                    z = index2 + 1,
                                    CubeObjects = colist1.Intersect(colist2).ToList()
                                })).ToList();
                }
                else if (xDefined) //X
                {
                    cells =
                        xAxisCubeObjects.Select((colist1, index1) =>
                            new Cell()
                            {
                                x = index1 + 1,
                                y = 1,
                                z = 0,
                                CubeObjects = colist1.ToList()
                            }).ToList();
                }
                else if (yDefined) //Y
                {
                    cells =
                        yAxisCubeObjects.Select((colist1, index1) =>
                            new Cell()
                            {
                                x = 1,
                                y = index1 + 1,
                                z = 0,
                                CubeObjects = colist1.ToList()
                            }).ToList();
                }
                else if (zDefined) //Z
                {
                    cells =
                        zAxisCubeObjects.Select((colist1, index1) =>
                            new Cell()
                            {
                                x = 0,
                                y = 1,
                                z = index1 + 1,
                                CubeObjects = colist1.ToList()
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
                            CubeObjects = filteredCubeObjects.ToList()
                        }
                    };
                }

                //If cells have no cubeObjects, remove them:
                cells.RemoveAll(c => !c.CubeObjects.Any());

                //Count unique cubeObjects in all the cells and update the information in the Page object
                result = updateTotalFileCountAndPageCount(result, cells);

                // Convert cells to publicCells
                result = GetPublicCellsInThisPage(result, cells, currentPageNumber);

                //Return OK with json result:
                return Ok(JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));
            }
        }

        #region HelperMethods:

        private IEnumerable<CubeObject> filterCubeObjectsWithDayOfWeekFilters(IEnumerable<CubeObject> cubeObjects, List<ParsedFilter> dayOfWeekFilers)
        { 
            return cubeObjects
                .Where(co =>
                    dayOfWeekFilers.Exists(f => co.ObjectTagRelations.Exists(otr => otr.TagId == f.Id))) //Must be tagged with at least one tag (OR search!)
                .ToList();
        }

        private IEnumerable<CubeObject> filterCubeObjectsWithTimeFilters(IEnumerable<CubeObject> cubeObjects, List<ParsedFilter> timeFilters)
        {
            //Getting tags per time filter:
            List<List<Tag>>
                tagsPerTimeFilter = timeFilters.Select(tf => extractTagsFromTimeFilter(tf)).ToList(); // Map into list of tags

            return cubeObjects.Where(co => tagsPerTimeFilter.TrueForAll( //CubeObject must be tagged with tag in each of the tag lists
                lstOfTags => co.ObjectTagRelations.Exists((  //For each tag list, there must exist a cube object where:
                    otr => lstOfTags.Exists(tag => tag.Id == otr.TagId))))).ToList();  //the cube object is tagged with one tag id from the taglist.
        }

        private List<Tag> extractTagsFromTimeFilter(ParsedFilter timeFilter)
        {
            var times = timeFilter.name.Split("-");
            TimeSpan start = parseToTimeSpan(times[0]);
            TimeSpan end = parseToTimeSpan(times[1]);

            using (var context = new ObjectContext())
            {
                var Tagset = context.Tagsets
                    .Include(ts => ts.Tags)
                    .FirstOrDefault(ts => ts.Name == "Time");
                if (start <= end)
                {
                    return Tagset.Tags.Where(t =>
                        ((TimeTag) t).Name >= start &&
                        ((TimeTag) t).Name <= end).ToList();
                }
                else
                {
                    // Case: Going over midnight. For example, 20:00-02:00
                    return Tagset.Tags.Where(t =>
                        (((TimeTag) t).Name >= start &&
                        ((TimeTag) t).Name < new TimeSpan(24, 0, 0))
                        || (((TimeTag) t).Name >= new TimeSpan(0, 0, 0) &&
                        ((TimeTag) t).Name <= end)).ToList();
                }
            }
        }

        private TimeSpan parseToTimeSpan(string timeString)
        {
            string[] hourMinute = timeString.Split(":");
            int hour = int.Parse(hourMinute[0]);
            int minute = int.Parse(hourMinute[1]);
            TimeSpan time = new TimeSpan(hour, minute, 0);
            return time;
        }

        private PublicPage updateTotalFileCountAndPageCount(PublicPage result, List<Cell> cells)
        {
            IEnumerable<CubeObject> union = new List<CubeObject>();
            foreach (Cell cell in cells)
            {
                union = union.Union(cell.CubeObjects);
            }
            result.TotalFileCount = union.Count();
            var totalpage = (double)result.TotalFileCount / pageSize;
            result.PageCount = (int)Math.Ceiling(totalpage);
            return result;
        }

        private PublicPage instantiatePageResult(int currentPage)
        {
            var result = new PublicPage();
            result.CurrentPage = currentPage;
            result.PageSize = pageSize;

            return result;
        }

        private int parseCurrentPage(string currentPage)
        {
            if (Int32.TryParse(currentPage, out int currentPageNumber)) // parsed successfully
            {
                currentPageNumber = currentPageNumber < 1 ? 1 : currentPageNumber;
            }
            else
            {
                currentPageNumber = 1;
            }

            return currentPageNumber;
        }

        private PublicPage GetPublicCellsInThisPage(PublicPage result, List<Cell> cells, int currentPage)
        {
            var skip = (currentPage - 1) * pageSize;
            result.Results = cells.Select(c => c.GetPublicCell(skip, pageSize)).ToList();

            return result;
        }

        /// <summary>
        /// Helper method that fetches all CubeObjects
        /// </summary>
        /// <returns></returns>
        private List<CubeObject> getAllCubeObjects()
        {
            List<CubeObject> allCubeObjects;
            using (var context = new ObjectContext())
            {
                allCubeObjects = context.CubeObjects
                    .Include(co => co.ObjectTagRelations)
                    .ToList();
            }
            return allCubeObjects;
        }


        private List<CubeObject> filterCubeObjectsWithTagsetFilters(IEnumerable<CubeObject> cubeObjects, List<ParsedFilter> tagsetFilters)
        {
            //Getting tags per tagset filter:
            List<List<Tag>>
                tagsPerTagsetFilter = tagsetFilters.Select(tsf => extractTagsFromTagsetFilter(tsf)).ToList(); // Map into list of tags

            return cubeObjects.Where(co => tagsPerTagsetFilter.TrueForAll( //CubeObject must be tagged with tag in each of the tag lists
                lstOfTags => co.ObjectTagRelations.Exists((  //For each tag list, there must exist a cube object where:
                    otr => lstOfTags.Exists(tag => tag.Id == otr.TagId))))).ToList();  //the cube object is tagged with one tag id from the taglist.
        }

        private List<Tag> extractTagsFromTagsetFilter(ParsedFilter tsf)
        {
            using var context = new ObjectContext();
            var Tagset = context.Tagsets
                    .Include(ts => ts.Tags)
                    .FirstOrDefault(ts => ts.Id == tsf.Id);
            return Tagset.Tags;
        }

        /// <summary>
        /// Helper method that given a list of cubeobjects and a list of filters, returns a new list of
        /// cubeobjects, where cubeobjects that doesn't pass through the filters are removed.
        /// </summary>
        /// <param name="cubeObjects"></param>
        /// <param name="tagFilters"></param>
        /// <returns></returns>
        private List<CubeObject> filterCubeObjectsWithTagFilters(IEnumerable<CubeObject> cubeObjects, List<ParsedFilter> tagFilters)
        {
            return cubeObjects
                .Where(co =>
                    tagFilters.TrueForAll(f => co.ObjectTagRelations.Exists(otr => otr.TagId == f.Id))) //Must be tagged with each tag
                .ToList();
        }

        /// <summary>
        /// Filters input list of cubeobjects with given hierarchy filters.
        /// If a cube object is tagged with a tag in a hierarchy, then it passes through the filter.
        /// </summary>
        /// <param name="cubeObjects"></param>
        /// <param name="hierarchyFilters"></param>
        /// <returns></returns>
        private List<CubeObject> filterCubeObjectsWithHierarchyFilters(IEnumerable<CubeObject> cubeObjects, List<ParsedFilter> hierarchyFilters)
        {
            //Getting all tags per hierarchy filter:
            List<List<Tag>> tagsPerHierarchyFilter = hierarchyFilters
                .Select(hf => extractTagsFromHierarchyFilter(hf)) //Map into list of tags
                .ToList();

            //If cubeObject has tag in hierarchy, let it pass through the filter:
            return cubeObjects
                .Where(co => tagsPerHierarchyFilter.TrueForAll( //CubeObject must be tagged with tag in each of the tag lists (flattened hierarchies)
                        lstOfTags => co.ObjectTagRelations.Exists( //For each tag list, there must exist a cube object where:
                            otr => lstOfTags.Exists(tag => tag.Id == otr.TagId) //the cube object is tagged with one tag id from the taglist.
                        )
                    )
                ).ToList();
        }

        private List<Tag> extractTagsFromHierarchyFilter(ParsedFilter pf)
        {
            //Get node and subnodes with tags:
            Node node = fetchWholeHierarchyFromRootNode(pf.Id);
            //Extract tags:
            List<Tag> tagsInNode = extractTagsFromHieararchy(node);
            return tagsInNode;
        }

        /// <summary>
        /// Given a boolean defined, a ParsedAxis and filteredCubeObjects, returns a List of List of CubeObjects.
        /// The indexes in the outer list represents each tag on an axis.
        /// The indexes in the inner list represents the cube objects tagged with the tag.
        /// </summary>
        /// <param name="defined"></param>
        /// <param name="parsedAxis"></param>
        /// <param name="filteredCubeObjects"></param>
        /// <returns></returns>
        private List<List<CubeObject>> getAllCubeObjectsFromAxis(bool defined, ParsedAxis parsedAxis, IEnumerable<CubeObject> filteredCubeObjects)
        {
            if (defined)
            {
                if (parsedAxis.AxisType.Equals("Tagset"))
                {
                    return getAllCubeObjectsFrom_Tagset_Axis(parsedAxis, filteredCubeObjects);
                }
                else if (parsedAxis.AxisType.Equals("Hierarchy"))
                {
                    return getAllCubeObjectsFrom_Hierarchy_Axis(parsedAxis, filteredCubeObjects);
                }
                else if (parsedAxis.AxisType.Equals("HierarchyLeaf")) //A HierarchyLeaf is a Node with no children.
                {
                    return getAllCubeObjectsFrom_HierarchyLeaf_Axis(parsedAxis, filteredCubeObjects);
                }
                else
                {
                    throw new Exception("AxisType: " + parsedAxis.AxisType + " was not recognized!");
                }
            }
            else return null;
        }

        /// <summary>
        /// Returns list of cubeObjects per tag. Called with ParsedAxis of type "Tagset".
        /// </summary>
        /// <param name="parsedAxis"></param>
        /// <param name="filteredCubeObjects"></param>
        /// <returns></returns>
        private List<List<CubeObject>> getAllCubeObjectsFrom_Tagset_Axis(ParsedAxis parsedAxis, IEnumerable<CubeObject> filteredCubeObjects)
        {
            //Getting tags from database:
            List<Tag> tags;
            using (var context = new ObjectContext())
            {
                var Tagset = context.Tagsets
                    .Include(ts => ts.Tags)
                    .FirstOrDefault(ts => ts.Id == parsedAxis.Id);

                Tagset.Tags.Sort();
                tags = Tagset.Tags.ToList();
            }
            return tags
                .Select(t => getAllCubeObjectsTaggedWith(t.Id, filteredCubeObjects))
                .ToList();
        }

        /// <summary>
        /// Returns list of cubeObjects per tag. Called with ParsedAxis of type "Hierarchy".
        /// </summary>
        /// <param name="parsedAxis"></param>
        /// <param name="filteredCubeObjects"></param>
        /// <returns></returns>
        private List<List<CubeObject>> getAllCubeObjectsFrom_Hierarchy_Axis(ParsedAxis parsedAxis, IEnumerable<CubeObject> filteredCubeObjects)
        {
            Node rootNode = fetchWholeHierarchyFromRootNode(parsedAxis.Id);
            List<Node> hierarchyNodes = rootNode.Children;
            return hierarchyNodes
                .Select(n => getAllCubeObjectsTaggedWith(extractTagsFromHieararchy(n), filteredCubeObjects) //Map hierarchy nodes to list of cube objects
                .GroupBy(co => co.Id).Select(grouping => grouping.First()).ToList()) //Getting unique cubeobjects
                .ToList();
        }

        /// <summary>
        /// Returns list of cubeObjects per tag. Called with ParsedAxis of type "HierarchyLeaf".
        /// </summary>
        /// <param name="parsedAxis"></param>
        /// <param name="filteredCubeObjects"></param>
        /// <returns></returns>
        private List<List<CubeObject>> getAllCubeObjectsFrom_HierarchyLeaf_Axis(ParsedAxis parsedAxis, IEnumerable<CubeObject> filteredCubeObjects)
        {
            Node currentNode = fetchWholeHierarchyFromRootNode(parsedAxis.Id);
            List<CubeObject> cubeObjectsTaggedWithTagFromNode = getAllCubeObjectsTaggedWith(currentNode.TagId, filteredCubeObjects);
            return new List<List<CubeObject>>() { cubeObjectsTaggedWithTagFromNode };
        }

        /// <summary>
        /// Fetches Node with Tag, Children and Children's Tags from a given nodeId.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private Node fetchWholeHierarchyFromRootNode(int nodeId)
        {
            Node currentNode;
            using (var context = new ObjectContext())
            {
                currentNode = context.Nodes
                    .Include(n => n.Tag)
                    .Include(n => n.Children)
                        .ThenInclude(cn => cn.Tag)
                    .FirstOrDefault(n => n.Id == nodeId);
            }
            currentNode.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
            List<Node> newChildNodes = new List<Node>();
            currentNode.Children.ForEach(cn => newChildNodes.Add(fetchWholeHierarchyFromRootNode(cn.Id)));
            currentNode.Children = newChildNodes;
            return currentNode;
        }

        /// <summary>
        /// Filters the given CubeObjects tagged with tagId.
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="filteredCubeObjects"></param>
        /// <returns></returns>
        private List<CubeObject> getAllCubeObjectsTaggedWith(int tagId, IEnumerable<CubeObject> filteredCubeObjects)
        {
            List<CubeObject> cubeObjects;
            cubeObjects = filteredCubeObjects
                    .Where(co => co.ObjectTagRelations.Any(otr => otr.TagId == tagId) ) //Is tagged with tagId at least once
                    .ToList();
                return cubeObjects;
        }

        /// <summary>
        /// Filters the given CubeObjects tagged with either of the tags in given list of tags.
        /// Warning: Remember to filter returned CubeObjects for duplicates!
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="filteredCubeObjects"></param>
        /// <returns></returns>
        private List<CubeObject> getAllCubeObjectsTaggedWith(List<Tag> tags, IEnumerable<CubeObject> filteredCubeObjects)
        {
            List<CubeObject> cubeObjects = new List<CubeObject>();
            foreach (Tag t in tags)
            {
                cubeObjects.AddRange(getAllCubeObjectsTaggedWith(t.Id, filteredCubeObjects));
            }
            return cubeObjects.Distinct().ToList();
        }
        
        /// <summary>
        /// Given a Node, returns all the tags in the hierarchy.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        private List<Tag> extractTagsFromHieararchy(Node hierarchy)
        {
            List<Tag> tags = new List<Tag>();
            tags.Add(hierarchy.Tag);
            var tagsFromSubHierarchies = hierarchy.Children
                .SelectMany(n => extractTagsFromHieararchy(n)) //Same as flatMap
                .ToList();
            tags.AddRange(tagsFromSubHierarchies);
            return tags;
        }
        #endregion
    }
}
