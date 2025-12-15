//using CustomerLiveDomain;

// Naive C# Model - comment out when showing F# version
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

/* Test Data:
* +-----------+------------+--------------+-------+----------------+------------------------------------+
* | Customer  | IsEligible | IsRegistered | Spend | Expected Total | Notes                              |
* +-----------+------------+--------------+-------+----------------+------------------------------------+
* | John      | true       | true         | £100  | £90            | ✓ Valid: Eligible & Registered     |
* | Mary      | true       | true         | £99   | £99            | ✓ Valid: Below threshold           |
* | Richard   | false      | true         | £100  | £100           | ✓ Valid: Registered but not elig.  |
* | Sarah     | false      | false        | £100  | £100           | ✓ Valid: Guest                     |
* | Grinch    | true       | false        | £100  | ???            | ❌ Invalid: Eligible but not reg.! |
* +-----------+------------+--------------+-------+----------------+------------------------------------+
* 
* 
* This business model example is from Chapter 1 of Essential F# by Ian Russell available at Leanpub.com
* 
*/

class Program
{
    static void Main(string[] args)
    {
        // Using C# model (uncomment above class)
        // var john = new Customer("John", true, true);
        
        // Using F# model (comment out C# class above)
        var john = new Customer("John", true, true);
        
        // The bug - illegal state (eligible but not registered)
        var grinch = new Customer("Grinch", true, false);
        
        Console.WriteLine(CalculateTotal(john, 100m));
        Console.WriteLine(CalculateTotal(grinch, 100m)); // Should not get discount!
    }
    
    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        // Correct version
        if (customer.IsEligible && customer.IsRegistered && spend >= 100)
            return spend * 0.9m;
        return spend;
        
        // Buggy version (show this too)
        // if (customer.IsEligible && spend >= 100)
        //     return spend * 0.9m;
        // return spend;
    }
}
