# Journey to Correctness: Book-Aligned Script

**Format:** Screenplay style with embedded code  
**Duration:** 25-30 minutes (core)  
**Style:** "Yes, and..." conversational improv  
**Audience:** .NET developers curious about F#

---

## Physical Setup

**[DEV A]** - At podium (or stage right)  
**[DEV 1]** - At screen position (or stage left)  
**Shared keyboard** - Natural handoffs during code transitions  
**Two windows visible** - F# (left) and C# (right) when applicable

---

## TIME CHECK: Minute 0

# ACT 1: INTRODUCTION & THE PROBLEM (8 minutes)

---

### Scene 1: Welcome & Set Expectations (3 min)

**[DEV A]** _(at podium, facing audience)_  
Hey everyone! Thanks for being here. I'm [Name], and this is my colleague [Name].

**[DEV 1]** _(waves from screen position)_  
Hi all!

**[DEV A]**  
Quick question - who here has written F# before?  
_(wait for hands)_  
Okay, cool. And who's never touched F# but is curious about it?  
_(wait for more hands)_  
Perfect. That's exactly who we're talking to today.

**[DEV 1]** _(steps forward a bit)_  
We're going to show you something practical. Not theory, not academic exercises. Real code that solves real problems.

**[DEV A]**  
Yes, and we're doing this conversationally. If you have questions, interrupt us. Seriously. This works best as a dialogue, not a lecture.

**[DEV 1]**  
The promise is simple: F#'s type system can catch bugs at compile time that would be runtime crashes in C#. We'll show you how, then we'll show you it actually works with your existing C# code.

**[DEV A]** _(gestures toward screen)_  
Should we just dive in?

**[DEV 1]**  
Let's do it.

---

### Scene 2: The Business Requirement (5 min)

**[DEV 1]** _(moves to keyboard, opens file)_  
Alright, here's our scenario. We're building a discount system for customers.

**[Screen shows: Program.cs with naive Customer class]**

```csharp
class Customer
{
    public string Id { get; set; }
    public bool IsEligible { get; set; }
    public bool IsRegistered { get; set; }
}
```

**[DEV 1]**  
The business rule is: "Eligible registered customers get 10% discount when they spend £100 or more."

**[DEV A]** _(from podium)_  
And we have some test cases to verify it works?

**[DEV 1]**  
Yeah, here's our sample data:

**[Screen shows test data table:]**

| Customer | IsEligible | IsRegistered | Spend | Expected Total | Notes |
|----------|------------|--------------|-------|----------------|-------|
| John     | true       | true         | £100  | £90            | ✓ Valid: Eligible & Registered |
| Mary     | true       | true         | £99   | £99            | ✓ Valid: Below threshold |
| Richard  | false      | true         | £100  | £100           | ✓ Valid: Registered but not eligible |
| Sarah    | false      | false        | £100  | £100           | ✓ Valid: Guest |
| Grinch   | true       | false        | £100  | ???            | ❌ Invalid: Eligible but not registered! |

**[DEV 1]**  
Most of these are valid states. But look at that last row - Grinch is eligible but not registered. That's an illegal state.

**[DEV A]** _(walking toward screen)_  
The business rule says only registered customers can be eligible. But nothing in the type system enforces that.

**[DEV 1]**  
Right. Let's look at the calculation logic:

**[DEV 1]** _(scrolls to CalculateTotal method)_

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    if (customer.IsEligible && customer.IsRegistered && spend >= 100)
    {
        return spend * 0.9m;  // 10% discount
    }
    return spend;
}
```

**[DEV A]**  
So you have to remember to check both flags. What happens if someone forgets?

**[DEV 1]**  
Like this?

**[DEV 1 types the buggy version:]**

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    // BUG: Forgot to check IsRegistered!
    if (customer.IsEligible && spend >= 100)
    {
        return spend * 0.9m;  // 10% discount
    }
    return spend;
}
```

**[DEV A]**  
Exactly. That compiles fine, but it's wrong. Anyone with `IsEligible = true` gets the discount, even guests.

**[DEV 1]**  
Yeah. The compiler doesn't warn you. You'd find it in testing... maybe.

**[DEV A]** _(pause for effect)_  
What if we could make this bug impossible to write?

**[DEV 1]**  
How?

---

## TIME CHECK: Minute 8

# ACT 2: MAKING REGISTRATION EXPLICIT (7 minutes)

---

### Scene 3: First Discriminated Union (4 min)

**[DEV A]** _(gestures toward F# window)_  
We use F#'s type system to encode the business rules. Let me show you.

**[DEV A takes keyboard, switches to F# file]**

**[Screen shows: Library.fs - Part 1]**

```fsharp
namespace CustomerDomain

type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
}
```

**[DEV 1]**  
Wait, that's the same problem. Just in F# syntax.

**[DEV A]**  
Exactly! This is valid F#, but we're not using its strengths yet. Watch this transformation.

**[DEV A types:]**

```fsharp
namespace CustomerDomain

type RegisteredCustomer = {
    Id: string
    IsEligible: bool
}

type UnregisteredCustomer = {
    Id: string
}

type Customer =
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
```

**[DEV A]**  
This is called a discriminated union. A customer is _either_ Registered _or_ Guest. Not both. Not neither.

**[DEV 1]** _(leaning in)_  
So you can't be eligible without being registered?

**[DEV A]**  
Right. Because `UnregisteredCustomer` doesn't even have an `IsEligible` field. The illegal state is literally unrepresentable.

**[DEV 1]**  
That's clever. But what does this do to my C# code?

**[DEV A]** _(rebuilds F# project, switches to C# window)_  
Great question. Let's see.

---

### Scene 4: C# Consumption Changes (3 min)

**[DEV 1]** _(takes keyboard, looks at Program.cs)_  
Oh. My old code doesn't compile anymore.

```csharp
// This breaks:
var customer = new Customer { Id = "123", IsEligible = true, IsRegistered = false };
// Error: Cannot initialize type 'Customer'
```

**[DEV A]**  
Because `Customer` isn't a class anymore. It's a discriminated union.

**[DEV 1]**  
How do I create one now?

**[DEV A]**  
Show the IntelliSense.

**[DEV 1]** _(types "Customer.")_

**[IntelliSense shows:]**
```
Customer.NewRegistered(RegisteredCustomer)
Customer.NewGuest(UnregisteredCustomer)
```

**[DEV 1]**  
Ah. Factory methods. So I'd do:

```csharp
var registeredData = new CustomerDomain.RegisteredCustomer
{
    Id = "John",
    IsEligible = true
};
var customer = Customer.NewRegistered(registeredData);
```

**[DEV A]**  
Exactly. And can you create the illegal state now?

**[DEV 1]** _(tries typing)_  
No... there's no way to be eligible without using `NewRegistered`. The compiler won't let me.

**[DEV A]**  
That's the power of making illegal states unrepresentable.

**[DEV 1]**  
But now my discount calculation doesn't work. I can't just check `customer.IsEligible`.

**[DEV A]**  
Right. You need pattern matching. Let me show you.

---

## TIME CHECK: Minute 15

# ACT 3: PATTERN MATCHING (7 minutes)

---

### Scene 5: C# Pattern Matching (4 min)

**[DEV A]** _(takes keyboard)_

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    if (customer.IsRegistered)
    {
        var registered = (RegisteredCustomer)customer;
        if (registered.IsEligible && spend >= 100)
        {
            return spend * 0.9m;
        }
    }
    return spend;
}
```

**[DEV 1]**  
Okay, check if registered, cast it, then check if eligible.

**[DEV A]**  
Right. And the key point: you _must_ check `IsRegistered` first. The type system forces you to handle both cases.

**[DEV 1]**  
So the bug we showed earlier is impossible now.

**[DEV A]**  
Exactly. If you try to access `IsEligible` without checking `IsRegistered` first, compile error.

**[DEV 1]**  
The F# type system is making my C# code safer.

**[DEV A]**  
Yes. And if you prefer, C# switch expressions work here too - but the if-statement is clearer for demos.

**[DEV A]** _(to audience)_  
Quick note: If you're on older C# without switch expressions, the if-statement pattern works perfectly.

---

### Scene 6: F# Pattern Matching (3 min)

**[DEV 1]**  
What does the F# version look like?

**[DEV A]** _(switches to F# window, adds function)_

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    let discount =
        match customer with
        | Registered c when c.IsEligible && spend >= 100m -> spend * 0.1m
        | _ -> 0.0m
    spend - discount
```

**[DEV 1]**  
That's more concise.

**[DEV A]**  
F# has specialized syntax for pattern matching. The match expression checks which case of the union we have, then extracts the data.

**[DEV 1]**  
The `c` is the RegisteredCustomer data?

**[DEV A]**  
Right. And the `when` clause is a guard - an additional condition. The underscore matches everything else.

**[DEV 1]**  
And this is exhaustive? You have to handle all cases?

**[DEV A]**  
Yes. If you forget a case, the compiler warns you. That's another safety net.

**[DEV 1]**  
So both languages can do pattern matching, F# just has more specialized syntax for it.

**[DEV A]**  
Exactly. The safety is the same. F# just reads more naturally for this kind of data processing.

---

## TIME CHECK: Minute 22

# ACT 4: MAKING ELIGIBILITY EXPLICIT (6 minutes)

---

### Scene 7: The Final Transformation (3 min)

**[DEV A]**  
We can go one step further. Right now, eligibility is still a boolean. What if we made it explicit?

**[DEV A types:]**

```fsharp
namespace CustomerDomain

type RegisteredCustomer = {
    Id: string
}

type UnregisteredCustomer = {
    Id: string
}

type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
```

**[DEV 1]**  
Whoa. So `Eligible` is its own case?

**[DEV A]**  
Right. A customer is either Eligible (registered + eligible), Registered (but not eligible), or Guest.

**[DEV 1]**  
That's three explicit states instead of combinations of flags.

**[DEV A]**  
Exactly. No booleans to forget. No invalid combinations.

---

### Scene 8: Updated Logic (3 min)

**[DEV A]** _(updates F# function)_

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    let discount =
        match customer with
        | Eligible _ when spend >= 100m -> spend * 0.1m
        | _ -> 0.0m
    spend - discount
```

**[DEV 1]**  
That's even simpler. No `IsEligible` check needed.

**[DEV A]**  
Because eligibility is encoded in the case itself. If you match `Eligible`, you know they qualify.

**[DEV 1]**  
What about the C# side?

**[DEV A]** _(switches to C# window)_

```csharp
var john = Customer.NewEligible(new RegisteredCustomer { Id = "John" });
var mary = Customer.NewEligible(new RegisteredCustomer { Id = "Mary" });
var richard = Customer.NewRegistered(new RegisteredCustomer { Id = "Richard" });
var sarah = Customer.NewGuest(new UnregisteredCustomer { Id = "Sarah" });
```

**[DEV 1]**  
That reads really well. The customer type tells you exactly what they are.

**[DEV A]**  
And the calculation:

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    if (customer.IsEligible && spend >= 100)
    {
        return spend * 0.9m;
    }
    return spend;
}
```

**[DEV 1]**  
Wait, that's simpler than what we had before!

**[DEV A]**  
Because the F# type system did the hard work. The discriminated union properties give us `IsEligible`, `IsRegistered`, `IsGuest` checks for free.

**[DEV 1]**  
So we went from error-prone boolean combinations to simple, type-safe checks.

**[DEV A]**  
Exactly. The compiler guides you to write correct code.

---

## TIME CHECK: Minute 28

# ACT 5: WRAP-UP (2 minutes)

---

### Scene 9: Key Takeaways

**[DEV A]** _(to audience)_  
Let's recap what we've seen.

**[DEV 1]**  
We started with booleans that allowed illegal states.

**[DEV A]**  
We used F#'s discriminated unions to make those states unrepresentable.

**[DEV 1]**  
The type system forced us to handle all cases correctly.

**[DEV A]**  
And the C# code became simpler and safer, not more complicated.

**[DEV 1]**  
So the key insight is: encode business rules in types, not in runtime checks.

**[DEV A]**  
Exactly. Make illegal states unrepresentable, and let the compiler catch bugs at compile time.

**[DEV 1]** _(to audience)_  
If you're curious about F#, start small. Pick one domain model. Try discriminated unions. See how it feels.

**[DEV A]**  
You don't have to rewrite everything. Add an F# library to your C# solution. Start with the core domain where correctness matters most.

**[DEV 1]**  
Questions?

**[Open floor for Q&A]**

---

## Production Notes

### Timing Checkpoints
- **Minute 8:** Should be starting Act 2 (DU introduction)
- **Minute 15:** Should be starting Act 3 (pattern matching)
- **Minute 22:** Should be starting Act 4 (explicit eligibility)
- **Minute 28:** Wrap-up

### Key Differences from VIP Journey
- No VIP tier, no nested DUs
- Follows book progression exactly (Part 1 → Part 2 → Part 3)
- Shorter (28 min vs 40 min)
- Simpler pattern matching examples
- No FSharpOption complexity

### If Running Behind
- Abbreviate Scene 2 (business requirement setup)
- Skip showing buggy version in Scene 2
- Type less code live, show more pre-built examples

### Physical Staging Tips
- Keep energy up with movement between podium and screen
- Dev A can gesture toward screen from podium to direct attention
- Dev 1 should step back during Dev A's typing moments
- Make keyboard handoffs smooth (verbal cue: "want to try that?")

### Audience Engagement
- Pause for reactions after "illegal state is unrepresentable"
- Watch for confused faces during pattern matching - offer to explain
- Invite questions during Act 4 transition

### Code Files Needed
- **F# project:** CustomerDomain with Library.fs (3 versions)
- **C# project:** CustomerDemo with Program.cs
- Keep both windows visible in split-screen when comparing

---

**END OF SCRIPT**

---

## Extension Points

This script can be extended with:
- VIP tier addition (branches to VIP journey script)
- SAFE Stack integration
- Testing with XUnit
- More complex domain modeling examples

The book-aligned version provides a solid foundation that clearly demonstrates the core concepts without the added complexity of nested discriminated unions.
