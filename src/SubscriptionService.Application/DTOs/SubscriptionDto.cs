namespace SubscriptionService.Application.DTOs;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string PlanCode { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
