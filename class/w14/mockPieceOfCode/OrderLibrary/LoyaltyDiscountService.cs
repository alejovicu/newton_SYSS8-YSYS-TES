namespace OrderLibrary;

public class LoyaltyDiscountService : ILoyaltyDiscountService
{

    // Implementation of the GetDiscountRate method that returns 
    // discount rates based on customer type
    public decimal GetDiscountRate(string customerType) =>
        customerType.ToUpperInvariant() switch
        {
            "VIP" => 0.20m,
            "STUDENT" => 0.10m,
            _ => 0.00m
        };
}