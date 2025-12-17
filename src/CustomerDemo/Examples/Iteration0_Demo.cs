using CustomerDomain.Iteration0;

Console.WriteLine("=== Iteration 0: Naive Model ===");
Console.WriteLine("Problem: Can create illegal states!\n");

// C# business logic with if statements
static decimal CalculateTotal(Customer customer, decimal spend)
{
    var discount = (customer.IsEligible && customer.IsRegistered && spend >= 100.0m)
        ? spend * 0.1m
        : 0.0m;
    return spend - discount;
}

// Valid customer
var john = new Customer {
    Id = "John",
    IsEligible = true,
    IsRegistered = true
};

var johnTotal = CalculateTotal(john, 100.0m);
Console.WriteLine($"John (Eligible + Registered) spends £100: £{johnTotal}");

// ILLEGAL STATE - but nothing stops us!
var badCustomer = new Customer {
    Id = "Bad",
    IsEligible = true,      // Says eligible...
    IsRegistered = false    // ...but NOT registered! ❌
};

var badTotal = CalculateTotal(badCustomer, 100.0m);
Console.WriteLine($"Bad (Eligible but NOT Registered) spends £100: £{badTotal}");
Console.WriteLine("^ This shouldn't be possible! Type system doesn't prevent it.");
