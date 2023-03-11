using DataLayer;
using HelperTypes;

namespace ServiceLayer
{
    public partial class PersonNameDto : ILinkToEntity<Person>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}