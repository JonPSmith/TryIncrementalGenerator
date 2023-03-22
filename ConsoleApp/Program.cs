// See https://aka.ms/new-console-template for more information
using DataLayer.DatabaseClasses;
using DataLayer.EfCode;
using TestSupport.EfHelpers;

Console.WriteLine("Hello, World!");

var options = SqliteInMemory.CreateOptions<DemoDbContext>();
using var context = new DemoDbContext(options);
context.Database.EnsureCreated();
context.AddRange(
    new Person { Name = "Joe", Email = "joe@gmail.com" },
    new Person { Name = "Jane", Email = "jane@gmail.com" });
context.SaveChanges();

