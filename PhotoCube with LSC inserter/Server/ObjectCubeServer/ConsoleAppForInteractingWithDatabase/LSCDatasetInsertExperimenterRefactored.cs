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
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace ConsoleAppForInteractingWithDatabase
{
    public class LSCDatasetInsertExperimenterRefactored
    {
        private int numOfImages;
        private string connectionString;
        private string pathToDataset;
        private string pathToTagFile;
        private string pathToHierarchiesFile;
        private string pathToErrorLogFile;
        private Stopwatch stopwatch;
        private string resultSCPath;
        private NameValueCollection sAll = ConfigurationManager.AppSettings;
        private int batchSize = 1000;

        public LSCDatasetInsertExperimenterRefactored(int numOfImages, string connectionString)
        {
            this.numOfImages = numOfImages;
            this.connectionString = connectionString;
            this.pathToDataset = sAll.Get("pathToLscData");
            this.resultSCPath = sAll.Get("resultSCPath");

            this.pathToTagFile = Path.Combine(pathToDataset, @sAll.Get("LscTagFilePath"));
            this.pathToHierarchiesFile = Path.Combine(pathToDataset, @sAll.Get("LscHierarchiesFilePath"));
            this.pathToErrorLogFile = Path.Combine(pathToDataset, @sAll.Get("LscErrorfilePath"));

            File.AppendAllText(pathToErrorLogFile, "Errors goes here:\n");
        }

        public void InsertLSCDataset()
        {
            var insertCubeObjects = true;
            var insertTags = true;
            var insertHierarchies = true;

            if (insertCubeObjects)
            {
                InsertCubeObjects();
            }
            else
            {
                Console.WriteLine("Skipping Photos");
            }

            if (insertTags)
            {
                stopwatch = new Stopwatch();
                InsertTags();
            }
            else
            {
                Console.WriteLine("Skipping Tags");
            }

            if (insertHierarchies)
            {
                InsertHierarchies();
            }
            else
            {
                Console.WriteLine("Skipping Hierarchies");
            }

            Console.WriteLine("Done with inserting LSC Dataset of size " + numOfImages);
        }


        /// <summary>
        /// Parses and inserts cube objects, photos and thumbnails.
        /// </summary>
        private void InsertCubeObjects()
        {
            Console.WriteLine("Inserting Cube Objects:");
            using (var context = new ObjectContext(connectionString))
            {
                try
                {
                    int fileCount = 1;
                    int insertCount = 0;
                    using (StreamReader reader = new StreamReader(pathToTagFile))
                    {
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals("") && fileCount <= numOfImages)
                        {
                            //File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
                            string filename = line.Split(":")[0];
                            string filepath = Path.Combine(pathToDataset, filename);
                            Console.WriteLine("Saving file: " + fileCount +
                                              " out of " + numOfImages + " files. " +
                                              "Filename: " + filename +
                                              ". (" +
                                              (((double)fileCount / (double)numOfImages) * 100).ToString("0.0") +
                                              @"%)");

                            // If Image is already in database(Assuming no two file has the same name):
                            if (context.CubeObjects
                                .FirstOrDefault(co => co.FileURI.Equals(filename)) != null)
                            {
                                //Don't add it again.
                                Console.WriteLine("Image " + filename + " is already in the database");
                            }

                            //Loading and saving image:
                            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(filepath))
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
                                    context.CubeObjects.Add(cubeObject);
                                }
                            }
                            fileCount++;
                            insertCount++;
                            if (insertCount == batchSize)
                            {
                                //Save cube object: 
                                context.SaveChanges();
                                insertCount = 0;
                            }
                        }
                    }

                    if (insertCount != 0)
                    {
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("File could not be read to insert the cube objects.");
                    Console.WriteLine(e.Message);
                }
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
        private void InsertTags()
        {
            Console.WriteLine("Inserting TagsSets and Tags:");

            using (var context = new ObjectContext(connectionString))
            {
                var tagsetSeen = new Dictionary<string, Tagset>();
                var tagSeen = new Dictionary<string, Tag>();
                try
                {
                    int lineCount = 1;
                    int insertCount = 0;
                    using (StreamReader reader = new StreamReader(pathToTagFile))
                    {
                        string experimentResult = "Rows,Elapsed Time\n";

                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals("") && lineCount <= numOfImages)
                        {
                            Console.WriteLine("Inserting line: " + lineCount + " out of " + numOfImages);
                            //File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
                            string[] split = line.Split(":");
                            string fileName = split[0];

                            CubeObject cubeObjectFromDb = selectCubeObjectWithFilenameOTRelations(context, fileName);

                            int numTagPairs = (split.Length - 2) / 2;
                            //Looping over each pair of tags:
                            for (int i = 0; i < numTagPairs; i++)
                            {
                                string tagsetName = split[(i * 2) + 1];
                                string tagName = split[(i * 2) + 2];

                                Tagset tagsetFromDb;
                                if (!tagsetSeen.ContainsKey(tagsetName))
                                {
                                    tagsetFromDb = createNewTagsetAndSaveInDB(tagsetName, tagsetSeen, context);

                                    //Also creates a tag with same name:
                                    Tag tagWithSameNameAsTagset = DomainClassFactory.NewTag(tagsetName, tagsetFromDb);
                                    //Add tag to tagset:
                                    addTagAndTagsetToEachOther(tagWithSameNameAsTagset, tagsetFromDb);
                                    //Add and update changes:
                                    updateTagMapAndSaveChangedTagAndTagSetInDB(tagSeen, tagsetName, tagWithSameNameAsTagset, context);
                                }
                                else
                                {
                                    tagsetFromDb = tagsetSeen[tagsetName];
                                }

                                //Checking if tag exists, and creates it if it doesn't exist.
                                Tag tagFromDb;
                                if (!tagSeen.ContainsKey(tagName))
                                {
                                    tagFromDb = createNewTagAndSaveInDB(tagName, tagsetFromDb, context, tagSeen);
                                }
                                else
                                {
                                    tagFromDb = tagSeen[tagName];
                                }

                                //Add tag to tagset if tagset doesn't have it:
                                if (!tagsetFromDb.Tags.Contains(tagFromDb))
                                {
                                    addTagAndTagsetToEachOther(tagFromDb, tagsetFromDb);
                                    //updateContextAndSaveInDB(context, tagsetFromDb, tagFromDb);
                                }

                                if (cubeObjectFromDb == null)
                                {
                                    File.AppendAllText(pathToErrorLogFile,
                                        "File " + fileName + " was not found while parsing line " + lineCount);
                                    //throw new Exception("Expected cubeobject to be in the DB already, but it isn't!");
                                }
                                else
                                {
                                    if (!containsTagInObjectTagRelation(cubeObjectFromDb, tagFromDb)) 
                                        //If Cubeobject does not already have tag asscociated with it, add it
                                    {
                                        createNewObjectTagRelationAndAddToContext(tagFromDb, cubeObjectFromDb, context);
                                    }
                                }
                            }

                            lineCount++;
                            insertCount++;
                            if (insertCount == batchSize)
                            {
                                stopwatch.Start();
                                context.SaveChanges(); // to save the updated cubeobject
                                TimeSpan ts = stopwatch.Elapsed;
                                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                                    ts.Hours, ts.Minutes, ts.Seconds);
                                stopwatch.Stop();

                                experimentResult += string.Join(",",lineCount-1, elapsedTime) + "\n";
                                File.AppendAllText(resultSCPath, experimentResult);
                                experimentResult = "";
                                insertCount = 0;
                            }
                        }
                    }

                    if (insertCount != 0)
                    {
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("File could not be read to insert the tags.");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException.Message);
                }
            }
        }

        private void createNewObjectTagRelationAndAddToContext(Tag tagFromDb, CubeObject cubeObjectFromDb, ObjectContext context)
        {
            ObjectTagRelation newObjectTagRelation =
                DomainClassFactory.NewObjectTagRelation(tagFromDb, cubeObjectFromDb);
            context.ObjectTagRelations.Add(newObjectTagRelation);
        }

        private bool containsTagInObjectTagRelation(CubeObject cubeObjectFromDb, Tag tagFromDb)
        {
            return cubeObjectFromDb.ObjectTagRelations
                       .FirstOrDefault(otr => otr.TagId == tagFromDb.Id) 
                   != null;
        }

        private void updateContextAndSaveInDB(ObjectContext context, Tagset tagsetFromDb, Tag tagFromDb)
        {
            context.Update(tagsetFromDb);
            context.Update(tagFromDb);
            //context.SaveChanges();
        }

        private Tag createNewTagAndSaveInDB(string tagName, Tagset tagsetFromDb, ObjectContext context, Dictionary<string, Tag> tagSeen)
        {
            Tag tagFromDb = DomainClassFactory.NewTag(tagName, tagsetFromDb);
            tagSeen.Add(tagName, tagFromDb);
            context.Tags.Add(tagFromDb);
            //context.SaveChanges();
            return tagFromDb;
        }

        private void updateTagMapAndSaveChangedTagAndTagSetInDB(Dictionary<string, Tag> tagSeen, string tagsetName, Tag tagWithSameNameAsTagset, ObjectContext context)
        {
            tagSeen.Add(tagsetName, tagWithSameNameAsTagset);
            context.Tags.Add(tagWithSameNameAsTagset);
            //context.SaveChanges();
        }

        private void addTagAndTagsetToEachOther(Tag tag, Tagset tagset)
        {
            tag.Tagset = tagset;
            tagset.Tags.Add(tag);
        }

        private Tagset createNewTagsetAndSaveInDB(string tagsetName, Dictionary<string, Tagset> tagsetSeen, ObjectContext context)
        {
            Tagset tagsetFromDb = DomainClassFactory.NewTagSet(tagsetName);
            tagsetSeen.Add(tagsetName, tagsetFromDb);
            context.Tagsets.Add(tagsetFromDb);
            //context.SaveChanges();
            return tagsetFromDb;
        }

        private CubeObject selectCubeObjectWithFilenameOTRelations(ObjectContext context, string fileName)
        {
            CubeObject cubeObjectFromDb = context.CubeObjects
                .Where(co => co.FileURI.Equals(fileName))
                .Include(co => co.ObjectTagRelations)
                .FirstOrDefault();
            return cubeObjectFromDb;
        }

        /// <summary>
        /// Inserts hierarchies
        /// </summary>
        private void InsertHierarchies()
        {
            Console.WriteLine("Inserting Hierarchies:");

            using (var context = new ObjectContext(connectionString))
            {
                var rootNodes = new Dictionary<string, Node>();
                try
                {
                    using (StreamReader reader = new StreamReader(pathToHierarchiesFile))
                    {
                        int lineCount = 1;
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals(""))
                        {
                            Console.WriteLine("Inserting hierarchy line number: " + lineCount);

                            //File format: TagsetName:HierarchyName:ParrentTag:ChildTag:ChildTag:ChildTag:(...)
                            string[] split = line.Split(":");
                            string tagsetName = split[0];
                            string hierarchyName = split[1];
                            string parentTagName = split[2];

                            //Finding tagset:
                            Tagset tagsetFromDb = selectTagsetWithTagsetNameAndTagsAndHierarchies(context, tagsetName);

                            //See if hierarchy exists:
                            Hierarchy hierarchyFromDb = selectHierarchyWithNodesAndHierarchyName(context, hierarchyName);

                            //If hierarchyFromDb does not exist, create it:
                            if (hierarchyFromDb == null)
                            {
                                hierarchyFromDb = createNewHierarchyAndSaveInDB(tagsetFromDb, context);
                            }

                            //Finding parent tag:
                            Tag parentTagFromDb = selectTagWithTagsetIdAndTagName(context, tagsetFromDb, parentTagName);

                            //If parentTag does not exist, create it:
                            if (parentTagFromDb == null)
                            {
                                parentTagFromDb = createNewParentTagAndSaveInDB(parentTagName, tagsetFromDb, context);
                            }

                            //Finding parent node:
                            Node parentNodeFromDb = selectNodeWithChildrenAndHierarchyIdAndTagId(hierarchyFromDb, parentTagFromDb, context);

                            //If parent node does not exist, create it:
                            if (parentNodeFromDb == null)
                            {
                                //Probably root node:
                                parentNodeFromDb = createParentNodeAndSaveInDB(parentTagFromDb, hierarchyFromDb, context);

                                if (hierarchyName.Equals(parentTagName))
                                {
                                    addRootNodeIdToHierarchyAndSaveInDB(hierarchyFromDb, parentNodeFromDb, context);
                                }
                            }

                            //Adding child nodes:
                            for (int i = 3; i < split.Length; i++)
                            {
                                string childTagName = split[i];

                                Tag childTagFromDb = selectTagWithTagsetIdAndTagName(context, tagsetFromDb, childTagName);

                                //If child tag does not exist, create it:
                                if (childTagFromDb == null)
                                {
                                    childTagFromDb = createNewChildTagAndSaveInDB(childTagName, tagsetFromDb, context);
                                }

                                //Finding child node:
                                Node childNodeFromDb = selectNodeWithChildrenAndHierarchyIdAndTagId(hierarchyFromDb, childTagFromDb, context);

                                if (childNodeFromDb == null)
                                {
                                    createNewChildNodeAndSaveInDB(childTagFromDb, hierarchyFromDb, parentNodeFromDb, context);
                                }
                                else
                                {
                                    addChildNodeToParentNodeAndSaveInDB(parentNodeFromDb, childNodeFromDb, context);
                                }
                            }

                            lineCount++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("File could not be read to insert the hierarchies.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void addChildNodeToParentNodeAndSaveInDB(Node parentNodeFromDb, Node childNodeFromDb, ObjectContext context)
        {
            parentNodeFromDb.Children.Add(childNodeFromDb);
            context.Update(parentNodeFromDb);
            context.SaveChanges();
        }

        private void createNewChildNodeAndSaveInDB(Tag childTagFromDb, Hierarchy hierarchyFromDb, Node parentNodeFromDb, ObjectContext context)
        {
            Node newChildNode = DomainClassFactory.NewNode(childTagFromDb, hierarchyFromDb);
            parentNodeFromDb.Children.Add(newChildNode);
            hierarchyFromDb.Nodes.Add(newChildNode);
            context.Update(parentNodeFromDb);
            context.Update(hierarchyFromDb);
            context.SaveChanges();
        }

        private Tag createNewChildTagAndSaveInDB(string childTagName, Tagset tagsetFromDb, ObjectContext context)
        {
            Tag childTagFromDb = DomainClassFactory.NewTag(childTagName, tagsetFromDb);
            childTagFromDb.Tagset = tagsetFromDb;
            tagsetFromDb.Tags.Add(childTagFromDb);
            context.Update(tagsetFromDb);
            context.SaveChanges();
            return childTagFromDb;
        }

        private void addRootNodeIdToHierarchyAndSaveInDB(Hierarchy hierarchyFromDb, Node parentNodeFromDb, ObjectContext context)
        {
            hierarchyFromDb.RootNodeId = parentNodeFromDb.Id;
            context.Update(hierarchyFromDb);
            context.SaveChanges();
        }

        private Node createParentNodeAndSaveInDB(Tag parentTagFromDb, Hierarchy hierarchyFromDb, ObjectContext context)
        {
            Node parentNodeFromDb = DomainClassFactory.NewNode(parentTagFromDb, hierarchyFromDb);
            hierarchyFromDb.Nodes.Add(parentNodeFromDb);
            context.Update(hierarchyFromDb);
            context.SaveChanges();
            return parentNodeFromDb;
        }

        private Node selectNodeWithChildrenAndHierarchyIdAndTagId(Hierarchy hierarchyFromDb, Tag tagFromDb, ObjectContext context)
        {
            return context.Nodes
                .Include(n => n.Children)
                .Where(n => n.HierarchyId == hierarchyFromDb.Id && n.TagId == tagFromDb.Id)
                .FirstOrDefault();
        }

        private Tag createNewParentTagAndSaveInDB(string parentTagName, Tagset tagsetFromDb, ObjectContext context)
        {
            Tag parentTagFromDb = DomainClassFactory.NewTag(parentTagName, tagsetFromDb);
            tagsetFromDb.Tags.Add(parentTagFromDb);
            context.Tags.Add(parentTagFromDb);
            context.Update(tagsetFromDb);
            context.SaveChanges();
            return parentTagFromDb;
        }

        private Tag selectTagWithTagsetIdAndTagName(ObjectContext context, Tagset tagsetFromDb, string tagName)
        {
            return context.Tags
                .Where(t => t.TagsetId == tagsetFromDb.Id && t.Name.Equals(tagName))
                .FirstOrDefault();
        }

        private Hierarchy createNewHierarchyAndSaveInDB(Tagset tagsetFromDb, ObjectContext context)
        {
            Hierarchy hierarchyFromDb = DomainClassFactory.NewHierarchy(tagsetFromDb);
            tagsetFromDb.Hierarchies.Add(hierarchyFromDb);
            //hierarchyFromDb.Tagset = tagsetFromDb;
            context.Update(tagsetFromDb);
            context.Update(hierarchyFromDb);
            context.SaveChanges();
            return hierarchyFromDb;
        }

        private Hierarchy selectHierarchyWithNodesAndHierarchyName(ObjectContext context, string hierarchyName)
        {
            return context.Hierarchies
                .Include(h => h.Nodes)
                .Where(h => h.Name.Equals(hierarchyName))
                .FirstOrDefault();
        }

        private Tagset selectTagsetWithTagsetNameAndTagsAndHierarchies(ObjectContext context, string tagsetName)
        {
            return context.Tagsets
                .Where(ts => ts.Name.Equals(tagsetName))
                .Include(ts => ts.Tags)
                .Include(ts => ts.Hierarchies)
                .FirstOrDefault();
        }
    }
}
