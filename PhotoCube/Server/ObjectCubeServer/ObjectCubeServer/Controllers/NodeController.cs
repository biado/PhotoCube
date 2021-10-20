using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ObjectCubeServer.Models.DomainClasses;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
using ObjectCubeServer.Models.PublicClasses;

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

        // GET: api/Node/5
        [HttpGet("{id:int}", Name = "GetNodes")]
        public async Task<ActionResult<Node>> Get(int id)
        {
            Node nodeFound = await coContext.Nodes
                .Where(n => n.Id == id)
                .Include(n => n.Tag)
                .Include(n => n.Children)
                    .ThenInclude(cn => cn.Tag)
                .FirstOrDefaultAsync();
            
            if (nodeFound == null) { return NotFound(); }

            nodeFound.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
            nodeFound = await RecursiveAddChildrenAndTags(nodeFound);
            return Ok(nodeFound);
        }

        // GET: api/Node/name=wood
        [HttpGet("name={tag}")]
        public async Task<ActionResult<IEnumerable<Node>>> GetNodeByName(string tag)
        {
            List<Node> nodesFound = await coContext.Nodes
                    .Include(n => n.Tag)
                    .Where(n => ((AlphanumericalTag)n.Tag).Name.ToLower().StartsWith(tag.ToLower()))
                    .ToListAsync();
            
            if (nodesFound == null) return NotFound();
            
            var result = new List<PublicNode>();
            foreach (Node node in nodesFound)
            {
                var publicNode = new PublicNode(node.Id, ((AlphanumericalTag)node.Tag).Name)
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
                .Select(n => n.Children.Select(cn => new PublicNode(cn.Id, ((AlphanumericalTag) cn.Tag).Name)))
                .FirstOrDefaultAsync();

            return Ok(childNodes);
        }

        #region HelperMethods:
        private async Task<PublicNode> GetParentNode(Node child)
        {
            PublicNode parentNode = await coContext.Nodes
                .Where(n => n.Children.Contains(child))
                .Include(n => n.Tag)
                .Select(n => new PublicNode(n.Id,((AlphanumericalTag)n.Tag).Name))
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
                
                childNodeWithTagAndChildren.Children.OrderBy(n => ((AlphanumericalTag)n.Tag).Name);
                childNodeWithTagAndChildren = await RecursiveAddChildrenAndTags(childNodeWithTagAndChildren);
                newChildNodes.Add(childNodeWithTagAndChildren);
            }
            parentNode.Children = newChildNodes;
            return parentNode;
        }
        #endregion
    }
}
