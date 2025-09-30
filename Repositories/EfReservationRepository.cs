using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;

namespace RetroTapes.Repositories
{
    // EF-implementation av vårt reservations-repository
    public sealed class EfReservationRepository : IReservationRepository
    {
        private readonly SakilaContext _db;
        public EfReservationRepository(SakilaContext db) => _db = db;

        public async Task<Reservation?> GetByIdAsync(int id, bool includeGraph = false, CancellationToken ct = default)
        {
            IQueryable<Reservation> q = _db.Reservations;
            if (includeGraph)
            {
                q = q.Include(r => r.Customer)
                     .Include(r => r.Inventory).ThenInclude(i => i.Film);
            }
            return await q.FirstOrDefaultAsync(r => r.ReservationId == id, ct);
        }

        public async Task<PagedResult<Reservation>> GetPageAsync(
            int page, int pageSize, int? customerId = null, int? inventoryId = null,
            ReservationStatus? status = null, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var q = _db.Reservations.AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Inventory).ThenInclude(i => i.Film)
                .AsQueryable();

            if (customerId is int cid) q = q.Where(r => r.CustomerId == cid);
            if (inventoryId is int iid) q = q.Where(r => r.InventoryId == iid);
            if (status is ReservationStatus st) q = q.Where(r => r.Status == st);
            if (from is DateTime f) q = q.Where(r => r.ReservedAt >= f);
            if (to is DateTime t) q = q.Where(r => r.ReservedAt < t);

            q = q.OrderByDescending(r => r.ReservedAt).ThenBy(r => r.ReservationId);

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<Reservation>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task AddAsync(Reservation res, CancellationToken ct = default)
            => await _db.Reservations.AddAsync(res, ct);

        public Task UpdateAsync(Reservation res, CancellationToken ct = default)
        {
            _db.Reservations.Update(res);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Reservation res, CancellationToken ct = default)
        {
            _db.Reservations.Remove(res);
            return Task.CompletedTask;
        }

        public Task<bool> HasActiveReservationAsync(int inventoryId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return _db.Reservations.AnyAsync(r =>
                r.InventoryId == inventoryId &&
                r.Status == ReservationStatus.Active &&
                r.ExpiresAt > now, ct);
        }

        public async Task MarkCancelledAsync(int id, CancellationToken ct = default)
        {
            var res = await _db.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id, ct)
                      ?? throw new KeyNotFoundException("Reservation not found");
            res.Status = ReservationStatus.Cancelled;
            res.LastUpdate = DateTime.UtcNow;
        }

        public async Task MarkFulfilledAsync(int id, CancellationToken ct = default)
        {
            var res = await _db.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id, ct)
                      ?? throw new KeyNotFoundException("Reservation not found");
            res.Status = ReservationStatus.Fulfilled;
            res.LastUpdate = DateTime.UtcNow;
        }

        public async Task MarkExpiredAsync(int id, CancellationToken ct = default)
        {
            var res = await _db.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id, ct)
                      ?? throw new KeyNotFoundException("Reservation not found");
            res.Status = ReservationStatus.Expired;
            res.LastUpdate = DateTime.UtcNow;
        }
    }
}
