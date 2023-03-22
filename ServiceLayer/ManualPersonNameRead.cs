// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.DatabaseClasses;
using DataLayer.EfCode;

namespace ServiceLayer;

public class ManualPersonNameRead
{
    public IList<PersonNameDto> ReadPersonNameList(DemoDbContext context)
    {
        return context.Set<Person>().Select(p => new PersonNameDto
        {
            Id = p.Id,
            Name = p.Name,
        }).ToList();
    }
}