# SAFE Stack Demo Plan - Customer Tier Management

## Overview
Build a Customer Tier Management Admin Panel using SAFE Stack to demonstrate:
- Shared F# domain model between client and server
- Type-safe full-stack development
- Elmish MVU architecture
- Compiler-enforced UI correctness

## Core Concept
**Admin Panel:** Manage customer discount tiers with live tier switching and discount preview

### Demo Focus (Priority Order)
1. **Shared Domain Model** (★★★ CRITICAL) - Same `Customer` DU in browser and server
2. **Shared Business Logic** (★★★ CRITICAL) - Same calculation functions client & server
3. **Elmish Immutability** (★★★ CRITICAL) - State cannot be corrupted, only transformed
4. **Type-Safe UI** (★★★ CRITICAL) - Pattern matching ensures all cases handled
5. **Live Tier Switching** (★★ HIGH) - Visual tier changes with instant feedback
6. **Discount Calculator** (★★ HIGH) - Shows shared logic in action
7. **Server API** (LOW) - Minimal CRUD, not the focus

---

## Domain Model (Shared.fs)

```fsharp
namespace Shared

type RegisteredCustomer = { 
    Id: string
    Name: string 
}

type UnregisteredCustomer = { 
    Id: string
    Name: string 
}

type Customer = 
    | VIP of RegisteredCustomer
    | Standard of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer

module Customer =
    // Idiomatic F# - extract common data from DU cases
    let getId = function
        | VIP c | Standard c | Registered c -> c.Id
        | Guest c -> c.Id
    
    let getName = function
        | VIP c | Standard c | Registered c -> c.Name
        | Guest c -> c.Name
    
    // Alternative: More explicit but verbose
    // let getId customer =
    //     match customer with
    //     | VIP c -> c.Id
    //     | Standard c -> c.Id
    //     | Registered c -> c.Id
    //     | Guest c -> c.Id
    
    let getTier = function
        | VIP _ -> "VIP"
        | Standard _ -> "Standard"
        | Registered _ -> "Registered"
        | Guest _ -> "Guest"
    
    // ⭐ CRITICAL: Business logic in Shared
    // These functions run IDENTICALLY on client (browser) and server
    let calculateDiscount customer spend =
        match customer with
        | VIP _ when spend >= 100m -> spend * 0.15m      // 15% for VIP
        | Standard _ when spend >= 100m -> spend * 0.1m  // 10% for Standard
        | _ -> 0m                                         // No discount
    
    let calculateTotal customer spend =
        spend - calculateDiscount customer spend
    
    // Discount percentage for display
    let getDiscountPercent = function
        | VIP _ -> 15
        | Standard _ -> 10
        | Registered _ | Guest _ -> 0
    
    // Helper for checking if customer is registered
    let isRegistered = function
        | VIP _ | Standard _ | Registered _ -> true
        | Guest _ -> false
    
    // ⭐ CRITICAL: Transition validation in Shared
    // Same validation logic used for UI button visibility AND server validation
    type TierAction = Promote | Demote | Register | Unregister
    
    let canTransition action customer =
        match action, customer with
        // Promotions (upward)
        | Promote, Registered _ -> true   // Registered → Standard
        | Promote, Standard _ -> true     // Standard → VIP
        // Demotions (downward)
        | Demote, VIP _ -> true          // VIP → Standard
        | Demote, Standard _ -> true     // Standard → Registered
        // Registration
        | Register, Guest _ -> true       // Guest → Registered
        | Unregister, Registered _ -> true // Registered → Guest
        // All other transitions invalid
        | _ -> false
    
    let applyTransition action customer =
        match action, customer with
        | Promote, Registered c -> Standard c
        | Promote, Standard c -> VIP c
        | Demote, VIP c -> Standard c
        | Demote, Standard c -> Registered c
        | Register, Guest c -> Registered { Id = c.Id; Name = c.Name }
        | Unregister, Registered c -> Guest { Id = c.Id; Name = c.Name }
        | _ -> customer  // No change if invalid

// API Contract
type CustomerDto = {
    Id: string
    Name: string
    Tier: string  // "VIP", "Standard", "Registered", "Guest"
}

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ICustomerApi = {
    getCustomers : unit -> Async<CustomerDto list>
    changeTier : string -> string -> Async<CustomerDto>
}
```

---

## UI Features

### 1. Customer List (Main View)
**Layout:**
```
┌─────────────────────────────────────────────────┐
│  Customer Tier Management                       │
├─────────────────────────────────────────────────┤
│  Name        Tier         Actions               │
├─────────────────────────────────────────────────┤
│  Alice       [VIP]        ⬇ Demote              │
│  Bob         [STANDARD]   ⬆ Promote  ⬇ Demote   │
│  Carol       [REGISTERED] ⬆ Promote  ⬇ Demote   │
│  Dave        [GUEST]      → Register             │
└─────────────────────────────────────────────────┘
```

**Color-Coded Tier Badges:**
- VIP: Gold/Yellow
- Standard: Blue
- Registered: Gray
- Guest: Light Gray

### 2. Tier Actions (Contextual Buttons)
**Valid Transitions:**
- Guest → Registered (Register)
- Registered → Standard (Promote)
- Standard → VIP (Promote)
- VIP → Standard (Demote)
- Standard → Registered (Demote)
- Registered → Guest (Unregister)

**Illegal Transitions (Not Allowed):**
- Guest → Standard (must register first)
- Guest → VIP (must register first)

### 3. Discount Calculator Widget
**Layout:**
```
┌─────────────────────────────────────┐
│  Discount Calculator                │
├─────────────────────────────────────┤
│  Spend: £ [____100____]             │
│                                     │
│  Alice (VIP):      £100 → £85  💰  │
│  Bob (Standard):   £100 → £90  💵  │
│  Carol (Registered): £100 → £100    │
│  Dave (Guest):     £100 → £100      │
└─────────────────────────────────────┘
```

---

## Elmish MVU Architecture

**Key Principle:** State corruption is impossible because:
1. **Immutable Model** - Model is never mutated, only copied with changes
2. **Pure Update Function** - `update` returns new model, doesn't modify existing
3. **Single Source of Truth** - Only one `Model` exists at any time
4. **Unidirectional Flow** - UI → Message → Update → New Model → UI

### Model
```fsharp
type Model = {
    Customers: Customer list      // Immutable list
    SpendAmount: decimal
    Loading: bool
    Error: string option
}

// Model is NEVER mutated - only transformed
// Old model is discarded, new model is created
```

### Messages
```fsharp
type Msg =
    | LoadCustomers
    | CustomersLoaded of Customer list
    | ChangeCustomerTier of customerId: string * newTier: string
    | CustomerTierChanged of CustomerDto
    | UpdateSpendAmount of decimal
    | ApiError of exn

// Every state change is explicit via a Message
// No hidden mutations, no side effects in UI
```

### Update
```fsharp
let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | LoadCustomers ->
        // Returns NEW model with Loading = true
        { model with Loading = true }, 
        Cmd.OfAsync.perform customerApi.getCustomers () CustomersLoaded
    
    | CustomersLoaded customers ->
        // Returns NEW model with customers updated
        { model with Customers = customers; Loading = false }, 
        Cmd.none
    
    | ChangeCustomerTier (id, tier) ->
        { model with Loading = true },
        Cmd.OfAsync.perform 
            (customerApi.changeTier id) tier CustomerTierChanged
    
    | CustomerTierChanged dto ->
        // CRITICAL: Customer list is transformed, not mutated
        // List.map creates NEW list with NEW customers
        let updatedCustomers = 
            model.Customers 
            |> List.map (fun c -> 
                if Customer.getId c = dto.Id 
                then dtoToCustomer dto  // NEW customer instance
                else c)                  // Keep existing customer
        
        // Returns NEW model with NEW customer list
        { model with Customers = updatedCustomers; Loading = false },
        Cmd.none
    
    | UpdateSpendAmount amount ->
        { model with SpendAmount = amount }, Cmd.none
    
    | ApiError ex ->
        { model with Error = Some ex.Message; Loading = false }, 
        Cmd.none

// Pure function: same input → same output
// No hidden state, no mutations, no surprises
```

### View
```fsharp
// Helper to render action buttons based on valid transitions
let actionButtons customer dispatch =
    let customerId = Customer.getId customer
    
    // ⭐ Use shared validation to determine which buttons to show
    [
        if Customer.canTransition Customer.Promote customer then
            button [ 
                OnClick (fun _ -> dispatch (ChangeCustomerTier (customerId, "Promote")))
                Class "btn-promote" 
            ] [ str "⬆ Promote" ]
        
        if Customer.canTransition Customer.Demote customer then
            button [ 
                OnClick (fun _ -> dispatch (ChangeCustomerTier (customerId, "Demote")))
                Class "btn-demote" 
            ] [ str "⬇ Demote" ]
        
        if Customer.canTransition Customer.Register customer then
            button [ 
                OnClick (fun _ -> dispatch (ChangeCustomerTier (customerId, "Register")))
                Class "btn-register" 
            ] [ str "→ Register" ]
        
        if Customer.canTransition Customer.Unregister customer then
            button [ 
                OnClick (fun _ -> dispatch (ChangeCustomerTier (customerId, "Unregister")))
                Class "btn-unregister" 
            ] [ str "✕ Unregister" ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    div [ Class "container" ] [
        h1 [] [ str "Customer Tier Management" ]
        
        // Customer List
        customerListView model.Customers dispatch
        
        // Discount Calculator
        discountCalculatorView model.Customers model.SpendAmount dispatch
        
        // Loading/Error
        if model.Loading then loadingSpinner ()
        match model.Error with
        | Some err -> errorMessage err
        | None -> nothing
    ]
```

---

## Server API (Minimal)

### In-Memory Store (Demo Only)
```fsharp
let mutable customers = [
    VIP { Id = "1"; Name = "Alice" }
    Standard { Id = "2"; Name = "Bob" }
    Registered { Id = "3"; Name = "Carol" }
    Guest { Id = "4"; Name = "Dave" }
]
```

### API Implementation
```fsharp
let customerApi : ICustomerApi = {
    getCustomers = fun () -> async {
        return customers |> List.map customerToDto
    }
    
    changeTier = fun customerId actionStr -> async {
        let customer = customers |> List.find (fun c -> Customer.getId c = customerId)
        
        // Parse action string to TierAction
        let action = 
            match actionStr with
            | "Promote" -> Some Customer.Promote
            | "Demote" -> Some Customer.Demote
            | "Register" -> Some Customer.Register
            | "Unregister" -> Some Customer.Unregister
            | _ -> None
        
        // ⭐ Use shared validation and transition logic
        let updated = 
            match action with
            | Some act when Customer.canTransition act customer ->
                Customer.applyTransition act customer
            | _ -> customer  // Invalid action or transition
        
        customers <- customers |> List.map (fun c -> 
            if Customer.getId c = customerId then updated else c)
        
        return customerToDto updated
    }
}
```

---

## Presentation Demo Script

### Setup (Before Demo)
1. SAFE app running on localhost
2. Browser open to admin panel
3. F# code editor showing `Shared.fs`

### Demo Flow (5-7 minutes)

**1. Show Shared Domain Model + Business Logic (2 min)** ⭐ CRITICAL
- Open `Shared.fs` in editor
- Point to `Customer` DU
- **Scroll down to business logic functions:**
  ```fsharp
  let calculateDiscount customer spend = ...
  let calculateTotal customer spend = ...
  ```
- **KEY POINT:** "These exact functions run in browser AND server"
- **Emphasize:** 
  - "Browser: Calculate discount preview instantly"
  - "Server: Validate discount before applying"
  - "Same code, same logic, impossible to drift"
- "No duplicate logic, no API contract mismatches"

**2. Show Customer List UI (1 min)**
- Show list with color-coded tiers
- Point out type-safe rendering: "Pattern match ensures all tiers handled"

**3. Live Tier Switching + Elmish Flow (3 min)** ⭐ CRITICAL
- Open browser dev tools (F12) → Show Elmish Debugger or console
- Click "Promote" on Bob: Standard → VIP
  - **Point out:** Message dispatched: `ChangeCustomerTier("2", "VIP")`
  - **Emphasize:** "Model never mutated - new model created"
- Show instant UI update (VIP badge, discount changes)
- Open `Index.fs` in editor alongside browser
- **Key Point:** "Look at update function - pure, no mutations"
  ```fsharp
  | CustomerTierChanged dto ->
      let updatedCustomers = 
          model.Customers 
          |> List.map (fun c -> if ... then NEW else c)
      { model with Customers = updatedCustomers }  // NEW model
  ```
- **Contrast with React/JavaScript:**
  - "In JS: `customer.tier = 'VIP'` // Mutation!"
  - "In Elmish: Create new customer, create new list, create new model"
  - "Old model discarded → no corruption possible"

- Try illegal transition: Dave (Guest) → Standard (show it prevents or no-ops)
- Register Dave first: Guest → Registered, then promote to Standard

**4. Discount Calculator - Shared Logic in Action (2 min)** ⭐ CRITICAL
- Show discount calculator widget
- Change spend amount (e.g., £50 → £100)
- **Watch all discounts recalculate instantly**
- Open browser dev tools → Console
- Type: `Shared.Customer.calculateTotal(customer, 100m)`
- **Show:** "Same function available in browser console!"
- **Key Point:** "Client calculates preview, server validates final price"
- Open `Shared.fs` again, point to `calculateDiscount`:
  ```fsharp
  | VIP _ when spend >= 100m -> spend * 0.15m
  | Standard _ when spend >= 100m -> spend * 0.1m
  ```
- **Emphasize:** 
  - "Change discount rules in ONE place"
  - "Client and server automatically stay in sync"
  - "Compiler verifies both use same logic"

**5. Add New Tier Live (2 min - IF TIME)**
- In `Shared.fs`, add: `| Premium of RegisteredCustomer`
- Compiler error in View: "Non-exhaustive pattern match"
- Add Premium case to view
- Show new tier badge appear in UI
- **KEY POINT:** "Compiler prevents UI bugs!"

---

## Key Talking Points

1. **"State Corruption is Impossible"** ⭐
   - Model is immutable record
   - Update function is pure: `Model → Msg → Model`
   - Old model discarded, new model created
   - Time-travel debugging possible (undo/redo for free)
   - **Contrast:** "No `customer.tier = 'VIP'` mutations like JavaScript"

2. **"One Codebase, Two Runtimes"** ⭐
   - Same F# DU in browser (Fable→JS) and server (.NET)
   - **Same business logic** - `calculateDiscount` runs client & server
   - **Same validation** - `canTransition` used for UI buttons AND server validation
   - No API contract drift - impossible by design
   - Change logic once, both sides update automatically
   - **Contrast:** "No duplicating validation rules between frontend/backend"
   - **Demo point:** "Change VIP eligibility rule → UI buttons AND server validation update automatically"

3. **"Compiler = QA Team"**
   - Add VIP tier → compiler forces UI update
   - Can't forget to handle a customer type
   - Refactoring is safe and easy

4. **"Type-Safe All The Way Down"**
   - Serialization: F# ↔ JSON ↔ F# (automatic)
   - API calls: Type-checked, no "any" or "unknown"
   - Business logic: Shared helper functions (client & server)

5. **"MVU = Predictable State"**
   - One-way data flow: View → Msg → Update → Model → View
   - Every state change explicit (no hidden mutations)
   - Pure functions = testable, debuggable, reliable

6. **"From C# to Full-Stack F#"**
   - Started with C# consuming F# library
   - Now: F# on client AND server
   - Progressive adoption path

---

## Why Elmish Prevents State Corruption (Deep Dive)

### The Problem in Traditional JavaScript/React
```javascript
// React/JavaScript - MUTABLE state
let customer = { id: 1, tier: 'Standard' };
customer.tier = 'VIP';  // MUTATION! 
// What if multiple components mutate simultaneously?
// What if async update arrives late?
// State can become inconsistent!
```

### The Elmish Solution - Immutability + Pure Functions
```fsharp
// Elmish - IMMUTABLE state
let model = { Customers = [...]; SpendAmount = 100m }

// Update is PURE - returns NEW model
let update msg model =
    match msg with
    | CustomerTierChanged dto ->
        let newCustomers = 
            model.Customers 
            |> List.map (fun c -> 
                if Customer.getId c = dto.Id 
                then dtoToCustomer dto  // NEW customer
                else c)                  // Unchanged
        
        { model with Customers = newCustomers }  // NEW model
        // Old model is garbage collected
        // Impossible to corrupt - old state is gone!
```

### Guarantees Provided by Elmish
1. **Single Source of Truth** - Only one `Model` instance exists
2. **Immutable Data** - F# records are immutable by default
3. **Pure Update** - No side effects, same input → same output
4. **Explicit State Changes** - Every change goes through `update` function
5. **Referential Transparency** - Can reason about state changes
6. **Time-Travel Debugging** - Keep history of models, undo/redo free

### Demo Contrast Points
- **JavaScript:** `customer.tier = newTier` (mutation, race conditions possible)
- **Elmish:** `{ customer with Tier = newTier }` (new instance, no races)

---

## Technical Requirements

### Prerequisites
- .NET 9.0 SDK
- Node.js (for SAFE template)
- SAFE Template: `dotnet new -i SAFE.Template`

### Project Structure
```
src/SafeDemo/
  src/
    Shared/
      Shared.fs          # Domain model (Customer DU)
    Server/
      Server.fs          # Minimal API
    Client/
      Index.fs           # Elmish MVU app
  paket.dependencies
  paket.lock
```

### Build Commands
```powershell
# Create SAFE project
dotnet new SAFE -o SafeDemo

# Run (starts server + client with hot reload)
cd SafeDemo
dotnet run
```

---

## Success Criteria

- ✅ Customer list displays with color-coded tier badges
- ✅ Tier switching works with instant UI feedback
- ✅ Discount calculator updates on tier or spend changes
- ✅ Illegal state transitions prevented (Guest can't jump to Standard)
- ✅ Shared.fs clearly shows same domain model
- ✅ Adding new tier causes compiler error (demonstrates safety)
- ✅ Demo completes in 5-7 minutes
- ✅ Non-F# devs can understand the benefits

---

## Optional Enhancements (If Time)

### 1. Customer History
- Show tier change log per customer
- Demonstrates immutable event sourcing

### 2. Bulk Operations
- Select multiple customers
- "Promote All Registered to Standard"
- Shows type-safe batch operations

### 3. Validation Rules
- "Must have 2+ years to be VIP"
- Server-side validation with same types
- Error messages flow back to UI

### 4. Real-Time Updates
- WebSocket connection
- Multiple admins see tier changes instantly
- Demonstrates SignalR with F# types

---

## Fallback Plan

**If SAFE template issues:**
- Use pre-built SAFE project from SafeStackDemos folder
- Have screenshots/video of working demo
- Show code walkthrough instead of live demo

**If time runs short:**
- Skip discount calculator
- Focus on: Shared model + Tier switching + Compiler safety
- 3-minute version possible
