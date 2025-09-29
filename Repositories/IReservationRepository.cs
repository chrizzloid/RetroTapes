using RetroTapes.Infrastructure;
using RetroTapes.Models;

namespace RetroTapes.Repositories
{
    // Repository för att kapsla EF-frågor kring bokningar.
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int id, bool includeGraph = false, CancellationToken ct = default);

        Task<PagedResult<Reservation>> GetPageAsync(
            int page, int pageSize,
            int? customerId = null,
            int? inventoryId = null,
            ReservationStatus? status = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default);

        Task AddAsync(Reservation res, CancellationToken ct = default);
        Task UpdateAsync(Reservation res, CancellationToken ct = default);
        Task DeleteAsync(Reservation res, CancellationToken ct = default);

        // Hjälpare: kolla om det redan finns en aktiv (icke-utgången) bokning på detta exemplar
        Task<bool> HasActiveReservationAsync(int inventoryId, CancellationToken ct = default);

        // Snabba status-helpers
        Task MarkCancelledAsync(int id, CancellationToken ct = default);
        Task MarkFulfilledAsync(int id, CancellationToken ct = default);
        Task MarkExpiredAsync(int id, CancellationToken ct = default);
    }
}
