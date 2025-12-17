# Presentation Setup Guide

## Overview

This guide covers the technical setup for delivering the F# Type-Driven Development presentation, including editor configuration, terminal setup, and live coding workflow.

---

## Pre-Presentation Checklist

### Software Requirements
- [ ] Visual Studio Code (or preferred editor)
- [ ] .NET 8.0 or 9.0 SDK installed
- [ ] F# language support installed
- [ ] PowerShell terminal
- [ ] Git (for switching between iterations if using branches)

### Verify Installation
```powershell
dotnet --version          # Should show 8.0+ or 9.0+
dotnet --list-sdks        # Verify F# SDK is available
```

### Repository Setup
```powershell
cd c:\git\NovuckFSharpPresentation
git checkout feature/presentation-creation
cd src\CustomerDemo
dotnet build              # Verify everything compiles
dotnet run                # Verify demo runs successfully
```

---

## Editor Layout Configuration

### Recommended: Two-Column Layout

**Goal:** Show F# types on LEFT, C# consumption on RIGHT

#### VS Code Setup:
1. Open VS Code in the `c:\git\NovuckFSharpPresentation` folder
2. Split editor into two columns: `View` → `Editor Layout` → `Split Right`
3. **LEFT Column:** 
   - Open `src/CustomerDomain/Library.fs` (or iteration file)
   - Font size: Increase for readability (`Ctrl+` or `Cmd+`)
4. **RIGHT Column:**
   - Open `src/CustomerDemo/Program.cs` (or example file)
   - Font size: Increase for readability

#### Terminal Setup:
1. Open integrated terminal: `View` → `Terminal` (or `` Ctrl+` ``)
2. Split terminal if needed: Click `+` dropdown → `Split Terminal`
3. Navigate to: `cd src\CustomerDemo`

---

## Live Output Setup: `dotnet watch run`

### Why Watch Mode?

Running `dotnet watch run` keeps the C# program continuously running and automatically rebuilds/restarts whenever you save changes to either F# or C# files.

**Benefits:**
- ✅ Instant feedback when you edit code
- ✅ Audience sees results immediately
- ✅ No need to manually rebuild between iterations
- ✅ Shows "real development" workflow

### Starting Watch Mode

**Terminal 1: Watch Mode Output**
```powershell
cd c:\git\NovuckFSharpPresentation\src\CustomerDemo
dotnet watch run
```

You'll see:
```
dotnet watch 🔥 Hot reload enabled. For a list of supported edits, see https://aka.ms/dotnet/hot-reload.
  💡 Press "Ctrl + R" to restart.
dotnet watch 🔧 Building...
  CustomerDomain succeeded
  CustomerDemo succeeded
dotnet watch 🚀 Started

=== F# Customer Domain Demo from C# ===
[... output ...]
```

**What Happens During Presentation:**
1. Edit F# type definition in LEFT window
2. Save file (`Ctrl+S`)
3. Watch detects change, rebuilds, reruns
4. Output appears automatically in terminal
5. Audience sees immediate feedback

### Optional: Second Terminal for Commands

If you want to show git commands or other operations without disrupting watch mode:

**Terminal 2: Commands**
```powershell
cd c:\git\NovuckFSharpPresentation\src\CustomerDomain
# Use this for git operations, file viewing, etc.
```

---

## Presentation Flow Setup

### Starting Position (Before Presentation)

**Files to Have Open:**
- LEFT: `src/CustomerDomain/Library.fs`
- RIGHT: `src/CustomerDemo/Program.cs`
- Terminal: `dotnet watch run` running in `CustomerDemo`
- Browser: Slides open (optional)

**Initial State:**
- `CustomerDomain.fsproj` should have `Library.fs` active (Iteration 2)
- `Program.cs` should have the full demo code
- Watch mode running, showing successful output

### Transitioning Between Iterations

You have two approaches:

#### Approach 1: Live Editing (Recommended)

Edit the project file and source files live during presentation:

1. **Show Iteration 0:**
   - Edit `CustomerDomain.fsproj`, change to `<Compile Include="Iteration0_Naive.fs" />`
   - Copy `Examples/Iteration0_Demo.cs` content into `Program.cs`
   - Save all files
   - Watch rebuilds automatically
   - Show output

2. **Transition to Iteration 1:**
   - Edit `CustomerDomain.fsproj`, change to `<Compile Include="Iteration1_DUWithBoolean.fs" />`
   - Copy `Examples/Iteration1_Demo.cs` content into `Program.cs`
   - Save all files
   - Watch rebuilds automatically
   - Show output

3. **Transition to Iteration 2:**
   - Edit `CustomerDomain.fsproj`, change to `<Compile Include="Iteration2_ExplicitEligibility.fs" />`
   - Copy `Examples/Iteration2_Demo.cs` content into `Program.cs`
   - Save all files
   - Watch rebuilds automatically
   - Show output

#### Approach 2: Git Branches (Alternative)

Create branches for each iteration beforehand:

**Setup (Do this before presentation):**
```powershell
# Create iteration branches
git checkout -b iteration-0
# Edit fsproj and Program.cs for Iteration 0
git add -A
git commit -m "Iteration 0 setup"

git checkout feature/presentation-creation
git checkout -b iteration-1
# Edit fsproj and Program.cs for Iteration 1
git add -A
git commit -m "Iteration 1 setup"

git checkout feature/presentation-creation
git checkout -b iteration-2
# Edit fsproj and Program.cs for Iteration 2
git add -A
git commit -m "Iteration 2 setup"

git checkout feature/presentation-creation
```

**During Presentation:**
```powershell
# Stop watch mode (Ctrl+C)
git checkout iteration-0
dotnet watch run
# Show demo...

# Transition to next iteration
# Stop watch mode (Ctrl+C)
git checkout iteration-1
dotnet watch run
# Show demo...
```

---

## Iteration Details

### Iteration 0: Naive Model
**F# File:** `Iteration0_Naive.fs`
**C# Demo:** `Examples/Iteration0_Demo.cs`
**Key Points:**
- Show illegal state can be created
- Boolean flags are error-prone
- No type safety

**Expected Output:**
```
=== Iteration 0: Naive Model ===
Problem: Can create illegal states!

John (Eligible + Registered) spends £100: £90.00
Bad (Eligible but NOT Registered) spends £100: £100.0
^ This shouldn't be possible! Type system doesn't prevent it.
```

### Iteration 1: DU with Boolean Flag
**F# File:** `Iteration1_DUWithBoolean.fs`
**C# Demo:** `Examples/Iteration1_Demo.cs`
**Key Points:**
- Discriminated union separates Registered/Guest
- UnregisteredCustomer doesn't have IsEligible property
- C# switch expression over F# DU

**Expected Output:**
```
=== Iteration 1: DU with Boolean Flag ===
Better: Can't be eligible without being registered!

John (Registered, Eligible) spends £100: £90.00
Richard (Registered, NOT Eligible) spends £100: £100.0
Sarah (Guest) spends £100: £100.0

Improvement: UnregisteredCustomer doesn't even HAVE IsEligible!
Can't create an eligible guest - the type prevents it.

Notice: C# switch expression works perfectly with F# discriminated union!
```

### Iteration 2: Explicit Eligibility
**F# File:** `Iteration2_ExplicitEligibility.fs` or `Library.fs`
**C# Demo:** `Examples/Iteration2_Demo.cs` or `Program.cs`
**Key Points:**
- Eligibility is explicit in type
- Domain language is clear
- C# switch expression is simplest

**Expected Output:**
```
=== Iteration 2: Explicit Eligibility ===
Best: Domain language is explicit in the type!

--- Test Cases (matching Ian Russell's examples) ---
John (Eligible) spends £100: £90.00 (expected £90)
Mary (Eligible) spends £99: £99.0 (expected £99)
Richard (Registered) spends £100: £100.0 (expected £100)
Sarah (Guest) spends £100: £100.0 (expected £100)

Benefits:
✅ No boolean flag to check or forget
✅ Domain language is explicit: Eligible, Registered, Guest
✅ Impossible states are... impossible!
✅ C# switch expression works beautifully with F# DU!
```

---

## Live Refactoring Demo (Minutes 28-33)

### Setup for VIP Customer Demo

**Starting Point:** Have Iteration 2 running

**Step 1: Add VIP to F# Type**

Edit the active F# file, add VIP case:
```fsharp
type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
    | VIP of RegisteredCustomer  // ADD THIS LINE
```

**Save file** - Watch will rebuild and FAIL

**Show the errors** in terminal:
- F# will show incomplete pattern match warnings (if using F# match)
- C# will show compilation errors in switch expressions

**Step 2: Fix C# Code**

Edit `Program.cs`, update the CalculateTotal function:
```csharp
static decimal CalculateTotal(Customer customer, decimal spend)
{
    var discount = customer switch
    {
        Customer.Eligible _ when spend >= 100.0m => spend * 0.1m,
        Customer.VIP _ when spend >= 100.0m => spend * 0.15m,  // ADD THIS
        _ => 0.0m
    };
    return spend - discount;
}
```

Update any other switch expressions that match on Customer type.

**Save file** - Watch rebuilds successfully!

**Point to emphasize:** 
"The F# type change forced us to update all C# switch expressions. The compiler guided us to every location. This is refactoring safety!"

---

## Troubleshooting

### Watch Mode Not Rebuilding
**Problem:** File changes don't trigger rebuild
**Solution:** 
```powershell
# Stop watch (Ctrl+C)
# Clear obj/bin folders
Remove-Item -Recurse -Force obj,bin
# Restart watch
dotnet watch run
```

### Build Errors During Transition
**Problem:** Compilation fails when switching iterations
**Solution:**
- Ensure `CustomerDomain.fsproj` has only ONE `<Compile Include="..." />` line active
- Ensure `Program.cs` matches the iteration's expected types
- Check that namespace imports are correct

### Terminal Output Not Visible
**Problem:** Output scrolls off screen
**Solution:**
- Clear terminal before each iteration: Type `clear` or `cls`
- Increase terminal font size for audience visibility
- Consider recording output separately if projector resolution is poor

### Watch Mode Shows Old Output
**Problem:** Output doesn't reflect latest changes
**Solution:**
```powershell
# Ctrl+R in watch mode terminal to force restart
# Or Ctrl+C and restart: dotnet watch run
```

---

## Backup Plan: No Watch Mode

If `dotnet watch` has issues during presentation, fall back to manual builds:

```powershell
# After each edit:
dotnet build
dotnet run
```

Less impressive but still functional.

---

## Screen Recording Setup (Optional)

Consider recording each iteration's output beforehand as backup:

```powershell
# Use Windows built-in recording (Win+G)
# Or OBS Studio for better quality
# Record each iteration's full run
# Have videos ready in case live demo fails
```

---

## Final Pre-Presentation Test

Run through this checklist 30 minutes before presenting:

- [ ] `dotnet watch run` starts successfully
- [ ] Can switch between iterations and see output
- [ ] VIP refactoring demo works smoothly
- [ ] All Example files are ready to copy/paste
- [ ] Editor layout is comfortable and visible to audience
- [ ] Font sizes are readable from back of room
- [ ] Terminal output is visible
- [ ] Have slides open in browser
- [ ] Presentation notes accessible

---

## Quick Command Reference

```powershell
# Start watch mode
cd src\CustomerDemo
dotnet watch run

# Restart watch mode
# Press Ctrl+R in watch terminal

# Stop watch mode
# Press Ctrl+C

# Manual build and run
dotnet build
dotnet run

# Clear terminal
clear   # or cls

# Switch git branches (if using branch approach)
git checkout iteration-0
git checkout iteration-1
git checkout iteration-2
```

---

## Tips for Smooth Delivery

1. **Practice Transitions:** Run through iteration switches several times beforehand
2. **Prepare Copy/Paste:** Have Example file contents ready to paste quickly
3. **Narrate Actions:** Explain what you're doing as you edit files
4. **Let Build Finish:** Wait for watch to complete rebuild before talking about output
5. **Handle Errors Gracefully:** If compilation fails, explain the error - it's a teaching moment!
6. **Zoom In:** Make sure code is readable from the back of the room
7. **Backup Files:** Keep backup copies of working iteration files just in case

---

## Post-Presentation Cleanup

After presentation, restore main branch:

```powershell
git checkout feature/presentation-creation
# If you made temporary edits, discard them:
git restore src/CustomerDomain/CustomerDomain.fsproj
git restore src/CustomerDemo/Program.cs
```

Or commit your final state for future reference:

```powershell
git add -A
git commit -m "Post-presentation state"
git push origin feature/presentation-creation
```
