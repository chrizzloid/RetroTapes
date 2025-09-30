using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Repositories;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Staff
{
    public class DetailsModel : PageModel
    {
        private readonly IStaffService _svc;
        public DetailsModel(IStaffService svc) => _svc = svc;

        public StaffBasicVm Vm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _svc.GetForEditAsync((byte)id);
            if (vm is null) return NotFound();
            Vm = vm; return Page();
        }
    }
}
