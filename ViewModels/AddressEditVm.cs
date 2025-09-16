using System.ComponentModel.DataAnnotations;

namespace RetroTapes.ViewModels
{
    public class AddressEditVm
    {
        public int? AddressId { get; set; }
        [Required, StringLength(50)]
        public string Address { get; set; } = "";
        public int CityId { get; set; }
        [StringLength(10)]
        public string? PostalCode { get; set; }
        public int CountryId { get; set; }

        public DateTime? LastUpdate { get; set; } // Concurrency Token
    }

    public class AddressListItemVm
    {
        public int AddressId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }

    public class CityVm
    {
        public int CityId { get; set; }
        public string Name { get; set; } = "";
    }
}
