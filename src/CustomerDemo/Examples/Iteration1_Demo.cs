using CustomerDomain.Iteration1;
using static CustomerDomain.Iteration1.Customer;

Console.WriteLine("=== Iteration 1: DU with Boolean Flag ===");
Console.WriteLine("Better: Can't be eligible without being registered!\n");

// C# business logic using switch expression over F# DU
static decimal CalculateTotal(Customer customer, decimal spend)
{
    var discount = customer switch
    {
        Registered c when c.Item.IsEligible && spend >= 100.0m => spend * 0.1m,
        _ => 0.0m
    };
    return spend - discount;
}

// Now we have to pick a case
var john = NewRegistered(new RegisteredCustomer {
    Id = "John",
    IsEligible = true
});

var richard = NewRegistered(new RegisteredCustomer {
    Id = "Richard",
    IsEligible = false
});

var sarah = NewGuest(new UnregisteredCustomer { Id = "Sarah" });

var johnTotal = CalculateTotal(john, 100.0m);
var richardTotal = CalculateTotal(richard, 100.0m);
var sarahTotal = CalculateTotal(sarah, 100.0m);

Console.WriteLine($"John (Registered, Eligible) spends £100: £{johnTotal}");
Console.WriteLine($"Richard (Registered, NOT Eligible) spends £100: £{richardTotal}");
Console.WriteLine($"Sarah (Guest) spends £100: £{sarahTotal}");

Console.WriteLine("\nImprovement: UnregisteredCustomer doesn't even HAVE IsEligible!");
Console.WriteLine("Can't create an eligible guest - the type prevents it.");
Console.WriteLine("\nNotice: C# switch expression works perfectly with F# discriminated union!");

// This would not compile:
// var bad = NewGuest(new UnregisteredCustomer { 
//     Id = "Bad", 
//     IsEligible = true  // ❌ Property doesn't exist!
// });
