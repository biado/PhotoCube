using System;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
using ObjectCubeServer.Models.DomainClasses.TagTypes;

namespace ObjectCubeServer.Models.HelperClasses
{
    /// <summary>
    /// Simplifies the creation of DomainClasses.
    /// Used in LaugavegurDatasetInserter.
    /// Also, DomainClasses can't have constructures cause it would interfere with EF CORE.
    /// </summary>
    public class DomainClassFactory
    {
        private static int cubeObjectId = 0;
        private static int tagSetId = 0;
        private static int tagId = 0;
        private static int hierarchyId = 0;
        private static int nodeId = 0;
        private static int tagtypeId = 0;

        public static CubeObject NewCubeObject(string fileURI, FileType fileType, string thumbnailURI)
        {
            if (fileURI == null) { throw new Exception("Given fileURI was null."); }

            cubeObjectId++;
            return new CubeObject()
            {
                Id = cubeObjectId,
                FileURI = fileURI,
                FileType = fileType,
                ThumbnailURI =  thumbnailURI

            };
        }

        public static Tagset NewTagSet(string name)
        {
            if (name == null) { throw new Exception("Given name was null."); }

            tagSetId++;
            return new Tagset()
            {
                Id = tagSetId,
                Name = name
            };
        }

        public static TagType NewTagType(string description)
        {
            if (description == null) { throw new Exception("Given name was null."); }

            tagtypeId++;
            return new TagType
            {
                Id = tagtypeId,
                Description = description
            };
        }

        public static AlphanumericalTag NewAlphanumericalTag(TagType tagtype, Tagset tagset, string name)
        {
            if (name == null) { throw new Exception("Given name was null."); }
            if (tagset == null) { throw new Exception("Given tagset was null."); }
            if (tagtype == null) { throw new Exception("Given tagtype was null."); }

            tagId++;
            return new AlphanumericalTag
            {
                Id = tagId,
                TagTypeId = tagtype.Id,
                TagsetId = tagset.Id,
                Name = name
            };
        }

        public static NumericalTag NewNumericalTag(TagType tagtype, Tagset tagset, int name)
        {
            if (tagset == null) { throw new Exception("Given tagset was null."); }
            if (tagtype == null) { throw new Exception("Given tagtype was null."); }

            tagId++;
            return new NumericalTag()
            {
                Id = tagId,
                TagTypeId = tagtype.Id,
                TagsetId = tagset.Id,
                Name = name
            };
        }

        public static TimeTag NewTimeTag(TagType tagtype, Tagset tagset, TimeSpan name)
        {
            if (tagset == null) { throw new Exception("Given tagset was null."); }
            if (tagtype == null) { throw new Exception("Given tagtype was null."); }
            if (name == null) { throw new Exception("Given name was null."); }

            tagId++;
            return new TimeTag()
            {
                Id = tagId,
                TagTypeId = tagtype.Id,
                TagsetId = tagset.Id,
                Name = name
            };
        }

        public static DateTag NewDateTag(TagType tagtype, Tagset tagset, DateTime name)
        {
            if (tagset == null) { throw new Exception("Given tagset was null."); }
            if (tagtype == null) { throw new Exception("Given tagtype was null."); }
            if (name == null) { throw new Exception("Given name was null."); }

            tagId++;
            return new DateTag()
            {
                Id = tagId,
                TagTypeId = tagtype.Id,
                TagsetId = tagset.Id,
                Name = name
            };
        }

        public static Hierarchy NewHierarchy(Tagset tagset, string hierarchyName)
        {
            if (tagset == null) { throw new Exception("Given tagset was null."); }

            hierarchyId++;
            return new Hierarchy()
            {
                Id = hierarchyId,
                Name = hierarchyName,
                TagsetId = tagset.Id
                // RootNodeId is set later
            };
        }

        public static Node NewNode(Tag tag, Hierarchy hierarchy)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }
            if (hierarchy == null) { throw new Exception("Given hierarchy was null."); }

            nodeId++;
            return new Node()
            {
                Id = nodeId,
                TagId = tag.Id,
                HierarchyId = hierarchy.Id
            };
        }

        public static ObjectTagRelation NewObjectTagRelation(Tag tag, CubeObject cubeObject)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }
            if (cubeObject == null) { throw new Exception("Given cubeObject was null."); }

            return new ObjectTagRelation()
            {
                ObjectId = cubeObject.Id,
                TagId = tag.Id
            };
        }
    }
}
