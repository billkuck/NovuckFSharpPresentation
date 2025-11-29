# Customer Domain Demo - Presentation Code

This folder contains the working code for the F# Type-Driven Development presentation, showing the progressive enhancement from naive C#-style modeling to sophisticated F# types.

## Structure

### F# Library (`CustomerDomain/`)
- **Library.fs** - Current/final version (Iteration 2 - Explicit Eligibility)
- **Iteration0_Naive.fs** - Naive model with boolean flags
- **Iteration1_DUWithBoolean.fs** - DU separating Registered/Guest, still using boolean
- **Iteration2_ExplicitEligibility.fs** - Final version with explicit eligibility cases

### C# Console App (`CustomerDemo/`)
- **Program.cs** - Full demo with all features (pattern matching, wrappers, etc.)
- **Examples/Iteration0_Demo.cs** - Demo code showing problems with naive model
- **Examples/Iteration1_Demo.cs** - Demo code showing improvement with DU
- **Examples/Iteration2_Demo.cs** - Demo code showing final version benefits

> Note: The Examples/ folder files are reference code for copying into Program.cs during the demo. They are excluded from compilation to avoid multiple top-level statements.

## How to Use During Presentation

### Side-by-Side Window Demo (Minutes 13-33)

The presentation calls for showing F# code on the LEFT and C# code on the RIGHT, evolving through three iterations. You'll need to manually edit files to switch between iterations.

**Preparation:**
1. Open VS Code with two editor groups side-by-side
2. LEFT: Open `CustomerDomain/CustomerDomain.fsproj` and the iteration file you want to show
3. RIGHT: Open `CustomerDemo/Program.cs`

**During the Demo:**

1. **Iteration 0** (Minutes 13-15 - Show the Problem):
   
   a. Edit `CustomerDomain/CustomerDomain.fsproj`:
   ```xml
   <Compile Include="Iteration0_Naive.fs" />
   ```
   
   b. Replace `CustomerDemo/Program.cs` content with `Examples/Iteration0_Demo.cs`
   
   c. Build and run:
   ```powershell
   dotnet build
   dotnet run
   ```
   
   d. Point out: Shows illegal state being created (IsEligible=true, IsRegistered=false)

   d. Point out: Shows illegal state being created (IsEligible=true, IsRegistered=false)

2. **Iteration 1** (Minutes 15-18 - First Improvement):
   
   a. Edit `CustomerDomain/CustomerDomain.fsproj`:
   ```xml
   <Compile Include="Iteration1_DUWithBoolean.fs" />
   ```
   
   b. Replace `CustomerDemo/Program.cs` content with `Examples/Iteration1_Demo.cs`
   
   c. Build and run
   
   d. Point out: `UnregisteredCustomer` doesn't have `IsEligible` property - can't create eligible guest!

3. **Iteration 2** (Minutes 18-25 - Final Form):
   
   a. Edit `CustomerDomain/CustomerDomain.fsproj`:
   ```xml
   <Compile Include="Iteration2_ExplicitEligibility.fs" />
   ```
   
   b. Replace `CustomerDemo/Program.cs` content with `Examples/Iteration2_Demo.cs`
   
   c. Build and run
   
   d. Point out: Domain language is explicit, no boolean flags, impossible states are impossible

### For Final Demo (Minutes 33-40)

Restore the full demo:

a. Edit `CustomerDomain/CustomerDomain.fsproj`:
```xml
<Compile Include="Library.fs" />
```

b. Restore original `Program.cs` (or use git to restore it)

c. Run to show all features: pattern matching, wrappers, extension methods

### Quick Command Reference

```powershell
# From the CustomerDemo folder:
cd c:\git\NovuckFSharpPresentation\src\CustomerDemo

# Build after changing F# iteration
dotnet build

# Run current version
dotnet run
```

### Alternative: Use Branches

For smoother transitions, consider creating git branches:
- `iteration-0` - With Iteration0 active
- `iteration-1` - With Iteration1 active  
- `iteration-2` - With Iteration2 active (current state)

Then during presentation: `git checkout iteration-1` → show code → rebuild → demo

## Running the Current Demo

The default setup shows the final version (Iteration 2):

```powershell
cd src\CustomerDemo
dotnet run
```

Expected output:
- All four test cases pass (John, Mary, Richard, Sarah)
- Pattern matching examples
- Bonus calculation
- C#-friendly wrapper demonstration

## Notes for Presenters

- The progression shows how F# types improve gradually
- Each iteration compiles and runs independently
- C# consumption code changes as F# types improve
- Emphasize that improvements benefit BOTH F# and C# code
- All test cases are based on Ian Russell's Essential F# Chapter 1

## Timing

- Total demo time: ~20 minutes for progressive enhancement
- 2-3 minutes per iteration
- Allow time for build/run between iterations
- Have test runs ready in case of build issues
