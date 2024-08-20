using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ESGAPI.Entities
{
    public class Customer
    {
        [Key]
        public string? CustomerRef { get; set; }
        public string? CustomerName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Town { get; set; }
        public string? County { get; set; }
        public string? Country { get; set; }
        public string? Postcode { get; set; }
    }
}
