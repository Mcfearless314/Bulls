namespace PaymentService.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public double Amount { get; set; }
}