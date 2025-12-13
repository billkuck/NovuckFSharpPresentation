# Add-on Module: SAFE Stack Introduction

**Duration:** 3 minutes  
**Insertion Point:** After VIP add-on, or after core script Scene 11A/11B  
**Style:** Fast-paced orientation, presentational  
**Purpose:** Quick introduction to SAFE Stack for web-focused audiences

---

## Physical Setup

**[DEV A]** - At screen, navigating project structure  
**[DEV 1]** - Standing nearby, asking clarifying questions  
**Screen focus** - VS Code or Rider showing SAFE project structure

---

## Entry Dialogue

**[DEV 1]**  
So we've got these great F# types. But this is still a console app. What about web applications?

**[DEV A]**  
Perfect question. Let me show you the SAFE Stack.

**[DEV 1]**  
SAFE?

**[DEV A]**  
It's an acronym. Full-stack F# web development.

---

## Scene 1: The Acronym (1 min)

**[DEV A]** _(pulls up diagram or types on screen)_

```
S - Saturn     (Server-side web framework)
A - Azure      (Cloud hosting - but you can host anywhere)
F - Fable      (F# to JavaScript compiler)
E - Elmish     (MVU architecture for UI)
```

**[DEV A]**  
Saturn handles HTTP routing on the server. Fable compiles F# to JavaScript for the browser. Elmish gives you a React-like architecture.

**[DEV 1]**  
So the whole stack is F#? Client and server?

**[DEV A]**  
Exactly. Same language, same types, everywhere.

**[DEV 1]**  
What does that buy you?

**[DEV A]**  
Let me show you the killer feature.

---

## Scene 2: Shared Types (1.5 min)

**[DEV A]** _(navigates to SAFE project structure)_

**[Screen shows folder structure:]**
```
src/
  Shared/
    Shared.fs          ← Domain types here
  Server/
    Server.fs          ← Saturn API
  Client/
    Index.fs           ← Fable/Elmish UI
```

**[DEV A]**  
See the `Shared` folder? That's where our `Customer` type lives.

**[Opens Shared/Shared.fs:]**

```fsharp
namespace Shared

type RegisteredCustomer = {
    Id: string
    Tier: CustomerTier option
}

type UnregisteredCustomer = {
    Id: string
}

type CustomerTier =
    | Standard
    | VIP

type Customer =
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
```

**[DEV 1]**  
Same types we've been working with.

**[DEV A]**  
Right. Now watch this.

**[Opens Server/Server.fs, shows API endpoint:]**

```fsharp
let getCustomer customerId = 
    // Returns a Customer type
    Customer.NewRegistered { Id = customerId; Tier = Some Standard }

let webApp = router {
    get "/api/customer" (fun ctx ->
        json (getCustomer "123") ctx)
}
```

**[DEV A]**  
Server returns a `Customer`. Now look at the client.

**[Opens Client/Index.fs, shows usage:]**

```fsharp
type Model = {
    Customer: Customer option
}

let loadCustomer() = async {
    let! customer = Http.get<Customer> "/api/customer"
    return CustomerLoaded customer
}
```

**[DEV 1]**  
Wait. The client just... knows what type the server is returning?

**[DEV A]**  
Exactly. No DTOs. No JSON mapping. No "frontend types" vs "backend types." It's the same type, compiled differently for each environment.

**[DEV 1]**  
That's... actually huge.

**[DEV A]**  
And if you change the type on the server, the client code breaks at compile time.

**[DEV 1]**  
Cross-environment type safety.

**[DEV A]**  
Precisely.

---

## Scene 3: The Payoff (30 seconds)

**[DEV 1]** _(to audience)_  
So you're saying I can refactor my domain model, and it updates everywhere? Server, client, database mapping?

**[DEV A]**  
Everywhere the type is used. One definition, full-stack safety.

**[DEV 1]**  
No DTOs, no mappers, no "frontend team vs backend team" mismatches.

**[DEV A]**  
Nope. Just types.

**[DEV 1]**  
Okay, I'm sold on the concept. Can you actually show it working?

**[DEV A]**  
Absolutely. Let me show you Elmish and the MVU pattern...

**[Transition to SAFE Elmish add-on]**

---

## Exit Options

### If continuing to SAFE Elmish:
**[DEV A]**  
Absolutely. Let me show you Elmish and the MVU pattern...

**[Continue to SAFE Elmish add-on]**

### If continuing to SAFE Server only:
**[DEV A]**  
Let me show you how to build the server-side API first...

**[Continue to SAFE Server add-on]**

### If wrapping up here:
**[DEV A]** _(to audience)_  
That's the SAFE Stack in a nutshell. Same types everywhere, type-safe APIs, full-stack F#.

**[DEV 1]**  
Questions about the SAFE Stack?

**[Open floor for Q&A]**

---

## Production Notes

### Key Moments
- **Aha Moment:** When they realize shared types mean no DTO mapping
- **Visual Impact:** Showing same type definition used in client and server files side-by-side

### If Running Behind
- Skip the acronym explanation (just say "full-stack F# framework")
- Show only the Shared folder, skip server/client details
- Just establish the concept, move to Elmish demo

### Common Questions to Expect
- "Does it work with existing .NET APIs?" → Yes, Fable can call any REST API
- "What about React?" → Fable.React exists, or use Elmish with React bindings
- "Performance?" → Fable output is fast, comparable to TypeScript
- "Can I mix with TypeScript?" → Yes, via bindings

### Physical Staging
- Stay at screen for this segment (fast file navigation)
- DEV 1 can point at specific parts of code structure
- Keep energy high with quick cuts between files

---

**END OF SAFE INTRO ADD-ON**

**Next possible modules:**
- → SAFE Elmish add-on (10-12 min) - MVU pattern deep-dive
- → SAFE Server add-on (5-7 min) - Saturn API implementation
- → Wrap-up and Q&A
