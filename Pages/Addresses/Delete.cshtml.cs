using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;

namespace RetroTapes.Pages.Addresses
{
    public class DeleteModel : PageModel
    {
        private readonly AddressService _service;

        public DeleteModel(AddressService service)
        {
            _service = service;
        }


        [BindProperty] public int Id { get; set; }
        [BindProperty] public DateTime LastUpdate { get; set; }
        public string DisplayName { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var address = await _service.GetDeleteInfoAsync(id);
            if (address == null) return NotFound();


            Id = address.AddressId;
            LastUpdate = address.LastUpdate;
            DisplayName = address.AddressName;
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var success = await _service.DeleteAsync(Id, LastUpdate);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Adressen ändrades/togs bort av någon annan Försök igen.");
                return Page();
            }

            TempData["Flash"] = "Adressen togs bort.";
            return RedirectToPage("Index");
        }
    }
}

