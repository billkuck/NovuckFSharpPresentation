# Journey to Correctness: High-Low Role Split

**Format:** Screenplay style with role separation  
**Duration:** 25-30 minutes (core)  
**Style:** Dev H (High-level) discusses concepts | Dev L (Low-level) demonstrates code  
**Audience:** .NET developers curious about F#

---

## Physical Setup

**[DEV H]** - At podium/stage position, no keyboard access  
**[DEV L]** - At keyboard full-time, screen visible to audience  
**Clear roles** - H discusses architecture/concepts, L shows implementation  
**Two windows visible** - F# (left) and C# (right) when applicable

---

## TIME CHECK: Minute 0

# ACT 1: INTRODUCTION & THE PROBLEM (8 minutes)

---

### Scene 1: Welcome & Set Expectations (3 min)

**[DEV H]** _(at podium, facing audience)_  
Hey everyone! Thanks for being here. I'm [Name], and at the keyboard we have [Name].

We're going to show you how type systems can prevent entire categories of bugs. I'll be talking about the high-level concepts - domain modeling, type safety, why bugs happen. My colleague here will show you the actual code that makes it work. We'll work through a real problem together.

Quick question - who here has written F# before?  
_(wait for hands)_  
And who's never touched F# but is curious about it?  
_(wait for more hands)_  
Perfect. That's exactly who we're talking to.

Here's the promise: most bugs in production happen because our type system allows us to create invalid states. If we encode business rules in types instead of runtime validation, the compiler catches those bugs for us. No tests needed. No code review needed. It just won't compile.

**[DEV L]** _(from keyboard)_  
And it works with your existing C# code. No rewrites required.

**[DEV H]**  
Let's show them.

---

### Scene 2: The Domain Problem (5 min)

**[DEV H]**  
Here's our scenario. We need a discount system. The business rule is simple: "Eligible registered customers get 10% discount when they spend £100 or more."

Now, there's a hidden constraint in that rule. Did you catch it? You have to be _registered_ to be _eligible_. That's not just two independent flags. That's a domain constraint. Only registered customers can be eligible. Guests cannot.

**[DEV L, show me what most developers would write.**

**[DEV L]** _(types and displays)_

```csharp
class Customer
{
    public string Id { get; set; }
    public bool IsEligible { get; set; }
    public bool IsRegistered { get; set; }
}
```

**[DEV H]**  
Right. Two independent booleans. Seems reasonable. But watch what happens when we think about the possible states this model allows:

**[DEV L displays table on screen:]**

| Customer | IsEligible | IsRegistered | Valid? | Notes |
|----------|------------|--------------|--------|-------|
| John     | true       | true         | ✓      | Eligible registered customer |
| Mary     | false      | true         | ✓      | Registered but not eligible |
| Sarah    | false      | false        | ✓      | Guest customer |
| Grinch   | true       | false        | ❌      | **Eligible but not registered - ILLEGAL!** |

**[DEV H]**  
See that last row? That's an illegal state according to our business rules. But nothing in our type system prevents creating it. The compiler is perfectly happy with it. This is what we call a "representable illegal state." Our model can express something that shouldn't exist in our domain.

And here's where the bugs creep in. **[DEV L, show the calculation logic.]**

**[DEV L]** _(types)_

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

**[DEV H]**  
This version is correct. You're checking both flags. But what happens six months later when someone else maintains this code? **[DEV L, show the bug.]**

**[DEV L]** _(modifies code)_

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

**[DEV H]**  
Someone forgot to check `IsRegistered`. That compiles fine. It might pass code review. It might even pass some tests. But now any customer with `IsEligible = true` gets a discount, even if they're not registered. That's a revenue leak. That's a production bug.

The root cause? Our type system doesn't encode the domain constraint. It allows invalid states and doesn't guide developers toward correct code. That's what we're going to fix.

---

## TIME CHECK: Minute 8

# ACT 2: MAKING INVALID STATES UNREPRESENTABLE (7 minutes)

---

### Scene 3: Discriminated Unions (5 min)

**[DEV H]**  
So how do we prevent this? We need a type system that makes the illegal state impossible to create. That's where F#'s discriminated unions come in.

The key insight is this: a customer isn't two boolean flags. A customer is one of exactly two states - either registered or a guest. That's a choice, not a combination. **[DEV L, show them the F# version.]**

**[DEV L]** _(switches to F# file, types)_

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

**[DEV H]**  
This is a discriminated union. Read that last type definition: "A Customer is either Registered containing RegisteredCustomer data, or Guest containing UnregisteredCustomer data." Not both. Not neither. Exactly one.

Now look at what happened to the `IsEligible` field. It only exists on `RegisteredCustomer`. Guests don't have it. You literally cannot create a guest who is eligible. The illegal state is not representable in this type structure.

This isn't runtime validation. This isn't a code review comment. This is enforced by the compiler at compile time. If you try to create an eligible guest, it won't compile. The bug we just showed you? It's impossible to write now.

**[DEV L, rebuild the F# project.]**

**[DEV L]** _(builds project)_  
Build succeeded.

**[DEV H]**  
Good. Now let's see what happens to the C# code.

---

### Scene 4: C# Interop (2 min)

**[DEV H]**  
When you compile an F# discriminated union, the F# compiler generates classes that C# can consume. **[DEV L, try to create a customer the old way.]**

**[DEV L]** _(switches to C# window, types)_

```csharp
// This breaks:
var customer = new Customer { Id = "123", IsEligible = true, IsRegistered = false };
// Error: Cannot initialize type 'Customer'
```

**[DEV H]**  
Right, that doesn't compile anymore. `Customer` isn't a class with properties. It's a discriminated union. **[DEV L, show what IntelliSense offers.]**

**[DEV L]** _(types "Customer." and waits for IntelliSense)_

```
Customer.NewRegistered(RegisteredCustomer)
Customer.NewGuest(UnregisteredCustomer)
```

**[DEV H]**  
Factory methods. The F# compiler generated these for us. You create customers like this:

**[DEV L]** _(types)_

```csharp
var registeredData = new CustomerDomain.RegisteredCustomer
{
    Id = "John",
    IsEligible = true
};
var john = Customer.NewRegistered(registeredData);

var guestData = new CustomerDomain.UnregisteredCustomer
{
    Id = "Sarah"
};
var sarah = Customer.NewGuest(guestData);
```

**[DEV H]**  
Notice what you can't do anymore. You can't create that illegal "Grinch" customer who's eligible but not registered. There's no factory method for it. There's no way to express it in the type system. The F# type structure is preventing you from writing buggy C# code.

---

## TIME CHECK: Minute 15

# ACT 3: HANDLING THE UNION (7 minutes)

---

### Scene 5: Pattern Matching Requirement (7 min)

**[DEV H]**  
Okay, but now we have a new problem. Our old discount calculation doesn't work anymore. We can't just check `customer.IsEligible` because not all customers have that property. We need to handle the fact that Customer is now a choice between two types.

This is called pattern matching. You ask "which case do I have?" and handle each one appropriately. **[DEV L, show the C# approach.]**

**[DEV L]** _(types)_

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

**[DEV H]**  
The F# compiler generated an `IsRegistered` property for us. First, we check which case we have. Then we cast to get the typed data. Then we can access `IsEligible`.

Here's the critical insight: you _must_ check `IsRegistered` first. If you try to access `IsEligible` without that check, you get a compile error. The type system forces you to handle both cases. That bug where someone forgot to check registration? It's impossible now. The code won't compile unless you handle both registered and guest cases.

The type system is guiding you toward correct code. This is what we mean by "making illegal states unrepresentable." You can't write the bug because the compiler won't let you.

**[DEV L, show the F# equivalent.]**

**[DEV L]** _(switches to F# window, types)_

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    let discount =
        match customer with
        | Registered c when c.IsEligible && spend >= 100m -> spend * 0.1m
        | _ -> 0.0m
    spend - discount
```

**[DEV H]**  
F# has built-in syntax for pattern matching. The `match` expression checks which case you have. If it's `Registered`, it binds the data to `c` and checks the guard condition. The underscore matches everything else - guests and ineligible customers.

F# also has exhaustiveness checking. If you forget to handle a case, the compiler warns you. That's another safety net.

But notice: both languages get the same safety guarantees. The safety comes from the discriminated union structure, not the pattern matching syntax. F# just makes it more concise to write.

---

## TIME CHECK: Minute 22

# ACT 4: REFINING THE MODEL FURTHER (6 minutes)

---

### Scene 6: Making Eligibility Explicit (6 min)

**[DEV H]**  
Now I want to show you something interesting. We've made registration explicit in the type structure. But eligibility is still a boolean flag. What if we took it one step further?

Think about what "eligible" really means in our domain. It's not just a property of a registered customer. It's a distinct state in the customer lifecycle. A customer can be in one of three states: they can be an eligible registered customer, a registered customer who's not eligible yet, or a guest. Three explicit states.

**[DEV L, show the refined F# model.]**

**[DEV L]** _(types)_

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

**[DEV H]**  
Now `Eligible` is its own case, not a boolean. A customer is one of three explicit states. No booleans. No combinations to forget. The state is encoded directly in the case identifier.

Look what this does to the logic. **[DEV L, show the calculation.]**

**[DEV L]** _(types)_

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    let discount =
        match customer with
        | Eligible _ when spend >= 100m -> spend * 0.1m
        | _ -> 0.0m
    spend - discount
```

**[DEV H]**  
We don't need to check `IsEligible` anymore. If we match the `Eligible` case, we know they qualify for a discount. The eligibility is encoded in the case itself. The underscore means we don't even need the customer data for this calculation - we only care about the state.

**[DEV L, rebuild and switch to C#.]**

**[DEV L]** _(builds F#, switches to C# window, types)_

```csharp
var john = Customer.NewEligible(new RegisteredCustomer { Id = "John" });
var mary = Customer.NewEligible(new RegisteredCustomer { Id = "Mary" });
var richard = Customer.NewRegistered(new RegisteredCustomer { Id = "Richard" });
var sarah = Customer.NewGuest(new UnregisteredCustomer { Id = "Sarah" });
```

**[DEV H]**  
Look how readable that is. The factory method name tells you exactly what kind of customer you're creating. And the calculation:

**[DEV L]** _(types)_

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

**[DEV H]**  
Wait - this is actually simpler than what we started with! The F# discriminated union generated `IsEligible`, `IsRegistered`, and `IsGuest` properties for us automatically. We went from error-prone boolean combinations to simple, type-safe property checks.

The compiler is doing all the validation work. We encoded the domain rules in types, and now the code guides you toward correctness.

---

## TIME CHECK: Minute 28

# ACT 5: WRAP-UP (2 minutes)

---

### Scene 7: Key Takeaways

**[DEV H]** _(addressing audience)_  
Let me summarize what we've demonstrated here. We started with a model that allowed illegal states - two independent booleans that could be in invalid combinations. That led to bugs because developers had to remember the domain constraints. The compiler couldn't help.

We refactored to use discriminated unions to make the illegal states unrepresentable. First we made registration explicit - you're either registered or a guest, not both. Then we made eligibility explicit - you're in one of three clear states, not a combination of flags.

The result? The compiler now enforces the domain rules. You can't create invalid customers. You can't forget to check registration before checking eligibility. The bugs we showed you earlier are impossible to write. Not hard to write - _impossible_.

And this isn't theoretical. This is practical F# code consumed from practical C# code. The C# side actually got simpler and more readable, not more complex.

**[DEV L]** _(from keyboard)_  
And you don't need to rewrite your whole application. Start with one domain model. Your core business logic where bugs are expensive. Model it in F#, consume it from C#.

**[DEV H]**  
Exactly. You're not replacing C#. You're adding a tool for domain modeling where correctness really matters. Encode your business rules in types, and let the compiler catch the bugs.

Questions?

**[Open floor for Q&A]**

---

## Production Notes

### Timing Checkpoints
- **Minute 8:** Should be starting Act 2 (DU introduction)
- **Minute 15:** Should be starting Act 3 (pattern matching)
- **Minute 22:** Should be starting Act 4 (explicit eligibility)
- **Minute 28:** Wrap-up

### Role Clarity
- **DEV H (High-level):** 
  - Explains WHY (domain constraints, bug patterns, design decisions)
  - Describes WHAT is happening conceptually
  - Never touches keyboard
  - Addresses audience directly
  - Provides context before code examples
  
- **DEV L (Low-level):**
  - Shows HOW (types code, demonstrates compiler behavior)
  - Executes specific code demonstrations
  - Stays at keyboard full-time
  - Responds to H's directions ("show them...", "try to...")
  - Minimal speaking (mostly code and brief confirmations)

### Presenter Handoff Pattern
H describes concept → L demonstrates in code → H explains implications

Example:
- **H:** "The illegal state is now unrepresentable. **[DEV L, rebuild the project.]**"
- **L:** _(builds)_ "Build succeeded."
- **H:** "Good. Now let's see what happens to the C# code."

### If Running Behind
- Abbreviate Scene 2 (skip showing both correct and buggy versions)
- In Scene 5, show only C# pattern matching (skip F# version)
- In Scene 6, skip creating all four customer examples

### Physical Staging Tips
- DEV H should use podium space, move around, make eye contact
- DEV L should be fully focused on screen, minimal movement
- H directs flow, L executes demonstrations
- Clear separation prevents confusion about who's speaking when

### Audience Engagement
- H handles all audience questions
- H pauses for reactions after key concepts
- L can show code in response to questions if H asks
- Q&A at end conducted by H, with L available for code demos

### Code Files Needed
- **F# project:** CustomerDomain with Library.fs (3 versions ready to switch)
- **C# project:** CustomerDemo with Program.cs
- Pre-stage each version for quick transitions
- Keep both windows visible side-by-side

### Key Phrases for Role Reinforcement
- **H says:** "Show them...", "Try to...", "Display...", "Type..."
- **L says:** "Here's...", "Building...", "Done.", _(minimal narration)_

---

**END OF SCRIPT**

---

## Extension Points

This script format works well for:
- VIP tier addition (H explains business case, L implements)
- SAFE Stack demo (H explains architecture, L shows running app)
- Testing demonstrations (H explains testing philosophy, L writes tests)
- More complex domain modeling (H describes domain, L models it)

The high-low split creates clear role separation and allows H to focus on concepts while L maintains code flow.
