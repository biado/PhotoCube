using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.TagTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models
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

        public static Tag NewTag(TagType tagtype,Tagset tagset)
        {
            if (tagset == null) { throw new Exception("Given tagset was null."); }

            tagId++;
            return new Tag()
            {
                Id =  tagId,
                TagTypeId = tagtype.Id,
                TagsetId = tagset.Id
            };
        }

        public static AlphanumericalTag NewAlphanumericalTag(Tag tag, string name)
        {
            if (name == null) { throw new Exception("Given name was null."); }
            if (tag == null) { throw new Exception("Given tag was null."); }

            return new AlphanumericalTag()
            {
                Id = tag.Id,
                Name = name
            };
        }

        public static NumericalTag NewNumericalTag(Tag tag, int name)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }

            return new NumericalTag()
            {
                Id = tag.Id,
                Name = name
            };
        }

        public static TimeTag NewTimeTag(Tag tag, DateTime name)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }
            if (name == null) { throw new Exception("Given name was null."); }

            return new TimeTag()
            {
                Id = tag.Id,
                Name = name
            };
        }

        public static DateTag NewDateTag(Tag tag, DateTime name)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }
            if (name == null) { throw new Exception("Given name was null."); }

            return new DateTag()
            {
                Id = tag.Id,
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
