// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.DatabaseClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode;

public class DemoDbContext : DbContext
{
    public DemoDbContext(DbContextOptions<DemoDbContext> options)
        : base(options)
    {}

    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
}