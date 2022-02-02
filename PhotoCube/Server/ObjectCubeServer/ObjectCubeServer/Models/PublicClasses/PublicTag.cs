namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicTag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Tagset {get; set; }

        public PublicTag(int id, string name, int tagset_id)
        //public PublicTag(int id, string name)
        {
            Id = id;
            Name = name;
            Tagset = tagset_id;
        }
    }
}
