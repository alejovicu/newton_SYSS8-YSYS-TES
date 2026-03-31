namespace OrderLibrary;


public class OrderManager

{

    // The implementation for the Interface LoyaltyDiscountService is not provided here, 
    // but we can assume it has a method GetDiscountRate
    private readonly ILoyaltyDiscountService _loyaltyDiscountService;

    public OrderManager(ILoyaltyDiscountService loyaltyDiscountService)
    {
        _loyaltyDiscountService = loyaltyDiscountService;
    }

    public OrderManager()
    {
        _loyaltyDiscountService = new LoyaltyDiscountService();
    }

    public decimal CalculateTotal(decimal subtotal, string customerType)
    {
        if (subtotal < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");
        }
    
        if (subtotal == 0)
        {
            // Adding m makes it explicitly a decimal, which is common 
            // for money/currency values.
            
            return 0m;
        }

        var discountRate = _loyaltyDiscountService.GetDiscountRate(customerType);
        var discountAmount = subtotal * discountRate;

        return subtotal - discountAmount;
    }
}
