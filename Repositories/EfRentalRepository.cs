using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;


namespace RetroTapes.Repositories
{
    public sealed class EfRentalRepository : IRentalRepository
    {
        private readonly SakilaContext _db;
        public EfRentalRepository(SakilaContext db) => _db = db;

        public async Task<Rental?> GetByIdAsync(int rentalId, bool includeGraph = false, CancellationToken ct = default)
        {
            IQueryable<Rental> q = _db.Rentals;

            if (includeGraph)
            {
                q = q
                    .Include(r => r.Customer)
                    .Include(r => r.Staff)
                    .Include(r => r.Inventory)
                        .ThenInclude(i => i.Film)
                    .Include(r => r.Payments);
            }

            return await q.FirstOrDefaultAsync(r => r.RentalId == rentalId, ct);
        }

        public async Task<PagedResult<Rental>> GetPageAsync(
            int page, int pageSize, bool activeOnly = false, int? customerId = null, int? storeId = null,
            DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var q = _db.Rentals.AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Inventory).ThenInclude(i => i.Film)
                .AsQueryable();

            if (activeOnly) q = q.Where(r => r.ReturnDate == null);
            if (customerId is int cid) q = q.Where(r => r.CustomerId == cid);
            if (storeId is int sid) q = q.Where(r => r.Inventory.StoreId == sid);
            if (from is DateTime f) q = q.Where(r => r.RentalDate >= f);
            if (to is DateTime t) q = q.Where(r => r.RentalDate < t);

            q = q.OrderByDescending(r => r.RentalDate).ThenBy(r => r.RentalId);

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<Rental> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public async Task AddAsync(Rental rental, CancellationToken ct = default)
            => await _db.Rentals.AddAsync(rental, ct);

        public Task UpdateAsync(Rental rental, CancellationToken ct = default)
        {
            _db.Rentals.Update(rental);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Rental rental, CancellationToken ct = default)
        {
            _db.Rentals.Remove(rental);
            return Task.CompletedTask;
        }

        public Task<bool> InventoryHasActiveRentalAsync(int inventoryId, CancellationToken ct = default)
            => _db.Rentals.AnyAsync(r => r.InventoryId == inventoryId && r.ReturnDate == null, ct);

        public async Task MarkReturnedAsync(int rentalId, DateTime returnDateUtc, CancellationToken ct = default)
        {
            var rental = await _db.Rentals.FirstOrDefaultAsync(r => r.RentalId == rentalId, ct)
                ?? throw new KeyNotFoundException("Rental not found");

            rental.ReturnDate = returnDateUtc;
            // LastUpdate kolumnen har default GETDATE i DB och uppdateras av trigger/EF – ingen extra kod krävs här.
        }
    }
}
