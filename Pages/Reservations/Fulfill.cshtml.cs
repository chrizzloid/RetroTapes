using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;

namespace RetroTapes.Pages.Reservations
{
    // Konverterar en aktiv bokning till en Rental via service
    public class FulfillModel : PageModel
    {
        private readonly ReservationService _svc;
        public FulfillModel(ReservationService svc) => _svc = svc;

        [BindProperty(SupportsGet = true)] public int Id { get; set; }

        [BindProperty, Required] public byte StaffId { get; set; } // byt till int om StaffId är int i er modell

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                var rentalId = await _svc.FulfillToRentalAsync(Id, StaffId);
                TempData["Flash"] = $"Reservation #{Id} utlämnad som Rental #{rentalId}.";
                return RedirectToPage("/Rentals/Index");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
