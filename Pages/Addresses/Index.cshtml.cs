using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetroTapes.Infrastructure;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Addresses
{
    public class IndexModel : PageModel
    {
        private readonly AddressService _service;
        public IndexModel(AddressService service) => _service = service;

        [BindProperty(SupportsGet = true)] public string? q { get; set; }
        [BindProperty(SupportsGet = true)] public int? cityId { get; set; }
        [BindProperty(SupportsGet = true)] public string? sort { get; set; }
        [BindProperty(SupportsGet = true)] public int pageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int pageSize { get; set; } = 20;

        public PagedResult<AddressListItemVm> Result { get; set; } = new();
        public SelectList CityOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Result = await _service.SearchAsync(q, cityId, sort, pageIndex, pageSize);

            CityOptions = new SelectList(
                await _service.GetCitiesAsync(),
                "CityId", "City1");
        }
    }
}
