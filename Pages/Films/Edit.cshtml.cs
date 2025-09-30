using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Repositories;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class EditModel : PageModel
    {
        private readonly FilmService _svc;
        private readonly ILogger<EditModel> _logger;
        private readonly LookupService _lookups;
        private readonly IInventoryService _inv;

        public EditModel(FilmService svc, ILogger<EditModel> logger, LookupService lookups, IInventoryService inv)
        {
            _svc = svc;
            _logger = logger;
            _lookups = lookups;
            _inv = inv;
        }

        [BindProperty] public FilmEditVm Vm { get; set; } = new();

        public SelectList LanguageOptions { get; set; } = default!;
        public MultiSelectList CategoryOptions { get; set; } = default!;
        public MultiSelectList ActorOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _svc.GetEditVmAsync(id);
            if (vm == null) return NotFound();

            var map = await _inv.GetOnHandForFilmsAsync(new[] { (short)vm.FilmId });
            vm.StockDesired = map.TryGetValue((short)vm.FilmId, out var val) ? val : 0;

            Vm = vm;

            await PopulateDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Vm.FilmId is null || Vm.FilmId <= 0)
                return BadRequest("FilmId saknas vid uppdatering.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value!.Errors.Select(e => e.ErrorMessage))}");
                _logger.LogWarning("Edit validation errors: {Errors}", string.Join(" | ", errors));

                await PopulateDropdownsAsync();
                return Page();
            }

            try
            {
                await _svc.UpsertAsync(Vm);

                if (Vm.StockDesired is int desired)
                {
                    var store = Vm.StoreId == 0 ? (byte)1 : Vm.StoreId;
                    await _inv.SetFilmStockAsync((short)Vm.FilmId, store, desired);
                }

                TempData["Flash"] = "Filmen uppdaterades.";
                return RedirectToPage("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Någon hann uppdatera filmen före dig. Ladda om sidan och försök igen.");
                await PopulateDropdownsAsync();
                return Page();
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            LanguageOptions = new SelectList(
                await _lookups.GetLanguagesAsync(), "LanguageId", "Name");

            CategoryOptions = new MultiSelectList(
                await _lookups.GetCategoriesAsync(), "CategoryId", "Name", Vm.CategoryIds);

            ActorOptions = new MultiSelectList(
                await _lookups.GetActorsAsync(), "ActorId", "LastName", Vm.ActorIds);

            ViewData["CategoryOptions"] = CategoryOptions;
            ViewData["ActorOptions"] = ActorOptions;
        }
    }
}
