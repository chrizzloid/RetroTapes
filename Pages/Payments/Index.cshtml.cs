using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Payments
{
    public class IndexModel : PageModel
    {
        private readonly IPaymentRepository _repo;
        public IndexModel(IPaymentRepository repo) => _repo = repo;

        [BindProperty(SupportsGet = true)] public int? CustomerId { get; set; }
        [BindProperty(SupportsGet = true)] public int? RentalId { get; set; }
        [BindProperty(SupportsGet = true)] public int? StaffId { get; set; }       // om StaffId=byte → ändra typen
        [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? To { get; set; }
        [BindProperty(SupportsGet = true, Name = "pageNo")] public int PageNo { get; set; } = 1;

        public PagedResult<Payment> PageData { get; private set; } = new() { Items = new List<Payment>(), Page = 1, PageSize = 20, TotalCount = 0 };

        public async Task OnGetAsync()
        {
            PageData = await _repo.GetPageAsync(
                page: PageNo,
                pageSize: 20,
                customerId: CustomerId,
                rentalId: RentalId,
                staffId: StaffId is null ? null : (byte?)StaffId, // ta bort cast om StaffId är int i modellen
                from: From,
                to: To);
        }
    }
}
