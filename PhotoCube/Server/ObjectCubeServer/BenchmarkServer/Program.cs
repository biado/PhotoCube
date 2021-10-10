using System;
using BenchmarkDotNet.Running;

namespace BenchmarkServer
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkHarness>();
            Console.ReadKey();
        }
    }
}