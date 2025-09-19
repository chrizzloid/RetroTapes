using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Models;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Rentals
{
    public class DetailsModel : PageModel
    {
        private readonly IRentalRepository _repo;
        public DetailsModel(IRentalRepository repo) => _repo = repo;

        public Rental? Item { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _repo.GetByIdAsync(id, includeGraph: true);
            return Item is null ? NotFound() : Page();
        }
    }
}
