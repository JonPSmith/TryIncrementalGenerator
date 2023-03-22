// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MySourceGenerator.SupportCode
{
    public class ExtractedQueryInfo
    {

        /// <summary>
        /// Used in Distinct to remove duplicates
        /// </summary>


        private const string StartOfIDbAndEntity = "IDbAndEntity<";
        private const char EndOfIDbAndEntity = '>';


        /// <summary>
        /// This contains the namespace of where the IDbAndEntity{TContext, TEntity} was found.
        /// This is needed when the provided database access code is built 
        /// </summary>
        public string? NamespaceName { get; }

        /// <summary>
        /// This contains the using namespaces in the code where the IDbAndEntity{TContext, TEntity} was found.
        /// </summary>
        public IList<string>? UsingProjectNames { get; }

        /// <summary>
        /// This holds the Type of TContext in the IDbAndEntity{TContext, TEntity} type 
        /// </summary>
        public Type? DbContextType { get; private set; }

        /// <summary>
        /// This holds the Type of TEntity in the IDbAndEntity{TContext, TEntity} type 
        /// </summary>
        public Type? EntityType { get; private set; }

        /// <summary>
        /// This contains of the type of the class that is used in the generated query
        /// </summary>
        public Type? QueryType { get; }

        /// <summary>
        /// This is valid if all three parts aren't null
        /// </summary>
        public bool IsValid => NamespaceName != null && UsingProjectNames != null && QueryType != null && EntityType != null;

        /// <summary>
        /// This runs back up the parent node looking for
        /// 1. This finds the name of the type in the IDbAndEntity{TContext, TEntity}
        /// 2. The <see cref="NamespaceDeclarationSyntax"/> node, which has the namespace of this complied unit
        /// 3. The <see cref="CompilationUnitSyntax"/> node, which defines what projects are used in the complied unit
        ///   NOTE: stage 3 must run after stage 2, as its working up the the parents.  
        /// 4. The type of <see cref="EntityType"/> used in the query is formed from stage 1 and 3.
        /// </summary>
        /// <param name="startNode">This is where the IDbAndEntity{TContext, TEntity} was found.</param>
        /// <returns></returns>
        public ExtractedQueryInfo(SyntaxNode startNode)
        {
            var node = startNode;

            //-----------------------------------------------------------
            //1. This finds the name of the type in the IDbAndEntity{TContext, TEntity}
            if (startNode.Parent is not SimpleBaseTypeSyntax simpleBase)
                //Error: Expected a SimpleBaseTypeSyntax
                return;

            var nameOfInterface = simpleBase.Type.ToString();
            if (!nameOfInterface.StartsWith(StartOfIDbAndEntity)
                || nameOfInterface.Last() != EndOfIDbAndEntity)
                //Error: Should be IDbAndEntity<>
                return;


            var innerParts = nameOfInterface
                .Substring(StartOfIDbAndEntity.Length,
                    nameOfInterface.Length - StartOfIDbAndEntity.Length - 1);

            //The innerParts has the form "DbContextTypeName, EntityClassName"
            var typeNames = innerParts.Split(',')
                .Select(x => x.Trim()).ToArray();

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
            //3. Get the namespace of the complied unit containing the IDbAndEntity{TContext, TEntity}

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
                    FindClassTypeByName(typeNames, 0); //finds the DbContext type
                    FindClassTypeByName(typeNames, 1); //finds the entity type
                }

                node = node.Parent;
            }
        }

        private Type? FindClassTypeByName(string[] typeNames, int index)
        {
            foreach (var usingName in UsingProjectNames!)
            {
                var projectName = usingName.Contains('.') 
                    ? usingName.Substring(0, usingName.IndexOf('.'))
                    : usingName;
                var typeFound = Type.GetType($"{usingName}.{typeNames[index]}, {projectName}");
                if (typeFound != null)
                {
                    if (index == 0)
                        DbContextType = typeFound;
                    else
                        EntityType = typeFound;
                    break;
                }
            }

            return null;
        }
    }


}
