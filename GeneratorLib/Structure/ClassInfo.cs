using System.Collections.Generic;

namespace GeneratorLib.Structure
{
    public class ClassInfo
    {
        public List<MethodInfo> Methods { get; }

        public string Name { get; }

        //public string NamespaceName => _name;
        public string NamespaceName { get; }

        public ClassInfo(string className,string namespaceName, List<MethodInfo> methods)
        {
            Name = className;
            NamespaceName = namespaceName;
            this.Methods = methods;
        }
    }
}