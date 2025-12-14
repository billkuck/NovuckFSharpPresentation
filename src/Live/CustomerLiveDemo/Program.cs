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
        Console.WriteLine("=== Customer Discount System - Naive Approach ===\n");

        // Valid customers
        var john = new Customer("John", true, true);
        var mary = new Customer("Mary", true, true);
        var richard = new Customer("Richard", false, true);
        var sarah = new Customer("Sarah", false, false);
        
        // ILLEGAL STATE - but compiler allows it!
        var grinch = new Customer("Grinch", true, false);

        Console.WriteLine($"John (Eligible+Registered, £100): £{CalculateTotal(john, 100):F2}");
        Console.WriteLine($"Mary (Eligible+Registered, £99): £{CalculateTotal(mary, 99):F2}");
        Console.WriteLine($"Richard (Registered only, £100): £{CalculateTotal(richard, 100):F2}");
        Console.WriteLine($"Sarah (Guest, £100): £{CalculateTotal(sarah, 100):F2}");
        Console.WriteLine($"Grinch (ILLEGAL STATE, £100): £{CalculateTotal(grinch, 100):F2}");
        
        Console.WriteLine("\n--- Problem: Grinch is Eligible but not Registered! ---");
        Console.WriteLine("The type system doesn't prevent this illegal state.\n");
    }

    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        if (customer.IsEligible && customer.IsRegistered && spend >= 100)
        {
            return spend * 0.9m;  // 10% discount
        }
        return spend;
    }
}
