using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.DataAccess;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Used in CellController to receive a JSON object repressenting an axis:
    /// Eg: {"AxisType":"Hierarchy","Id":1}
    /// </summary>
    public class ParsedAxis
    {
        public string AxisType { get; set; }
        // Either Tagset or Node id
        public int Id { get; set; }
        public List<int> Ids { get; set; }

        internal void initializeIds()
        {
            List<int> IdList = new List<int>();
            switch (AxisType)
            {
                case "Tagset":
                    List<Tag> tags = extractTagsFromTagsetAxis(); // Could not query Tags table using raw sql - seems like it doesn't support TPT hierarchies.

                    foreach (var tag in tags)
                    {
                        IdList.Add(tag.Id);
                    }

                    Ids = IdList;
                    break;

                case "Hierarchy":
                    // Find children nodes that is 1 level below
                    List<Node> childNodes = extractChildNodesFromHierarchyAxis();
                    foreach (var node in childNodes)
                    {
                        IdList.Add(node.TagId);
                    }

                    Ids = IdList;
                    break;
            }
        }

        private List<Node> extractChildNodesFromHierarchyAxis()
        {
            using var context = new ObjectContext();
            var currentNode = context.Nodes.FirstOrDefault(n => n.Id == Id);
            string query = String.Format("select * from nodes where id in (select id from get_level_from_parent_node({0},{1}))", Id,
                currentNode.HierarchyId);

            List<Node> childNodes = context.Nodes
                .FromSqlRaw(query)
                .ToList();

            return childNodes;
        }
        private List<Tag> extractTagsFromTagsetAxis()
        {
            using var context = new ObjectContext();
            var Tagset = context.Tagsets
                .Include(ts => ts.Tags)
                .FirstOrDefault(ts => ts.Id == Id);
            // Exclude 'Root(-1)' and tag that has same name as tagset (temporary fix - ideally need to fix the InsertSQLGenerator)
            var Tags = Tagset.Tags.Where(t => !(t.GetTagName().Equals("Root(-1)") || t.GetTagName().Equals(Tagset.Name))).ToList();
            return Tags;
        }
    }
}
