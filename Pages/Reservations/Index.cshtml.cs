using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.Repositories;

namespace RetroTapes.Pages.Reservations
{
    // Listar bokningar med enkel filtrering
    public class IndexModel : PageModel
    {
        private readonly IReservationRepository _repo;
        public IndexModel(IReservationRepository repo) => _repo = repo;

        [BindProperty(SupportsGet = true)] public int? CustomerId { get; set; }
        [BindProperty(SupportsGet = true)] public int? InventoryId { get; set; }
        [BindProperty(SupportsGet = true)] public ReservationStatus? Status { get; set; }

        public PagedResult<Reservation> PageData { get; private set; } = new() { Page = 1, PageSize = 20, TotalCount = 0, Items = new List<Reservation>() };
        public ReservationStatus[] AllStatuses { get; } = (ReservationStatus[])Enum.GetValues(typeof(ReservationStatus));

        public async Task OnGetAsync(int pageNo = 1)
        {
            PageData = await _repo.GetPageAsync(page: pageNo, pageSize: 20,
                customerId: CustomerId, inventoryId: InventoryId, status: Status);
        }
    }
}
