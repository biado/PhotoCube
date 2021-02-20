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
using System.Configuration;
using System.Collections.Specialized;

namespace ConsoleAppForInteractingWithDatabase
{
    /// <summary>
    /// Program that parses and adds the LSC data to the database.
    /// </summary>
    class Program

    {
        private static NameValueCollection sAll = ConfigurationManager.AppSettings;

        static void Main(string[] args)
        {
            Console.WriteLine("Started up!");

            int[] N = new int[] { 50 }; // 191418 = Total number of LSC images, based on VisualConcept file.
            string[] DB = new string[] { "lsc50" };

            string resultPath = sAll.Get("resultPath");
            string experimentResult = "DB Name,Number of Images,Elapsed Time\n";

            for (int i = 0; i < N.Length; i++)
            {
                int num = N[i];
                string dbName = DB[i];

                OperatingSystem OS = Environment.OSVersion;
                PlatformID platformId = OS.Platform;
                string connectionString;

                switch (platformId)
                {
                    case PlatformID.Unix: //Mac 
                        connectionString = sAll.Get("connectionStringWithoutDB") + "Database = " + dbName + ";";
                        break;
                    case PlatformID.Win32NT: //Windows
                        string[] splitConnectionStringFormat = sAll.Get("connectionStringWithoutDB").Split("***");
                        connectionString = splitConnectionStringFormat[0] + dbName + splitConnectionStringFormat[1] +
                                           dbName + splitConnectionStringFormat[2];
                        break;
                    default:
                        throw new System.Exception("Connection String is not defined");
                }

                Console.WriteLine("Inserting " + num + " images into " + dbName + " with RefactoredLSCInserter.");

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
