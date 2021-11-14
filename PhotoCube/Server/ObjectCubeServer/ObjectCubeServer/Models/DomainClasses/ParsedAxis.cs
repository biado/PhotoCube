using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.Contexts;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Used in CellController to receive a JSON object repressenting an axis:
    /// Eg: {"AxisType":"Hierarchy","Id":1}
    /// </summary>
    public class ParsedAxis
    {
        public string Type { get; set; }
        // Either Tagset or Node id
        public int Id { get; set; }
        internal Dictionary<int,int> Ids { get;  set; }

        internal void initializeIds(ObjectContext context)
        {
            var IdList = new Dictionary<int,int>();
            int i = 1;
            switch (Type)
            {
                case "tagset":
                    List<Tag> tags = ExtractTagsFromTagsetAxis(context); // Could not query Tags table using raw sql - seems like it doesn't support TPT hierarchies.

                    foreach (var tag in tags)
                    {
                        IdList.Add(tag.Id, i++);
                    }

                    Ids = IdList;
                    break;

                case "node":
                    // Find children nodes that is 1 level below
                    List<Node> childNodes = ExtractChildNodesFromHierarchyAxis(context);

                    foreach (var node in childNodes)
                    {
                        IdList.Add(node.Id, i++);
                    }

                    Ids = IdList;
                    break;

                default:
                    IdList.Add(1, 1);
                    Ids = IdList;
                    break;
            }
        }

        private List<Node> ExtractChildNodesFromHierarchyAxis(ObjectContext context)
        {
            var currentNode = context.Nodes.FirstOrDefault(n => n.Id == Id);
            if (currentNode != null)
            {
                string query =
                    $"select N.* from (select N.* from nodes N where N.id in (select id from get_level_from_parent_node({Id},{currentNode.HierarchyId}))) N join alphanumerical_tags A on N.tag_id = A.id order by A.name";

                List<Node> childNodes = context.Nodes
                    .FromSqlRaw(query)
                    .ToList();

                return childNodes;
            }

            throw new NullReferenceException("currentNode");
        }
        private List<Tag> ExtractTagsFromTagsetAxis(ObjectContext context)
        {
            var tagset = context.Tagsets
                .Include(ts => ts.Tags)
                .FirstOrDefault(ts => ts.Id == Id);
            // Exclude tag that has same name as tagset (temporary fix - ideally need to fix the InsertSQLGenerator)
            var tags = tagset?.Tags.Where(t => !t.GetTagName().Equals(tagset.Name)).OrderBy(t => t.GetTagName()).ToList();
            return tags;
        }

        public override string ToString()
        {
            return $"Type = {Type}\n Id{Id.ToString()}\n Ids{Ids}";
        }
    }
}
