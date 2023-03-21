using DataLayer;
using DataLayer.DatabaseClasses;
using DataLayer.EfCode;

namespace ServiceLayer
{
    public partial class AddressDto : IDbAndEntity<DemoDbContext, Address>
    {
        public int Id { get; set; }
        public string? ZipCode { get; set; }
    }
}