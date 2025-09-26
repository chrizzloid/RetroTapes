using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly CustomerService _service;

        public DeleteModel(CustomerService service)
        {
            _service = service;
        }


        [BindProperty] public int Id { get; set; }
        [BindProperty] public DateTime LastUpdate { get; set; }
        public string DisplayName { get; set; } = "";


        public async Task<IActionResult> OnGetAsync(int id)
        {
            CustomerDeleteVm customer = await _service.GetDeleteInfoAsync(id);

            if (customer == null) return NotFound();


            Id = customer.CustomerId;
            DisplayName = customer.Name;
            LastUpdate = customer.LastUpdate;
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                bool success = await _service.DeleteAsync(Id, LastUpdate);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Kan inte ta bort kunden. Kontrollera att den inte är kopplad till andra tabeller eller redan ändrad.");
                    return Page();
                }

                TempData["Flash"] = "Kunden togs bort.";
                return RedirectToPage("Index");


            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Kunden ändrades eller togs redan bort av någon annan.");
                return Page();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Kunde inte ta bort kunden på grund av relaterade poster.");
                return Page();
            }

        }
    }
}
