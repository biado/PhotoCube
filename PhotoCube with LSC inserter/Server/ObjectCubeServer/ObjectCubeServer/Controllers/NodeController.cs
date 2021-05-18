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

        // GET: api/Node/name=wood
        [HttpGet("name={tag}")]
        public IActionResult GetNodeByName(string tag)
        {
            List<Node> nodesFound;
            using (var context = new ObjectContext())
            {
                nodesFound = context.Nodes
                    .Include(n => n.Tag)
                    .Where(n => ((AlphanumericalTag)n.Tag).Name.ToLower().StartsWith(tag.ToLower()))
                    .ToList();
            }

            if (nodesFound != null)
            {
                var result = new List<PublicNode>();
                foreach (Node node in nodesFound)
                {
                    var publicNode = new PublicNode(node.Id, ((AlphanumericalTag)node.Tag).Name)
                    {
                        ParentNode = GetParentNode(node)
                    };
                    result.Add(publicNode);
                }
                return Ok(JsonConvert.SerializeObject(result));
            }
            return null;
        }

        // GET: api/Node/123/Parent
        [HttpGet("{nodeId}/parent")]
        public IActionResult GetParentNode(int nodeId)
        {
            PublicNode parentNode;
            using (var context = new ObjectContext())
            {
                var childNode = context.Nodes
                    .Where(n => n.Id == nodeId)
                    .FirstOrDefault();
                parentNode = GetParentNode(childNode);
            }
            return Ok(JsonConvert.SerializeObject(parentNode));
        }

        // GET: api/Node/123/Children
        [HttpGet("{nodeId}/children")]
        public IActionResult GetChildNodes(int nodeId)
        {
            IEnumerable childNodes;
            using (var context = new ObjectContext())
            {
                childNodes = context.Nodes
                    .Include(n => n.Children)
                    .Where(n => n.Id == nodeId)
                    .Select(n => n.Children.Select(cn => new PublicNode(cn.Id, ((AlphanumericalTag)cn.Tag).Name)))
                    .FirstOrDefault();
            }
            if (childNodes == null)
            {
                return null;
            }
            return Ok(JsonConvert.SerializeObject(childNodes));
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
