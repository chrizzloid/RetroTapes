using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class EditModel : PageModel
    {
        private readonly SakilaContext _db;
        private readonly FilmService _svc;
        private readonly ILogger<EditModel> _logger;
        public EditModel(SakilaContext db, FilmService svc, ILogger<EditModel> logger)
        {
            _db = db;
            _svc = svc;
            _logger = logger;
        }

        [BindProperty] public FilmEditVm Vm { get; set; } = new();

        public SelectList LanguageOptions { get; set; } = default!;
        public MultiSelectList CategoryOptions { get; set; } = default!;
        public MultiSelectList ActorOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var f = await _db.Films
                .AsNoTracking()
                .Include(x => x.FilmCategories).ThenInclude(fc => fc.Category)
                .Include(x => x.FilmActors).ThenInclude(fa => fa.Actor)
                .FirstOrDefaultAsync(x => x.FilmId == id);

            if (f == null) return NotFound();

            Vm = new FilmEditVm
            {
                FilmId = f.FilmId,
                Title = f.Title,
                Description = f.Description,
                ReleaseYear = f.ReleaseYear,
                LanguageId = f.LanguageId,
                LastUpdate = f.LastUpdate,
                CategoryIds = f.FilmCategories.Select(fc => fc.CategoryId).ToList(),
                ActorIds = f.FilmActors.Select(fa => fa.ActorId).ToList()
            };

            // Läs shadow property 'OriginalLanguageId' om den finns
            var entry = _db.Entry(f);
            if (entry.Metadata.FindProperty("OriginalLanguageId") != null)
                Vm.OriginalLanguageId = (byte?)entry.Property("OriginalLanguageId").CurrentValue;

            await PopulateDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return Page();
            }

            try
            {
                await _svc.UpsertAsync(Vm);                      // sparar
                TempData["Flash"] = "Filmen uppdaterades.";
                return RedirectToPage("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                // Bra att lämna spår – hjälper felsökning
                ModelState.AddModelError(string.Empty, "Någon annan hann ändra filmen. Granska och försök igen.");
                await PopulateDropdownsAsync();
                return Page();
            }
        }


        private async Task PopulateDropdownsAsync()
        {
            LanguageOptions = new SelectList(
                await _db.Languages.AsNoTracking().OrderBy(l => l.Name).ToListAsync(),
                "LanguageId", "Name");

            CategoryOptions = new MultiSelectList(
                await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                "CategoryId", "Name", Vm.CategoryIds);

            ActorOptions = new MultiSelectList(
                await _db.Actors.AsNoTracking().OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToListAsync(),
                "ActorId", "LastName", Vm.ActorIds);

            ViewData["CategoryOptions"] = CategoryOptions;
            ViewData["ActorOptions"] = ActorOptions;
        }

    }
}

