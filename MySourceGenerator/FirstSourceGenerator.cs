// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MySourceGenerator;

[Generator]
public class FirstSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //throw new Exception("Test exception!"); // delete me after test

        IncrementalValuesProvider<ClassDeclarationSyntax> enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => HasILinkToEntity(s), // select enums with attributes
                transform: static (ctx, _) => ctx.GenerateQueryPartialClass())
            .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

    }

    private bool HasILinkToEntity(SyntaxNode node)
        => node is ClassDeclarationSyntax;
}