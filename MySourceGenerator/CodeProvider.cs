// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using Microsoft.CodeAnalysis;
using MySourceGenerator.SupportCode;

namespace MySourceGenerator;

public static class CodeProvider
{

    public static ExtractedQueryInfo? GatherDataForBuildingQuery(this GeneratorSyntaxContext context)
    {
        var queryParts = new ExtractedQueryInfo(context.Node);

        return queryParts.IsValid ? queryParts : null;
    }


}