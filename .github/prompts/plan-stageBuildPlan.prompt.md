---
title: Presentation Stage Build Plan
description: Instructions for building presentation stages as git worktrees with progressive domain model refinement from C# to F#
tags: [presentation, git-worktree, domain-modeling, fsharp, csharp]
author: GitHub Copilot
version: 1.0.0
date: 2025-12-14
---

# Presentation Stage Build Plan

## Overview
This document defines how to build each presentation stage using git worktrees (for efficient building without branch switching), allowing us to demonstrate progressive refinement of a domain model from C# to F# while maintaining working code at each stage.

**Build Process:** Worktrees are used ONLY during the build process to create the stage branches efficiently.

**Presentation Approach:** Live code the changes during presentation (Stage 1 → Stage 2 → Stage 3). The pre-built stage branches are fallback options if live coding goes wrong - you can quickly `git checkout` a working stage and continue.

---

## Repository Structure Strategy

### Main Branch
- Contains: Documentation, scripts, plans, .gitignore
- **Does NOT contain**: Working demo code (that lives in stage branches)

### Stage Branches (Sequential Development)
Each stage branches from the previous stage, creating a linear progression.

**Branch Naming Convention:**
- `demo/stage-0-blank` - Off of `main` - Empty project structure
- `demo/stage-1-naive-csharp` - Off of `demo/stage-0-blank` - Naive C# with booleans
- `demo/stage-2-first-du` - Off of `demo/stage-1-naive-csharp` - First discriminated union
- `demo/stage-3-pattern-matching` - Off of `demo/stage-2-first-du` - Pattern matching refinement
- `demo/stage-4-final` - Off of `demo/stage-3-pattern-matching` - Final explicit tiers

**Iteration Naming:**
- Current branches will be renamed with `-old1` suffix (e.g., `demo/stage-0-old1`)
- Previous `-old` branches already exist (e.g., `demo/stage-0-old`)
- We'll reuse the original `demo/stage-X` names for this third iteration

```
Branch Hierarchy (Third Iteration):
  main
  └── demo/stage-0-blank                  # Empty project structure
      └── demo/stage-1-naive-csharp       # Naive C# with booleans
          └── demo/stage-2-first-du       # First discriminated union
              └── demo/stage-3-pattern-matching  # Pattern matching refinement
                  └── demo/stage-4-final  # Final explicit tiers

Old Branches (For Reference):
  demo/stage-0-blank-old1              # Second iteration
  demo/stage-1-naive-csharp-old1
  demo/stage-2-first-du-old1
  demo/stage-3-pattern-matching-old1
  demo/stage-4-final-old1
  demo/stage-0-blank-old               # First iteration
  demo/stage-1-naive-csharp-old
  demo/stage-2-first-du-old
  demo/stage-3-pattern-matching-old
  demo/stage-4-final-old

Worktrees (During Build Only):
  src/Live/                 # Build workspace (not used during presentations)

Presentation (Live Coding):
  Start on demo/stage-1-naive-csharp and live code refactorings
  Branch fallbacks available: demo/stage-2-first-du, demo/stage-3-pattern-matching
```

---

## Stage Definitions

### Stage 0: Empty Project Structure (Foundation)
**Branch:** `demo/stage-0-blank` (off of `main`)  
**Build Location:** `src/Live/` worktree (build-time only)  
**Presentation Location:** Current directory (after `git checkout demo/stage-0-blank`)  
**Purpose:** Establish project structure with empty C# and F# projects  
**Timing:** Setup before presentation

**Contents:**
```
src/Live/
  CustomerLive.sln           # Solution file
  CustomerLiveDomain/        # F# Class Library (empty structure)
    CustomerLiveDomain.fsproj
    Library.fs               # Empty or minimal template
  CustomerLiveDemo/          # C# Console App (empty structure)
    CustomerLiveDemo.csproj
    Program.cs               # Minimal Main method
```

**Key Setup:**
- Solution with two projects: F# library and C# console app
- C# project references F# project
- Both projects build successfully but contain no domain logic yet
- This is the clean slate for live coding

**Build Steps:**
```powershell
# On demo/stage-0-blank branch in src/Live/
dotnet new sln -n CustomerLive
dotnet new classlib -lang F# -n CustomerLiveDomain
dotnet new console -n CustomerLiveDemo
dotnet sln add CustomerLiveDomain/CustomerLiveDomain.fsproj
dotnet sln add CustomerLiveDemo/CustomerLiveDemo.csproj
cd CustomerLiveDemo
dotnet add reference ../CustomerLiveDomain/CustomerLiveDomain.fsproj
cd ..
dotnet build
# Stash for confirmation
git add .
git stash push -m "Stage 0: Empty project structure"
```

---

### Stage 1: Naive Models (C# and F#)
**Branch:** `demo/stage-1-naive-csharp` (off of `demo/stage-0-blank`)  
**Build Location:** `src/Live/` worktree  
**Presentation Location:** Current directory (after `git checkout demo/stage-1-naive-csharp`)  
**Purpose:** Show both naive C# and naive F# approaches - same problem, different syntax  
**Timing:** Minutes 5-15 in presentation

**Contents:**
```
src/Live/
  CustomerLive.sln
  CustomerLiveDomain/        # F# Class Library
    CustomerLiveDomain.fsproj
    Library.fs               # F# record with boolean flags
  CustomerLiveDemo/          # C# Console App
    CustomerLiveDemo.csproj
    Program.cs               # Both C# class and F# usage (switchable via comments)
```

**Key Code in Program.cs:**
```csharp
using CustomerLiveDomain;

// Naive C# Model - comment out when showing F# version
/*
public class Customer
{
    public string Id { get; }
    public bool IsEligible { get; }
    public bool IsRegistered { get; }
    
    // Constructor required for immutable properties
    public Customer(string id, bool isEligible, bool isRegistered)
    {
        Id = id;
        IsEligible = isEligible;
        IsRegistered = isRegistered;
    }
}
*/

class Program
{
    static void Main(string[] args)
    {
        // Using C# model (uncomment above class)
        // var john = new Customer("John", true, true);
        
        // Using F# model (comment out C# class above)
        var john = new Customer { Id = "John", IsEligible = true, IsRegistered = true };
        
        // The bug - illegal state (eligible but not registered)
        var grinch = new Customer { Id = "Grinch", IsEligible = true, IsRegistered = false };
        
        Console.WriteLine(CalculateTotal(john, 100m));
        Console.WriteLine(CalculateTotal(grinch, 100m)); // Should not get discount!
    }
    
    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        // Correct version
        if (customer.IsEligible && customer.IsRegistered && spend >= 100)
            return spend * 0.9m;
        return spend;
        
        // Buggy version (show this too)
        // if (customer.IsEligible && spend >= 100)
        //     return spend * 0.9m;
        // return spend;
    }
}
```

**Key Code in Library.fs:**
```fsharp
namespace CustomerLiveDomain

// Naive F# record - same problem as C# class
type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
}
```

**Presentation Points:**
- Can switch between C# and F# by commenting/uncommenting
- Both allow illegal state: `IsEligible=true, IsRegistered=false`
- F# records are nice (immutable) but don't solve the domain problem yet
- Demonstrate the bug: forgetting to check `IsRegistered`

**Build Steps:**
```powershell
# On demo/stage-1-naive-csharp branch in src/Live/
# Implement naive C# class with constructor
# Implement naive F# record
# Implement Program.cs with both versions switchable
dotnet build
# Test both versions
# Stash for confirmation
git add .
git stash push -m "Stage 1: Naive C# and F# models"
```

---

### Stage 2: Discriminated Union with Boolean (First Refactoring)
**Branch:** `demo/stage-2-first-du` (off of `demo/stage-1-naive-csharp`)  
**Build Location:** `src/Live/` worktree  
**Presentation Location:** Current directory (after `git checkout demo/stage-2-first-du`)  
**Purpose:** Introduce discriminated unions - make registration explicit  
**Timing:** Minutes 15-20 in presentation

**Contents:**
```
src/Live/
  CustomerLive.sln
  CustomerLiveDomain/        # F# Class Library
    CustomerLiveDomain.fsproj
    Library.fs               # DU: Registered | Guest
  CustomerLiveDemo/          # C# Console App
    CustomerLiveDemo.csproj
    Program.cs               # C# pattern matching over F# DU
```

**Key Code in Library.fs:**
```fsharp
namespace CustomerLiveDomain

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

**Key Code in Program.cs:**
```csharp
using CustomerLiveDomain;
using static CustomerLiveDomain.Customer;

class Program
{
    static void Main(string[] args)
    {
        // Factory methods generated by F#
        var john = NewRegistered(new RegisteredCustomer { Id = "John", IsEligible = true });
        var sarah = NewGuest(new UnregisteredCustomer { Id = "Sarah" });
        
        // Can't create illegal state anymore!
        // var grinch = NewGuest(new UnregisteredCustomer { Id = "Grinch", IsEligible = true });
        // ❌ Error: UnregisteredCustomer has no IsEligible property!
        
        Console.WriteLine($"John: £{CalculateTotal(john, 100m)}");
        Console.WriteLine($"Sarah: £{CalculateTotal(sarah, 100m)}");
    }
    
    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        var discount = customer switch
        {
            Registered c when c.Item.IsEligible && spend >= 100 => spend * 0.1m,
            _ => 0m
        };
        return spend - discount;
    }
}
```

**Presentation Points:**
- ✅ Can't create eligible guest anymore (UnregisteredCustomer has no IsEligible field)
- ✅ C# switch expressions work perfectly with F# DU
- ✅ Compiler forces handling both cases
- ⚠️ Still using boolean for eligibility (next refinement)

**Build Steps:**
```powershell
# On demo/stage-2-first-du branch in src/Live/
# Refactor Library.fs to use DU
# Update Program.cs to use pattern matching
dotnet build
# Test all scenarios
# Stash for confirmation
git add .
git stash push -m "Stage 2: DU with boolean (registration explicit)"
```

---

### Stage 3: Explicit Eligibility Tiers (Final Refactoring)
**Branch:** `demo/stage-3-pattern-matching` (off of `demo/stage-2-first-du`)  
**Build Location:** `src/Live/` worktree  
**Presentation Location:** Current directory (after `git checkout demo/stage-3-pattern-matching`)  
**Purpose:** Make discount tiers a first-class domain concept  
**Timing:** Minutes 20-30 in presentation

**Contents:**
```
src/Live/
  CustomerLive.sln
  CustomerLiveDomain/        # F# Class Library
    CustomerLiveDomain.fsproj
    Library.fs               # DU: Standard | Registered | Guest
  CustomerLiveDemo/          # C# Console App
    CustomerLiveDemo.csproj
    Program.cs               # Simplified C# logic
```

**Key Code in Library.fs:**
```fsharp
namespace CustomerLiveDomain

type RegisteredCustomer = { Id: string }

type UnregisteredCustomer = { Id: string }

type Customer = 
    | Standard of RegisteredCustomer    // Eligible tier explicit
    | Registered of RegisteredCustomer  // Registered but not eligible
    | Guest of UnregisteredCustomer
```

**Key Code in Program.cs:**
```csharp
using CustomerLiveDomain;
using static CustomerLiveDomain.Customer;

class Program
{
    static void Main(string[] args)
    {
        // Clean factory methods - domain language explicit
        var john = NewStandard(new RegisteredCustomer { Id = "John" });
        var mary = NewStandard(new RegisteredCustomer { Id = "Mary" });
        var richard = NewRegistered(new RegisteredCustomer { Id = "Richard" });
        var sarah = NewGuest(new UnregisteredCustomer { Id = "Sarah" });
        
        Console.WriteLine($"John (£100): £{CalculateTotal(john, 100m)}");      // £90
        Console.WriteLine($"Mary (£99): £{CalculateTotal(mary, 99m)}");        // £99
        Console.WriteLine($"Richard (£100): £{CalculateTotal(richard, 100m)}"); // £100
        Console.WriteLine($"Sarah (£100): £{CalculateTotal(sarah, 100m)}");    // £100
    }
    
    static decimal CalculateTotal(Customer customer, decimal spend)
    {
        // Simpler logic - no boolean checks!
        var discount = customer switch
        {
            Standard _ when spend >= 100 => spend * 0.1m,
            _ => 0m
        };
        return spend - discount;
    }
}
```

**Presentation Points:**
- ✅ Domain language is explicit (Standard, Registered, Guest)
- ✅ No boolean flags to check or forget
- ✅ Logic is simpler in both F# and C#
- ✅ Compiler-enforced correctness
- ✅ Reads like the business requirement!

**Live Demo Extension (VIP Tier):**
```fsharp
// Add VIP tier to DU
type Customer = 
    | VIP of RegisteredCustomer         // NEW!
    | Standard of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
```

```csharp
// C# compiler warns about unhandled case!
static decimal CalculateTotal(Customer customer, decimal spend)
{
    var discount = customer switch
    {
        VIP _ when spend >= 100 => spend * 0.15m,     // Fix: Add VIP case
        Standard _ when spend >= 100 => spend * 0.1m,
        _ => 0m
    };
    return spend - discount;
}
```

**Build Steps:**
```powershell
# On demo/stage-3-pattern-matching branch in src/Live/
# Refactor Library.fs to make eligibility explicit
# Simplify Program.cs logic
dotnet build
# Test all four customer types
# Stash for confirmation
git add .
git stash push -m "Stage 3: Explicit eligibility tiers"
```

---

### Stage 4 (Optional): SAFE Stack Integration
**Branch:** `demo/stage-4-final` (off of `demo/stage-3-pattern-matching`)  
**Build Location:** `src/Live/` worktree or separate directory  
**Presentation Location:** Current directory (after `git checkout demo/stage-4-final`)  
**Purpose:** Show F# in full-stack web context  
**Timing:** Minutes 40-58 (if time permits)

**Contents:**
```
src/Live/  (or src/Demo-Stage4/)
  src/
    Shared/
      Shared.fs              # Domain model (copied from Stage 3)
    Server/
      Server.fs              # Saturn API endpoints
    Client/
      Index.fs               # Elmish MVU app
  paket.dependencies
  paket.lock
```

**Key Code:**
- Model-View-Update pattern
- Shared types between client/server
- Simple customer editor UI
- Demonstrates type safety across stack

**Presentation Points:**
- Same types client and server
- Elmish MVU architecture
- Type-safe serialization
- Modern F# web development

---

## Build Instructions

### Pre-Build: Rename Existing Branches

Before starting the third iteration, rename current branches to preserve them:

```powershell
# Rename current branches to -old1
git branch -m demo/stage-0-blank demo/stage-0-blank-old1
git branch -m demo/stage-1-naive-csharp demo/stage-1-naive-csharp-old1
git branch -m demo/stage-2-first-du demo/stage-2-first-du-old1
git branch -m demo/stage-3-pattern-matching demo/stage-3-pattern-matching-old1
git branch -m demo/stage-4-final demo/stage-4-final-old1 2>$null  # Ignore error if doesn't exist

# Push renamed branches to origin
git push origin demo/stage-0-blank-old1
git push origin demo/stage-1-naive-csharp-old1
git push origin demo/stage-2-first-du-old1
git push origin demo/stage-3-pattern-matching-old1
git push origin demo/stage-4-final-old1 2>$null

# Delete old remote tracking references
git push origin --delete demo/stage-0-blank
git push origin --delete demo/stage-1-naive-csharp
git push origin --delete demo/stage-2-first-du
git push origin --delete demo/stage-3-pattern-matching
```

### Initial Setup (Third Iteration)

1. **Ensure main has .gitignore:**
   ```powershell
   git checkout main
   git pull origin main
   # Verify .gitignore exists and is comprehensive
   ```

2. **Create fresh demo/stage-0-blank branch from main:**
   ```powershell
   git checkout main
   git checkout -b demo/stage-0-blank
   ```

3. **Create worktree for development:**
   ```powershell
   git worktree add src/Live demo/stage-0-blank
   cd src/Live
   ```

### Building Each Stage (Progressive Development)

#### Stage 0: Empty Project Structure
```powershell
# In src/Live/ on demo/stage-0-blank branch
dotnet new sln -n CustomerLive
dotnet new classlib -lang F# -n CustomerLiveDomain
dotnet new console -n CustomerLiveDemo

# Add projects to solution
dotnet sln add CustomerLiveDomain/CustomerLiveDomain.fsproj
dotnet sln add CustomerLiveDemo/CustomerLiveDemo.csproj

# Add F# reference to C# project
cd CustomerLiveDemo
dotnet add reference ../CustomerLiveDomain/CustomerLiveDomain.fsproj
cd ..

# Verify it builds
dotnet build

# Stash for confirmation
git add .
git stash push -m "Stage 0: Empty project structure"
```

**WAIT FOR CONFIRMATION before proceeding to Stage 1**

#### Stage 1: Naive Models
```powershell
# After Stage 0 is confirmed and committed
cd src/Live

# Implement naive C# class in Program.cs (with constructor)
# Implement naive F# record in Library.fs
# Add business logic with both correct and buggy versions

# Build and test
dotnet build
dotnet run --project CustomerLiveDemo

# Stash for confirmation
git add .
git stash push -m "Stage 1: Naive C# and F# models"
```

**WAIT FOR CONFIRMATION before proceeding to Stage 2**

Once confirmed, branch to stage-1-naive-csharp:
```powershell
git stash pop
git add .
git commit -m "Stage 1: Naive C# with booleans - shows illegal state problem"
git checkout -b demo/stage-1-naive-csharp
git push -u origin demo/stage-1-naive-csharp
```

#### Stage 2: First Refactoring (DU with Boolean)
```powershell
# In src/Live/ on demo/stage-1-naive-csharp branch
# Refactor Library.fs to use discriminated union
# Update Program.cs to use pattern matching

# Build and test
dotnet build
dotnet run --project CustomerLiveDemo

# Stash for confirmation
git add .
git stash push -m "Stage 2: DU with boolean (registration explicit)"
```

**WAIT FOR CONFIRMATION before proceeding to Stage 3**

Once confirmed, branch to stage-2-first-du:
```powershell
git stash pop
git add .
git commit -m "Stage 2: First discriminated union - prevents illegal states"
git checkout -b demo/stage-2-first-du
git push -u origin demo/stage-2-first-du
```

#### Stage 3: Final Refactoring (Explicit Tiers)
```powershell
# In src/Live/ on demo/stage-2-first-du branch
# Refactor Library.fs to make eligibility explicit
# Simplify Program.cs logic

# Build and test all customer types
dotnet build
dotnet run --project CustomerLiveDemo

# Stash for confirmation
git add .
git stash push -m "Stage 3: Explicit eligibility tiers"
```

**WAIT FOR CONFIRMATION before final commit**

Once confirmed, branch to stage-3-pattern-matching:
```powershell
git stash pop
git add .
git commit -m "Stage 3: Pattern matching in both C# and F#"
git checkout -b demo/stage-3-pattern-matching
git push -u origin demo/stage-3-pattern-matching
```

---

## Worktree Management

### Primary Worktree
```powershell
# Create main development worktree
git worktree add src/Live demo/stage-0-blank
```

### Temporary Comparison Worktrees (Optional)
During development, you can create temporary worktrees to compare stages:

```powershell
# To compare current work with previous stage
git worktree add src/Stage1-compare demo/stage-1-naive-csharp
git worktree add src/Stage2-compare demo/stage-2-first-du

# When done comparing, remove them
git worktree remove src/Stage1-compare
git worktree remove src/Stage2-compare
```

### List All Worktrees
```powershell
git worktree list
```

### Remove Primary Worktree (When Rebuilding)
```powershell
git worktree remove src/Live --force
git worktree prune
```

### Working with Stashes

```powershell
# View stashed changes
git stash list

# View specific stash contents
git stash show -p stash@{0}

# Apply (keep stash) or pop (remove stash) after confirmation
git stash pop stash@{0}

# Discard stash if not needed
git stash drop stash@{0}
```

---

## Presentation Flow with Stages

**Preparation:** Start on `demo/stage-1-naive-csharp` branch. Live code the refactorings. Stage branches are fallbacks if needed.

| Time | Stage | Activity | Approach | Fallback Branch |
|------|-------|----------|----------|--------|
| 0-5 | Intro | Welcome, poll audience | Show README | `main` |
| 5-10 | Stage 1 | Show naive models (C# vs F#) | Demo existing code | `demo/stage-1-naive-csharp` |
| 10-15 | Stage 1 | Demonstrate illegal state bug | Show the problem | `demo/stage-1-naive-csharp` |
| 15-20 | Stage 2 | Show DU refactoring | **LIVE CODE** refactor to DU | `demo/stage-2-first-du` |
| 20-25 | Stage 3 | Show explicit tiers | **LIVE CODE** explicit tiers | `demo/stage-3-pattern-matching` |
| 25-30 | Stage 3 | Live demo: Add VIP tier | **LIVE CODE** VIP tier | `demo/stage-3-pattern-matching` |
| 30-35 | Stage 3 | C# interop deep dive | Explore patterns | `demo/stage-3-pattern-matching` |
| 35+ | Optional | SAFE stack if time | Show pre-built | `demo/stage-4-final` |

### Navigation During Presentation

**Primary Approach: Live Coding (Recommended)**
1. Start on `demo/stage-1-naive-csharp` branch
2. Show Stage 1 code and demonstrate the problem
3. **Live code** the refactoring to discriminated union (Stage 2)
4. **Live code** the refactoring to explicit tiers (Stage 3)
5. **Live code** adding VIP tier as extension

**Fallback: Branch Checkout (If Live Coding Fails)**
```powershell
# If something goes wrong during live coding
git reset --hard  # Discard changes
git checkout demo/stage-2-first-du  # Jump to working Stage 2
# Continue from there

git checkout demo/stage-3-pattern-matching  # Or jump to Stage 3 if needed
```

**Alternative: Pre-Switch for Comparison**
You can checkout stages to show before/after comparisons:
```powershell
# Show Stage 1, then switch to show final result
git checkout demo/stage-1-naive-csharp  # Show problem
git checkout demo/stage-3-pattern-matching  # Show solution
# Then switch back and live code the progression
git checkout demo/stage-1-naive-csharp
# Live code Stage 1 → Stage 2 → Stage 3
```

---

## Workflow Summary

### Development Workflow (Building Stages)
1. Start on `demo/stage-0-blank` branch in `src/Live/`
2. Build Stage 0 → Stash → **WAIT FOR CONFIRMATION** → Commit
3. Build Stage 1 → Stash → **WAIT FOR CONFIRMATION** → Commit + Branch to `demo/stage-1-naive-csharp`
4. Build Stage 2 → Stash → **WAIT FOR CONFIRMATION** → Commit + Branch to `demo/stage-2-first-du`
5. Build Stage 3 → Stash → **WAIT FOR CONFIRMATION** → Commit + Branch to `demo/stage-3-pattern-matching`
6. Push all branches to origin

### Confirmation Workflow
After each stash:
1. Review stashed changes: `git stash show -p stash@{0}`
2. Test stashed code: `git stash pop` → build → test → `git stash push` if more changes needed
3. Once confirmed correct: `git stash pop` → `git add .` → `git commit` → `git branch stage-X` (for checkpoints)

### Presentation Workflow (Live Coding with Branch Fallbacks)
1. Start on `demo/stage-1-naive-csharp` branch
2. Present Stage 1 - show the problem with naive approach
3. **Live code** refactoring to Stage 2 (discriminated union)
4. **Live code** refactoring to Stage 3 (explicit tiers)
5. **Live code** VIP tier extension
6. Branch fallbacks: If live coding fails, `git reset --hard` and `git checkout demo/stage-X` to continue

### Iteration Workflow (Rebuilding)
1. Rename existing branches to `-old1` suffix
2. Create fresh `demo/stage-0-blank` from `main`
3. Build progressively with stash-confirm-commit cycle
4. Branch to next stage after each confirmation

---

## Backup Strategy

### Before Building
1. Rename current branches to `-old1`
2. Push renamed branches to origin
3. Verify `main` has latest documentation and .gitignore

### During Building
1. **Always stash before confirmation** - never commit directly
2. Keep stashes until confirmed correct
3. Can revert to previous stage branch if needed
4. Test build at every stage before stashing

### Before Presentation
1. Ensure all stage branches are pushed to origin (as fallbacks)
2. Test each stage builds successfully
3. **Practice live coding** the refactorings Stage 1 → Stage 2 → Stage 3
4. Know the refactoring steps by heart
5. Test fallback workflow: `git reset --hard` + `git checkout demo/stage-X`
6. Start presentation on `demo/stage-1-naive-csharp` branch

### During Presentation
- **Primary approach:** Live code all refactorings (Stage 1 → 2 → 3 → VIP)
- **If live coding fails:** `git reset --hard` then `git checkout demo/stage-X` to jump to working fallback
- **Quick recovery:** Branches are pre-built and tested, can switch in seconds
- **After recovery:** Continue presenting from the fallback branch
- **VIP extension:** Can live code on any stage (1, 2, or 3 depending on where you are)

### After Presentation
- Commit any interesting variations from live demo
- Tag successful presentation states: `git tag presentation-yyyy-mm-dd`
- Push tags: `git push --tags`

---

## Rebuild Process (Third Iteration)

### Clean Slate
```powershell
# Remove worktree
git worktree remove src/Live --force

# Rename current branches to -old1
git branch -m demo/stage-0-blank demo/stage-0-blank-old1
git branch -m demo/stage-1-naive-csharp demo/stage-1-naive-csharp-old1
git branch -m demo/stage-2-first-du demo/stage-2-first-du-old1
git branch -m demo/stage-3-pattern-matching demo/stage-3-pattern-matching-old1
git branch -m demo/stage-4-final demo/stage-4-final-old1 2>$null

# Push renamed branches
git push origin demo/stage-0-blank-old1 demo/stage-1-naive-csharp-old1 demo/stage-2-first-du-old1 demo/stage-3-pattern-matching-old1 demo/stage-4-final-old1 2>$null

# Delete old remote references
git push origin --delete demo/stage-0-blank demo/stage-1-naive-csharp demo/stage-2-first-du demo/stage-3-pattern-matching

# Prune references
git worktree prune
```

### Rebuild from This Plan
1. Follow "Pre-Build: Rename Existing Branches" section
2. Follow "Initial Setup (Third Iteration)" section
3. Build each stage following "Building Each Stage" section
4. Use stash-confirm-commit workflow for each stage
5. Test each stage independently before proceeding
6. Create checkpoint branches after confirmation
7. Verify presentation flow works

---

## Notes & Considerations

### Why This Approach?

**Sequential Branch Development:**
- Build naturally from Stage 0 → Stage 1 → Stage 2 → Stage 3
- Each stage branches from the previous one
- Shows real refactoring progression
- Each branch preserves working code for presentation

**Stash-Confirm-Commit Workflow:**
- Allows review before committing
- Can test changes before making them permanent
- Easy to discard if something is wrong
- Maintains clean commit history

**Worktrees for Building Only:**
- Use `src/Live/` worktree during the build process
- Allows building stages without constantly switching branches
- Once stages are built and pushed, remove the worktree

**Presentations Use Live Coding:**
- Start on `demo/stage-1-naive-csharp` branch
- Live code all refactorings during presentation
- Branches are fallback safety nets if things go wrong
- Simple recovery: `git reset --hard` + `git checkout demo/stage-X`
- Works with standard VS Code git integration

### Branch Strategy Clarification

**Old Iterations Preserved:**
- `-old` suffix: First iteration (historical)
- `-old1` suffix: Second iteration (current before rebuild)
- No suffix: Third iteration (current build)

**Sequential Stage Branches:**
- `demo/stage-0-blank`, `demo/stage-1-naive-csharp`, `demo/stage-2-first-du`, `demo/stage-3-pattern-matching`: Sequential development branches
- Each branch is created from the previous stage with `git checkout -b`
- Forms a linear git history showing progression
- Used during presentation to show specific stages

### C# Model Constructor Requirement

The C# `Customer` class in Stage 1 **must have getter-only properties with a constructor** to maintain immutability (like F# records). This demonstrates that both languages can achieve immutability, though F# makes it the default.

```csharp
public class Customer
{
    public string Id { get; }
    public bool IsEligible { get; }
    public bool IsRegistered { get; }
    
    // Required for immutable properties
    public Customer(string id, bool isEligible, bool isRegistered)
    {
        Id = id;
        IsEligible = isEligible;
        IsRegistered = isRegistered;
    }
}
```

### Switching Between C# and F# in Stage 1

Use comments to switch between demonstrating naive C# vs naive F#:

```csharp
// Comment out this entire C# class to show F# version
/*
public class Customer { ... }
*/

// Then toggle between:
var customer = new Customer("John", true, true);           // C# version
var customer = new Customer { Id = "John", IsEligible = true, IsRegistered = true };  // F# version
```

### Main Branch Strategy
- Main contains only documentation, scripts, plans, .gitignore
- No working code (reduces confusion)
- .gitignore properly handles build artifacts
- Easy to onboard new presenters (clone, read plan, build from main)

### Presenter Workflow Options

**Option 1: Live Coding (Recommended - Your Approach)**
```powershell
# Start on Stage 1
git checkout demo/stage-1-naive-csharp
# Show Stage 1 problem
# Live code refactor to Stage 2 (DU)
# Live code refactor to Stage 3 (explicit tiers)
# Live code VIP extension

# If something breaks:
git reset --hard
git checkout demo/stage-2-first-du  # Or stage-3, depending on where you are
# Continue from fallback
```

**Option 2: Pre-Built Stage Navigation**
```powershell
# Alternative: Show completed stages without live coding
git checkout demo/stage-1-naive-csharp
# Present Stage 1...
git checkout demo/stage-2-first-du
# Present Stage 2...
git checkout demo/stage-3-pattern-matching
# Present Stage 3... and live code VIP extension only
```

**Option 3: Hybrid Approach**
- Show Stage 1 from `demo/stage-1-naive-csharp`
- Live code Stage 2 refactoring
- If successful, continue; if not, `git checkout demo/stage-2-first-du`
- Live code Stage 3 refactoring  
- If successful, continue; if not, `git checkout demo/stage-3-pattern-matching`
- Live code VIP tier (with `demo/stage-3-pattern-matching` as fallback)

---

## Success Criteria

- ✅ Each stage builds and runs independently
- ✅ Each stage demonstrates one clear concept
- ✅ Can navigate between stages in under 5 seconds
- ✅ Live demo branch can be reset cleanly between sessions
- ✅ C# devs can understand and run any stage
- ✅ Documentation clearly explains what each stage shows
- ✅ .gitignore prevents committing bin/obj/etc
- ✅ Presentation flow aligns with timing in scripts

---

## Future Extensions

### Additional Branches to Consider
- `stage-3-vip`: Add VIP tier as fourth customer type
- `stage-safe-minimal`: Minimal SAFE stack (just MVU)
- `stage-safe-full`: Full SAFE with database, authentication
- `stage-testing`: XUnit tests for domain model
- `stage-railway`: Railway-oriented programming with Result types

### Alternative Presentation Paths
- **Express Version (30 min):** Stages 0 → 2 → 3 only
- **Deep Dive (90 min):** All stages + VIP + SAFE full
- **Workshop Format:** Attendees build stages themselves following plan
