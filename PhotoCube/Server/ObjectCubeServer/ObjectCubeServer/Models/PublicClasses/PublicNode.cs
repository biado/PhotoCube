namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }

        public PublicNode(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public PublicNode(int id, string name, int parentid)
        {
            Id = id;
            Name = name;
            ParentId = parentid;
        }
    }
}
