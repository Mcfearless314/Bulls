using PaymentService.Core.Entities;
using PaymentService.Core.Enums;
using PaymentService.Core.Interfaces;

namespace PaymentService.Application.Services;

public class PaymentService
{
private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public bool CreatePayment(Guid orderId, int userId, decimal amount)
    {
        var random = new Random();
        if (random.Next(1, 10) < 8)
        {
            Console.WriteLine("Payment failed");
            var failedPayment = new Payment
            {
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                Status = PaymentStatus.Failed,
            };
            _paymentRepository.CreateAsync(failedPayment);
            throw new Exception("Payment processing failed due to insufficient funds.");
        }
        var payment = new Payment
        {
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            Status = PaymentStatus.Confirmed
        };
        _paymentRepository.CreateAsync(payment);
        return true;
    }

    public async Task UpdatePayment(Guid orderId, int userId, PaymentStatus status)
    {
        await _paymentRepository.UpdateAsync(orderId, userId, status);
    }

    public async Task RefundPayment(Guid orderId, int userId)
    {
        await _paymentRepository.UpdateAsync(orderId, userId, PaymentStatus.Refunded);
    }
}