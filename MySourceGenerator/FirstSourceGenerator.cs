// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using MySourceGenerator.SupportCode;

namespace MySourceGenerator;

[Generator]
public class FirstSourceGenerator : IIncrementalGenerator
{
    //public Action<string>? Logger { get; set; } = null;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //throw new Exception("Test exception!"); // delete me after test

        IncrementalValuesProvider<ExtractedQueryInfo> classesWithLinkToEntity = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => FindAllDatabaseAndEntityTypes(s),
                transform: static (ctx, _) => ctx.GatherDataForBuildingQuery())
            .Where(static m => m is not null)!;

        //!!! add 
        context.RegisterSourceOutput(classesWithLinkToEntity,
            ExecuteCodeBuilds);
    }

    private bool FindAllDatabaseAndEntityTypes(SyntaxNode node)
    {
        //Logger?.Invoke($"{node.GetType().Name}, {node.FullSpan}");
        return node is GenericNameSyntax { Identifier.ValueText: "IDbAndEntity" };
    }

    private void ExecuteCodeBuilds(SourceProductionContext context,
        ExtractedQueryInfo queryInfo)
    {
        var code = queryInfo.CreateReadCode();
        if (code == null) return;
        context.AddSource($"{queryInfo.QueryType!.Name}.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}