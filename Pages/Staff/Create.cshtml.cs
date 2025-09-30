using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Repositories;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Staff
{
    public class CreateModel : PageModel
    {
        private readonly IStaffService _svc;
        public CreateModel(IStaffService svc) => _svc = svc;

        [BindProperty] public StaffBasicVm Vm { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var (ok, err, id) = await _svc.CreateAsync(Vm);
            if (!ok) { ModelState.AddModelError("", err ?? "Kunde inte skapa personal."); return Page(); }

            TempData["StaffFlash"] = "Anställd tillagd";
            TempData["StaffFlashColor"] = "success"; //grön
            return RedirectToPage("Index");
        }
    }
}
