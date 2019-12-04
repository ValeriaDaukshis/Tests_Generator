using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace GeneratorLib
{
    public class Pipeline
    {
        private readonly string _inputFile;
        private readonly string _outputFile;
        private static FileReader _reader;
        private static FileWriter _writer;
        
        public Pipeline(string inputFile, string outputFile)
        {
            this._inputFile = inputFile;
            this._outputFile = outputFile;
            _reader = new FileReader();
            _writer = new FileWriter();
        }
        
        public TransformBlock<string, string> CreatePipeline(Action<bool> resultCallback)
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var step1 = new TransformBlock<string, string>((data) => _reader.Read(_inputFile) , 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 3,
                    BoundedCapacity = 5,
                });
            var step2 = new TransformBlock<string, IEnumerable<GeneratedModel>>((word) => new TestGenerator().Generate(word), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 1,
                    BoundedCapacity = 13,
                });
            var step3 = new ActionBlock<IEnumerable<GeneratedModel>>((text) =>_writer.Write(_outputFile, text), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 11,
                    BoundedCapacity = 6,
                });
            step1.LinkTo(step2, linkOptions);
            step2.LinkTo(step3, linkOptions);
            return step1;
        }
    }
}