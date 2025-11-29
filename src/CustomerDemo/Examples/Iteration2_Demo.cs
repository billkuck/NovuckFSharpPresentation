using CustomerDomain.Iteration2;
using static CustomerDomain.Iteration2.Customer;

Console.WriteLine("=== Iteration 2: Explicit Eligibility ===");
Console.WriteLine("Best: Domain language is explicit in the type!\n");

// C# business logic using switch expression over F# DU
static decimal CalculateTotal(Customer customer, decimal spend)
{
    var discount = customer switch
    {
        Eligible _ when spend >= 100.0m => spend * 0.1m,
        _ => 0.0m
    };
    return spend - discount;
}

// The type TELLS you what kind of customer this is
var john = NewEligible(new RegisteredCustomer { Id = "John" });
var mary = NewEligible(new RegisteredCustomer { Id = "Mary" });
var richard = NewRegistered(new RegisteredCustomer { Id = "Richard" });
var sarah = NewGuest(new UnregisteredCustomer { Id = "Sarah" });

Console.WriteLine("--- Test Cases (matching Ian Russell's examples) ---");

// Test all four cases
var johnTotal = CalculateTotal(john, 100.0m);
var maryTotal = CalculateTotal(mary, 99.0m);
var richardTotal = CalculateTotal(richard, 100.0m);
var sarahTotal = CalculateTotal(sarah, 100.0m);

Console.WriteLine($"John (Eligible) spends £100: £{johnTotal} (expected £90)");
Console.WriteLine($"Mary (Eligible) spends £99: £{maryTotal} (expected £99)");
Console.WriteLine($"Richard (Registered) spends £100: £{richardTotal} (expected £100)");
Console.WriteLine($"Sarah (Guest) spends £100: £{sarahTotal} (expected £100)");

Console.WriteLine("\nBenefits:");
Console.WriteLine("✅ No boolean flag to check or forget");
Console.WriteLine("✅ Domain language is explicit: Eligible, Registered, Guest");
Console.WriteLine("✅ Impossible states are... impossible!");
Console.WriteLine("✅ C# switch expression works beautifully with F# DU!");

Console.WriteLine("\n--- Pattern Matching in C# ---");

string GetCustomerType(Customer customer) => customer switch
{
    Customer.Eligible c => $"VIP Customer: {c.Item.Id}",
    Customer.Registered c => $"Regular Customer: {c.Item.Id}",
    Customer.Guest c => $"Guest: {c.Item.Id}",
    _ => throw new ArgumentException("Unknown customer type")
};

Console.WriteLine(GetCustomerType(john));
Console.WriteLine(GetCustomerType(richard));
Console.WriteLine(GetCustomerType(sarah));
