using System;
namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicNode
    {
        public int Id { get; set; }
        public string Name;

        public PublicNode(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
