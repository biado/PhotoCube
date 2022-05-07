using System;
using System.Diagnostics;
using System.IO;
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

            int[] N = new int[] { 1000000 }; // 191524 = Total number of LSC images, based on lsc2020.txt file.

            string resultPath = sAll.Get("resultPath");
            string experimentResult = "DB Name,Number of Images,Elapsed Time\n";

            for (int i = 0; i < N.Length; i++)
            {
                int num = N[i];

                Console.WriteLine("Generating SQL INSERT queries for up to " + num + " images using DataInsertSQLGenerator.");

                // Starting the timer
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                //Insert data:
                new DatasetInsertSQLGenerator(num);

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Hours, ts.Minutes, ts.Seconds);

                experimentResult += string.Join(",", "Total time for GenerateSQLQueries", num, elapsedTime) + "\n";

                File.AppendAllText(resultPath, experimentResult);
                experimentResult = "";

                Console.WriteLine("Done! Generated SQL INSERT queries for " + num + " images.");
                Console.WriteLine("Took: " + elapsedTime + " in format: hh:mm:ss\n");

            }

            Console.WriteLine("Experiment results are saved at: " + resultPath);
            Console.WriteLine("Press any key to shut down.");
            Console.ReadKey();
        }
    }
}
