using CustomerLiveDomain;

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
*/

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Customer Discount System - Pattern Matching ===\n");

        // Valid customers - now using discriminated union
        var john = Customer.NewRegistered(new RegisteredCustomer("John", true));
        var mary = Customer.NewRegistered(new RegisteredCustomer("Mary", true));
        var richard = Customer.NewRegistered(new RegisteredCustomer("Richard", false));
        var sarah = Customer.NewGuest(new UnregisteredCustomer("Sarah"));

        Console.WriteLine("--- C# Pattern Matching (if statements) ---");
        Console.WriteLine($"John (Eligible+Registered, £100): £{CalculateTotalCSharp(john, 100):F2}");
        Console.WriteLine($"Mary (Eligible+Registered, £99): £{CalculateTotalCSharp(mary, 99):F2}");
        Console.WriteLine($"Richard (Registered only, £100): £{CalculateTotalCSharp(richard, 100):F2}");
        Console.WriteLine($"Sarah (Guest, £100): £{CalculateTotalCSharp(sarah, 100):F2}");
        
        Console.WriteLine("\n--- F# Pattern Matching (match expression) ---");
        Console.WriteLine($"John (Eligible+Registered, £100): £{CustomerOperations.calculateTotal(john, 100):F2}");
        Console.WriteLine($"Mary (Eligible+Registered, £99): £{CustomerOperations.calculateTotal(mary, 99):F2}");
        Console.WriteLine($"Richard (Registered only, £100): £{CustomerOperations.calculateTotal(richard, 100):F2}");
        Console.WriteLine($"Sarah (Guest, £100): £{CustomerOperations.calculateTotal(sarah, 100):F2}");
        
        Console.WriteLine("\n--- Both approaches produce identical results! ---\n");
    }

    static decimal CalculateTotalCSharp(Customer customer, decimal spend)
    {
        if (customer.IsRegistered)
        {
            var registered = (customer as Customer.Registered).Item;
            if (registered.IsEligible && spend >= 100)
            {
                return spend * 0.9m;  // 10% discount
            }
        }
        return spend;
    }
}
