using CustomerDomain;
using static CustomerDomain.Customer;

Console.WriteLine("=== F# Customer Domain Demo from C# ===\n");

// C# business logic using modern switch expression over F# discriminated union
static decimal CalculateTotal(Customer customer, decimal spend)
{
    var discount = customer switch
    {
        Customer.Standard _ when spend >= 100.0m => spend * 0.1m,
        _ => 0.0m
    };
    return spend - discount;
}

// Creating customers using F# discriminated union constructors
var john = NewStandard(new RegisteredCustomer("John"));
var mary = NewStandard(new RegisteredCustomer("Mary"));
var richard = NewRegistered(new RegisteredCustomer("Richard"));
var sarah = NewGuest(new UnregisteredCustomer("Sarah"));

Console.WriteLine("--- Test Cases (matching Ian Russell's examples) ---");

// Test case 1: John spends £100 (standard tier, meets threshold)
var johnTotal = CalculateTotal(john, 100.0m);
Console.WriteLine($"John (Standard) spends £100: £{johnTotal} (expected £90)");

// Test case 2: Mary spends £99 (standard tier, but under threshold)
var maryTotal = CalculateTotal(mary, 99.0m);
Console.WriteLine($"Mary (Standard) spends £99: £{maryTotal} (expected £99)");

// Test case 3: Richard spends £100 (registered but not eligible)
var richardTotal = CalculateTotal(richard, 100.0m);
Console.WriteLine($"Richard (Registered) spends £100: £{richardTotal} (expected £100)");

// Test case 4: Sarah spends £100 (guest)
var sarahTotal = CalculateTotal(sarah, 100.0m);
Console.WriteLine($"Sarah (Guest) spends £100: £{sarahTotal} (expected £100)");

Console.WriteLine("\n✅ C# switch expression works perfectly with F# discriminated unions!");

Console.WriteLine("\n--- Pattern Matching in C# ---");

// Using C# switch expressions to pattern match over F# discriminated unions
string GetCustomerType(Customer customer) => customer switch
{
    Customer.Standard c => $"Standard Tier: {c.Item.Id}",
    Customer.Registered c => $"Regular Customer: {c.Item.Id}",
    Customer.Guest c => $"Guest: {c.Item.Id}",
    _ => throw new ArgumentException("Unknown customer type")
};

Console.WriteLine(GetCustomerType(john));
Console.WriteLine(GetCustomerType(richard));
Console.WriteLine(GetCustomerType(sarah));

Console.WriteLine("\n--- Bonus Calculation Example ---");

// More complex logic using pattern matching
decimal CalculateWithBonus(Customer customer, decimal spend)
{
    var baseTotal = CalculateTotal(customer, spend);
    
    return customer switch
    {
        Customer.Standard c => baseTotal * 0.95m,  // Extra 5% off for standard tier
        _ => baseTotal
    };
}

var johnBonus = CalculateWithBonus(john, 100.0m);
Console.WriteLine($"John with bonus: £{johnBonus} (£90 - 5% = £85.50)");

Console.WriteLine("\n--- C#-Friendly Wrapper Pattern ---");

// Demonstrating optional C#-friendly wrappers
var wrapperCustomer = CustomerService.CreateStandard("Alice");
var wrapperTotal = CustomerService.CalculateTotal(wrapperCustomer, 150.0m);
var wrapperId = wrapperCustomer.GetId();
var wrapperIsStandard = wrapperCustomer.IsStandardTier();

Console.WriteLine($"Customer {wrapperId} (Standard: {wrapperIsStandard}) spends £150: £{wrapperTotal}");

Console.WriteLine("\n=== All tests completed successfully! ===");

// C#-friendly wrapper service (optional pattern)
public static class CustomerService
{
    public static Customer CreateStandard(string id) =>
        Customer.NewStandard(new RegisteredCustomer(id));
    
    public static Customer CreateRegistered(string id) =>
        Customer.NewRegistered(new RegisteredCustomer(id));
    
    public static Customer CreateGuest(string id) =>
        Customer.NewGuest(new UnregisteredCustomer(id));
    
    public static decimal CalculateTotal(Customer customer, decimal spend)
    {
        var discount = customer switch
        {
            Customer.Standard _ when spend >= 100.0m => spend * 0.1m,
            _ => 0.0m
        };
        return spend - discount;
    }
    
    // Extension methods for more C#-idiomatic usage
    public static bool IsStandardTier(this Customer customer) =>
        customer.IsStandard;
    
    public static string GetId(this Customer customer) => customer switch
    {
        Customer.Standard c => c.Item.Id,
        Customer.Registered c => c.Item.Id,
        Customer.Guest c => c.Item.Id,
        _ => throw new ArgumentException("Unknown customer type")
    };
}
