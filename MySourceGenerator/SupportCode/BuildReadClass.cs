// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Reflection;

namespace MySourceGenerator.SupportCode;

public class BuildReadClass
{

    public string CreateReadCode(ExtractedQueryInfo queryInfo)
    {
        if (!queryInfo.IsValid) return null;

        var readQueryCode = string.Join("", queryInfo.UsingProjectNames
            .Select(x => $"using {x};{Environment.NewLine}"));

        readQueryCode += Environment.NewLine + $"namespace {queryInfo.NamespaceName}"
                                             + @"
{
    public partial class ";

        readQueryCode += queryInfo.QueryType!.Name + @"
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}";

        return readQueryCode;
    }

    /// <summary>
    /// This matches the database class's properties to the query class properties
    /// This is super-simple mapping at this version (i.e no AutoMapper features)
    /// </summary>
    /// <param name="databaseType"></param>
    /// <param name="readType"></param>
    /// <returns></returns>
    private IList<(string databaseProp, string readProp)> DatabaseToReadClassMapping(Type databaseType, Type readType)
    {
        var dbPropertiesDict = databaseType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(key => key.Name, data => data.PropertyType);

        var result = new List<(string databaseProp, string readProp)>();
        foreach (var readProp in readType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (dbPropertiesDict.TryGetValue(readProp.Name, out var databasePropType) 
                && readProp.GetType() == databasePropType)
            {
                result.Add( (readProp.Name, readProp.Name));
                dbPropertiesDict.Remove(readProp.Name);
            }
        }

        return result;
    }
}