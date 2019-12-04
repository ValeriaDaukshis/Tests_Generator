using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using GeneratorLib.Structure;

namespace GeneratorLib
{
    public class Pipeline
    {
        private readonly string _inputFile;
        private readonly string _outputFile;
        private static FileReader _reader;
        private static FileWriter _writer;
        private PipelineConfiguration configuration;
        
        public Pipeline(string inputFile, string outputFile, PipelineConfiguration configuration)
        {
            this._inputFile = inputFile;
            this._outputFile = outputFile;
            this.configuration = configuration;
            _reader = new FileReader();
            _writer = new FileWriter();
        }
        
        public TransformBlock<string, string> CreatePipeline(Action<bool> resultCallback)
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var step1 = new TransformBlock<string, string>((data) => _reader.Read(_inputFile) , 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = configuration.MaxReadingTasks,
                });
            var step2 = new TransformBlock<string, List<GeneratedModel>>((word) => GenerateTestClasses(word), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = configuration.MaxProcessingTasks,
                });
            var step3 = new ActionBlock<IEnumerable<GeneratedModel>>((text) =>_writer.Write(_outputFile, text), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = configuration.MaxWritingTasks,
                });
            step1.LinkTo(step2, linkOptions);
            step2.LinkTo(step3, linkOptions);
            return step1;
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