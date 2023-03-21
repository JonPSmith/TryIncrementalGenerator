using DataLayer;
using DataLayer.DatabaseClasses;
using DataLayer.EfCode;

namespace ServiceLayer
{
    public partial class PersonNameDto : IDbAndEntity<DemoDbContext, Person>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}