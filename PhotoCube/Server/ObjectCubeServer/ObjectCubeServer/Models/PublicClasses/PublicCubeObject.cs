namespace ObjectCubeServer.Models.PublicClasses
{
    /// <summary>
    /// Represents an Object in the M^3 datamodel. (Simplified public model)
    /// Has an Id (CubeObjectId) and a FileURI.
    /// </summary>
    public class PublicCubeObject
    {
        public int Id { get; set; }
        public string FileURI { get; set; }

        public PublicCubeObject(int id, string fileUri)
        {
            Id = id;
            FileURI = fileUri;
        }
    }
}
