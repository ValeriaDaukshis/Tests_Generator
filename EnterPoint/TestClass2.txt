using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestsGeneratorLib.DataStructures;

namespace TestsGeneratorLib
{
    public class TestsGenerator
    {
        private TestsGeneratorConfig _config;

        public TestsGenerator(TestsGeneratorConfig config)
        {
            _config = config;
        }

        public Task Generate(string path)
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };//цель получает уведомление о завершении/сбое
            ExecutionDataflowBlockOptions readBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config._maxReadTasksCount
            };
            ExecutionDataflowBlockOptions processBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config._maxProcessingTasksCount
            };
            ExecutionDataflowBlockOptions writeBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config._maxWriteTasksCount
            };

            TransformBlock<string, string> readBlock = new TransformBlock<string, string>(new Func<string, Task<string>>(AsyncReader.Read), readBlockOptions);
            //TransformBlock<> processBlock;
            //ActionBlock<> writeBlock;

        }

        private void GenerateTestClass(string sourceCode)
        {
            ParsingResultBuilder builder = new ParsingResultBuilder();
            ParsingResultStructure result = builder.GetResult(sourceCode);
            //here we can genearte test class with result
        }
		
		public string Method1()
		{
			//do simethong
		}
		
		public int Method2()
		{
			//do something
		}

    }
}
