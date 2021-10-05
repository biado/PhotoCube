using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class HierarchyController : ControllerBase
    {
        // GET: api/Hierarchy
        [HttpGet]
        public IActionResult Get()
        {
            List<Hierarchy> allHierarchies;
            using (var context = new ObjectContext())
            {
                allHierarchies = context.Hierarchies
                    .Include(h => h.Nodes)
                        .ThenInclude(n => n.Tag)
                    .ToList();
            }
            //Add rootnode and recursively add subnodes and their tags:
            allHierarchies.ForEach(h => h.Nodes = new List<Node>()
            {
                RecursiveAddChildrenAndTags(h.Nodes.FirstOrDefault(n => n.Id == h.RootNodeId))
            });

            return Ok(allHierarchies);

        }

        // GET: api/Hierarchy/5
        [HttpGet("{id}", Name = "GetHirarchy")]
        public IActionResult Get(int id)
        {
            Hierarchy hierarchyFound;
            using (var context = new ObjectContext())
            {
                hierarchyFound = context.Hierarchies
                    .Include(h => h.Nodes)
                    .ThenInclude(node => node.Tag)
                    .FirstOrDefault(h => h.Id == id);
            }
            if(hierarchyFound == null)
            {
                return NotFound();
            }
            return Ok(hierarchyFound);
        }

        #region HelperMethods:
        private Node RecursiveAddChildrenAndTags(Node parentNode)
        {
            //we need to find the type of the tag which is at the root of the hierarchy

            List<Node> newChildNodes = new List<Node>();
            foreach (Node childNode in parentNode.Children)
            {
                Node childNodeWithTagAndChildren;
                using (var context = new ObjectContext())
                {
                    childNodeWithTagAndChildren = context.Nodes
                        .Where(n => n.Id == childNode.Id)
                        .Include(n => n.Tag)
                        .Include(n => n.Children)
                            .ThenInclude(cn => cn.Tag)
                        .FirstOrDefault();
                }
                childNodeWithTagAndChildren?.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
                childNodeWithTagAndChildren = RecursiveAddChildrenAndTags(childNodeWithTagAndChildren);
                newChildNodes.Add(childNodeWithTagAndChildren);
            }
            parentNode.Children = newChildNodes;
            return parentNode;
        }

        #endregion
    }
}
