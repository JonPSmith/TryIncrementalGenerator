// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Helpers;
using Xunit;

namespace Test.UnitTests;

public class TestFirstSourceGenerator
{
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
        var result = testCode.GetGeneratedOutput();

        //VERIFY
    }
}