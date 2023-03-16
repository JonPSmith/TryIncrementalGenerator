using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MySourceGenerator;
using Xunit.Extensions.AssertExtensions;

namespace Test.Helpers;

public static class DemoSourceGeneratorTests
{
    public static string? GetGeneratedOutput(this string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference
                .CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create("SourceGeneratorTests",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Source Generator to test
        var generator = new FirstSourceGenerator();

        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation,
                out var outputCompilation,
                out var diagnostics);

        // optional
        diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ShouldBeEmpty();
            

        return outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString();
    }
}