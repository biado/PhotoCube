namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicTagset
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PublicTagset(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
