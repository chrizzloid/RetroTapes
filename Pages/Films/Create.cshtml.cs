using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class CreateModel : PageModel
    {
        private readonly FilmService _svc;
        private readonly LookupService _lookups;
        private readonly IInventoryService _inv;

        public CreateModel(FilmService svc, LookupService lookups, IInventoryService inv)
        {
            _svc = svc;
            _lookups = lookups;
            _inv = inv;
        }

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

            if (Vm.FilmId > 0 && Vm.StockDesired is int desired)
            {
                await _inv.SetFilmStockAsync((short)Vm.FilmId, Vm.StoreId, desired);
            }

            TempData["Flash"] = "Filmen skapades.";
            return RedirectToPage("Index");
        }

        private async Task PopulateDropdownsAsync()
        {
            LanguageOptions = new SelectList(
                await _lookups.GetLanguagesAsync(), "LanguageId", "Name");

            CategoryOptions = new MultiSelectList(
                await _lookups.GetCategoriesAsync(), "CategoryId", "Name");

            ActorOptions = new MultiSelectList(
                await _lookups.GetActorsAsync(), "ActorId", "LastName");

            ViewData["LanguageOptions"] = LanguageOptions;
            ViewData["CategoryOptions"] = CategoryOptions;
            ViewData["ActorOptions"] = ActorOptions;
        }
    }
}
