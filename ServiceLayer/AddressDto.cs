using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class AddressDto : ILinkToEntity<Address>
    {
        public int Id { get; set; }
        public string? ZipCode { get; set; }
    }
}