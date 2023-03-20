// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MySourceGenerator.SupportCode
{
    internal class ExtractQueryParts
    {
        private const string StartOfILinkToEntity = "ILinkToEntity<";
        private const char EndOfILinkToEntity = '>';


        /// <summary>
        /// This contains the namespace of where the ILinkToEntity{T} was found.
        /// This is needed when the provided database access code is built 
        /// </summary>
        public string NamespaceName { get; }

        /// <summary>
        /// This contains of the type of the class that is used in the generated query
        /// </summary>
        public Type QueryType { get; }

        /// <summary>
        /// This contains all the using names in the code containing the ILinkToEntity{T} was found.
        /// These are used to find the {T} type 
        /// </summary>
        public Type DatabaseType { get; }



        /// <summary>
        /// This runs back up the parent node looking for
        /// 1. This finds the name of the type in the ILinkToEntity{T}
        /// 2. The <see cref="NamespaceDeclarationSyntax"/> node, which has the namespace of this complied unit 
        /// 3. The <see cref="CompilationUnitSyntax"/> node, which defines what projects are used in the complied unit
        ///    and it looks for type of <see cref="DatabaseType"/> used in the query.
        /// </summary>
        /// <param name="startNode">This is where the ILinkToEntity{T} was found.</param>
        /// <returns></returns>
        public ExtractQueryParts(SyntaxNode startNode)
        {
            var node = startNode;

            //-----------------------------------------------------------
            //This finds the name of the type in the ILinkToEntity{T}
            if (startNode.Parent is not SimpleBaseTypeSyntax simpleBase)
                //Error: Expected a SimpleBaseTypeSyntax
                return;

            var nameOfInterface = simpleBase.Type.ToString();
            if (!nameOfInterface.StartsWith(StartOfILinkToEntity)
                || nameOfInterface.Last() != EndOfILinkToEntity)
                //Error: Should be ILinkToEntity<>
                return;

            var typeName = nameOfInterface
                .Substring(StartOfILinkToEntity.Length,
                    nameOfInterface.Length - StartOfILinkToEntity.Length - 1);

            //----------------------------------------------------------
            //2. Find the query database 

            //This goes up the parents to find the  and extract the namespace name
            while (node != null && NamespaceName == null)
            {
                if (node is ClassDeclarationSyntax classDeclaration)
                {
                    //QueryType = classDeclaration
                }
                else
                    node = node.Parent;
            }

            //----------------------------------------------------------
            //2. Get the namespace of the complied unit containing the ILinkToEntity{T}

            //This goes up the parents to find the  and extract the namespace name
            while (node != null && NamespaceName == null)
            {
                if (node is NamespaceDeclarationSyntax namespaceDec)
                {
                    NamespaceName = namespaceDec.NamespaceKeyword.ValueText;
                }
                else
                    node = node.Parent;
            }

            //----------------------------------------------------------
            //3. This finds the root of this unit, which has the "usings", and then uses
            //   a combination of the using assembly names and the class name to find the correct database type

            //see https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/get-started/syntax-analysis#traversing-trees
            while (node != null)
            {
                if (node is CompilationUnitSyntax root)
                {
                    foreach (var usingDir in root.Usings)
                    {

                        var foundType = Type.GetType($"{usingDir}.{typeName}, {usingDir}");
                        if (foundType != null)
                            DatabaseType = foundType;
                    }
                    break;
                }

                node = node.Parent;
            }
        }
    }
}
