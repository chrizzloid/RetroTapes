using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Rentals
{
    public class IndexModel : PageModel
    {
        private readonly IRentalRepository _repo;
        public IndexModel(IRentalRepository repo) => _repo = repo;

        // Bindningsbara filter via querystring
        [BindProperty(SupportsGet = true)] public int? CustomerId { get; set; }
        [BindProperty(SupportsGet = true)] public int? StoreId { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? To { get; set; }
        [BindProperty(SupportsGet = true)] public bool ActiveOnly { get; set; } = false;
        [BindProperty(SupportsGet = true, Name = "pageNo")] public int PageNo { get; set; } = 1;

        public PagedResult<Rental> PageData { get; private set; } = new() { Items = new List<Rental>(), Page = 1, PageSize = 20, TotalCount = 0 };

        public async Task OnGetAsync()
        {
            PageData = await _repo.GetPageAsync(
                page: PageNo,
                pageSize: 20,
                activeOnly: ActiveOnly,
                customerId: CustomerId,
                storeId: StoreId,
                from: From,
                to: To);
        }
    }
}
