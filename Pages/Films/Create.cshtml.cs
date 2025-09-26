using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using RetroTapes.Data;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class CreateModel : PageModel
    {
        //private readonly SakilaContext _db;
        private readonly FilmService _svc;
        private readonly LookupService _lookups;
        //public CreateModel(SakilaContext db, FilmService svc) { _db = db; _svc = svc; }

        public CreateModel(FilmService svc, LookupService lookups) { _svc = svc; _lookups = lookups; }


        [BindProperty] public FilmEditVm Vm { get; set; } = new();

        public SelectList LanguageOptions { get; set; } = default!;
        public MultiSelectList CategoryOptions { get; set; } = default!;
        public MultiSelectList ActorOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await PopulateDropdownsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return Page();
            }

            await _svc.UpsertAsync(Vm);
            TempData["Flash"] = "Filmen skapades.";
            return RedirectToPage("Index");
        }

        private async Task PopulateDropdownsAsync()
        {
            LanguageOptions = new SelectList(
                //await _db.Languages.AsNoTracking().OrderBy(l => l.Name).ToListAsync(),
                //"LanguageId", "Name");
                await _lookups.GetLanguagesAsync(), "LanguageId", "Name");


            CategoryOptions = new MultiSelectList(
                //await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                //"CategoryId", "Name");
                await _lookups.GetCategoriesAsync(), "CategoryId", "Name");

            ActorOptions = new MultiSelectList(
                //await _db.Actors.AsNoTracking().OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToListAsync(),
                //"ActorId", "LastName");
                await _lookups.GetActorsAsync(), "ActorId", "LastName");

            ViewData["LanguageOptions"] = LanguageOptions;
            ViewData["CategoryOptions"] = CategoryOptions;
            ViewData["ActorOptions"] = ActorOptions;
        }

    }
}
