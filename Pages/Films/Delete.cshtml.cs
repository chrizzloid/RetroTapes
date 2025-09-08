using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;

namespace RetroTapes.Pages.Films
{
    public class DeleteModel : PageModel
    {
        private readonly SakilaContext _db;
        public DeleteModel(SakilaContext db) => _db = db;

        [BindProperty] public int Id { get; set; }
        [BindProperty] public System.DateTime LastUpdate { get; set; }

        public string Title { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var film = await _db.Films.AsNoTracking().FirstOrDefaultAsync(f => f.FilmId == id);
            if (film == null) return NotFound();
            Id = film.FilmId;
            Title = film.Title;
            LastUpdate = film.LastUpdate;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var film = new Film { FilmId = Id, LastUpdate = LastUpdate };
            _db.Entry(film).Property(f => f.LastUpdate).OriginalValue = LastUpdate;
            _db.Films.Attach(film);
            _db.Films.Remove(film);

            try
            {
                await _db.SaveChangesAsync();
                TempData["Flash"] = "??? Filmen togs bort.";
                return RedirectToPage("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "?? Filmen ändrades (eller togs bort) av någon annan.");
                return Page();
            }
        }
    }
}
