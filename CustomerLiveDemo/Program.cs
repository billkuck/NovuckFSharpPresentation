using CustomerLiveDomain;
using static CustomerLiveDomain.Customer;

// Naive C# Model - comment out when showing F# version
/*
public class Customer
{
    public string Id { get; }
    public bool IsEligible { get; } // Eligible for discount
    public bool IsRegistered { get; }
    
    // Constructor required for immutable properties
    public Customer(string id, bool isEligible, bool isRegistered)
    {
        Id = id;
        IsEligible = isEligible;
        IsRegistered = isRegistered;
    }
}
*/

/* Test Data:
* +-----------+------------+--------------+-------+----------------+------------------------------------+
* | Customer  | IsEligible | IsRegistered | Spend | Expected Total | Notes                              |
* +-----------+------------+--------------+-------+----------------+------------------------------------+
* | John      | true       | true         | £100  | £90            | ✓ Standard: Explicit tier          |
* | Mary      | true       | true         | £99   | £99            | ✓ Standard: Below threshold        |
* | Richard   | false      | true         | £100  | £100           | ✓ Registered: Not eligible         |
* | Sarah     | false      | false        | £100  | £100           | ✓ Guest: Not registered            |
* +-----------+------------+--------------+-------+----------------+------------------------------------+
* 
* Stage 3: Explicit eligibility tiers - domain language in code!
* - No boolean flags to check or forget
* - Compiler enforces handling all customer types
* - Reads like the business requirement
* 
* This business model example is from Chapter 1 of Essential F# by Ian Russell available at Leanpub.com
* 
*/

class Program
{
    static void Main(string[] args)
    {
        // Clean factory methods - domain language explicit
        var john = NewStandard(new RegisteredCustomer("John"));
        var mary = NewStandard(new RegisteredCustomer("Mary"));
        var richard = NewRegistered(new RegisteredCustomer("Richard"));
        var sarah = NewGuest(new UnregisteredCustomer("Sarah"));
        
        Console.WriteLine($"John (£100): £{CalculateTotal(john, 100m)}");      // £90
        Console.WriteLine($"Mary (£99): £{CalculateTotal(mary, 99m)}");        // £99
        Console.WriteLine($"Richard (£100): £{CalculateTotal(richard, 100m)}"); // £100
        Console.WriteLine($"Sarah (£100): £{CalculateTotal(sarah, 100m)}");    // £100
    }
    
    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        // Current: Simpler logic - no boolean checks needed!
        if (customer.IsStandard && spend >= 100)
            return spend * 0.9m;
        return spend;
        
        // Stage 2 version (for reference - commented out)
        /*
        if (customer.IsRegistered)
        {
            var registered = customer as Customer.Registered;
            if (registered.Item.IsEligible && spend >= 100)
                return spend * 0.9m;
        }
        return spend;
        */
        
        // Stage 1 correct version (for reference - commented out)
        /*
        if (customer.IsEligible && customer.IsRegistered && spend >= 100)
            return spend * 0.9m;
        return spend;
        */
        
        // Stage 1 buggy version - forgot to check IsRegistered (for reference - commented out)
        /*
        if (customer.IsEligible && spend >= 100)
            return spend * 0.9m;
        return spend;
        */
        
        // Alternative: C# 8.0+ switch expression (cleaner but requires modern C#)
        /*
        var discount = customer switch
        {
            Standard _ when spend >= 100 => spend * 0.1m,
            _ => 0m
        };
        return spend - discount;
        */
    }
}
