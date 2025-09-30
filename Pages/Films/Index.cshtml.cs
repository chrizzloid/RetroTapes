using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetroTapes.Infrastructure;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class IndexModel : PageModel
    {
        private readonly LookupService _lookups;
        private readonly FilmService _svc;
        private readonly IInventoryService _inventoryService;

        public IndexModel(LookupService lookups, FilmService svc, IInventoryService inventoryService)
        {
            _lookups = lookups;
            _svc = svc;
            _inventoryService = inventoryService;
        }

        [BindProperty(SupportsGet = true)] public string? q { get; set; }
        [BindProperty(SupportsGet = true)] public byte? categoryId { get; set; }
        [BindProperty(SupportsGet = true)] public byte? languageId { get; set; }
        [BindProperty(SupportsGet = true)] public string? sort { get; set; }
        [BindProperty(SupportsGet = true)] public int pageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int pageSize { get; set; } = 20;

        public Dictionary<short, int> OnHandMap { get; set; } = new();
        public PagedResult<FilmListItemVm> Result { get; set; } = new();
        public SelectList CategoryOptions { get; set; } = default!;
        public SelectList LanguageOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Result = await _svc.SearchAsync(q, languageId, categoryId, sort, pageIndex, pageSize);

            var filmIds = Result.Items.Select(i => (short)i.FilmId).ToArray();
            OnHandMap = await _inventoryService.GetOnHandForFilmsAsync(filmIds);

            // Dropdowns via lookups
            CategoryOptions = new SelectList(
                await _lookups.GetCategoriesAsync(), "CategoryId", "Name");

            LanguageOptions = new SelectList(
                await _lookups.GetLanguagesAsync(), "LanguageId", "Name");
        }
    }
}
