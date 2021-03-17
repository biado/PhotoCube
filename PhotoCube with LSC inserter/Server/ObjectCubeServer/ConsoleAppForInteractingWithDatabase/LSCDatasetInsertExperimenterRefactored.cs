using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models;
using ObjectCubeServer.Models.DataAccess;
using ObjectCubeServer.Models.DomainClasses;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace ConsoleAppForInteractingWithDatabase
{
    public class LSCDatasetInsertExperimenterRefactored
    {
        private int numOfImages;
        private string pathToDataset;
        private string pathToTagFile;
        private string pathToHierarchiesFile;
        private string pathToErrorLogFile;
        private string resultPath;
        private Stopwatch stopwatch;
        private int batchSize = 10000;
        private string SQLPath;
        private NameValueCollection sAll = ConfigurationManager.AppSettings;

        private Dictionary<string, CubeObject> cubeObjects = new Dictionary<string, CubeObject>();
        private Dictionary<string, Tagset> tagsets = new Dictionary<string, Tagset>();
        private Dictionary<string, Dictionary <int,Tag>> tags = new Dictionary<string, Dictionary<int,Tag>>();
        private Dictionary<string, Dictionary<int, ObjectTagRelation>> objectTagRelations = new Dictionary<string, Dictionary<int,ObjectTagRelation>>();
        private Dictionary<string, Hierarchy> hierarchies = new Dictionary<string, Hierarchy>();
        private Dictionary<int, Node> nodes = new Dictionary<int, Node>();

        public LSCDatasetInsertExperimenterRefactored(int numOfImages)
        {
            this.numOfImages = numOfImages;
            this.pathToDataset = sAll.Get("pathToLscData");
            this.resultPath = sAll.Get("resultPath");
            this.SQLPath = sAll.Get("SQLPath");

            this.pathToTagFile = Path.Combine(pathToDataset, @sAll.Get("LscTagFilePath"));
            this.pathToHierarchiesFile = Path.Combine(pathToDataset, @sAll.Get("LscHierarchiesFilePath"));
            this.pathToErrorLogFile = Path.Combine(pathToDataset, @sAll.Get("LscErrorfilePath"));

            File.AppendAllText(pathToErrorLogFile, "Errors goes here:\n");

            this.stopwatch = new Stopwatch();
            stopwatch.Start();
            BuildCubeObjects();
            BuildTagsetsAndTags();
            BuildHierarchiesAndNodes();
            WriteInsertStatementsToFile();
        }

        private void LogTimeToFile(string entities, int row)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Hours, ts.Minutes, ts.Seconds);
            string log = string.Join(",", entities, row, elapsedTime) + "\n";
            File.AppendAllText(resultPath, log);
        }

        /// <summary>
        /// Parses and inserts cube objects, photos and thumbnails.
        /// </summary>
        private void BuildCubeObjects()
        {
            Console.WriteLine("Building Cube Objects.");
            try
            {
                    int fileCount = 1;

                    using (StreamReader reader = new StreamReader(pathToTagFile))
                    {
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals("") && fileCount <= numOfImages)
                        {
                            Console.WriteLine("CubeObject line: " + fileCount);

                            //File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
                            string filename = line.Split(":")[0];
                            string filepath = Path.Combine(pathToDataset, filename);

                            // If Image is already in Map(Assuming no two file has the same name):
                            if (cubeObjects.ContainsKey(filename))
                            {
                                //Don't add it again.
                                Console.WriteLine("Image " + filename + " is already in the database");
                            }

                            //Loading and saving image:
                            using (Image<Rgba32> image = Image.Load<Rgba32>(filepath))
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    image.SaveAsJpeg(ms); //Copy to ms

                                    bool modified = false;
                                    //Creating and saving thumbnail:
                                    //Thumbnails needs to be power of two in width and height to avoid extra image manipulation client side.
                                    if (image.Width > image.Height)
                                    {
                                        modified = resizeOriginalImageToMakeThumbnails(image, image.Height);
                                    }
                                    else
                                    {
                                        modified = resizeOriginalImageToMakeThumbnails(image, image.Width);
                                    }

                                    string thumbnailURI = modified ? saveThumbnail(image, filename) : filename;

                                    CubeObject cubeObject = DomainClassFactory.NewCubeObject(
                                        filename,
                                        FileType.Photo,
                                        thumbnailURI);
                                    cubeObjects[filename] = cubeObject;
                                }
                            }

                            if (fileCount % batchSize == 0)
                            {
                                LogTimeToFile("CubeObject", fileCount);
                            }
                            fileCount++;
                        }
                    }
                    LogTimeToFile("CubeObject", fileCount-1);
            }
            catch (Exception e) 
            {
                    Console.WriteLine("File could not be read to insert the cube objects.");
                    Console.WriteLine(e.Message);
            }
        }

        private string saveThumbnail(Image<Rgba32> image, string filename)
        {
            string thumbnailURI = Path.Combine(pathToDataset, "Thumbnails", filename);
            image.Save(thumbnailURI + ".jpg");
            return thumbnailURI;
        }

        private bool resizeOriginalImageToMakeThumbnails(Image<Rgba32> image, int shortSide)
        {
            if (shortSide > 1024)
            {
                int destinationShortSide = 1024; //1024px
                double downscaleFactor = Convert.ToDouble(destinationShortSide) / image.Height;
                int newWidth = (int)(image.Width * downscaleFactor);
                int newHeight = (int)(image.Height * downscaleFactor);
                image.Mutate(i => i
                        .Resize(newWidth, newHeight) //Scale
                        .Crop(destinationShortSide, destinationShortSide) //Crop
                );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses and inserts tags and tagsets. Also tags Photos.
        /// </summary>
        private void BuildTagsetsAndTags()
        {
            Console.WriteLine("Building TagsSets and Tags.");

            try
                {
                    int lineCount = 1;

                    using (StreamReader reader = new StreamReader(pathToTagFile))
                    {
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals("") && lineCount <= numOfImages)
                        {
                            Console.WriteLine("Tagset & Tag line: " + lineCount);

                        //File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
                        string[] split = line.Split(":");
                            string fileName = split[0];

                            CubeObject cubeObject = cubeObjects[fileName];

                            int numTagPairs = (split.Length - 2) / 2;
                            //Looping over each pair of tags:
                            for (int i = 0; i < numTagPairs; i++)
                            {
                                string tagsetName = split[(i * 2) + 1];
                                string tagName = split[(i * 2) + 2];

                                Tagset tagset;
                                if (!tagsets.ContainsKey(tagsetName))
                                {
                                    tagset = createNewTagset(tagsetName, tagsets);

                                    //Also creates a tag with same name:
                                    Tag tagWithSameNameAsTagset = DomainClassFactory.NewTag(tagName, tagset);
                                    Dictionary<int, Tag> tagWithSameNameAsTagsetList = new Dictionary<int, Tag>();
                                    tagWithSameNameAsTagsetList[tagset.Id] = tagWithSameNameAsTagset;
                                    tags[tagName] = tagWithSameNameAsTagsetList;
                                }
                                else
                                {
                                    tagset = tagsets[tagsetName];
                                }

                                //Checking if tag exists, and creates it if it doesn't exist.
                                Tag tag;
                                Dictionary<int, Tag> tagList;
                                if (!tags.ContainsKey(tagName))
                                {
                                    tag = DomainClassFactory.NewTag(tagName, tagset);
                                    tagList = new Dictionary<int, Tag>();
                                    tagList[tagset.Id] = tag;
                                    tags[tagName] = tagList;
                                }
                                else
                                {
                                    tagList = tags[tagName];
                                    if (!tagList.ContainsKey(tagset.Id))
                                    {
                                        tag = DomainClassFactory.NewTag(tagName, tagset);
                                        tagList[tagset.Id] = tag;
                                        tags[tagName] = tagList;
                                    }
                                    else
                                    {
                                        tag = tagList[tagset.Id];
                                    }
                                }

                                if (cubeObject == null)
                                {
                                    File.AppendAllText(pathToErrorLogFile,
                                        "File " + fileName + " was not found while parsing line " + lineCount);
                                    //throw new Exception("Expected cubeobject to be in the DB already, but it isn't!");
                                }
                                else
                                {
                                    Dictionary<int, ObjectTagRelation> OTRelations;
                                    if (!objectTagRelations.ContainsKey(fileName))
                                    {
                                        OTRelations = new Dictionary<int, ObjectTagRelation>();
                                        ObjectTagRelation otr =
                                            DomainClassFactory.NewObjectTagRelation(tag, cubeObject);
                                        OTRelations[tag.Id] = otr;
                                    }
                                    else
                                    {
                                        OTRelations = objectTagRelations[fileName];
                                        if (!containsObjectTagRelation(fileName, tag.Id))
                                        {
                                            //create new otr
                                            ObjectTagRelation otr =
                                                DomainClassFactory.NewObjectTagRelation(tag, cubeObject);
                                            OTRelations[tag.Id] = otr;
                                        }
                                    }

                                    objectTagRelations[fileName] = OTRelations;
                                }
                            }
                            if (lineCount % batchSize == 0)
                            {
                                LogTimeToFile("Tagset & Tag", lineCount);
                            }
                        lineCount++;
                        }
                    }
                    LogTimeToFile("Tagset & Tag", lineCount-1);

            }
            catch (Exception e)
                {
                    Console.WriteLine("File could not be read to insert the tags.");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException.Message);
                }
        }

        private bool containsObjectTagRelation(string fileName, int tagId)
        {
            Dictionary<int, ObjectTagRelation> OTRelations = objectTagRelations[fileName];
            return OTRelations.ContainsKey(tagId);
        }

        private Tagset createNewTagset(string tagsetName, Dictionary<string, Tagset> tagsets)
        {
            Tagset tagset = DomainClassFactory.NewTagSet(tagsetName);
            tagsets.Add(tagsetName, tagset);
            return tagset;
        }

        /// <summary>
        /// Inserts hierarchies
        /// </summary>
        private void BuildHierarchiesAndNodes()
        {
            Console.WriteLine("Building Hierarchies."); 
            try 
            {
                int lineCount = 1;

                using (StreamReader reader = new StreamReader(pathToHierarchiesFile))
                    {
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals(""))
                        {
                            Console.WriteLine("Hierarchy & Node line: " + lineCount);

                        //File format: TagsetName:HierarchyName:ParrentTag:ChildTag:ChildTag:ChildTag:(...)
                        string[] split = line.Split(":");
                            string tagsetName = split[0];
                            string hierarchyName = split[1];
                            string parentTagName = split[2];

                            //Finding tagset:
                            Tagset tagset = tagsets[tagsetName];

                            Hierarchy hierarchy;
                            //If hierarchyFromDb does not exist, create it:
                            if (!hierarchies.ContainsKey(hierarchyName))
                            {
                                hierarchy = DomainClassFactory.NewHierarchy(tagset, hierarchyName);
                                hierarchies[hierarchyName] = hierarchy;
                            }
                            else
                            {
                                hierarchy = hierarchies[hierarchyName];
                            }

                            //Finding parent tag:
                            Tag parentTag;
                            Dictionary<int, Tag> tagList;

                            //If parentTag does not exist, create it:
                            if (!tags.ContainsKey(parentTagName))
                            {
                                parentTag = DomainClassFactory.NewTag(parentTagName, tagset);
                                tagList = new Dictionary<int, Tag>();
                                tagList[parentTag.TagsetId] = parentTag;
                                tags[parentTagName] = tagList;
                            }
                            else
                            {
                                tagList = tags[parentTagName];
                                if (!tagList.ContainsKey(tagset.Id)) 
                                {
                                    parentTag = DomainClassFactory.NewTag(parentTagName, tagset);
                                    tagList[parentTag.TagsetId] = parentTag;
                                    tags[parentTagName] = tagList;
                                }
                                else
                                {
                                    parentTag = tagList[tagset.Id];
                                }
                            }

                            // nodes dictionary (k,v) = (tagId, node)
                            //Finding parent node:
                            Node parentNode;

                            //If parent node does not exist, create it:
                            if (!nodes.ContainsKey(parentTag.Id))
                            {
                                parentNode = DomainClassFactory.NewNode(parentTag, hierarchy);
                                nodes[parentTag.Id] = parentNode;

                                if (hierarchyName.Equals(parentTagName))
                                {
                                    // It is a rootNode
                                    hierarchy.RootNodeId = parentNode.Id;
                                }
                            }
                            else
                            {
                                parentNode = nodes[parentTag.Id];
                            }

                            //Adding child nodes:
                            for (int i = 3; i < split.Length; i++)
                            {
                                string childTagName = split[i];

                                Tag childTag;
                                Dictionary<int, Tag> childTagList;

                                //If child tag does not exist, create it:
                                if (!tags.ContainsKey(childTagName))
                                {
                                    childTag = DomainClassFactory.NewTag(childTagName, tagset);
                                    childTagList = new Dictionary<int, Tag>();
                                    childTagList[childTag.TagsetId] = childTag;
                                    tags[childTagName] = childTagList;
                                }
                                else
                                {
                                    childTagList = tags[childTagName];
                                    if (!childTagList.ContainsKey(tagset.Id))
                                    {
                                        childTag = DomainClassFactory.NewTag(childTagName, tagset);
                                        childTagList[childTag.TagsetId] = childTag;
                                        tags[childTagName] = childTagList;
                                    }
                                    else
                                    {
                                        childTag = childTagList[tagset.Id];
                                    }
                                }

                                //Finding child node:
                                Node childNode;
                                if (!nodes.ContainsKey(childTag.Id))
                                {
                                    childNode = DomainClassFactory.NewNode(childTag, hierarchy);
                                    childNode.ParentNodeId = parentNode.Id;
                                    nodes[childTag.Id] = childNode;
                                }
                                else
                                {
                                    childNode = nodes[childTag.Id];
                                    childNode.ParentNodeId = parentNode.Id;
                                }
                            }
                            if (lineCount % batchSize == 0)
                            {
                                LogTimeToFile("Hierarchy & Node", lineCount);
                            }
                        lineCount++;
                        }
                    }
                    LogTimeToFile("Hierarchy & Node", lineCount-1);
            }
            catch (Exception e) 
            {
                Console.WriteLine("File could not be read to insert the hierarchies.");
                Console.WriteLine(e.Message);
            }
        }

        private void WriteInsertStatementsToFile()
        {
            // insert into [tableName]
            // (column1, column2, ..)
            // values
            // (value1, value2, ..);

            OperatingSystem OS = Environment.OSVersion;
            PlatformID platformId = OS.Platform;

            int insertCount = 0;

            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET QUOTED_IDENTIFIER ON\nGO\nSET ANSI_NULLS ON\nGO\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT cubeobjects ON;\nGO\n");
            }
            //Insert all CubeObjects
            foreach (var co in cubeObjects.Values)
            {
                string insertStatement = "INSERT INTO cubeobjects(id, file_uri, file_type, thumbnail_uri) VALUES(" + co.Id + ",'" + co.FileURI + "'," + (int) co.FileType + ",'" + co.ThumbnailURI + "'); \n";
                File.AppendAllText(SQLPath, insertStatement);
                insertCount++;
                if (insertCount % 100 == 0 && platformId == PlatformID.Win32NT)
                {
                    File.AppendAllText(SQLPath, "GO\n");
                }
            }
            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT cubeobjects OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tagsets ON;\nGO\n");
            }
            //Insert all Tagsets
            foreach (var ts in tagsets.Values)
            {
                string insertStatement = "INSERT INTO tagsets(id, name) VALUES(" + ts.Id + ",'" + ts.Name + "'); \n";
                File.AppendAllText(SQLPath, insertStatement);
            }
            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tagsets OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tags ON;\nGO\n");
            }
            //Insert all Tags 
            insertCount = 0;
            foreach (var list in tags.Values)
            {
                foreach (var t in list.Values)
                {
                    string insertStatement = "INSERT INTO tags(id, name, tagset_id) VALUES(" + t.Id + ",'" + t.Name + "'," + t.TagsetId + "); \n";
                    File.AppendAllText(SQLPath, insertStatement);
                    insertCount++;
                    if (insertCount % 100 == 0 && platformId == PlatformID.Win32NT)
                    {
                        File.AppendAllText(SQLPath, "GO\n");
                    }
                }
            }
            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tags OFF;\n");
            }
            //Insert all ObjectTagRelations (IDENTITY_INSERT should be off)
            insertCount = 0;
            foreach (var co in objectTagRelations.Values)
            {
                foreach (var otr in co.Values)
                {
                    string insertStatement = "INSERT INTO objecttagrelations(object_id, tag_id) VALUES(" + otr.ObjectId + "," + otr.TagId + "); \n";
                    File.AppendAllText(SQLPath, insertStatement);
                    insertCount++;
                    if (insertCount % 100 == 0 && platformId == PlatformID.Win32NT)
                    {
                        File.AppendAllText(SQLPath, "GO\n");
                    }
                }
            }
            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT hierarchies ON;\nGO\n");
            }
            //Insert all Hierarchies
            foreach (var h in hierarchies.Values)
            {
                string insertStatement = "INSERT INTO hierarchies(id, name, tagset_Id, rootnode_id) VALUES(" + h.Id + ",'" + h.Name + "'," + h.TagsetId + "," + h.RootNodeId + "); \n";
                File.AppendAllText(SQLPath, insertStatement);
            }
            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT hierarchies OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT nodes ON;\nGO\n");
            }
            //Insert all Nodes first without setting parent node to avoid violating FK constraint
            insertCount = 0;
            foreach (var n in nodes.Values)
            {
                string insertStatement = "INSERT INTO nodes(id, tag_id, hierarchy_id) VALUES(" + n.Id + "," + n.TagId + "," + n.HierarchyId + "); \n";
                File.AppendAllText(SQLPath, insertStatement);
                insertCount++;
                if (insertCount % 100 == 0 && platformId == PlatformID.Win32NT)
                {
                    File.AppendAllText(SQLPath, "GO\n");
                }
            }
            //Upate all Nodes and set parent node
            foreach (var n in nodes.Values)
            {
                //Root nodes should have no parent node
                if (n.ParentNodeId != null)
                {
                    string insertStatement = "UPDATE nodes SET parentnode_id=" + n.ParentNodeId + " WHERE id=" + n.Id + "; \n";
                    File.AppendAllText(SQLPath, insertStatement);
                }
            }
            if (platformId == PlatformID.Win32NT)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT nodes OFF;\nGO");
            }
        }
    }
}
