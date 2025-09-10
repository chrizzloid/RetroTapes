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
        [BindProperty] public DateTime LastUpdate { get; set; }

        public string? Title { get; set; }

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
            // 1) rensa M:N först (annars FK stoppar delete)  
            _db.FilmCategories.RemoveRange(_db.FilmCategories.Where(fc => fc.FilmId == Id));
            _db.FilmActors.RemoveRange(_db.FilmActors.Where(fa => fa.FilmId == Id));

            if (await _db.Inventories.AnyAsync(inv => inv.FilmId == Id))
            {
                ModelState.AddModelError(string.Empty, "Kan inte ta bort filmen: det finns exemplarkopplingar (Inventory). Ta bort dem först.");
                return Page();
            }

            // 2) concurrency: tala om vilket LastUpdate vi hade när sidan öppnades  
            var film = new Film { FilmId = Id, LastUpdate = LastUpdate };
            _db.Entry(film).Property(f => f.LastUpdate).OriginalValue = LastUpdate;

            // 3) ta bort  
            _db.Films.Attach(film);
            _db.Films.Remove(film);

            try
            {
                await _db.SaveChangesAsync();
                TempData["Flash"] = "Filmen togs bort.";
                return RedirectToPage("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Filmen ändrades eller togs redan bort av någon annan.");
                return Page();
            }
            catch (Exception ex) when (ex is DbUpdateException && !(ex is DbUpdateConcurrencyException))
            {
                ModelState.AddModelError(string.Empty, "Kunde inte ta bort filmen. relaterade poster. Ta bort dem först.");
                return Page();
            }
        }


    }
}
