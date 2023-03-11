using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test1()
    {
        //SETUP
        var syntaxTree = CSharpSyntaxTree.ParseText(@"using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class PersonNameDto : ILinkToEntity<Person>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}");

        var root = syntaxTree.GetRoot();
        var childs = root.DescendantNodes();


    }
}