// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace DataLayer.DatabaseClasses;

public class Address
{
    public int Id { get; set; }
    public string FirstLine { get; set; }
    public string ZipCode { get; set; }
}