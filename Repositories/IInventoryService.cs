using RetroTapes.ViewModels;

namespace RetroTapes.Repositories
{
    public interface IInventoryService
    {
        Task<Dictionary<short, int>> GetOnHandForFilmsAsync(IEnumerable<short> filmIds, CancellationToken ct = default);
        Task SetFilmStockAsync(short filmId, byte storeId, int desired, CancellationToken ct = default);
        Task<List<InventoryEditVm>> GetActiveRentalsForFilmAsync(short filmId, byte? storeId = null, CancellationToken ct = default);
    }
}
