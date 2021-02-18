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
        private NameValueCollection sAll = ConfigurationManager.AppSettings;

        public LSCDatasetInsertExperimenterRefactored(int numOfImages, string connectionString)
        {
            this.numOfImages = numOfImages;
            this.connectionString = connectionString;
            this.pathToDataset = sAll.Get("pathToLscData");
      
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
                    using (StreamReader reader = new StreamReader(pathToTagFile))
                    {
                        int fileCount = 1;
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals("") && fileCount <= numOfImages)
                        {
                            //File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
                            String filename = Path.Combine(pathToDataset, line.Split(":")[0]);
                            Console.WriteLine("Saving file: " + fileCount +
                                              " out of " + numOfImages + " files. " +
                                              "Filename: " + filename +
                                              ". (" +
                                              (((double)fileCount / (double)numOfImages) * 100).ToString("0.0") +
                                              @"%)");

                            //Loading and saving image:
                            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(filename))
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    image.SaveAsJpeg(ms); //Copy to ms

                                    //Create Cube and Photo objects:
                                    CubeObject cubeObject = DomainClassFactory.NewCubeObject(
                                        filename,
                                        FileType.Photo,
                                        DomainClassFactory.NewPhoto(
                                            ms.ToArray(),
                                            filename
                                        )
                                    );

                                    //Creating and saving thumbnail:
                                    //Thumbnails needs to be power of two in width and height to avoid extra image manipulation client side.
                                    if (image.Width > image.Height)
                                    {
                                        int destinationHeight = 1024; //1024px
                                        decimal downscaleFactor = Decimal.Parse(destinationHeight + "") /
                                                                  Decimal.Parse(image.Height + "");
                                        int newWidth = (int)(image.Width * downscaleFactor);
                                        int newHeight = (int)(image.Height * downscaleFactor);
                                        image.Mutate(i => i
                                                .Resize(newWidth, newHeight) //Scale
                                                .Crop(destinationHeight, destinationHeight) //Crop
                                        );
                                    }
                                    else
                                    {
                                        int destinationWidth = 1024; //1024px
                                        decimal downscaleFactor = Decimal.Parse(destinationWidth + "") /
                                                                  Decimal.Parse(image.Width + "");
                                        int newWidth = (int)(image.Width * downscaleFactor);
                                        int newHeight = (int)(image.Height * downscaleFactor);
                                        image.Mutate(i => i
                                                .Resize(newWidth, newHeight) //Scale
                                                .Crop(destinationWidth, destinationWidth) //Crop
                                        );
                                    }

                                    //int destinationWidth = 1024; //1024px
                                    //decimal downscaleFactor = Decimal.Parse(destinationWidth+"") / Decimal.Parse(image.Width+"");
                                    //int newWidth = (int)(image.Width * downscaleFactor);
                                    //int newHeight = (int)(image.Height * downscaleFactor);
                                    //image.Mutate(i => i
                                    //    .Resize(newWidth, newHeight));
                                    using (MemoryStream ms2 = new MemoryStream())
                                    {
                                        image.SaveAsJpeg(ms2); //Copy to ms
                                        cubeObject.Thumbnail = new Thumbnail() { Image = ms2.ToArray() };
                                    }

                                    //Save cube object: 
                                    context.CubeObjects.Add(cubeObject);
                                    context.SaveChanges();
                                }
                            }

                            fileCount++;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("File could not be read to insert the cube objects.");
                    Console.WriteLine(e.Message);
                }
            }

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
                    using (StreamReader reader = new StreamReader(pathToTagFile))
                    {
                        int lineCount = 1;
                        string line = reader.ReadLine(); // Skipping the first line
                        while ((line = reader.ReadLine()) != null && !line.Equals("") && lineCount <= numOfImages)
                        {
                            Console.WriteLine("Inserting line: " + lineCount + " out of " + numOfImages);
                            //File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
                            string[] split = line.Split(":");
                            string fileName = Path.Combine(pathToDataset, split[0]);

                            CubeObject cubeObjectFromDb = context.CubeObjects
                                .Where(co => co.Photo.FileName.Equals(fileName))
                                .Include(co => co.Photo)
                                .Include(co => co.ObjectTagRelations)
                                .FirstOrDefault();

                            int numTagPairs = (split.Length - 2) / 2;
                            //Looping over each pair of tags:
                            for (int i = 0; i < numTagPairs; i++)
                            {
                                string tagsetName = split[(i * 2) + 1];
                                string tagName = split[(i * 2) + 2];

                                Tagset tagsetFromDb;
                                if (!tagsetSeen.ContainsKey(tagsetName))
                                {
                                    tagsetFromDb = DomainClassFactory.NewTagSet(tagsetName);
                                    tagsetSeen.Add(tagsetName, tagsetFromDb);
                                    context.Tagsets.Add(tagsetFromDb);
                                    context.SaveChanges();

                                    //Also creates a tag with same name:
                                    Tag tagWithSameNameAsTagset = DomainClassFactory.NewTag(tagsetName, tagsetFromDb);
                                    //Add tag to tagset:
                                    tagWithSameNameAsTagset.Tagset = tagsetFromDb;
                                    tagsetFromDb.Tags.Add(tagWithSameNameAsTagset);
                                    //Add and update changes:
                                    tagSeen.Add(tagsetName, tagWithSameNameAsTagset);
                                    context.Tags.Add(tagWithSameNameAsTagset);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    tagsetFromDb = tagsetSeen[tagsetName];
                                }

                                //Checking if tag exists, and creates it if it doesn't exist.
                                Tag tagFromDb;
                                if (!tagSeen.ContainsKey(tagName))
                                {
                                    tagFromDb = DomainClassFactory.NewTag(tagName, tagsetFromDb);
                                    tagSeen.Add(tagName, tagFromDb);
                                    context.Tags.Add(tagFromDb);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    tagFromDb = tagSeen[tagName];
                                }

                                //Add tag to tagset if tagset doesn't have it:
                                if (!tagsetFromDb.Tags.Contains(tagFromDb))
                                {
                                    tagsetFromDb.Tags.Add(tagFromDb);
                                    tagFromDb.Tagset = tagsetFromDb;
                                    context.Update(tagsetFromDb);
                                    context.Update(tagFromDb);
                                    context.SaveChanges();
                                }

                                if (cubeObjectFromDb == null)
                                {
                                    File.AppendAllText(pathToErrorLogFile,
                                        "File " + fileName + " was not found while parsing line " + lineCount);
                                    //throw new Exception("Expected cubeobject to be in the DB already, but it isn't!");
                                }
                                else
                                {
                                    if (cubeObjectFromDb.ObjectTagRelations
                                            .FirstOrDefault(otr => otr.TagId == tagFromDb.Id) ==
                                        null) //If Cubeobject does not already have tag asscociated with it, add it
                                    {
                                        ObjectTagRelation newObjectTagRelation =
                                            DomainClassFactory.NewObjectTagRelation(tagFromDb, cubeObjectFromDb);
                                        context.ObjectTagRelations.Add(newObjectTagRelation);
                                    }
                                }
                            }

                            lineCount++;
                            context.SaveChanges(); // to save the updated cubeobject
                        }
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
                            Tagset tagsetFromDb = context.Tagsets
                                .Where(ts => ts.Name.Equals(tagsetName))
                                .Include(ts => ts.Tags)
                                .Include(ts => ts.Hierarchies)
                                .FirstOrDefault();

                            //See if hierarchy exists:
                            Hierarchy hierarchyFromDb = context.Hierarchies
                                .Include(h => h.Nodes)
                                .Where(h => h.Name.Equals(hierarchyName))
                                .FirstOrDefault();

                            //If hierarchyFromDb does not exist, create it:
                            if (hierarchyFromDb == null)
                            {
                                hierarchyFromDb = DomainClassFactory.NewHierarchy(tagsetFromDb);
                                tagsetFromDb.Hierarchies.Add(hierarchyFromDb);
                                //hierarchyFromDb.Tagset = tagsetFromDb;
                                context.Update(tagsetFromDb);
                                context.Update(hierarchyFromDb);
                                context.SaveChanges();
                            }

                            //Finding parent tag:
                            Tag parentTagFromDb = context.Tags
                                .Where(t => t.TagsetId == tagsetFromDb.Id && t.Name.Equals(parentTagName))
                                .FirstOrDefault();

                            //If parentTag does not exist, create it:
                            if (parentTagFromDb == null)
                            {
                                parentTagFromDb = DomainClassFactory.NewTag(parentTagName, tagsetFromDb);
                                tagsetFromDb.Tags.Add(parentTagFromDb);
                                context.Tags.Add(parentTagFromDb);
                                context.Update(tagsetFromDb);
                                context.SaveChanges();
                            }

                            //Finding parent node:
                            Node parentNodeFromDb = context.Nodes
                                .Include(n => n.Children)
                                .Where(n => n.HierarchyId == hierarchyFromDb.Id && n.TagId == parentTagFromDb.Id)
                                .FirstOrDefault();

                            //If parent node does not exist, create it:
                            if (parentNodeFromDb == null)
                            {
                                //Probably root node:
                                parentNodeFromDb = DomainClassFactory.NewNode(parentTagFromDb, hierarchyFromDb);
                                hierarchyFromDb.Nodes.Add(parentNodeFromDb);
                                context.Update(hierarchyFromDb);
                                context.SaveChanges();

                                if (hierarchyName.Equals(parentTagName))
                                {
                                    hierarchyFromDb.RootNodeId = parentNodeFromDb.Id;
                                    context.Update(hierarchyFromDb);
                                    context.SaveChanges();
                                }
                            }

                            //Adding child nodes:
                            for (int i = 3; i < split.Length; i++)
                            {
                                string childTagName = split[i];

                                Tag childTagFromDb = context.Tags
                                    .Where(t => t.TagsetId == tagsetFromDb.Id && t.Name.Equals(childTagName))
                                    .FirstOrDefault();

                                //If child tag does not exist, create it:
                                if (childTagFromDb == null)
                                {
                                    childTagFromDb = DomainClassFactory.NewTag(childTagName, tagsetFromDb);
                                    childTagFromDb.Tagset = tagsetFromDb;
                                    tagsetFromDb.Tags.Add(childTagFromDb);
                                    context.Update(tagsetFromDb);
                                    context.SaveChanges();
                                }

                                //Finding child node:
                                Node childNodeFromDb = context.Nodes
                                    .Include(n => n.Children)
                                    .Where(n => n.HierarchyId == hierarchyFromDb.Id && n.TagId == childTagFromDb.Id)
                                    .FirstOrDefault();

                                if (childNodeFromDb == null)
                                {
                                    Node newChildNode = DomainClassFactory.NewNode(childTagFromDb, hierarchyFromDb);
                                    parentNodeFromDb.Children.Add(newChildNode);
                                    hierarchyFromDb.Nodes.Add(newChildNode);
                                    context.Update(parentNodeFromDb);
                                    context.Update(hierarchyFromDb);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    parentNodeFromDb.Children.Add(childNodeFromDb);
                                    context.Update(parentNodeFromDb);
                                    context.SaveChanges();
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
    }
}
