using Microsoft.EntityFrameworkCore;
using PaymentService.Core.Entities;
using PaymentService.Core.Enums;
using PaymentService.Core.Interfaces;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        var createdPayment = await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return createdPayment.Entity;
    }

    public async Task UpdateAsync(int orderId, int userId, PaymentStatus status)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        if (payment == null) throw new KeyNotFoundException("Payment not found.");
        
        if (payment.UserId != userId) throw new UnauthorizedAccessException("User is not authorized to update this payment.");
        
        payment.Status = status;

        await _context.SaveChangesAsync();
    }
}