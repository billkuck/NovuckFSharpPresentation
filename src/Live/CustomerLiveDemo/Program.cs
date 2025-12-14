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
        Console.WriteLine("=== Customer Discount System - Final Version ===\n");

        // Customers with explicit eligibility in the type
        var john = Customer.NewEligible(new RegisteredCustomer("John"));
        var mary = Customer.NewEligible(new RegisteredCustomer("Mary"));
        var richard = Customer.NewRegistered(new RegisteredCustomer("Richard"));
        var sarah = Customer.NewGuest(new UnregisteredCustomer("Sarah"));

        Console.WriteLine($"John (Eligible, £100): £{CalculateTotal(john, 100):F2}");
        Console.WriteLine($"Mary (Eligible, £99): £{CalculateTotal(mary, 99):F2}");
        Console.WriteLine($"Richard (Registered, £100): £{CalculateTotal(richard, 100):F2}");
        Console.WriteLine($"Sarah (Guest, £100): £{CalculateTotal(sarah, 100):F2}");
        
        Console.WriteLine("\n--- Journey Complete! ---");
        Console.WriteLine("Eligibility is now explicit in the type.");
        Console.WriteLine("No booleans to check. No illegal states possible.\n");
    }

    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        if (customer.IsEligible && spend >= 100)
        {
            return spend * 0.9m;  // 10% discount
        }
        return spend;
    }
}
