using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeneratorLib.Structure
{
    public class ParsingResultBuilder
    {
        public ParsingResultStructure GetResult(string sourceCode)
        {
            SyntaxTree codeTree = CSharpSyntaxTree.ParseText(sourceCode);
            CompilationUnitSyntax root = codeTree.GetCompilationUnitRoot();

            return new ParsingResultStructure(GetClasses(root));
        }

        private List<ClassInfo> GetClasses(CompilationUnitSyntax root)
        {
            List<ClassInfo> classes = new List<ClassInfo>();

            foreach (ClassDeclarationSyntax classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var namespaceName = ((NamespaceDeclarationSyntax)classDeclaration.Parent).Name.ToString();
                var className = classDeclaration.Identifier.ValueText;

                classes.Add(new ClassInfo(className,namespaceName,GetMethods(classDeclaration)));
            }
            return classes;
        }

        private List<MethodInfo> GetMethods(ClassDeclarationSyntax classDeclaration)
        {
            HashSet<string> methodNameSet = new HashSet<string>();
            List<MethodInfo> methods = new List<MethodInfo>();
            
            foreach (MethodDeclarationSyntax methodDeclaration in classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where((methodDeclaration) => methodDeclaration.Modifiers.Any((modifier) => modifier.IsKind(SyntaxKind.PublicKeyword))))
            {
                var methodName = methodDeclaration.Identifier.ValueText;
                if(methodNameSet.Add(methodName))
                {
                    methods.Add(new MethodInfo(methodName));
                }
            }
            return methods;
        }
    }
}