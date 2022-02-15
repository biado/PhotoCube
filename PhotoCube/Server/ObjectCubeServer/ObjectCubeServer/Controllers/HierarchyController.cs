﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HierarchyController : ControllerBase
    {
        private readonly ObjectContext coContext;

        public HierarchyController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/Hierarchy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hierarchy>>> Get()
        {
            List<Hierarchy> allHierarchies = await coContext.Hierarchies
                .Include(h => h.Nodes)
                    .ThenInclude(n => n.Tag)
                .ToListAsync();
            
            //TODO see if this can be made partly parallel
            //Add rootnode and recursively add subnodes and their tags:
            allHierarchies.ForEach(h => h.Nodes = new List<Node>()
            {
                RecursiveAddChildrenAndTags(h.Nodes.FirstOrDefault(n => n.Id == h.RootNodeId))
            });

            return Ok(allHierarchies);
        }

        // GET: api/Hierarchy/5
        [HttpGet("{id}", Name = "GetHirarchy")]
        public async Task<ActionResult<Hierarchy>> Get(int id)
        {
            Hierarchy hierarchyFound = await coContext.Hierarchies
                .Include(h => h.Nodes)
                .ThenInclude(node => node.Tag)
                .FirstOrDefaultAsync(h => h.Id == id);
            
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
               
                childNodeWithTagAndChildren = coContext.Nodes
                    .Where(n => n.Id == childNode.Id)
                    .Include(n => n.Tag)
                    .Include(n => n.Children)
                        .ThenInclude(cn => cn.Tag)
                    .FirstOrDefault();
                
                childNodeWithTagAndChildren?.Children.OrderBy(n => n.Tag.GetTagName());
                //childNodeWithTagAndChildren?.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
                childNodeWithTagAndChildren = RecursiveAddChildrenAndTags(childNodeWithTagAndChildren);
                newChildNodes.Add(childNodeWithTagAndChildren);
            }
            parentNode.Children = newChildNodes;
            return parentNode;
        }

        #endregion
    }
}
