namespace OrderLibrary;

// Defines that the order Would get a discount rate but it doesn't
// specifies how the discount rate is calculated, allowing for different
// implementations (e.g., based on customer type, purchase history, etc.)
public interface ILoyaltyDiscountService
{
    decimal GetDiscountRate(string customerType);
}