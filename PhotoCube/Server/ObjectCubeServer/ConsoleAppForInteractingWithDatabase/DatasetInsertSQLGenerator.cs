using ObjectCubeServer.Models.DomainClasses;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
using ObjectCubeServer.Models.HelperClasses;

namespace ConsoleAppForInteractingWithDatabase
{
    public class DatasetInsertSQLGenerator
    {
        private int numOfImages;
        private string pathToDataset;
        private string pathToTagFile;
        private string pathToErrorLogFile;
        private string resultPath;
        private bool mssqlFormat;
        private Stopwatch stopwatch;
        private int batchSize = 10000;
        private string SQLPath;
        private static string delimiter = "||";
        private NameValueCollection sAll = ConfigurationManager.AppSettings;

        private Dictionary<string, CubeObject> cubeObjects = new Dictionary<string, CubeObject>();
        private Dictionary<string, Tagset> tagsets = new Dictionary<string, Tagset>();
        private Dictionary<string, Dictionary<int, Tag>> tags = new Dictionary<string, Dictionary<int, Tag>>();

        private Dictionary<string, Dictionary<int, ObjectTagRelation>> objectTagRelations =
            new Dictionary<string, Dictionary<int, ObjectTagRelation>>();

        private Dictionary<string, Hierarchy> hierarchies = new Dictionary<string, Hierarchy>();

        private HashSet<Node> nodes = new HashSet<Node>();

        // nodes is changed from dictionary to hashset.
        // Now tagId-node is not 1:1. And no need to look up because we just create nodes. JSNode-Node is 1:1. (Tag-Node is 1:0..M)
        private Dictionary<string, TagType> tagtypes = new Dictionary<string, TagType>();
        private Dictionary<string, string> datatypes;
        private JSNode root;

        private int missingfiles;

        public DatasetInsertSQLGenerator(int numOfImages)
        {
            this.numOfImages = numOfImages;
            this.pathToDataset = sAll.Get("pathToMtbData");
            this.resultPath = sAll.Get("MtbResultPath");
            this.SQLPath = sAll.Get("MtbSQLPath");

            this.pathToTagFile = Path.Combine(pathToDataset, @sAll.Get("MtbTagFilePath"));
            this.pathToErrorLogFile = Path.Combine(pathToDataset, @sAll.Get("MtbErrorfilePath"));
            this.mssqlFormat = Convert.ToBoolean(sAll.Get("mssqlFormat"));

            File.AppendAllText(pathToErrorLogFile, "Errors goes here:\n");
            datatypes = MapDataTypestoTagTypes();
            //root = new JsonHierarchyParser().root;

            this.stopwatch = new Stopwatch();
            stopwatch.Start();
            BuildCubeObjects();
            BuildTagTypes();
            BuildTagsetsAndTags();
            //BuildHierarchiesAndNodes();
            WriteInsertStatementsToFile();
        }

        private void LogTimeToFile(string entities, int row)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Hours, ts.Minutes, ts.Seconds);
            string log = string.Join(",", entities, row, elapsedTime) + "\n";
            //Console.WriteLine(resultPath);
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

                        //File format: "FileName,,TagSet,,Tag,,TagSet,,Tag:(...)"
                        string[] split = line.Split(delimiter);
                        string filename = split[0];

                        // If Image is already in Map(Assuming no two file has the same name):
                        if (cubeObjects.ContainsKey(filename))
                        {
                            //Don't add it again.
                            Console.WriteLine("Image " + filename + " is already in the database");
                            missingfiles++;
                        }

                        String thumbnail = "";
                        if (line.Contains("thumbnail")){
                            thumbnail = split[36];
                        } else {
                            int colorindex = Array.IndexOf(split, "color") + 1; 
                            thumbnail = split[colorindex]+".jpg";
                        }
                        //string thumbnailURI = Path.Combine("Thumbnails", thumbnail);

                        CubeObject cubeObject = DomainClassFactory.NewCubeObject(
                            filename,
                            FileType.Photo,
                            thumbnail);
                            //thumbnailURI);
                        cubeObjects[filename] = cubeObject;

                        if (fileCount % batchSize == 0)
                        {
                            LogTimeToFile("CubeObject", fileCount);
                        }

                        fileCount++;
                    }
                }

                LogTimeToFile("CubeObject", fileCount - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("File could not be read to insert the cube objects.");
                Console.WriteLine(e.Message);
            }
        }

        //Not in use for LSC
        private string saveThumbnail(Image<Rgba32> image, string filename)
        {
            string thumbnailURI = Path.Combine(pathToDataset, "Thumbnails", filename);
            image.Save(thumbnailURI + ".jpg");
            return thumbnailURI;
        }

        //Not in use for LSC
        private bool resizeOriginalImageToMakeThumbnails(Image<Rgba32> image, int shortSide)
        {
            if (shortSide > 1024)
            {
                int destinationShortSide = 1024; //1024px
                double downscaleFactor = Convert.ToDouble(destinationShortSide) / image.Height;
                int newWidth = (int) (image.Width * downscaleFactor);
                int newHeight = (int) (image.Height * downscaleFactor);
                image.Mutate(i => i
                        .Resize(newWidth, newHeight) //Scale
                        .Crop(destinationShortSide, destinationShortSide) //Crop
                );
                return true;
            }

            return false;
        }

        private Dictionary<string, string> MapDataTypestoTagTypes()
        {
            return new Dictionary<string, string>()
            {
                {"Entity", "alphanumerical"},
                {"Location name", "alphanumerical"},
                {"Timestamp", "timestamp"},
                {"Time", "time"},
                {"Date", "date"},
                {"Timezone", "alphanumerical"},
                {"Elevation", "numerical"},
                {"Speed", "numerical"},
                {"Heart", "numerical"},
                {"Calories", "numerical"},
                {"Activity type", "alphanumerical"},
                {"Steps", "numerical"},
                {"Day of week (number)", "numerical"},
                {"Day of week (string)", "alphanumerical"},
                {"Day within month", "numerical"},
                {"Day within year", "numerical"},
                {"Month (number)", "numerical"},
                {"Month (string)", "alphanumerical"},
                {"Year", "numerical"},
                {"Hour", "numerical"},
                {"Minute", "numerical"}
            };
        }

        private void BuildTagTypes()
        {
            Console.WriteLine("Building TagTypes.");

            foreach (var description in datatypes.Values.Distinct())
            {
                tagtypes.Add
                (
                    description, DomainClassFactory.NewTagType(description)
                );
            }
        }

        private Tag CreateNewTag(string description, Tagset tagset, string tagName)
        {
            Console.WriteLine(description);
            Console.WriteLine(tagset);
            Console.WriteLine(tagName);
            TagType tagType = tagtypes[description];
            switch (description)
            {
                case "alphanumerical":
                    return DomainClassFactory.NewAlphanumericalTag(tagType, tagset, tagName);
                case "numerical":
                    return DomainClassFactory.NewNumericalTag(tagType, tagset, int.Parse(tagName));
                case "timestamp":
                    DateTime timestamp = DateTime.ParseExact(tagName, "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    return DomainClassFactory.NewTimestampTag(tagType, tagset, timestamp);
                case "time":
                    TimeSpan time = DateTime
                        .ParseExact(tagName, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
                    return DomainClassFactory.NewTimeTag(tagType, tagset, time);
                case "date":
                    DateTime date = DateTime.ParseExact(tagName, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture);
                    return DomainClassFactory.NewDateTag(tagType, tagset, date);
                default: return null;
            }
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

                        //Console.WriteLine(line);
                        //File format: "FileName,,TagSet,,Tag,,TagSet,,Tag:(...)"
                        string[] split = line.Split(delimiter);
                        string fileName = split[0];
                        
                        //Console.WriteLine(fileName);

                        CubeObject cubeObject = cubeObjects[fileName];

                        int numTagPairs = (split.Length - 1) / 2;
                        //Looping over each pair of tags:
                        for (int i = 0; i < numTagPairs; i++)
                        {
                            string tagsetName = split[(i * 2) + 1];
                            string tagName = split[(i * 2) + 2];

                            Tagset tagset;
                            if (!tagsets.ContainsKey(tagsetName))
                            {
                                tagset = createNewTagset(tagsetName, tagsets);
                            }
                            else
                            {
                                tagset = tagsets[tagsetName];
                            }

                            if (datatypes.ContainsKey(tagsetName))
                            {
                                //Checking if tag exists, and creates it if it doesn't exist.
                                Tag tag;
                                string tagtype = datatypes[tagsetName];
                                Dictionary<int, Tag> tagList;
                                if (!tags.ContainsKey(tagName))
                                {
                                    tag = CreateNewTag(tagtype, tagset, tagName);
                                    tagList = new Dictionary<int, Tag>();
                                    tagList[tagset.Id] = tag;
                                    tags[tagName] = tagList;
                                }
                                else
                                {
                                    tagList = tags[tagName];
                                    if (!tagList.ContainsKey(tagset.Id))
                                    {
                                        tag = CreateNewTag(tagtype, tagset, tagName);
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
                        }

                        if (lineCount % batchSize == 0)
                        {
                            LogTimeToFile("Tagset & Tag", lineCount);
                        }

                        lineCount++;
                    }
                }

                LogTimeToFile("Tagset & Tag", lineCount - 1);
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
        /// Create hierarchies and nodes while parsing the JSNode tree.
        /// </summary>
        private void BuildHierarchiesAndNodes()
        {
            Console.WriteLine("Building Hierarchies and nodes.");
            int rootnodeCount = 0;
            foreach (JSNode rootChild in root.children)
            {
                string tagsetName = rootChild.name;
                string hierarchyName = rootChild.name;

                // Finding Tagset;
                Tagset tagset = tagsets[tagsetName];

                Hierarchy hierarchy;
                // If the corresponding hierarchy does not exist, create it:
                if (!hierarchies.ContainsKey(hierarchyName))
                {
                    hierarchy = DomainClassFactory.NewHierarchy(tagset, hierarchyName);
                    hierarchies[hierarchyName] = hierarchy;
                }
                else
                {
                    hierarchy = hierarchies[hierarchyName];
                }

                BuildHierarchiesAndNodes_Recursive(rootChild, tagset, hierarchy, null);
                rootnodeCount++;
                Console.WriteLine("Hierarchy & Node rootnode: " + rootnodeCount);
                LogTimeToFile("Hierarchy & Node", rootnodeCount);
            }
        }

        private void BuildHierarchiesAndNodes_Recursive(JSNode currentJSNode, Tagset tagset, Hierarchy hierarchy,
            Node parentNode)
        {
            //handle when parent is null - eg. ROOT(-1)
            if (currentJSNode.parentJSNode != null)
            {
                //Finding parent tag:
                string parentTagName = currentJSNode.parentJSNode.name;
                Tag parentTag;
                Dictionary<int, Tag> tagList;

                //If parentTag does not exist, create it:
                if (!tags.ContainsKey(parentTagName))
                {
                    parentTag = DomainClassFactory.NewAlphanumericalTag(tagtypes["alphanumerical"], tagset, parentTagName);
                    tagList = new Dictionary<int, Tag>();
                    tagList[parentTag.TagsetId] = parentTag;
                    tags[parentTagName] = tagList;
                }
                else
                {
                    tagList = tags[parentTagName];
                    if (!tagList.ContainsKey(tagset.Id))
                    {
                        parentTag = DomainClassFactory.NewAlphanumericalTag(tagtypes["alphanumerical"], tagset,
                            parentTagName);
                        tagList[parentTag.TagsetId] = parentTag;
                        tags[parentTagName] = tagList;
                    }
                    else
                    {
                        parentTag = tagList[tagset.Id];
                    }
                }
            }

            // Find tag for current node:
            string currentTagName = currentJSNode.name;
            Tag currentTag;
            Dictionary<int, Tag> childTagList; // (tagsetId, tags)

            //If child tag does not exist, create it:
            if (!tags.ContainsKey(currentTagName))
            {
                currentTag =
                    DomainClassFactory.NewAlphanumericalTag(tagtypes["alphanumerical"], tagset, currentTagName);
                childTagList = new Dictionary<int, Tag>();
                childTagList[currentTag.TagsetId] = currentTag;
                tags[currentTagName] = childTagList;
            }
            else
            {
                childTagList = tags[currentTagName];
                if (!childTagList.ContainsKey(tagset.Id))
                {
                    currentTag =
                        DomainClassFactory.NewAlphanumericalTag(tagtypes["alphanumerical"], tagset, currentTagName);
                    childTagList[currentTag.TagsetId] = currentTag;
                    tags[currentTagName] = childTagList;
                }
                else
                {
                    currentTag = childTagList[tagset.Id];
                }
            }

            // Create current node and add to nodes set:
            Node currentNode = DomainClassFactory.NewNode(currentTag, hierarchy);
            if (parentNode != null)
            {
                currentNode.ParentNodeId = parentNode.Id;
            }

            nodes.Add(currentNode);

            // Add RootNodeId to hierarchy (null check)
            // If parentNode == null, currentNode is RootNode
            if (parentNode == null)
            {
                // It is a rootNode
                hierarchy.RootNodeId = currentNode.Id;
            }

            // Recursive call continues until it gets to the leaves of the json tree.
            if (currentJSNode.children != null)
            {
                foreach (JSNode child in currentJSNode.children)
                {
                    BuildHierarchiesAndNodes_Recursive(child, tagset, hierarchy, currentNode);
                }
            }
        }

        private void WriteInsertStatementsToFile()
        {
            Console.WriteLine(SQLPath);
            Console.WriteLine("duplicate sptify uri's "+missingfiles);
            // insert into [tableName]
            // (column1, column2, ..)
            // values
            // (value1, value2, ..);
            int insertCount = 0;
            
            //SQL server specific requirement
            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET QUOTED_IDENTIFIER ON\nGO\nSET ANSI_NULLS ON\nGO\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT cubeobjects ON;\nGO\n");
            }
            else
            {
                File.AppendAllText(SQLPath,"SELECT NOW();\n\\set AUTOCOMMIT off\nBEGIN;\n");
            }

            //Insert all CubeObjects
            foreach (var co in cubeObjects.Values)
            {
                string insertStatement = "INSERT INTO cubeobjects(id, file_uri, file_type, thumbnail_uri) VALUES(" +
                                         co.Id + ",'" + co.FileURI + "'," + (int) co.FileType + ",'" + co.ThumbnailURI +
                                         "'); \n";
                File.AppendAllText(SQLPath, insertStatement);
                insertCount++;
                if (insertCount % 100 == 0 && mssqlFormat)
                {
                    File.AppendAllText(SQLPath, "GO\n");
                } else if (insertCount % 100 == 0)
                {
                    File.AppendAllText(SQLPath,"COMMIT;\n");
                    File.AppendAllText(SQLPath,"BEGIN;\n");
                }
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT cubeobjects OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tagsets ON;\nGO\n");
            }
            else
            {
                File.AppendAllText(SQLPath, "COMMIT;\n");
                File.AppendAllText(SQLPath,"BEGIN;\n");
            }
            

            //Insert all Tagsets
            foreach (var ts in tagsets.Values)
            {
                string insertStatement = "INSERT INTO tagsets(id, name) VALUES(" + ts.Id + ",'" + ts.Name + "'); \n";
                File.AppendAllText(SQLPath, insertStatement);
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tagsets OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tag_types ON;\n");
            }
            

            //Insert all TagTypes
            foreach (var tt in tagtypes.Values)
            {
                //Console.WriteLine(tt.Id);
                string insertStatement = "INSERT INTO tag_types(id, description) VALUES(" + tt.Id + ",'" +
                                         tt.Description + "'); \n";
                File.AppendAllText(SQLPath, insertStatement);
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tag_types OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tags ON;\n");
            }

            //Insert all Tags 
            insertCount = 0;
            foreach (var list in tags.Values)
            {
                foreach (var t in list.Values)
                {
                    //Insert the tag first to avoid violation of FK constraint
                    string insertStatement = "INSERT INTO tags(id, tagtype_id, tagset_id) VALUES(" + t.Id + "," +
                                             t.TagTypeId + "," + t.TagsetId + "); \n";
                    switch (t)
                    {
                        //Insert typed tag next, including a replicate of tagset_id
                        case AlphanumericalTag at:
                            insertStatement += "INSERT INTO alphanumerical_tags(id, name, tagset_id) VALUES(" + at.Id +
                                               ",'" + at.Name.Replace("'", "''") + "'," + at.TagsetId + "); \n";
                            break;
                        case NumericalTag nt:
                            insertStatement += "INSERT INTO numerical_tags(id, name, tagset_id) VALUES(" + nt.Id + "," +
                                               nt.Name + "," + nt.TagsetId + "); \n";
                            break;
                        case TimestampTag tst:
                            String timestamp = tst.Name.ToString("yyyy-MM-dd HH:mm:ss").Replace('.',':');
                            insertStatement += "INSERT INTO timestamp_tags(id, name, tagset_id) VALUES(" + tst.Id + ",'" +
                                               timestamp + "'," + tst.TagsetId + "); \n";
                            break;
                        case DateTag dt:
                            String date = dt.Name.ToString("yyyy-MM-dd");
                            insertStatement += "INSERT INTO date_tags(id, name, tagset_id) VALUES(" + dt.Id + ",'" +
                                               date + "'," + dt.TagsetId + "); \n";
                            break;
                        case TimeTag tt:
                            insertStatement += "INSERT INTO time_tags(id, name, tagset_id) VALUES(" + tt.Id + ",'" +
                                               tt.Name.ToString() + "'," + tt.TagsetId + "); \n";
                            break;
                    }

                    File.AppendAllText(SQLPath, insertStatement);
                    insertCount++;
                    if (insertCount % 100 == 0 && mssqlFormat)
                    {
                        File.AppendAllText(SQLPath, "GO\n");
                    } else if (insertCount % 100 == 0)
                    {
                        File.AppendAllText(SQLPath,"COMMIT;\n");
                        File.AppendAllText(SQLPath,"BEGIN;\n");
                    }
                }
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT tags OFF;\n");
            }

            //Insert all ObjectTagRelations (IDENTITY_INSERT should be off)
            insertCount = 0;
            foreach (var co in objectTagRelations.Values)
            {
                foreach (var otr in co.Values)
                {
                    string insertStatement = "INSERT INTO objecttagrelations(object_id, tag_id) VALUES(" +
                                             otr.ObjectId + "," + otr.TagId + "); \n";
                    File.AppendAllText(SQLPath, insertStatement);
                    insertCount++;
                    if (insertCount % 100 == 0 && mssqlFormat)
                    {
                        File.AppendAllText(SQLPath, "GO\n");
                    } else if (insertCount % 100 == 0)
                    {
                        File.AppendAllText(SQLPath,"COMMIT;\n");
                        File.AppendAllText(SQLPath,"BEGIN;\n");
                    }
                }
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT hierarchies ON;\nGO\n");
            }
            else
            {
                File.AppendAllText(SQLPath, "COMMIT;\n");
                File.AppendAllText(SQLPath, "BEGIN;\n");
            }

            //Insert all Hierarchies
            foreach (var h in hierarchies.Values)
            {
                string insertStatement = "INSERT INTO hierarchies(id, name, tagset_Id, rootnode_id) VALUES(" + h.Id +
                                         ",'" + h.Name + "'," + h.TagsetId + "," + h.RootNodeId + "); \n";
                File.AppendAllText(SQLPath, insertStatement);
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT hierarchies OFF;\n");
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT nodes ON;\nGO\n");
            }
            else
            {
                File.AppendAllText(SQLPath, "COMMIT;\n");
                File.AppendAllText(SQLPath, "BEGIN;\n");
            }

            //Insert all Nodes first without setting parent node to avoid violating FK constraint
            insertCount = 0;
            foreach (var n in nodes)
            {
                string insertStatement = "INSERT INTO nodes(id, tag_id, hierarchy_id) VALUES(" + n.Id + "," + n.TagId +
                                         "," + n.HierarchyId + "); \n";
                File.AppendAllText(SQLPath, insertStatement);
                insertCount++;
                if (insertCount % 100 == 0 && mssqlFormat)
                {
                    File.AppendAllText(SQLPath, "GO\n");
                } else if (insertCount % 100 == 0)
                {
                    File.AppendAllText(SQLPath, "COMMIT;\n");
                    File.AppendAllText(SQLPath, "COMMIT;\n");
                }
            }

            //Upate all Nodes and set parent node
            foreach (var n in nodes)
            {
                //Root nodes should have no parent node
                if (n.ParentNodeId != null)
                {
                    string insertStatement = "UPDATE nodes SET parentnode_id=" + n.ParentNodeId + " WHERE id=" + n.Id +
                                             "; \n";
                    File.AppendAllText(SQLPath, insertStatement);
                }
            }

            if (mssqlFormat)
            {
                File.AppendAllText(SQLPath, "SET IDENTITY_INSERT nodes OFF;\nGO");
            }
            else
            {
                File.AppendAllText(SQLPath, "COMMIT;\n");
                File.AppendAllText(SQLPath,"\\set AUTOCOMMIT on\n");
                File.AppendAllText(SQLPath,"SELECT NOW();\n");
            }
        }
    }
}
