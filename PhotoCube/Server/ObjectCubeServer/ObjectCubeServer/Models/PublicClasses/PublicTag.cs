namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int TagsetId { get; set; }
        
        public string TagTypeDescription { get; set; }

        public PublicTag(int id, string name, int tagsetId, string tagTypeDescription)
        {
            Id = id;
            Name = name;
            TagsetId = tagsetId;
            TagTypeDescription = tagTypeDescription;
        }
    }
}
