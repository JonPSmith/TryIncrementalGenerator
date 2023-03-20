// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestFirstSourceGenerator
{
    private readonly ITestOutputHelper _output;

    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public TestFirstSourceGenerator(ITestOutputHelper output)
    {
        _output = output;
    }

    private string oneClassSource = @"using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class PersonNameDto : ILinkToEntity<Person>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}";
    private string twoClassesSource = @"using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class PersonNameDto : ILinkToEntity<Person>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class AddressDto : ILinkToEntity<Address>
    {
        public int Id { get; set; }
        public string? ZipCode { get; set; }
    }
}";

    [Fact]
    public void TestGetType()
    {
        //SETUP

        //ATTEMPT
        var databaseClassType = Type.GetType("DataLayer.Person, DataLayer");

        //VERIFY
        databaseClassType.ShouldNotBeNull();
    }

    [Fact]
    public void TestOneClassGenerate()
    {
        //SETUP

        //ATTEMPT
        var result = oneClassSource.GetGeneratedOutput(_output.WriteLine);

        //VERIFY
    }

    [Fact]
    public void TestTwoClassesGenerate()
    {
        //SETUP

        //ATTEMPT
        var result = twoClassesSource.GetGeneratedOutput(_output.WriteLine);

        //VERIFY
    }
}