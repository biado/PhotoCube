namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PublicNode ParentNode { get; set; }

        public PublicNode(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
