# Journey to Correctness: Book-Aligned Script (Alternate Flow)

**Format:** Screenplay style with embedded code  
**Duration:** 25-30 minutes (core)  
**Style:** Conversational with sustained presenter segments  
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

We're going to do something a little different today. Instead of slides and theory, we're going to show you real code solving a real problem. We'll work through it together, conversationally - so if you have questions, interrupt us. This works best as a dialogue, not a lecture.

Quick question - who here has written F# before?  
_(wait for hands)_  
And who's never touched F# but is curious about it?  
_(wait for more hands)_  
Perfect. That's exactly who we're talking to.

**[DEV 1]** _(steps forward)_  
The promise is simple: F#'s type system can catch bugs at compile time that would be runtime crashes in C#. We'll show you how that works, then we'll show you it actually integrates with your existing C# code. No ivory tower stuff - practical code you could use Monday morning.

**[DEV A]**  
Should we just dive in?

**[DEV 1]**  
Let's do it.

---

### Scene 2: The Business Requirement (5 min)

**[DEV 1]** _(moves to keyboard, opens file)_  
Alright, here's our scenario. We're building a discount system for customers. Simple enough, right? The business rule is: "Eligible registered customers get 10% discount when they spend £100 or more."

Here's how most of us would start:

**[Screen shows: Program.cs with naive Customer class]**

```csharp
class Customer
{
    public string Id { get; set; }
    public bool IsEligible { get; set; }
    public bool IsRegistered { get; set; }
}
```

Just a class with an ID and two booleans. Clean, straightforward C#. And here are our test cases:

**[Screen shows test data table:]**

| Customer | IsEligible | IsRegistered | Spend | Expected Total | Notes |
|----------|------------|--------------|-------|----------------|-------|
| John     | true       | true         | £100  | £90            | ✓ Valid: Eligible & Registered |
| Mary     | true       | true         | £99   | £99            | ✓ Valid: Below threshold |
| Richard  | false      | true         | £100  | £100           | ✓ Valid: Registered but not eligible |
| Sarah    | false      | false        | £100  | £100           | ✓ Valid: Guest |
| Grinch   | true       | false        | £100  | ???            | ❌ Invalid: Eligible but not registered! |

Most of these are valid states. But look at that last row - Grinch is eligible but not registered. According to our business rules, that shouldn't be possible. Only registered customers can be eligible. But nothing in our type system prevents creating this invalid state.

Now let's look at the calculation logic:

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

This works, but you have to remember to check both flags. What if someone forgets? Like this:

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

That compiles fine, but it's wrong. Anyone with `IsEligible = true` gets the discount, even guests who aren't registered. The compiler doesn't warn you. You'd find it in testing... maybe. Or maybe it ships to production.

**[DEV A]** _(from podium)_  
So the problem is that our type system doesn't encode the business rule. It lets us create invalid states, and it doesn't guide us toward correct code.

**[DEV 1]**  
Exactly. That's what we're going to fix.

---

## TIME CHECK: Minute 8

# ACT 2: MAKING REGISTRATION EXPLICIT (7 minutes)

---

### Scene 3: Introducing Discriminated Unions (5 min)

**[DEV A]** _(moves to keyboard, switches to F# file)_  
Let me show you how F# can help. We'll start with the same naive approach, just to show you the F# syntax:

**[Screen shows: Library.fs - Part 1]**

```fsharp
namespace CustomerDomain

type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
}
```

This is an F# record type. It's similar to C# records, but this has the same problem - booleans that can be in invalid combinations. Now watch what happens when we use F#'s discriminated unions:

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

This is called a discriminated union. It's saying "a customer is _either_ Registered _or_ Guest. Not both. Not neither. Pick one."

Notice what happened to the illegal state. An unregistered customer doesn't even have an `IsEligible` field. You literally cannot create a customer who is eligible but not registered. The type system won't allow it. We've made the illegal state unrepresentable.

This isn't just a validation rule you have to remember. It's baked into the type structure itself. The compiler enforces it automatically.

**[DEV A rebuilds F# project, switches to C# window]**

Now let's see what this does to the C# code.

---

### Scene 4: C# Consumption (2 min)

**[DEV 1]** _(takes keyboard)_  
Okay, so my old C# code doesn't compile anymore:

```csharp
// This breaks:
var customer = new Customer { Id = "123", IsEligible = true, IsRegistered = false };
// Error: Cannot initialize type 'Customer'
```

That's because `Customer` isn't a class anymore. It's a discriminated union. Let me see what IntelliSense offers...

**[DEV 1 types "Customer."]**

**[IntelliSense shows:]**
```
Customer.NewRegistered(RegisteredCustomer)
Customer.NewGuest(UnregisteredCustomer)
```

Ah, factory methods. So I create customers like this:

```csharp
var registeredData = new CustomerDomain.RegisteredCustomer
{
    Id = "John",
    IsEligible = true
};
var customer = Customer.NewRegistered(registeredData);
```

And now I can't create that illegal state. There's no way to be eligible without using `NewRegistered`. The F# type system is preventing me from writing buggy C# code. That's actually pretty cool.

But wait - my discount calculation doesn't work anymore. I can't just check `customer.IsEligible`.

---

## TIME CHECK: Minute 15

# ACT 3: PATTERN MATCHING (7 minutes)

---

### Scene 5: Pattern Matching in Both Languages (7 min)

**[DEV A]** _(takes keyboard)_  
Right, because you need to handle the fact that Customer is a union. You have to check which case you're dealing with. In C#, you do it like this:

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

First check if they're registered using the `IsRegistered` property that F# generated for us. Then cast to get the `RegisteredCustomer` data. Then check eligibility.

The key thing here is that you _must_ check `IsRegistered` first. If you try to access `IsEligible` without that check, you'll get a compile error. The type system forces you to handle both cases correctly. That bug we showed earlier? It's impossible to write now.

If you prefer, C# switch expressions work here too, but the if-statement is clearer for demos. And if you're on older C# without pattern matching support, this if-statement approach works perfectly.

**[DEV A switches to F# window]**

Now let me show you what this looks like in F#:

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    let discount =
        match customer with
        | Registered c when c.IsEligible && spend >= 100m -> spend * 0.1m
        | _ -> 0.0m
    spend - discount
```

This is a match expression. It checks which case of the union we have. If it's `Registered`, we bind the RegisteredCustomer data to `c` and can check if they're eligible. The `when` clause is a guard - an additional condition. The underscore matches everything else - guests and ineligible registered customers.

Pattern matching in F# is exhaustive. If you forget to handle a case, the compiler warns you. That's another safety net.

**[DEV 1]**  
So both languages can express the same safety guarantees. F# just has more specialized syntax for it.

**[DEV A]**  
Exactly. The safety comes from the discriminated union structure. F# just makes it more concise to work with. But your C# code gets the same type safety benefits.

---

## TIME CHECK: Minute 22

# ACT 4: MAKING ELIGIBILITY EXPLICIT (6 minutes)

---

### Scene 6: The Final Transformation (6 min)

**[DEV A]**  
Now I want to show you one more refinement. Right now, eligibility is still a boolean flag. What if we made it an explicit part of the type structure?

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

Now `Eligible` is its own case. A customer is one of three explicit states: Eligible (which means registered and eligible), Registered (but not eligible), or Guest. No booleans to forget. No invalid combinations. The state is right there in the case identifier.

Watch what this does to the calculation logic:

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    let discount =
        match customer with
        | Eligible _ when spend >= 100m -> spend * 0.1m
        | _ -> 0.0m
    spend - discount
```

We don't need to check `IsEligible` anymore. If we match the `Eligible` case, we know they qualify. The eligibility is encoded in the case itself. The underscore means we don't need access to the RegisteredCustomer data for this calculation.

**[DEV A switches to C# window]**

And here's how you create customers in C# now:

```csharp
var john = Customer.NewEligible(new RegisteredCustomer { Id = "John" });
var mary = Customer.NewEligible(new RegisteredCustomer { Id = "Mary" });
var richard = Customer.NewRegistered(new RegisteredCustomer { Id = "Richard" });
var sarah = Customer.NewGuest(new UnregisteredCustomer { Id = "Sarah" });
```

That reads really well. The factory method name tells you exactly what kind of customer you're creating. And the calculation:

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

Wait - this is actually simpler than what we started with! The F# discriminated union generated `IsEligible`, `IsRegistered`, and `IsGuest` properties for free. We went from error-prone boolean combinations to simple, type-safe checks.

**[DEV 1]**  
So the compiler is basically doing the validation work for us.

**[DEV A]**  
Exactly. We encoded the business rules in the type structure, and now the compiler guides you toward correct code.

---

## TIME CHECK: Minute 28

# ACT 5: WRAP-UP (2 minutes)

---

### Scene 7: Key Takeaways

**[DEV A]** _(steps back from keyboard)_  
Let me recap what we've seen here. We started with a simple class using booleans - easy to write, easy to mess up. We introduced discriminated unions to make registration explicit. That prevented creating customers in invalid states. Then we made eligibility explicit too, which simplified the logic even further.

The key insight is this: encode your business rules in types, not in runtime validation. Make illegal states unrepresentable, and let the compiler catch bugs at compile time instead of hoping your tests catch them at runtime.

**[DEV 1]**  
And the C# code didn't get more complicated - it got simpler and safer. The type system is doing the heavy lifting.

**[DEV A]** _(to audience)_  
So if you're curious about F#, start small. Pick one domain model in your application - maybe your core business logic where correctness really matters. Model it with discriminated unions in an F# library. Consume it from your existing C# code. See how it feels. You don't have to rewrite everything. You're just adding a tool to your toolbox.

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

### Key Differences from Original Book Script
- **Longer presenter segments** - Less back-and-forth, more sustained explanations
- **Same technical content** - Still follows book progression (Part 1 → Part 2 → Part 3)
- **Better flow** - Each presenter gets 2-3 minutes of sustained speaking time
- **Clearer handoffs** - Transitions happen at natural code boundaries

### Presenter Roles
- **DEV A:** Primarily handles F# code and theory explanations
- **DEV 1:** Primarily handles C# code and practical implications
- Natural role swaps happen at keyboard handoffs

### If Running Behind
- Abbreviate Scene 2 (skip showing the buggy version)
- In Scene 5, show only one language's pattern matching
- Skip creating all four customer examples in Scene 6

### Physical Staging Tips
- Presenter at keyboard should be fully engaged with code
- Other presenter can move around, gesture, address audience
- Keyboard handoffs are natural pause points
- Make eye contact with audience during explanations, not just screen

### Audience Engagement
- Pause after "illegal state is unrepresentable" for reactions
- Watch for confused faces during pattern matching section
- Invite questions during Act 4 transition
- Q&A at end should address practical adoption concerns

### Code Files Needed
- **F# project:** CustomerDomain with Library.fs (3 versions)
- **C# project:** CustomerDemo with Program.cs
- Keep both windows visible in split-screen when comparing
- Have each version pre-prepared for quick switching

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
