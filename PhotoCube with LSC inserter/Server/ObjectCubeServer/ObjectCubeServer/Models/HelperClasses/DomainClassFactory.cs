using ObjectCubeServer.Models.DomainClasses;
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
        public static CubeObject NewCubeObject(string fileURI, FileType fileType, string thumbnailURI)
        {
            if (fileURI == null) { throw new Exception("Given fileURI was null."); }
            return new CubeObject()
            {
                FileURI = fileURI,
                FileType = fileType,
                ThumbnailURI =  thumbnailURI,
                ObjectTagRelations = new List<ObjectTagRelation>()
            };
        }

        public static Tagset NewTagSet(string name)
        {
            if (name == null) { throw new Exception("Given name was null."); }
            return new Tagset()
            {
                Name = name,
                Tags = new List<Tag>(),
                Hierarchies = new List<Hierarchy>()
            };
        }

        public static Tag NewTag(string name, Tagset tagset)
        {
            if (name == null) { throw new Exception("Given name was null."); }
            if (tagset == null) { throw new Exception("Given tagset was null."); }
            return new Tag()
            {
                Name = name,
                Tagset = tagset,
                ObjectTagRelations = new List<ObjectTagRelation>()
            };
        }

        public static Hierarchy NewHierarchy(Tagset tagset)
        {
            if (tagset == null) { throw new Exception("Given tagset was null."); }
            return new Hierarchy()
            {
                Name = tagset.Name,
                Tagset = tagset,
                Nodes = new List<Node>()
            };
        }

        public static Node NewNode(Tag tag, Hierarchy hierarchy)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }
            if (hierarchy == null) { throw new Exception("Given hierarchy was null."); }
            return new Node()
            {
                Tag = tag,
                Hierarchy = hierarchy,
                Children = new List<Node>()
            };
        }

        public static ObjectTagRelation NewObjectTagRelation(Tag tag, CubeObject cubeObject)
        {
            if (tag == null) { throw new Exception("Given tag was null."); }
            if (cubeObject == null) { throw new Exception("Given cubeObject was null."); }
            return new ObjectTagRelation()
            {
                CubeObject = cubeObject,
                Tag = tag
            };
        }
    }
}
