using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Services;

namespace RetroTapes.Pages.Films
{
    public class DeleteModel : PageModel
    {
        private readonly FilmService _svc;
        public DeleteModel(FilmService svc) => _svc = svc;

        [BindProperty] public int Id { get; set; }
        [BindProperty] public DateTime LastUpdate { get; set; }

        public string? Title { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var film = await _svc.GetDeatailAsync(id);
            if (film == null) return NotFound();
            Id = film.FilmId;
            Title = film.Title;
            LastUpdate = film.LastUpdate;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                bool success = await _svc.DeleteAsync(Id, LastUpdate);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Kan inte ta bort filmen. Kontrollera att den inte är kopplad till Inventory eller redan ändrad.");
                    return Page();
                }

                TempData["Flash"] = "Filmen togs bort.";
                return RedirectToPage("Index");


            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Filmen ändrades eller togs redan bort av någon annan.");
                return Page();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Kunde inte ta bort filmen på grund av relaterade poster.");
                return Page();
            }
        }


    }
}
