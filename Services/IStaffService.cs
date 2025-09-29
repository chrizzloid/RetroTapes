using RetroTapes.Infrastructure;
using RetroTapes.ViewModels;

namespace RetroTapes.Services
{
    public interface IStaffService
    {
        Task<List<StaffBasicVm>> ListAsync(CancellationToken ct = default);
        Task<StaffBasicVm?> GetForEditAsync(byte id, CancellationToken ct = default);
        Task<(bool ok, string? error, byte id)> CreateAsync(StaffBasicVm vm, CancellationToken ct = default);
        Task<(bool ok, string? error)> UpdateAsync(StaffBasicVm vm, CancellationToken ct = default);
        Task<(bool ok, string? error)> DeleteAsync(byte id, CancellationToken ct = default);

    }
}
