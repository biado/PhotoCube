using ObjectCubeServer.Models.DomainClasses;

namespace ObjectCubeServer.Models.HelperClasses
{
    public class HelperMethods
    {
        public static void AddTagToTagset(Tag tag, Tagset tagset)
        {
            tag.Tagset = tagset;
            tagset.Tags.Add(tag);
        }
    }
}
