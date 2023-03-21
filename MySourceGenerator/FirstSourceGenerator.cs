// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySourceGenerator.SupportCode;

namespace MySourceGenerator;

[Generator]
public class FirstSourceGenerator : IIncrementalGenerator
{
    public Action<string> Logger { get; set; }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //throw new Exception("Test exception!"); // delete me after test

        IncrementalValuesProvider<ExtractedQueryInfo> classesWithLinkToEntity = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => FindAllDatabaseAndEntityTypes(s),
                transform: static (ctx, _) => ctx.GatherDataForBuildingQuery())
            .Where(static m => m is not null)!;

        context.RegisterSourceOutput(classesWithLinkToEntity, BuildNewCode);
    }

    private bool FindAllDatabaseAndEntityTypes(SyntaxNode node)
    {
        Logger?.Invoke($"{node.GetType().Name}, {node.FullSpan}");
        return node is GenericNameSyntax { Identifier.ValueText: "IDbAndEntity" };
    }

    private void BuildNewCode(SourceProductionContext context,
        ExtractedQueryInfo queryInfo)
    {

    }
}