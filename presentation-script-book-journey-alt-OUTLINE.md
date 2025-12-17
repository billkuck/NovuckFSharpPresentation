# Journey to Correctness: Quick Outline

**Duration:** 30 minutes | **Format:** Two presenters, live code  
**Goal:** Show F# type safety in action with C# interop

---

## The Journey (5 Acts)

### **ACT 1: THE PROBLEM** (8 min)
- **Scene 1** (3 min): Welcome, poll audience, set expectations
- **Scene 2** (5 min): Business rule - "Eligible registered customers get 10% discount at £100+"
  - Show C# class with two booleans: `IsEligible`, `IsRegistered`
  - **Problem**: Can create illegal state (eligible but not registered)
  - Show bug: forgetting to check both flags compiles fine

**Checkpoint: Minute 8**

### **ACT 2: DISCRIMINATED UNIONS** (7 min)
- **Scene 3** (5 min): F# discriminated union makes illegal state unrepresentable
  ```fsharp
  type Customer = Registered of RegisteredCustomer | Guest of UnregisteredCustomer
  ```
- **Scene 4** (2 min): C# consumes via factory methods - can't create invalid customers anymore

**Checkpoint: Minute 15**

### **ACT 3: PATTERN MATCHING** (7 min)
- Show pattern matching in both C# and F#
- **Key**: Compiler enforces checking all cases
- Both languages get same type safety

**Checkpoint: Minute 22**

### **ACT 4: EXPLICIT ELIGIBILITY** (6 min)
- Final refinement: Three explicit states (`Eligible`, `Registered`, `Guest`)
- Result: C# code simpler than original AND safer

**Checkpoint: Minute 28**

### **ACT 5: WRAP-UP** (2 min)
- **Principle**: Encode business rules in types, not runtime validation
- **Advice**: Start small, use F# for core logic, consume from C#

---

## Key Concepts
1. **Illegal states made unrepresentable** - Type structure prevents invalid data
2. **Compile-time safety** - Compiler catches missing case checks
3. **Practical interop** - F# generates factory methods and properties for C#

## Roles
- **DEV A**: F# code, theory | **DEV 1**: C# code, practical implications

## Materials
- F# project: `CustomerDomain/Library.fs` (3 versions)
- C# project: `CustomerDemo/Program.cs`
- Split-screen setup (F# left, C# right)
