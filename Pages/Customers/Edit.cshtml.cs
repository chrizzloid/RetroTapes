using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Customers
{
    public class EditModel : PageModel
    {
        private CustomerService _service;
        private readonly LookupService _lookups;

        public EditModel(CustomerService service, LookupService lookups)
        {
            _service = service;
            _lookups = lookups;
        }

        [BindProperty] public CustomerEditVm Vm { get; set; } = new();
        public IEnumerable<SelectListItem> StoreOptions { get; set; } = [];
        public IEnumerable<SelectListItem> AddressOptions { get; set; } = [];
        public IEnumerable<SelectListItem> AddressIdOptions { get; set; } = [];

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
                TempData["Flash"] = "Kund sparad.";
                return RedirectToPage();

            }
            catch (DbUpdateConcurrencyException)
            {
                // Bra att lämna spår – hjälper felsökning
                ModelState.AddModelError(string.Empty, "Någon annan hann ändra filmen. Granska och försök igen.");
                await PopulateDropDownAsync();
                TempData["Flash"] = "Kund Sparad.";
                return Page();
            }


        }

        private async Task PopulateDropDownAsync()
        {
            StoreOptions = new SelectList(
                await _lookups.GetStoresAsync(), "StoreId", "StoreId");
            AddressOptions = new SelectList(
                await _lookups.GetAddressesAsync(), "AddressId", "Address1");
            ViewData["StoreOptions"] = StoreOptions;
            ViewData["AddressOptions"] = AddressOptions;
        }

    }
}
