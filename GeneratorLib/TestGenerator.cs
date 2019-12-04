using System.Collections.Generic;
using GeneratorLib.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace GeneratorLib
{
    public class TestGenerator
    {
        public List<GeneratedModel> Generate(ParsingResultStructure parsingResult)
        {
            string fileName, content;
            List<GeneratedModel> generationResult = new List<GeneratedModel>();

            foreach (ClassInfo classInfo in parsingResult.Classes)
            {
                CompilationUnitSyntax unit = CompilationUnit()
                    .WithUsings(GetUsingDeclarations(classInfo))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(GetNamespaceDeclaration(classInfo)
                        .WithMembers(SingletonList<MemberDeclarationSyntax>(ClassDeclaration(classInfo.Name + "Tests")
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                IdentifierName("TestClass")))) ))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithMembers(GetMembersDeclarations(classInfo))))
                        )
                     );

                fileName = classInfo.Name+"Test.dat";
                content = unit.NormalizeWhitespace().ToFullString();

                generationResult.Add(new GeneratedModel(fileName, content));
            }
            return generationResult;
        }

        private NamespaceDeclarationSyntax GetNamespaceDeclaration(ClassInfo classInfo)
        {
            NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(QualifiedName(
                IdentifierName(classInfo.NamespaceName), IdentifierName("Tests")));

            return namespaceDeclaration;
        }

        private SyntaxList<UsingDirectiveSyntax> GetUsingDeclarations(ClassInfo classInfo)
        {
            List<UsingDirectiveSyntax> usings = new List<UsingDirectiveSyntax>();

            usings.Add(UsingDirective(IdentifierName("System")));
            usings.Add(UsingDirective(IdentifierName("System.Collections.Generic")));
            usings.Add(UsingDirective(IdentifierName("System.Linq")));
            usings.Add(UsingDirective(IdentifierName("System.Text")));
            usings.Add(UsingDirective(IdentifierName("Microsoft.VisualStudio.TestTools.UnitTesting")));
            usings.Add(UsingDirective(IdentifierName(classInfo.NamespaceName)));

            return new SyntaxList<UsingDirectiveSyntax>(usings);
        }

        private AttributeListSyntax GetAttributesDeclarations(string identifier)
        {
            AttributeListSyntax attributes = AttributeList(
                SingletonSeparatedList<AttributeSyntax>(
                    Attribute(
                        IdentifierName(identifier))));

            return attributes;
        }

        private SyntaxList<MemberDeclarationSyntax> GetMembersDeclarations(ClassInfo classInfo)
        {
            List<MemberDeclarationSyntax> methods = new List<MemberDeclarationSyntax>();

            foreach (MethodInfo method in classInfo.Methods)
            {
                methods.Add(GetMethodDeclaration(method));
            }
            return new SyntaxList<MemberDeclarationSyntax>(methods);
        }

        private MethodDeclarationSyntax GetMethodDeclaration(MethodInfo method)
        {
            MethodDeclarationSyntax methodDeclaration;
            List<StatementSyntax> bodyMembers = new List<StatementSyntax>();

            bodyMembers.Add(
                ExpressionStatement(
                    InvocationExpression(
                        GetAssertFail())
                    .WithArgumentList(GetMemberArgs())));

            methodDeclaration = MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier(method.Name+"Test"))
                .WithAttributeLists(
                    SingletonList<AttributeListSyntax>(
                        AttributeList(
                            SingletonSeparatedList<AttributeSyntax>(
                                Attribute(
                                    IdentifierName("TestMethod"))))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBody(Block(bodyMembers));

            return methodDeclaration;       
        }

        private MemberAccessExpressionSyntax GetAssertFail()
        {
            MemberAccessExpressionSyntax assertFail = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("Assert"),
                IdentifierName("Fail"));

            return assertFail;
        }

        private ArgumentListSyntax GetMemberArgs()
        {
            ArgumentListSyntax args = ArgumentList(
                SingletonSeparatedList(
                    Argument(
                        LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal("autogenerated")))));

            return args;
        }
    }
}