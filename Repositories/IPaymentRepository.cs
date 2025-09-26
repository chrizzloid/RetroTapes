using RetroTapes.Infrastructure;
using RetroTapes.Models;

namespace RetroTapes.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int paymentId, bool includeGraph = false, CancellationToken ct = default);

        Task<PagedResult<Payment>> GetPageAsync(
            int page, int pageSize,
            int? customerId = null,
            int? rentalId = null,
            byte? staffId = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default);

        Task AddAsync(Payment payment, CancellationToken ct = default);
        Task UpdateAsync(Payment payment, CancellationToken ct = default);
        Task DeleteAsync(Payment payment, CancellationToken ct = default);
        Task CreatePaymentAsync(int customerId, int? rentalId, byte staffId, decimal amount);
    }
}
