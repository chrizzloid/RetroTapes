using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Addresses
{
    public class EditModel : PageModel
    {
        private AddressService _service;

        public EditModel(AddressService service)
        {
            _service = service;
        }

        [BindProperty] public AddressEditVm Vm { get; set; } = new();
        public IEnumerable<SelectListItem> CityOptions { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _service.GetEditVmAsync(id) ?? null!;

            if (vm == null)
            {
                return NotFound();
            }

            await PopulateDropDownAsync();
            Vm = vm;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {

                TempData["Error"] = "Valideringsfel: "; // visas via _Flash
                await PopulateDropDownAsync();
                return Page();
            }

            try
            {
                await _service.UpsertAsync(Vm);
                TempData["Flash"] = "Adress sparad.";
                return RedirectToPage();

            }
            catch (DbUpdateConcurrencyException)
            {
                // Bra att l�mna sp�r � hj�lper fels�kning
                ModelState.AddModelError(string.Empty, "N�gon annan hann �ndra. Granska och f�rs�k igen.");
                await PopulateDropDownAsync();
                TempData["Flash"] = "Adress Sparad.";
                return Page();
            }


        }

        private async Task PopulateDropDownAsync()
        {
            var cities = await _service.GetCityOptionsAsync();
            CityOptions = cities;
            ViewData["CityOptions"] = CityOptions;
        }

    }
}
