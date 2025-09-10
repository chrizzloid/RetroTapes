using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;

namespace RetroTapes.Pages.Films
{
    public class DetailsModel : PageModel
    {
        private readonly SakilaContext _db;
        public DetailsModel(SakilaContext db) => _db = db;

        public record Vm(
            int FilmId,
            string Title,
            string? Description,
            string? ReleaseYear,
            string Language,
            string Categories,
            string Actors,
            System.DateTime LastUpdate);

        public Vm Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var f = await _db.Films
                .AsNoTracking()
                .Where(x => x.FilmId == id)
                .Select(x => new Vm(
                    x.FilmId,
                    x.Title,
                    x.Description,
                    x.ReleaseYear,
                    x.Language.Name,
                    string.Join(", ", x.FilmCategories.Select(fc => fc.Category.Name).OrderBy(n => n)),
                    string.Join(", ", x.FilmActors.Select(fa => fa.Actor.FirstName + " " + fa.Actor.LastName).OrderBy(n => n)),
                    x.LastUpdate))
                .FirstOrDefaultAsync();

            if (f == null) return NotFound();
            Data = f;
            return Page();
        }
    }
}
