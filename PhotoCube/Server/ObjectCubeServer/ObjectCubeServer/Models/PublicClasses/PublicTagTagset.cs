namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicTagTagset
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TagsetId { get; set; }
        public string TagsetName { get; set; }

        public PublicTagTagset(int id, string name, int tagsetId, string tagsetName)
        {
            Id = id;
            Name = name;

            TagsetId = tagsetId;
            TagsetName = tagsetName;
        }
    }
}
