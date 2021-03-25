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
                        .ThenInclude(cn => cn.Tag as AlphanumericalTag)
                    .FirstOrDefault();
            }
            if (nodeFound == null) { return NotFound(); }
            else
            {
                nodeFound.Children.OrderBy(n => n.Tag.AlphanumericalTag.Name);
                //nodeFound.Children.Sort((cn1, cn2) => cn1.Tag.AlphanumericalTag.Name.CompareTo(cn2.Tag.AlphanumericalTag.Name));
                nodeFound = RecursiveAddChildrenAndTags(nodeFound);
                return Ok(JsonConvert.SerializeObject(nodeFound));
            }
        }

        #region HelperMethods:
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
                            .ThenInclude(cn => cn.Tag as AlphanumericalTag)
                        .FirstOrDefault();
                }
                childNodeWithTagAndChildren.Children.OrderBy(n => n.Tag.AlphanumericalTag.Name);
                //childNodeWithTagAndChildren.Children.Sort((cn1, cn2) => cn1.Tag.AlphanumericalTag.Name.CompareTo(cn2.Tag.AlphanumericalTag.Name));
                childNodeWithTagAndChildren = RecursiveAddChildrenAndTags(childNodeWithTagAndChildren);
                newChildNodes.Add(childNodeWithTagAndChildren);
            }
            parentNode.Children = newChildNodes;
            return parentNode;
        }
        #endregion
    }
}
