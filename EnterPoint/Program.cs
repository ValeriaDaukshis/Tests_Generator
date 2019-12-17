using System;
using System.Collections.Generic;
using System.IO;
using GeneratorLib;

namespace EnterPoint
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            PipelineConfiguration configuration = new PipelineConfiguration(3, 3, 9);
            List<string> fileNames = new List<string>();
            fileNames.Add("TestClass1.txt");
            fileNames.Add("TestClass2.txt");
            Pipeline generator = new Pipeline(configuration);
            string outputFileName = "Output";
            generator.CreatePipeline(fileNames, outputFileName).Wait();
            Console.WriteLine("The generated tests was created");
        }
    }
}