using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;

namespace RetroTapes.Pages.Reservations
{
    // Enkel sida för att avboka
    public class CancelModel : PageModel
    {
        private readonly ReservationService _svc;
        public CancelModel(ReservationService svc) => _svc = svc;

        [BindProperty(SupportsGet = true)] public int Id { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            await _svc.CancelAsync(Id);
            TempData["Flash"] = $"Reservation #{Id} avbokad.";
            return RedirectToPage("./Index");
        }
    }
}

