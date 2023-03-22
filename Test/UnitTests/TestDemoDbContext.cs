// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.DatabaseClasses;
using DataLayer.EfCode;
using ServiceLayer;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests;

public class TestDemoDbContext
{
    [Fact]
    public void TestManualPersonNameRead()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<DemoDbContext>();
        using var context = new DemoDbContext(options);
        context.Database.EnsureCreated();
        context.AddRange(
            new Person{Name = "Joe", Email = "joe@gmail.com"},
            new Person { Name = "Jane", Email = "jane@gmail.com" });
        context.SaveChanges();

        //ATTEMPT
        var service = new ManualPersonNameRead();
        var entries = service.ReadPersonNameList(context);

        //VERIFY
        entries.Count.ShouldEqual(2);
        entries.Select(x => x.Name).ShouldEqual(
            new string[] { "Joe", "Jane"});
    }
}