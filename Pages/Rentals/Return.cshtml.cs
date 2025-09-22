using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Data;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Rentals
{
    public class ReturnModel : PageModel
    {
        private readonly IRentalRepository _repo;
        public ReturnModel(IRentalRepository repo) => _repo = repo;

        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        public int RentalId => Id;

        public async Task<IActionResult> OnGetAsync()
        {
            var exists = await _repo.GetByIdAsync(Id);
            if (exists is null) return NotFound();
            return Page();
        }

        private readonly SakilaContext _db;
        public ReturnModel(IRentalRepository repo, SakilaContext db) { _repo = repo; _db = db; }

        public async Task<IActionResult> OnPostAsync()
        {
            await _repo.MarkReturnedAsync(Id, DateTime.UtcNow);
            return RedirectToPage("./Index");
        }
    }
}
