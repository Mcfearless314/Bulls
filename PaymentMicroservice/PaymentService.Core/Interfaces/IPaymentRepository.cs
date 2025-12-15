using PaymentService.Core.Entities;
using PaymentService.Core.Enums;

namespace PaymentService.Core.Interfaces;

public interface IPaymentRepository
{
    public Task<IEnumerable<Payment>> GetAllAsync();
    public Task<Payment> CreateAsync(Payment payment);
    public Task UpdateAsync(int orderId, int userId, PaymentStatus status);
}