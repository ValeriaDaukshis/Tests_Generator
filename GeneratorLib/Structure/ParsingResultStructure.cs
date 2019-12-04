using System.Collections.Generic;
using GeneratorLib.Structure;

namespace GeneratorLib
{
    public class ParsingResultStructure
    {
        public List<ClassInfo> Classes { get; }

        public ParsingResultStructure(List<ClassInfo> classes)
        {
            this.Classes = classes;
        }
    }
}