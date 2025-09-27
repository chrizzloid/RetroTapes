using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Data;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Payments
{
    public class CreateModel : PageModel
    {
        private readonly IPaymentRepository _repo;
        //private readonly SakilaContext _db; // för SaveChanges
        public CreateModel(IPaymentRepository repo, SakilaContext db)
        { _repo = repo; }

        public class PaymentVm
        {
            [Required] public int CustomerId { get; set; }
            public int? RentalId { get; set; } // i Sakila kan Payment sakna kopplat rental
            [Required] public byte StaffId { get; set; } // om int i din modell ? byt till int
            [Range(0.01, 9999)] public decimal Amount { get; set; }
        }

        [BindProperty] public PaymentVm Vm { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _repo.CreatePaymentAsync(Vm.CustomerId, Vm.RentalId, Vm.StaffId, Vm.Amount);

            TempData["Flash"] = "Payment created.";
            return RedirectToPage("./Index");
        }
    }
}
