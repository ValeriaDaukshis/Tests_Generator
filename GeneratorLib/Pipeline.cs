using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GeneratorLib.Structure;

namespace GeneratorLib
{
    public class Pipeline
    {
        private readonly PipelineConfiguration _configuration;
        private static FileReader _reader;
        private static FileWriter _writer;

        public Pipeline(PipelineConfiguration configuration)
        {
            this._configuration = configuration;
            _reader = new FileReader();
            _writer = new FileWriter();
        }
        
        public async Task CreatePipeline(IEnumerable<string> inputFiles, string outputFile)
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var step1 = new TransformBlock<string, string>(fileName => _reader.Read(fileName) , 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _configuration.MaxReadingTasks,
                });
            var step2 = new TransformBlock<string, List<GeneratedModel>>(word => GenerateTestClasses(word), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _configuration.MaxProcessingTasks,
                });
            var step3 = new ActionBlock<List<GeneratedModel>>(text =>_writer.Write(outputFile, text), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _configuration.MaxWritingTasks,
                });
            step1.LinkTo(step2, linkOptions);
            step2.LinkTo(step3, linkOptions);
            
            foreach (string inputFile in inputFiles)
            {
                await step1.SendAsync(inputFile);
            }
            
            step1.Complete();

            await step3.Completion;
        }
        
        private List<GeneratedModel> GenerateTestClasses(string sourceCode)
        {
            ParsingResultBuilder builder = new ParsingResultBuilder();
            ParsingResultStructure result = builder.GetResult(sourceCode);
            TestGenerator generator = new TestGenerator();
            List<GeneratedModel> generatedTests = generator.Generate(result);

            return generatedTests;
        }
    }
}