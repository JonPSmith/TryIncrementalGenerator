// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests;

public class TestFirstSourceGenerator
{
    private readonly ITestOutputHelper _output;

    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public TestFirstSourceGenerator(ITestOutputHelper output)
    {
        _output = output;
    }

    private string testCode = @"using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class PersonNameDto : ILinkToEntity<Person>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}";


    [Fact]
    public void Test1()
    {
        //SETUP

        //ATTEMPT
        var result = testCode.GetGeneratedOutput(_output.WriteLine);

        //VERIFY
    }
}