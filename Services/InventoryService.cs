using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Repositories;
using RetroTapes.ViewModels;
using InventoryEntity = RetroTapes.Models.Inventory;

namespace RetroTapes.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly SakilaContext _db;
        public InventoryService(SakilaContext db) => _db = db;

        public async Task<Dictionary<short,int>> GetOnHandForFilmsAsync(IEnumerable<short> filmIds, CancellationToken ct = default)
        {
            var result = new Dictionary<short, int>();
            var Ids = filmIds.Distinct().ToList();
            if (Ids.Count == 0) return result;

            foreach (var filmId in Ids)
            { var total = await _db.Inventories
                    .Where(i => i.FilmId == filmId)
                    .CountAsync(ct);

              var outNow = await _db.Rentals
                    .Where(r => r.ReturnDate == null && r.Inventory.FilmId == filmId)
                    .CountAsync(ct);

              var onHand = total - outNow;
              result.Add(filmId, onHand);
            }
            return result;
        }
 
        public async Task SetFilmStockAsync(short filmId, byte storeId, int desired, CancellationToken ct = default)
        {
            if (desired < 0) desired = 0;

            var current = await _db.Inventories
                .Where(i => i.FilmId == filmId && i.StoreId == storeId)
                .ToListAsync(ct);

                 var diff = desired - current.Count;

            if (diff > 0)
            {
                for (int i=0; i < diff; i++)
                {
                    _db.Inventories.Add(new InventoryEntity
                    {
                        FilmId = filmId,
                        StoreId = storeId,
                        LastUpdate = DateTime.UtcNow
                    });
                }
                await _db.SaveChangesAsync(ct);
                return;
            }

            if (diff < 0)
            {
                var activeInvids = await _db.Rentals
                    .Where(r => r.ReturnDate == null
                             && r.Inventory.FilmId == filmId
                             && r.Inventory.StoreId == storeId)
                    .Select(r => r.InventoryId)                   
                    .ToListAsync(ct);

                var available = await _db.Inventories
                    .Where(i => i.FilmId == filmId
                             && i.StoreId == storeId
                             && !activeInvids.Contains(i.InventoryId))
                    .OrderBy(i => i.InventoryId)
                    .ToListAsync(ct);

                var needToRemove = -diff;
                if (available.Count < needToRemove)
                {
                    throw new InvalidOperationException($"Kan inte ta bort {needToRemove} st. endast {available.Count} är lediga.");
                }

                var toDelete = available.Take(needToRemove).ToList();
                _db.Inventories.RemoveRange(toDelete);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<List<InventoryEditVm>> GetActiveRentalsForFilmAsync(short filmID, byte? storeId = null, CancellationToken ct = default)
        { 
            var rentalsQuery = _db.Rentals
                .AsNoTracking()
                .Where(r => r.ReturnDate == null && r.Inventory.FilmId == filmID);

            if (storeId.HasValue)
                rentalsQuery = rentalsQuery.Where(r => r.Inventory.StoreId == storeId.Value);

            var rentalDuration = await _db.Films
                .Where(f => f.FilmId == filmID)
                .Select(f => f.RentalDuration)
                .FirstAsync(ct);

            var items = await rentalsQuery
                .Select(r => new
                {
                    r.RentalId,
                    r.RentalDate,
                    StoreId = r.Inventory.StoreId,
                    CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                    StaffName = r.Staff.FirstName + " " + r.Staff.LastName
                })
                .ToListAsync(ct);

            var result = new List<InventoryEditVm>(items.Count);
            foreach (var x in items)
            {
                result.Add(new InventoryEditVm
                {
                    RentalId = x.RentalId,
                    StoreId = (byte)x.StoreId,
                    RentalDate = x.RentalDate,
                    DueDate = x.RentalDate.AddDays(rentalDuration),
                    CustomerName = x.CustomerName,
                    StaffName = x.StaffName
                });
            }
            return result.OrderBy(v => v.DueDate).ToList();
        }
    }
}
