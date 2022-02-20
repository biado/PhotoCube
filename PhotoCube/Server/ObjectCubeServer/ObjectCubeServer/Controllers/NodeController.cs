using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ObjectCubeServer.Models.DomainClasses;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
using ObjectCubeServer.Models.PublicClasses;
using System;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly ObjectContext coContext;

        public NodeController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        // GET: api/Node
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Node>>> Get()
        {
            List<Node> allNodes = await coContext.Nodes
                .Include(n => n.Children)
                .ToListAsync();
            
            return Ok(allNodes);
        }
        
        [HttpGet("{id:int}", Name = "GetNodeById")]
        public async Task<ActionResult<PublicNodeDetails>> GetNodeById(int id)
        {
            PublicNodeDetails nodeFound = await coContext.Nodes
                .Where(n => n.Id == id)
                .Include(n => n.Tag)
                .Select(n => new PublicNodeDetails(n.Id, n.Tag.GetTagName(), n.TagId,n.HierarchyId))
                //.Select(n => new PublicNodeDetails(n.Id,((AlphanumericalTag)n.Tag).Name, n.TagId,n.HierarchyId))
                .FirstOrDefaultAsync();
            
            if (nodeFound == null) { return NotFound(); }
            
            nodeFound.ParentId = await GetParentNodeId(nodeFound.Id);

            return Ok(nodeFound);
        }

        // GET: api/Node/5
        [HttpGet("{id:int}/tree", Name = "GetNodeTree")]
        public async Task<ActionResult<Node>> GetNodeByIdTree(int id)
        {
            Node nodeFound = await coContext.Nodes
                .Where(n => n.Id == id)
                .Include(n => n.Tag)
                .Include(n => n.Children)
                    .ThenInclude(cn => cn.Tag)
                .FirstOrDefaultAsync();
            
            if (nodeFound == null) { return NotFound(); }

            nodeFound.Children.OrderBy(n => n.Tag.GetTagName());
            //nodeFound.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
            nodeFound = await RecursiveAddChildrenAndTags(nodeFound);
            return Ok(nodeFound);
        }

        // GET: api/Node/name=wood
        [HttpGet("name={tag}")]
        public async Task<ActionResult<IEnumerable<PublicNode>>> GetNodeByName(string tag)
        {
            List<Node> nodesFound = await coContext.Nodes
                    .Include(n => n.Tag)
                    //*Here it does not work with GetTagName() because the body of the lambda cannot be translated into SQL
                    .Where(n => ((AlphanumericalTag)n.Tag).Name.ToLower().StartsWith(tag.ToLower()))
                    .ToListAsync();

            // List<Node> nodesFound = coContext.Nodes //seems slower?
            //         .Include(n => n.Tag)
            //         //*Here it does not work with GetTagName() because the body of the lambda cannot be translated into SQL
            //         .AsEnumerable()
            //         .Where(n => n.Tag.GetTagName().ToLower().StartsWith(tag.ToLower()))
            //         .ToList();

            if (nodesFound == null) {Console.WriteLine(nodesFound); return NotFound(); }
            
            var result = new List<PublicNode>();
            foreach (Node node in nodesFound)
            {
                var publicNode = new PublicNode(node.Id, node.Tag.GetTagName())
                //var publicNode = new PublicNode(node.Id, ((AlphanumericalTag)node.Tag).Name)
                {
                    ParentNode = await GetParentNode(node)
                };
                result.Add(publicNode);
            }
            return Ok(result);
        }

        // GET: api/Node/123/Parent
        [HttpGet("{nodeId:int}/parent")]
        public async Task<ActionResult<PublicNode>> GetParentNode(int nodeId)
        {
            Node childNode = await coContext.Nodes
                    .FirstOrDefaultAsync(n => n.Id == nodeId);
            PublicNode parentNode = await GetParentNode(childNode);

            if (parentNode == null) return NotFound();
            
            return Ok(parentNode);
        }

        // GET: api/Node/123/Children
        [HttpGet("{nodeId}/children")]
        public async Task<ActionResult<IEnumerable<PublicNode>>> GetChildNodes(int nodeId)
        {
            IEnumerable<PublicNode> childNodes = await coContext.Nodes.Include(n => n.Children)
                .Where(n => n.Id == nodeId)
                .Select(n => n.Children.Select(cn => new PublicNode(cn.Id, cn.Tag.GetTagName())))
                //.Select(n => n.Children.Select(cn => new PublicNode(cn.Id, ((AlphanumericalTag) cn.Tag).Name)))
                .FirstOrDefaultAsync();

            return Ok(childNodes);
        }

        #region HelperMethods:
        
        private async Task<int?> GetParentNodeId(int childId)
        {
            int parentId = await coContext.Nodes
                .Where(n => n.Children.Any(c => c.Id == childId))
                .Select(n => n.Id)
                .FirstOrDefaultAsync();

            if(parentId == 0) return null;
            
            return parentId;
        }

        private async Task<PublicNode> GetParentNode(Node child)
        {
            PublicNode parentNode = await coContext.Nodes
                .Where(n => n.Children.Contains(child))
                .Include(n => n.Tag)
                .Select(n => new PublicNode(n.Id,n.Tag.GetTagName()))
                //.Select(n => new PublicNode(n.Id,((AlphanumericalTag)n.Tag).Name))
                .FirstOrDefaultAsync();
            
            return parentNode;
        }

        private async Task<Node> RecursiveAddChildrenAndTags(Node parentNode)
        {
            List<Node> newChildNodes = new List<Node>();
            foreach(Node childNode in parentNode.Children)
            {
                Node childNodeWithTagAndChildren;
                
                childNodeWithTagAndChildren = await coContext.Nodes
                    .Where(n => n.Id == childNode.Id)
                    .Include(n => n.Tag)
                    .Include(n => n.Children)
                        .ThenInclude(cn => cn.Tag)
                    .FirstOrDefaultAsync();
                
                childNodeWithTagAndChildren.Children.OrderBy(n => n.Tag.GetTagName());
                //childNodeWithTagAndChildren.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
                childNodeWithTagAndChildren = await RecursiveAddChildrenAndTags(childNodeWithTagAndChildren);
                newChildNodes.Add(childNodeWithTagAndChildren);
            }
            parentNode.Children = newChildNodes;
            return parentNode;
        }
        #endregion
    }
}
