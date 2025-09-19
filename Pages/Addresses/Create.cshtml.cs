using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Addresses
{
    public class CreateModel : PageModel
    {
        private readonly AddressService _service;
        [BindProperty]
        public AddressEditVm Vm { get; set; } = new();
        public IEnumerable<SelectListItem> CityOptions { get; set; } = [];


        public CreateModel(AddressService service)
        {
            _service = service;
        }


        public async Task OnGet()
        {
            await PopulateDropDownAsync();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync();
                return Page();
            }

            await _service.UpsertAsync(Vm);
            TempData["Flash"] = "Adress skapades.";
            return RedirectToPage("Index");
        }

        private async Task PopulateDropDownAsync()
        {
            var cities = await _service.GetCityOptionsAsync();
            CityOptions = cities;
            ViewData["CityOptions"] = CityOptions;
        }

    }
}
