using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObjectCubeServer.Models.DataAccess;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.TagTypes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        // GET: api/Node
        [HttpGet]
        public IActionResult Get()
        {
            List<Node> allNodes;
            using (var context = new ObjectContext())
            {
                allNodes = context.Nodes
                    .Include(n => n.Children)
                    .ToList();
            }
            return Ok(JsonConvert.SerializeObject(allNodes));
        }

        // GET: api/Node/5
        [HttpGet("{id}", Name = "GetNodes")]
        public IActionResult Get(int id)
        {
            Node nodeFound;
            using (var context = new ObjectContext())
            {
                nodeFound = context.Nodes
                    .Where(n => n.Id == id)
                    .Include(n => n.Tag)
                    .Include(n => n.Children)
                        .ThenInclude(cn => cn.Tag)
                    .FirstOrDefault();
            }
            if (nodeFound == null) { return NotFound(); }
            else
            {
                nodeFound.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
                nodeFound = RecursiveAddChildrenAndTags(nodeFound);
                return Ok(JsonConvert.SerializeObject(nodeFound,
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
        }

        // GET: api/Node/name=coffee
        [HttpGet("name={tag}")]
        public IActionResult GetNodeByName(string tag)
        {
            List<Node> nodesFound;
            using (var context = new ObjectContext())
            {
                nodesFound = context.AlphanumericalTags
                    .Include(at => at.Nodes)
                    .Where(at => at.Name.Equals(tag))
                    .Select(at => at.Nodes)
                    .FirstOrDefault();
            }

            var result = new List<List<PublicNode>>();
            foreach(Node node in nodesFound)
            {
                var publicNodes = new List<PublicNode>() {
                     new PublicNode(node.Id, tag), GetParentNode(node)
                };
                result.Add(publicNodes);
 
            }

            if (result == null)
            {
                return null;
            }
            return Ok(JsonConvert.SerializeObject(result));
            
        }

        #region HelperMethods:
        private PublicNode GetParentNode(Node child)
        {
            PublicNode parentNode;
            using (var context = new ObjectContext())
            {
                parentNode = context.Nodes
                    .Where(n => n.Children.Contains(child))
                    .Include(n => n.Tag)
                    .Select(n => new PublicNode(n.Id,((AlphanumericalTag)n.Tag).Name))
                    .FirstOrDefault();
            }

            return parentNode;
        }

        private Node RecursiveAddChildrenAndTags(Node parentNode)
        {
            List<Node> newChildNodes = new List<Node>();
            foreach(Node childNode in parentNode.Children)
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
                childNodeWithTagAndChildren.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
                childNodeWithTagAndChildren = RecursiveAddChildrenAndTags(childNodeWithTagAndChildren);
                newChildNodes.Add(childNodeWithTagAndChildren);
            }
            parentNode.Children = newChildNodes;
            return parentNode;
        }
        #endregion
    }
}
