using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class DetailsModel : PageModel
    {
        private readonly FilmService _svc;
        public DetailsModel(FilmService svc) => _svc = svc;

        public FilmDetailVm Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _svc.GetDeatailAsync(id);
            if (vm == null) return NotFound();
            Data = vm;
            return Page();
        }
    }
}
