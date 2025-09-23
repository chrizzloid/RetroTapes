using System.ComponentModel.DataAnnotations;

namespace RetroTapes.ViewModels
{
    public class CustomerEditVm
    {
        public int? CustomerId { get; set; }
        [Required, StringLength(45)]
        public string FirstName { get; set; } = "";
        [Required, StringLength(45)]
        public string LastName { get; set; } = "";

        [EmailAddress, StringLength(50)]
        public string? Email { get; set; }
        public bool Active { get; set; } = true;

        public string? Address { get; set; }

        [Required] public int StoreId { get; set; } // Sakila kräver store
        [Required] public int AddressId { get; set; } // Sakila kräver address
        public DateTime? LastUpdate { get; set; } // Concurrency Token
    }

    public class CustomerListItemVm
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; } // För- efternamn
        public string? Email { get; set; }
        public bool Active { get; set; }
        public int ActiveRentals { get; set; } // indikerar om någon aktiv hyra


    }

    public class CustomerDetailVm
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = " ";
        public string? Email { get; set; }
        public bool Active { get; set; }
        public int ActiveRentalsCount { get; set; }

        //public IList<RentalItemVm> ActiveRentals { get; set; } = new List<RentalItemVm>(); // För att kunna visa ev aktiva hyror dvs, ej återlämnad. 
        // public IList<PaymentItemVm> Payments { get; set; } = new List<PaymentItemVm>(); // För att kunna visa betalningar för hyror framåt




    }


}
