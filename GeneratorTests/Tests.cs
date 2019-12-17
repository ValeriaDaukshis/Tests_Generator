using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneratorLib;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace GeneratorTests
{
    [TestFixture]
    public class Tests
    {
        private CompilationUnitSyntax _root;
        
        [SetUp]
        public void SetUp()
        {
             PipelineConfiguration configuration = new PipelineConfiguration(3, 3, 9);
            
            Directory.SetCurrentDirectory(Path.GetDirectoryName(GetType().Assembly.Location));
            string a = Directory.GetCurrentDirectory();
//            //string workPath = @"C:\Users\dauks\source\repos\Spp_lab4_Tests_Generator\TestsGenerator\GeneratorTests\";
            List<string> fileNames = new List<string>();
            fileNames.Add("TestClass1.txt");
            
            Pipeline generator = new Pipeline(configuration);
            string outputFileName = "Output";
            generator.CreatePipeline(fileNames, outputFileName).Wait();
            //var sourceCode = File.ReadAllText(@"C:\Users\dauks\source\repos\Spp_lab4_Tests_Generator\TestsGenerator\EnterPoint\bin\Debug\Output_TestsGeneratorTest.dat");
            var sourceCode = File.ReadAllText("Output_TestsGeneratorTest.dat");
            
            var codeTree = CSharpSyntaxTree.ParseText(sourceCode);
            _root = codeTree.GetCompilationUnitRoot();
        }

        [Test]
        public void UsingDeclarationsTest()
        {
            Assert.AreEqual("System", _root.Usings[0].Name.ToString());
            Assert.AreEqual("System.Collections.Generic", _root.Usings[1].Name.ToString());
            Assert.AreEqual("System.Linq", _root.Usings[2].Name.ToString());
            Assert.AreEqual("System.Text", _root.Usings[3].Name.ToString());
            Assert.AreEqual("Microsoft.VisualStudio.TestTools.UnitTesting", _root.Usings[4].Name.ToString());
            Assert.AreEqual("TestsGeneratorLib", _root.Usings[5].Name.ToString());
        }

        [Test]
        public void NamespaceDeclarationsTest()
        {
            IEnumerable<NamespaceDeclarationSyntax>  namespaces = _root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();

            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual("TestsGeneratorLib.Tests", namespaces.ElementAt<NamespaceDeclarationSyntax>(0).Name.ToString());
        }

        [Test]
        public void ClassTest()
        {
            IEnumerable<ClassDeclarationSyntax> classes = _root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            Assert.AreEqual(1, classes.Count());
            Assert.AreEqual("TestsGeneratorTests", classes.ElementAt<ClassDeclarationSyntax>(0).Identifier.ToString());
            Assert.AreEqual(1, classes.ElementAt<ClassDeclarationSyntax>(0).AttributeLists.Count);
            Assert.AreEqual("TestClass", classes.ElementAt<ClassDeclarationSyntax>(0).AttributeLists[0].Attributes[0].Name.ToString());
        }

        public void MethodAttributesTest(MethodDeclarationSyntax method)
        {
            Assert.AreEqual(1, method.AttributeLists.Count);
            Assert.AreEqual(1, method.AttributeLists[0].Attributes.Count);
            Assert.AreEqual("TestMethod", method.AttributeLists[0].Attributes[0].Name.ToString());
        }

        [Test]
        public void MethodsTest()
        {
            IEnumerable<MethodDeclarationSyntax> methods = _root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            Assert.AreEqual(3, methods.Count());
            Assert.AreEqual("GenerateTest", methods.ElementAt<MethodDeclarationSyntax>(0).Identifier.ToString());
            MethodAttributesTest(methods.ElementAt<MethodDeclarationSyntax>(0));
            Assert.AreEqual("Method1Test", methods.ElementAt<MethodDeclarationSyntax>(1).Identifier.ToString());
            MethodAttributesTest(methods.ElementAt<MethodDeclarationSyntax>(1));
            Assert.AreEqual("Method2Test", methods.ElementAt<MethodDeclarationSyntax>(2).Identifier.ToString());
            MethodAttributesTest(methods.ElementAt<MethodDeclarationSyntax>(2));
        }

        [Test]
        public void AssertFailTest()
        {
            IEnumerable<MethodDeclarationSyntax> methods = _root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            int actual = methods.ElementAt<MethodDeclarationSyntax>(0).Body.Statements.OfType<ExpressionStatementSyntax>().Where((statement) => statement.ToString().Contains("Assert.Fail")).Count(); 
            Assert.AreEqual(1,actual);
        }

    }
}