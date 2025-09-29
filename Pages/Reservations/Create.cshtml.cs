using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;

namespace RetroTapes.Pages.Reservations
{
    // Skapar en ny bokning genom v�r service (som g�r validering)
    public class CreateModel : PageModel
    {
        private readonly ReservationService _svc;
        public CreateModel(ReservationService svc) => _svc = svc;

        // Enkel ViewModel f�r form-bindning
        public class CreateVm
        {
            [Required] public int CustomerId { get; set; }
            [Required] public int InventoryId { get; set; }
            [Range(1, 72)] public int HoldHours { get; set; } = 24;
        }

        [BindProperty] public CreateVm Vm { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                var id = await _svc.CreateAsync(Vm.CustomerId, Vm.InventoryId, Vm.HoldHours);
                TempData["Flash"] = $"Reservation #{id} skapad.";
                return RedirectToPage("./Index");
            }
            catch (ValidationException ex)
            {
                // Jag l�gger valideringsfel i ModelState s� det syns i formul�ret
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}

