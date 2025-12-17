# Add-on Module: VIP Refactoring Demo

**Duration:** 5 minutes  
**Insertion Point:** After Scene 11B (bridge ending) of core script  
**Style:** Dramatic demo with tension-building  
**Purpose:** Show compiler-guided refactoring across F# and C# boundaries

---

## Physical Setup

**[DEV A]** - At F# window  
**[DEV 1]** - At C# window  
**Split screen visible** - Show both environments reacting to changes

---

## Entry Dialogue (from Scene 11B bridge)

**[DEV A]**  
Great questions. Let's keep going. First, let me show you how we can refactor this even more...

**[DEV 1]**  
What are we refactoring?

**[DEV A]**  
Well, we have Standard tier customers. What if the business wants to add a VIP tier with better discounts?

**[DEV 1]**  
Oh, like a premium tier above Standard?

**[DEV A]**  
Exactly. 15% discount instead of 10%. Watch what happens when I add it.

---

## Scene 1: Making the F# Change (2 min)

**[DEV A]** _(at F# window, typing)_

**[Shows current code:]**
```fsharp
type CustomerTier =
    | Standard
    | VIP  // ← Already exists from iteration!
```

**[DEV A]**  
Wait, we already have VIP in the type definition. But we're not using it yet.

**[DEV 1]**  
Right, the demo has only been creating Standard tier customers.

**[DEV A]**  
So the type system is already prepared for VIP. Let's actually use it. I'll add a helper function.

**[Adds to F# code:]**
```fsharp
module Customer =
    
    let createStandard id =
        let registered = { 
            Id = id
            Tier = Some Standard 
        }
        Registered registered
    
    let createVIP id =
        let registered = { 
            Id = id
            Tier = Some VIP 
        }
        Registered registered
    
    let createGuest id =
        Guest { Id = id }
```

**[DEV A]**  
Now let's rebuild the F# project.

**[DEV A rebuilds, success]**

**[DEV 1]**  
F# is happy. What about my C# code?

---

## Scene 2: The C# Compiler Reacts (2 min)

**[DEV A]** _(gestures to C# window)_  
Check your error list.

**[DEV 1]** _(switches to C# window, looks at CalculateTotal method)_

**[Screen shows existing C# code with squiggly lines:]**
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
            {
                return spend * 0.9m;  // 10% discount
            }
            // ⚠️ Warning: Not all cases handled - VIP is missing!
        }
    }
    return spend;
}
```

**[DEV 1]**  
Huh. I'm getting warnings now. "Not all cases handled."

**[DEV A]**  
That's the F# type system telling your C# code: "Hey, you forgot about VIP!"

**[DEV 1]**  
Even though I didn't write the F# code, my C# code now has warnings?

**[DEV A]**  
Exactly. The discriminated union has two cases: Standard and VIP. Your code only handles Standard.

**[DEV 1]**  
So the compiler is guiding me to fix it.

**[DEV A]**  
Right. What happens if a VIP customer hits this code right now?

**[DEV 1]** _(looks at logic)_  
Oh. They'd get no discount. The code would fall through to `return spend`.

**[DEV A]**  
Bug avoided at compile time. Let's fix it.

---

## Scene 3: Fixing the C# Code (1 min)

**[DEV 1]** _(typing)_

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
            {
                return spend * 0.9m;  // 10% discount
            }
            
            if (tier.IsVIP && spend >= 100)
            {
                return spend * 0.85m;  // 15% discount for VIP!
            }
        }
    }
    return spend;
}
```

**[DEV 1]**  
There. Now VIP customers get 15% off.

**[DEV A]**  
And the warnings are gone?

**[DEV 1]** _(checks)_  
Yep. Green across the board.

**[DEV A]**  
That's cross-language refactoring safety. The F# type guided your C# implementation.

**[DEV 1]**  
Let me test it with a VIP customer.

**[Adds to Program.cs:]**
```csharp
var vipCustomer = Customer.createVIP("Alice");
var vipTotal = CalculateTotal(vipCustomer, 100m);
Console.WriteLine($"VIP Customer Alice: £{vipTotal}");
// Output: VIP Customer Alice: £85.00
```

**[Runs program]**

**[Console shows:]**
```
Standard Tier: John - £90.00
VIP Tier: Alice - £85.00
Guest: Bob - £50.00
```

**[DEV A]**  
Perfect. Three tiers, all working correctly.

---

## Scene 4: The Payoff (30 seconds)

**[DEV 1]** _(turns to audience)_  
So when we added VIP to the F# type, the C# compiler immediately told us everywhere we needed to update.

**[DEV A]**  
That's what "making illegal states unrepresentable" buys you. Not just at initial development, but during maintenance and refactoring.

**[DEV 1]**  
The type system is your refactoring safety net.

**[DEV A]**  
Exactly. Now, should we show how this works in a real web application?

**[Transition to SAFE Stack Intro, or wrap up here]**

---

## Exit Options

### If continuing to SAFE modules:
**[DEV 1]**  
Yes! Let's see this in a web context.

**[DEV A]**  
Alright, let me show you the SAFE Stack...

**[Continue to SAFE Intro add-on]**

### If wrapping up here:
**[DEV 1]**  
I think we've made the point. This is really powerful.

**[DEV A]** _(to audience)_  
Any questions about the refactoring process?

**[Open floor for Q&A, then conclude]**

---

## Production Notes

### Key Moments
- **Tension Build:** When DEV 1 sees warnings appear in their C# code
- **Aha Moment:** Realizing the F# change guided C# refactoring
- **Payoff:** Running the program with VIP customer successfully

### If Running Behind
- Skip the helper function creation in F#
- Just add VIP case to existing switch/if logic in C#
- Still shows the warning/guidance behavior

### Common Questions to Expect
- "What if I ignore the warnings?" → Code compiles, but runtime behavior wrong
- "Does this work with switch expressions?" → Yes, even better! (can demo if time)
- "What about C# 12 pattern matching?" → Improves even more (mention briefly)

### Graceful Exit (if audience disengaged)
**[DEV A]**  
Actually, you get the idea. Let's move on...

**[Skip to wrap-up or next add-on]**

---

**END OF VIP REFACTORING ADD-ON**

**Next possible modules:**
- → SAFE Stack Intro (if web-focused audience)
- → Advanced C# Patterns (if C#-heavy audience)
- → Wrap-up and Q&A (if time-constrained)
