# Journey to Correctness: A Two-Presenter Script

**Format:** Screenplay style with embedded code  
**Duration:** 35-40 minutes (core), extensible with add-ons  
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

# ACT 1: INTRODUCTION & THE PROBLEM (10 minutes)

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

### Scene 2: The Business Requirement (4 min)

**[DEV 1]** _(moves to keyboard, opens file)_  
Alright, here's our scenario. We're building a discount system for customers.

**[Screen shows: Program.cs with naive Customer class]**

```csharp
class Customer
{
    public string Id { get; set; }
    public bool IsStandard { get; set; }
    public bool IsRegistered { get; set; }
}
```

**[DEV 1]**  
The business rule is: "Standard tier registered customers get 10% discount when they spend £100 or more."

**[DEV A]** _(from podium)_  
And we have some test cases to verify it works?

**[DEV 1]**  
Yeah, here's our sample data:

**[Screen shows test data table:]**

| Customer | IsStandard | IsRegistered | Spend | Expected Total | Notes |
|----------|------------|--------------|-------|----------------|-------|
| John     | true       | true         | £100  | £90            | ✓ Valid: Gets discount |
| Mary     | true       | true         | £99   | £99            | ✓ Valid: Below threshold |
| Richard  | false      | true         | £100  | £100           | ✓ Valid: No discount tier |
| Sarah    | false      | false        | £100  | £100           | ✓ Valid: Guest |
| Grinch   | true       | false        | £100  | ???            | ❌ Invalid: Standard but not registered! |

**[DEV 1]**  
Most of these are valid states. But look at that last row - Grinch is Standard tier but not registered. That shouldn't be possible according to the business rules.

**[DEV A]** _(walking toward screen)_  
But nothing stops you from creating it?

**[DEV 1]**  
Nope. The type system doesn't know about the business rule.

**[DEV A]**  
That's the problem. The type system doesn't encode the constraint.

**[DEV 1]**  
Yes, and when we write the discount calculation logic, we'll probably have bugs because of this.

**[DEV A]**  
Let's see that logic.

**[DEV 1]** _(scrolls to CalculateTotal method)_

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    if (customer.IsRegistered && customer.IsStandard && spend >= 100)
    {
        return spend * 0.9m;  // 10% discount
    }
    return spend;
}
```

**[DEV A]**  
Okay, so that checks both flags. But what happens if someone forgets to check `IsRegistered`?

**[DEV 1]**  
Like this?

**[DEV 1 types the buggy version:]**

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    // BUG: Forgot to check IsRegistered!
    if (customer.IsStandard && spend >= 100)
    {
        return spend * 0.9m;  // 10% discount
    }
    return spend;
}
```

**[DEV A]**  
Exactly. That compiles fine, but it's wrong.

**[DEV 1]**  
Yeah, now anyone with `IsStandard = true` gets the discount, even if `IsRegistered = false`.

**[DEV A]**  
Right. And the compiler doesn't warn you about that mistake.

**[DEV 1]**  
No. It compiles fine. You'd find it in testing... maybe.

**[DEV A]** _(pause for effect)_  
What if we could make this impossible to mess up?

---

### Scene 3: The Journey Begins (3 min)

**[DEV 1]**  
How would you do that?

**[DEV A]** _(gestures toward F# window)_  
We use F#'s type system to encode the business rules. Let me show you the first iteration.

**[DEV A takes keyboard, switches to F# file]**

**[Screen shows: Library.fs]**

```fsharp
namespace CustomerDomain

type Customer = {
    Id: string
    IsStandard: bool
    IsRegistered: bool
}
```

**[DEV 1]**  
Wait, that's the same problem. Just in F# syntax.

**[DEV A]**  
Exactly! This is valid F#, but we're not using its strengths yet. This has the same issues as the C# version.

**[DEV 1]**  
So what's the improvement?

**[DEV A]**  
We're going to take a journey. Each step, we'll improve the model, and you'll see the C# consumer code get simpler and safer.

**[DEV 1]**  
And this actually compiles? Like, the C# code can call this F# library?

**[DEV A]**  
Yes. That's the whole point. This isn't theoretical. Ready to see iteration 1?

**[DEV 1]**  
Let's go.

---

## TIME CHECK: Minute 10

# ACT 2: FIRST DU IMPROVEMENTS (10 minutes)

---

### Scene 4: Introducing Discriminated Unions (5 min)

**[DEV A]** _(typing in F# window)_  
Here's the key insight: a customer is _either_ registered _or_ a guest. Not both. Not neither.

**[Updates F# code to:]**

```fsharp
namespace CustomerDomain

type RegisteredCustomer = {
    Id: string
    IsStandard: bool
}

// guest
type UnregisteredCustomer = {
    Id: string
}

type Customer =
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
```

**[DEV A]**  
This is called a discriminated union. It's like an enum on steroids.

**[DEV 1]** _(leaning in)_  
So `Customer` is either `Registered` with some data, or `Guest` with different data?

**[DEV A]**  
Exactly. And notice: you _cannot_ be Standard tier without being Registered.

**[DEV 1]** _(pointing at UnregisteredCustomer)_  
Because `UnregisteredCustomer` doesn't even have an `IsStandard` field.

**[DEV A]**  
Right. The illegal state is literally unrepresentable.

**[DEV 1]**  
Okay, that's clever. But what does this do to my C# code?

**[DEV A]** _(gestures toward C# window)_  
Great question. Let's rebuild and see.

**[DEV A rebuilds F# project, switches to C# window]**

**[DEV 1]** _(takes keyboard, looks at Program.cs)_  
Oh. My old code doesn't compile anymore.

```csharp
// This breaks:
var customer = new Customer { Id = "123", IsStandard = true, IsRegistered = false };
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
    IsStandard = true
};
var customer = Customer.NewRegistered(registeredData);
```

**[DEV A]**  
Exactly. And can you make an illegal state?

**[DEV 1]** _(tries typing)_  
No... there's no way to be Standard without using `NewRegistered`. The compiler won't let me.

**[DEV A]**  
That's the power of making illegal states unrepresentable.

---

### Scene 5: Pattern Matching Emerges (5 min)

**[DEV 1]**  
Okay, but now how do I check if they're Standard tier? My old if-statement doesn't work.

```csharp
// Old code:
if (customer.IsRegistered && customer.IsStandard)
// Error: Customer doesn't have these properties
```

**[DEV A]**  
You need to check which case of the union it is first. Let me show you.

**[DEV A]** _(takes keyboard, types)_

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    if (customer.IsRegistered)
    {
        var registered = (RegisteredCustomer)customer;
        if (registered.IsStandard && spend >= 100)
        {
            return spend * 0.9m;
        }
    }
    return spend;
}
```

**[DEV 1]**  
Okay, check if registered, cast it, then check if standard. That's straightforward.

**[DEV A]**  
Right. And the key point: you _must_ check `IsRegistered` first. If you try to access `IsStandard` without that check, you get a compile error.

**[DEV 1]**  
The F# type system forces the C# code to be correct.

**[DEV A]**  
Exactly. And if you prefer, C# switch expressions can do pattern matching here too - but the if-statement is more explicit for demos.

**[DEV A]** _(to audience)_  
Quick note: If you're still on .NET Framework or older C# versions without switch expressions, the if-statement pattern works perfectly. Pattern matching just gives you more options.

**[DEV 1]**  
So the logic _has_ to be correct by construction.

**[DEV A]**  
Exactly. But we're not done yet. There's still a problem.

**[DEV 1]**  
What problem?

---

## TIME CHECK: Minute 20

# ACT 3: EXPLICIT TIERS (10 minutes)

---

### Scene 6: Discovering the Next Problem (3 min)

**[DEV A]**  
What happens when the business wants to add a VIP tier with a 15% discount?

**[DEV 1]** _(looks at code)_  
I'd add another boolean? `IsVIP`?

**[DEV A]**  
Right. And now we're back to the same problem. Can you be both Standard and VIP?

**[DEV 1]**  
That doesn't make sense. You'd be one or the other.

**[DEV A]**  
Exactly. But with booleans, nothing stops someone from setting both to true. Or both to false.

**[DEV 1]**  
So we're still modeling tiers incorrectly. We have `IsStandard` as a boolean, but tiers should be mutually exclusive.

**[DEV A]**  
Right. And what about customers who are registered but haven't chosen a paid tier yet?

**[DEV 1]**  
That would be `IsStandard = false` and `IsVIP = false`... which looks the same as a guest in the old model.

**[DEV A]**  
Exactly. The types aren't expressing the business rules. What if we made the tiers explicit?

**[DEV 1]**  
Like... another discriminated union?

**[DEV A]**  
Now you're thinking in types. Let me show you.

---

### Scene 7: Nested Discriminated Unions (4 min)

**[DEV A takes keyboard, updates F# code]**

```fsharp
namespace CustomerDomain

type CustomerTier =
    | Standard
    | VIP

type RegisteredCustomer = {
    Id: string
    Tier: CustomerTier option  // Some(tier) or None
}

type UnregisteredCustomer = {
    Id: string
}

type Customer =
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
```

**[DEV 1]**  
Whoa. So `CustomerTier` is its own union with `Standard` and `VIP` cases?

**[DEV A]**  
Right. And a registered customer has an _optional_ tier. `None` means they're registered but haven't picked a tier yet.

**[DEV 1]**  
That's three states: Guest, Registered-no-tier, Registered-with-tier.

**[DEV A]**  
And if they have a tier, it's explicitly `Standard` or `VIP`. No booleans.

**[DEV 1]** _(stares at screen)_  
That's... actually really clear. Let me see what happens in C#.

**[DEV A]** _(rebuilds, hands keyboard to DEV 1)_

**[DEV 1 switches to C# window, types]**

```csharp
var registered = new RegisteredCustomer
{
    Id = "John",
    Tier = FSharpOption<CustomerTier>.Some(CustomerTier.Standard)
};
var customer = Customer.NewRegistered(registered);
```

**[DEV 1]**  
Okay, `FSharpOption` is a bit verbose, but it's clear what it means.

**[DEV A]**  
Yes, and there are helper methods. You can use `OptionModule.Some()` or even extension methods if you set them up.

**[DEV 1]**  
Let me update the discount logic.

---

### Scene 8: The Logic Becomes Obvious (3 min)

**[DEV 1]** _(typing in C# window)_

```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    if (customer.IsRegistered)
    {
        var registered = (RegisteredCustomer)customer;
        if (FSharpOption<CustomerTier>.get_IsSome(registered.Tier))
        {
            var tier = registered.Tier.Value;
            if (tier.IsStandard && spend >= 100)
                return spend * 0.9m;
            if (tier.IsVIP && spend >= 100)
                return spend * 0.85m;
        }
    }
    return spend;
}
```

**[DEV 1]**  
There we go. Check registered, check has tier, then check which tier.

**[DEV A]** _(watching)_  
That's clear and explicit. Each check is on its own line. You can't screw it up - if you try to check `IsStandard` without first checking `IsRegistered` and `IsSome`, it won't compile.

**[DEV 1]**  
Though I have to admit, the nested checks are still pretty verbose.

**[DEV A]**  
True. That's just the nature of checking optional values and nested discriminated unions. Want to see what the equivalent F# looks like?

**[DEV A]** _(switches to F# window, adds a function)_

```fsharp
let calculateTotal (customer: Customer) (spend: decimal) =
    match customer with
    | Registered { Tier = Some Standard } when spend >= 100m -> spend * 0.9m
    | Registered { Tier = Some VIP } when spend >= 100m -> spend * 0.85m
    | _ -> spend
```

**[DEV 1]**  
Wow. That's... way more concise.

**[DEV A]**  
That's the power of F#'s pattern matching syntax. It's designed for exactly this kind of data processing. The safety guarantees are the same in both languages - F# just has more specialized syntax for expressing it.

**[DEV 1]**  
So both versions are correct and type-safe. F# just reads more naturally for this kind of logic.

**[DEV A]**  
Exactly. That's when you might put the business logic in F# - not because C# can't be type-safe, but because F# excels at expressing data transformations concisely.

**[DEV A]**  
Exactly. The type system is guiding you to write correct code.

**[DEV 1]**  
Can we clean up the `FSharpOption` stuff?

**[DEV A]**  
Definitely. You can write extension methods or helper functions. But first, let's run this and make sure it works.

**[DEV 1]** _(runs the program)_

**[Console output:]**
```
Standard Tier: John
Total: £90.00

No Tier: Jane
Total: £150.00

Guest: Bob
Total: £50.00
```

**[DEV A]**  
There we go. Three customers, three different states, all handled correctly.

---

## TIME CHECK: Minute 30

# ACT 4: C# INTEROP & WRAP-UP (10 minutes)

---

### Scene 9: Making C# Consumption Prettier (4 min)

**[DEV A]**  
Let's make the C# side less verbose. We can add some helper methods.

**[DEV A takes keyboard, creates new file in C# project]**

```csharp
public static class CustomerExtensions
{
    public static Customer CreateStandard(string id)
    {
        var registered = new RegisteredCustomer
        {
            Id = id,
            Tier = FSharpOption<CustomerTier>.Some(CustomerTier.Standard)
        };
        return Customer.NewRegistered(registered);
    }

    public static Customer CreateGuest(string id)
    {
        var unregistered = new UnregisteredCustomer { Id = id };
        return Customer.NewGuest(unregistered);
    }

    public static bool IsStandardTier(this Customer customer)
    {
        if (!customer.IsRegistered) return false;
        var registered = (RegisteredCustomer)customer;
        if (!FSharpOption<CustomerTier>.get_IsSome(registered.Tier)) return false;
        return registered.Tier.Value.IsStandard;
    }
}
```

**[DEV 1]** _(looking over)_  
Ah, so now the C# code can be cleaner.

**[DEV A]**  
Right. Let's update the main program.

**[Updates Program.cs:]**

```csharp
var john = CustomerExtensions.CreateStandard("John");
var jane = Customer.NewRegistered(new RegisteredCustomer { Id = "Jane", Tier = null });
var bob = CustomerExtensions.CreateGuest("Bob");

Console.WriteLine($"Is John standard tier? {john.IsStandardTier()}");
```

**[DEV 1]**  
That's much nicer. The F# types are doing the heavy lifting, and we're just wrapping them in C#-friendly APIs.

**[DEV A]**  
Exactly. And all the type safety is preserved.

---

### Scene 10: The Bigger Picture (3 min)

**[DEV 1]** _(steps back from screen)_  
So let's recap what just happened. We started with booleans and illegal states.

**[DEV A]**  
Yes, and we progressively refined the model using discriminated unions.

**[DEV 1]**  
First, we made `Registered` vs `Guest` explicit.

**[DEV A]**  
Then we made `Standard` and `VIP` explicit tiers, using `option` to handle "no tier yet."

**[DEV 1]**  
And at every step, the C# code got _clearer_, not more complicated.

**[DEV A]**  
Because the F# types encoded the business rules. The C# compiler enforces those rules when you consume the library.

**[DEV 1]**  
So this isn't "F# for F# developers." This is "F# for better .NET applications."

**[DEV A]**  
Exactly. You can keep your C# UI, your C# APIs, your C# tests. Just use F# for the core domain model where correctness matters most.

---

### Scene 11A: Wrap-up (Standalone Ending) (3 min)

**[Use this ending if NO add-ons follow]**

**[DEV 1]**  
What are the key takeaways here?

**[DEV A]** _(counts on fingers)_  
One: Make illegal states unrepresentable. If your business rules say "you must be registered to be Standard tier," the type system should enforce that.

**[DEV 1]**  
Two: Discriminated unions are incredibly powerful. They let you model "this OR that" in a way C# enums can't.

**[DEV A]**  
Three: F# and C# interop works really well. You're not rewriting your whole codebase. You're adding a library.

**[DEV 1]**  
And four: The compiler becomes your pair programmer. It catches mistakes before runtime.

**[DEV A]** _(to audience)_  
So if you're curious about F#, start small. Pick one domain model. Model it with discriminated unions. Consume it from C#. See how it feels.

**[DEV 1]**  
And let us know how it goes! Questions?

**[Open floor for Q&A]**

**[End of core script - standalone version]**

---

### Scene 11B: Bridge to Add-ons (Alternate Ending) (3 min)

**[Use this ending if add-ons follow]**

**[DEV 1]**  
Okay, so we've seen how F# types make our domain model better. But this is still a console app.

**[DEV A]**  
Yes, and you're probably wondering: what about real applications?

**[DEV 1]**  
Right. Like, what if I wanted to refactor further? Or integrate this with a web API?

**[DEV A]**  
Great questions. Let's keep going. First, let me show you how we can refactor this even more...

**[Transition to VIP Refactoring add-on, or other selected modules]**

**[End of core script - bridge version]**

---

## Production Notes

### Timing Checkpoints
- **Minute 10:** Should be starting Act 2 (DU introduction)
- **Minute 20:** Should be starting Act 3 (explicit tiers)
- **Minute 30:** Should be in Act 4 (C# interop)
- **Minute 35-40:** Wrap-up or transition to add-ons

### If Running Behind
- Abbreviate Scene 2 (business requirement setup)
- Skip creating full `CustomerExtensions` class, just show concept
- Use pre-built code instead of live typing

### Physical Staging Tips
- Keep energy up with movement between podium and screen
- Dev A can gesture toward screen from podium to direct attention
- Dev 1 should step back during Dev A's typing moments
- Make keyboard handoffs smooth (verbal cue: "want to try that?")

### Audience Engagement
- Pause for reactions after "illegal state is unrepresentable"
- Watch for confused faces during FSharpOption - offer to explain
- Invite questions during Act 3 transition

### Code Switching Notes
- F# file: `src/CustomerDomain/Library.fs`
- C# file: `src/CustomerDemo/Program.cs`
- Keep both windows visible in split-screen when comparing
- Single window during deep-dive moments

---

**END OF CORE SCRIPT**

---

## Add-on Attachment Points

**After Scene 11B (Bridge ending):**
- → VIP Refactoring add-on (5 min)
- → Advanced C# Patterns add-on (5-7 min)

**After Scene 11A or 11B:**
- → SAFE Stack Intro add-on (3 min)
- → (Then) SAFE Server add-on (5-7 min)
- → (Then) SAFE Elmish add-on (10-12 min)

**After any SAFE module:**
- → Copilot add-on (4 min)
