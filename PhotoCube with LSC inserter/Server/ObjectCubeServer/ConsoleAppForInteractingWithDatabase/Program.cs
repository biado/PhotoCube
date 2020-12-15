using ObjectCubeServer.Models;
using ObjectCubeServer.Models.DataAccess;
using ObjectCubeServer.Models.DomainClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ObjectCubeServer;
using SixLabors.ImageSharp;

namespace ConsoleAppForInteractingWithDatabase
{
    /// <summary>
    /// Program that parses and adds the LSC data to the database.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started up!");

            int[] N = new int[] { 1000, 2000, 3000, 4000, 5000 }; // 191418 = Total number of LSC images, based on VisualConcept file.
            string[] DB = new string[] { "LSC1KRefactor", "LSC2KRefactor", "LSC3KRefactor", "LSC4KRefactor", "LSC5KRefactor" };

            string resultPath = "C:\\LSCImportExp\\Result_Doubling_No_Metadata.csv";
            string experimentResult = "DB Name,Number of Images,Elapsed Time\n";

            for (int i = 0; i < N.Length; i++)
            {
                int num = N[i];
                string dbName = DB[i];

                string connectionString = "Server = (localdb)\\mssqllocaldb; Database = " + dbName +
                                          "; Trusted_Connection = True; AttachDbFileName=C:\\Databases\\" + dbName + ".mdf";

                Console.WriteLine("Inserting " + num + " images into " + dbName);

                // Starting the timer
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                //Insert data:
                new LSCDatasetInsertExperimenterRefactored(num, connectionString).InsertLSCDataset();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Hours, ts.Minutes, ts.Seconds);

                experimentResult += string.Join(",", dbName, num, elapsedTime) + "\n";

                File.AppendAllText(resultPath, experimentResult);
                experimentResult = "";

                Console.WriteLine("Done! Inserted " + num + " images to " + dbName + " database.");
                Console.WriteLine("Took: " + elapsedTime + " in format: hh:mm:ss\n");
            }

            Console.WriteLine("Experiment results are saved at: " + resultPath);
            Console.WriteLine("Press any key to shut down.");
            Console.ReadKey();
        }
    }
}
