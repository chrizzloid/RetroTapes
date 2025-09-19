using RetroTapes.Infrastructure;
using RetroTapes.Models;

namespace RetroTapes.Repositories
{
    public interface IRentalRepository
    {
        Task<Rental?> GetByIdAsync(int rentalId, bool includeGraph = false, CancellationToken ct = default);

        // Lista/paginera hyror (vanliga filter)
        Task<PagedResult<Rental>> GetPageAsync(
            int page, int pageSize,
            bool activeOnly = false,
            int? customerId = null,
            int? storeId = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default);

        Task AddAsync(Rental rental, CancellationToken ct = default);
        Task UpdateAsync(Rental rental, CancellationToken ct = default);
        Task DeleteAsync(Rental rental, CancellationToken ct = default);

        // Små hjälpare för typiska use-cases
        Task<bool> InventoryHasActiveRentalAsync(int inventoryId, CancellationToken ct = default);
        Task MarkReturnedAsync(int rentalId, DateTime returnDateUtc, CancellationToken ct = default);
    }
}
