using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Staff
{
    public class IndexModel : PageModel
    {
        private readonly IStaffService _svc;
        public IndexModel(IStaffService svc) => _svc = svc;

        public List<StaffBasicVm> Items { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _svc.ListAsync();
        }
    }
}
