using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Models;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Payments
{
    public class DetailsModel : PageModel
    {
        private readonly IPaymentRepository _repo;
        public DetailsModel(IPaymentRepository repo) => _repo = repo;

        public Payment? Item { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _repo.GetByIdAsync(id, includeGraph: true);
            return Item is null ? NotFound() : Page();
        }
    }
}
