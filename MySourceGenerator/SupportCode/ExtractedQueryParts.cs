// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MySourceGenerator.SupportCode
{
    public class ExtractedQueryInfo
    {
        private sealed class ExtractedQueryInfoEqualityComparer : IEqualityComparer<ExtractedQueryInfo>
        {
            public bool Equals(ExtractedQueryInfo x, ExtractedQueryInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.NamespaceName == y.NamespaceName && x.UsingProjectNames.Equals(y.UsingProjectNames) &&
                       Equals(x.QueryType, y.QueryType) && Equals(x.DatabaseType, y.DatabaseType);
            }

            public int GetHashCode(ExtractedQueryInfo obj)
            {
                unchecked
                {
                    var hashCode = (obj.NamespaceName != null ? obj.NamespaceName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.UsingProjectNames.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.QueryType != null ? obj.QueryType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.DatabaseType != null ? obj.DatabaseType.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<ExtractedQueryInfo> ExtractedQueryInfoComparer { get; } = new ExtractedQueryInfoEqualityComparer();

        /// <summary>
        /// Used in Distinct to remove duplicates
        /// </summary>


        private const string StartOfIDbAndEntity = "IDbAndEntity<";
        private const char EndOfIDbAndEntity = '>';


        /// <summary>
        /// This contains the namespace of where the IDbAndEntity{T} was found.
        /// This is needed when the provided database access code is built 
        /// </summary>
        public string? NamespaceName { get; }

        /// <summary>
        /// This contains the using namespaces in the code where the IDbAndEntity{T} was found.
        /// </summary>
        public IList<string> UsingProjectNames { get; }

        /// <summary>
        /// This contains of the type of the class that is used in the generated query
        /// </summary>
        public Type? QueryType { get; }

        /// <summary>
        /// This contains all the using names in the code containing the IDbAndEntity{T} was found.
        /// These are used to find the {T} type 
        /// </summary>
        public Type? DatabaseType { get; }

        /// <summary>
        /// This is valid if all three parts aren't null
        /// </summary>
        public bool IsValid => NamespaceName != null && UsingProjectNames != null && QueryType != null && DatabaseType != null;

        /// <summary>
        /// This runs back up the parent node looking for
        /// 1. This finds the name of the type in the IDbAndEntity{T}
        /// 2. The <see cref="NamespaceDeclarationSyntax"/> node, which has the namespace of this complied unit
        /// 3. The <see cref="CompilationUnitSyntax"/> node, which defines what projects are used in the complied unit
        ///   NOTE: stage 3 must run after stage 2, as its working up the the parents.  
        /// 4. The type of <see cref="DatabaseType"/> used in the query is formed from stage 1 and 3.
        /// </summary>
        /// <param name="startNode">This is where the IDbAndEntity{T} was found.</param>
        /// <returns></returns>
        public ExtractedQueryInfo(SyntaxNode startNode)
        {
            var node = startNode;

            //-----------------------------------------------------------
            //1. This finds the name of the type in the IDbAndEntity{T}
            if (startNode.Parent is not SimpleBaseTypeSyntax simpleBase)
                //Error: Expected a SimpleBaseTypeSyntax
                return;

            var nameOfInterface = simpleBase.Type.ToString();
            if (!nameOfInterface.StartsWith(StartOfIDbAndEntity)
                || nameOfInterface.Last() != EndOfIDbAndEntity)
                //Error: Should be IDbAndEntity<>
                return;

            var typeName = nameOfInterface
                .Substring(StartOfIDbAndEntity.Length,
                    nameOfInterface.Length - StartOfIDbAndEntity.Length - 1);

            //----------------------------------------------------------
            //2. Find the query database name

            //This goes up the parents to find the  and extract the namespace name
            string? className = null; 
            while (node != null && className == null)
            {
                if (node is ClassDeclarationSyntax classDeclaration)
                {
                    className = classDeclaration.Identifier.ValueText;
                    break;
                }
                node = node.Parent;
            }

            //----------------------------------------------------------
            //3. Get the namespace of the complied unit containing the IDbAndEntity{T}

            //This goes up the parents to find the  and extract the namespace name
            while (node != null && NamespaceName == null)
            {
                if (node is NamespaceDeclarationSyntax)
                {
                    NamespaceName = ((IdentifierNameSyntax?)node.ChildNodes()
                        .FirstOrDefault(x => x is IdentifierNameSyntax))?.Identifier.Text;
                    if (className != null && NamespaceName != null)
                    {
                        QueryType =  Type.GetType($"{NamespaceName}.{className}, {NamespaceName}");
                    }
                    break;
                }
                node = node.Parent;
            }

            //----------------------------------------------------------
            //4. This finds the root of this unit, which has the "usings", and then uses
            //   a combination of the using assembly names and the class name to find the correct database type

            //see https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/get-started/syntax-analysis#traversing-trees
            while (node != null)
            {
                if (node is CompilationUnitSyntax root)
                {
                    UsingProjectNames = root.Usings.Select(x => x.Name.ToString()).ToList();
                    foreach (var usingName in UsingProjectNames)
                    {
                        var foundType = Type.GetType($"{usingName}.{typeName}, {usingName}");
                        if (foundType != null)
                        {
                            DatabaseType = foundType;
                            break;
                        }
                    }
                }

                node = node.Parent;
            }
        }
    }
}
