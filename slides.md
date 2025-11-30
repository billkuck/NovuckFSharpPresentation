# F# Type-Driven Development: Slides

## Slide 1: Title Slide
**F# Type-Driven Development**
*From C# to Correctness*

- [Your Names]
- [Date]
- [User Group Name]

---

## Slide 2: Today's Journey

**What We'll Explore:**

1. 🔧 Progressive Domain Modeling (C# → F#)
2. 🔗 C# ↔ F# Interoperability
3. 🌐 SAFE Stack Web Application

**Promise:** Make more bugs impossible at compile time

---

## Slide 3: The Business Rule

**Scenario:** Customer Discount System

> "Eligible Registered Customers get 10% discount when they spend £100 or more"

**Questions This Raises:**
- Can a customer be Eligible but NOT Registered?
- Can a Guest be Eligible?
- How do we prevent invalid states?

---

## Slide 4: Key Terminology - Correctness

**Compile-Time vs. Runtime**

| Aspect | Compile-Time | Runtime |
|--------|-------------|---------|
| **When** | During build | During execution |
| **Catches** | Type errors, missing cases | Logic errors, invalid data |
| **Cost** | Free - instant feedback | Expensive - requires testing, monitoring |
| **Examples** | Missing switch case, wrong type | Null reference, illegal state |

**Goal:** Push as many errors LEFT (toward compile-time) as possible

---

## Slide 5: Key Terminology - Illegal States

**Making Illegal States Unrepresentable**

**Bad:** Type allows invalid combinations
```csharp
class Customer {
  bool IsEligible;
  bool IsRegistered;
}
// Can create: IsEligible=true, IsRegistered=false ❌
```

**Good:** Type prevents invalid combinations
```fsharp
type Customer =
  | Eligible of RegisteredCustomer
  | Registered of RegisteredCustomer
  | Guest of UnregisteredCustomer
// Cannot create eligible guest! ✅
```

**Principle:** If it compiles, it should be correct

---

## Slide 6: Key Terminology - Type Safety

**What is Type Safety?**

The compiler guarantees:
- You cannot pass wrong types to functions
- You cannot forget to handle a case
- You cannot create invalid domain states

**Implications:**

| With Strong Types | Without Strong Types |
|-------------------|---------------------|
| ✅ Compiler finds bugs | ❌ Runtime discovers bugs |
| ✅ Fewer unit tests needed | ❌ More tests required |
| ✅ Refactoring is safe | ❌ Breaking changes hidden |
| ✅ Self-documenting code | ❌ Need extensive docs |

---

## Slide 7: Key Terminology - Discriminated Unions

**What Are They?**

A type that can be ONE OF several cases (OR types)

```fsharp
type Customer =
  | Eligible of RegisteredCustomer
  | Registered of RegisteredCustomer
  | Guest of UnregisteredCustomer
```

**Think of it as:** Type-safe enums with associated data

**Compiler Enforces:**
- Must handle all cases in pattern matching
- Cannot mix cases incorrectly
- Invalid combinations impossible

---

## Slide 8: Key Terminology - Pattern Matching

**Exhaustive Case Analysis**

**C# 8.0+ Switch Expressions:**
```csharp
var discount = customer switch
{
    Customer.Eligible _ when spend >= 100 => spend * 0.1m,
    Customer.Registered _ => 0,
    Customer.Guest _ => 0,
    // ⚠️ Compiler warns if you forget a case!
};
```

**Benefits:**
- Compiler ensures all cases handled
- Safe refactoring - add new case, compiler shows all places to update
- Self-documenting logic

---

## Slide 9: The Three Iterations

**Progressive Enhancement:**

**Iteration 0: Naive**
- Boolean flags
- ❌ Illegal states possible
- ❌ Easy to forget checks

**Iteration 1: Discriminated Union + Boolean**
- Separate types for Registered/Guest
- ✅ Can't be eligible without registered
- ⚠️ Still using boolean flag

**Iteration 2: Explicit Domain Concepts**
- Eligibility is part of the type
- ✅ Domain language is explicit
- ✅ Impossible states are impossible

---

## Slide 10: Code Complexity Impact

**Traditional Approach (Runtime Validation):**

```csharp
decimal CalculateTotal(Customer customer, decimal spend)
{
    if (!customer.IsRegistered && customer.IsEligible)
        throw new InvalidStateException("Guest cannot be eligible!");
    
    if (customer.IsEligible && !customer.IsRegistered)
        throw new InvalidStateException("Eligible requires registered!");
    
    // Add 10 more validation checks...
    // Add logging...
    // Add unit tests for all invalid combinations...
}
```

**Type-Driven Approach (Compile-Time Safety):**

```csharp
decimal CalculateTotal(Customer customer, decimal spend)
{
    return customer switch
    {
        Customer.Eligible _ when spend >= 100 => spend * 0.9m,
        _ => spend
    };
    // That's it. Invalid states can't exist.
}
```

---

## Slide 11: Testing Implications

**Without Type Safety:**
- ❌ Test valid combinations
- ❌ Test ALL invalid combinations
- ❌ Test boundary conditions
- ❌ Test null/missing values
- ❌ Test state transitions
- Result: 100+ unit tests

**With Type Safety:**
- ✅ Test business logic only
- ✅ Invalid states can't be created
- ✅ Compiler ensures exhaustiveness
- ✅ Fewer integration tests needed
- Result: 10-20 focused tests

**Savings:** ~80% fewer tests, higher confidence

---

## Slide 12: Refactoring Safety

**Scenario:** Add "VIP" customer type

**Without Strong Types:**
1. Add new type/flag
2. Search codebase for "customer"
3. Hope you found everything
4. Run all tests
5. Deploy and hope
6. ⚠️ Production bug: Forgot one place!

**With Strong Types:**
1. Add new DU case: `| VIP of RegisteredCustomer`
2. Build
3. Compiler shows red squigglies everywhere that needs updating
4. Fix each location
5. Build succeeds = refactor complete ✅

---

## Slide 13: F# + C# = Best of Both Worlds

**Architecture Pattern:**

```
┌─────────────────────────────────┐
│   C# Application Layer          │
│   - APIs, Controllers           │
│   - Business Logic              │
│   - Pattern Matching            │
└─────────────┬───────────────────┘
              │ references
┌─────────────▼───────────────────┐
│   F# Domain Layer               │
│   - Type Definitions            │
│   - Domain Models               │
│   - Constraints Encoded         │
└─────────────────────────────────┘
```

**Benefits:**
- Team uses familiar C#
- Domain gets F# type safety
- Gradual adoption path
- Both languages benefit

---

## Slide 14: Real-World Adoption Strategy

**Start Small:**

1. **Week 1:** One domain model in F#
2. **Week 2:** One background service
3. **Week 3:** One API endpoint
4. **Month 2:** Shared validation library
5. **Month 3:** Core domain layer
6. **Result:** Team sees value, adoption grows organically

**Low Risk:**
- F# library is just a .NET DLL
- No runtime dependencies
- Can mix freely in solution
- Easy to revert if needed

---

## Slide 15: Common Concerns Addressed

**"F# is too different!"**
→ Start with types only, write logic in C#

**"Team doesn't know F#!"**
→ F# type definitions are simple, C# devs learn in hours

**"What about tooling?"**
→ VS Code, Visual Studio, Rider all have excellent F# support

**"Performance?"**
→ F# compiles to same IL as C#, zero overhead

**"Hiring?"**
→ C# developers can learn F# types in a day, full F# in weeks

---

## Slide 16: Key Takeaways

**Remember These Five Points:**

1. 🎯 **Push errors to compile-time** - cheaper to fix
2. 🚫 **Make illegal states unrepresentable** - prevent bugs entirely
3. 🔄 **Pattern matching ensures exhaustiveness** - safe refactoring
4. 🤝 **F# + C# work together seamlessly** - gradual adoption
5. 📉 **Fewer tests, more confidence** - types prove correctness

**Bottom Line:** Let the type system do the work for you

---

## Slide 17: Resources

**Learning F#:**
- 🌐 fsharpforfunandprofit.com - Excellent tutorials
- 📚 "Domain Modeling Made Functional" by Scott Wlaschin
- 📖 "Essential F#" by Ian Russell
- 💬 F# Slack Community (fsharp.org/slack)

**SAFE Stack:**
- 🌐 safe-stack.github.io
- 📦 Templates: `dotnet new -i SAFE.Template`

**Today's Code:**
- 📂 github.com/[your-repo] - All examples from this talk

---

## Slide 18: Questions?

**Let's Discuss:**

- How could this apply to your projects?
- What domain model causes you pain?
- Where do you spend time on validation tests?

**Try It Monday:**
- Pick one small domain model
- Define it in F#
- Consume from C#
- See the difference!

**Contact:**
- [Your email/social]
- [Co-presenter email/social]

---

## Bonus Slide: Quick F# Type Syntax

**For C# Developers:**

| F# | C# Equivalent |
|----|---------------|
| `type Person = { Name: string }` | `record Person(string Name);` |
| `type Option = Some \| None` | `enum Option { Some, None }` |
| `type Result = Ok \| Error of string` | No direct equivalent |
| `let x = 42` | `var x = 42;` |
| `let add x y = x + y` | `int Add(int x, int y) => x + y;` |

**That's 90% of what you need to know!**

---

## Bonus Slide: Type-Driven Development Flow

```
┌──────────────────┐
│ 1. Model Domain  │
│    with Types    │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ 2. Compiler      │
│    Validates     │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ 3. Implement     │
│    Logic         │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ 4. Refactor with │
│    Confidence    │
└──────────────────┘
```

**Contrast with Test-Driven:**
- Types = compile-time tests
- Faster feedback loop
- More comprehensive coverage
- Complements (doesn't replace) unit tests
