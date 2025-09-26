using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly CustomerService _service;
        private readonly LookupService _lookups;

        public CreateModel(CustomerService service, LookupService lookups)
        {
            _service = service;
            _lookups = lookups;
        }

        [BindProperty] public CustomerEditVm Vm { get; set; } = new();
        public IEnumerable<SelectListItem> StoreOptions { get; set; } = [];
        public IEnumerable<SelectListItem> AddressOptions { get; set; } = [];

        public async Task OnGet()
        {
            await PopulateDropDownAsync();

        }



        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync();
                return Page();
            }

            await _service.UpsertAsync(Vm);
            TempData["Flash"] = "Kund skapades.";
            return RedirectToPage("Index");
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
