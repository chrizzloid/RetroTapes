using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Addresses
{
    public class DetailsModel : PageModel
    {
        private AddressService _service;
        public DetailsModel(AddressService service) => _service = service;


        public AddressDetailVm Vm { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _service.GetDetailAsync(id);
            if (vm == null) { return NotFound(); }

            Vm = vm;
            return Page();


        }
    }
}
