# Add-on Module: SAFE Stack Elmish/MVU Pattern

**Duration:** 10-12 minutes  
**Insertion Point:** After SAFE Intro add-on  
**Style:** Narrative with conversational moments, more presentation-focused  
**Purpose:** Show complete MVU (Model-View-Update) cycle with Customer type editor

---

## Physical Setup

**[DEV A]** - Primary narrator, at keyboard  
**[DEV 1]** - Supporting questions, highlights key concepts  
**Screen focus** - Client/Index.fs in SAFE project, browser split-screen for demo

---

## Prerequisites
- SAFE Intro add-on completed
- Audience understands Shared types concept
- Browser window ready with app running (can start in background)

---

## Entry Dialogue (from SAFE Intro)

**[DEV A]**  
Let me show you Elmish and the MVU pattern...

**[DEV 1]**  
MVU?

**[DEV A]**  
Model-View-Update. It's Elm architecture for F#. You might know it from Redux or similar patterns.

**[DEV 1]**  
State, actions, reducers?

**[DEV A]**  
Exactly. But fully type-safe and functional. Let me build a simple customer editor to show you.

---

## Scene 1: The Model (2 min)

**[DEV A]** _(opens Client/Index.fs)_  
First, we define our application state. This is the Model.

**[Types:]**

```fsharp
namespace Client

open Shared

type Model = {
    CustomerId: string
    CustomerType: Customer option
    ErrorMessage: string option
}

let init() : Model * Cmd<Msg> =
    let initialModel = {
        CustomerId = ""
        CustomerType = None
        ErrorMessage = None
    }
    initialModel, Cmd.none
```

**[DEV 1]**  
So the Model is just the current state of the UI?

**[DEV A]**  
Right. What the user has entered, what customer they've created, any errors. Everything.

**[DEV 1]**  
And `Cmd<Msg>`?

**[DEV A]**  
Commands to run. Like "fetch data from server" or "navigate to a page." We'll see that in a minute.

**[DEV 1]**  
Okay, so `init()` returns the starting state and no commands.

**[DEV A]**  
Exactly. Now let's define what events can happen.

---

## Scene 2: The Messages (2 min)

**[DEV A]** _(adds to file)_  
These are all the things a user can do, or that can happen in the app.

```fsharp
type Msg =
    | IdChanged of string
    | CreateStandard
    | CreateRegistered  
    | CreateGuest
    | CreateVIP
    | ClearCustomer
```

**[DEV 1]**  
Those are like events? Or actions in Redux terms?

**[DEV A]**  
Exactly. User types in the ID field → `IdChanged` message. User clicks "Make Standard" button → `CreateStandard` message.

**[DEV 1]**  
And these are discriminated unions too.

**[DEV A]**  
Yep. Everything's a type in F#. Now let's handle these messages.

---

## Scene 3: The Update Function (3 min)

**[DEV A]**  
This is the heart of Elmish. Pure function: given current model and a message, return new model.

**[Types:]**

```fsharp
let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | IdChanged newId ->
        { model with CustomerId = newId }, Cmd.none
    
    | CreateStandard ->
        let customer = Customer.NewRegistered {
            Id = model.CustomerId
            Tier = Some CustomerTier.Standard
        }
        { model with 
            CustomerType = Some customer
            ErrorMessage = None 
        }, Cmd.none
    
    | CreateRegistered ->
        let customer = Customer.NewRegistered {
            Id = model.CustomerId
            Tier = None
        }
        { model with 
            CustomerType = Some customer
            ErrorMessage = None 
        }, Cmd.none
    
    | CreateGuest ->
        let customer = Customer.NewGuest {
            Id = model.CustomerId
        }
        { model with 
            CustomerType = Some customer
            ErrorMessage = None 
        }, Cmd.none
    
    | CreateVIP ->
        let customer = Customer.NewRegistered {
            Id = model.CustomerId
            Tier = Some CustomerTier.VIP
        }
        { model with 
            CustomerType = Some customer
            ErrorMessage = None 
        }, Cmd.none
    
    | ClearCustomer ->
        { model with CustomerType = None }, Cmd.none
```

**[DEV 1]**  
So every message creates a new version of the model?

**[DEV A]**  
Right. Immutable updates. Old model → message → new model.

**[DEV 1]**  
And the pattern matching ensures you handle every case.

**[DEV A]**  
Exactly. If I add a new `Msg` case, the compiler tells me to update this function.

**[DEV 1]**  
What if the ID is empty when they click "Create Standard"?

**[DEV A]**  
Good catch. We could add validation. For this demo, we'll keep it simple.

**[DEV 1]**  
Fair enough. So now we have state and state transitions. How do we show it?

---

## Scene 4: The View Function (4 min)

**[DEV A]**  
The View is a pure function too. Model → HTML.

**[Types:]**

```fsharp
open Feliz
open Feliz.Bulma

let view (model: Model) (dispatch: Msg -> unit) =
    Bulma.container [
        Bulma.title.h1 "Customer Type Editor"
        
        // Input field for ID
        Bulma.field.div [
            Bulma.label "Customer ID"
            Bulma.input.text [
                prop.placeholder "Enter customer ID"
                prop.value model.CustomerId
                prop.onChange (fun (value: string) -> dispatch (IdChanged value))
            ]
        ]
        
        // Buttons to create different customer types
        Bulma.field.div [
            prop.className "buttons"
            prop.children [
                Bulma.button.button [
                    color.isPrimary
                    prop.text "Create Standard"
                    prop.onClick (fun _ -> dispatch CreateStandard)
                ]
                Bulma.button.button [
                    color.isInfo
                    prop.text "Create VIP"
                    prop.onClick (fun _ -> dispatch CreateVIP)
                ]
                Bulma.button.button [
                    color.isLight
                    prop.text "Create Registered (no tier)"
                    prop.onClick (fun _ -> dispatch CreateRegistered)
                ]
                Bulma.button.button [
                    color.isWarning
                    prop.text "Create Guest"
                    prop.onClick (fun _ -> dispatch CreateGuest)
                ]
            ]
        ]
        
        // Display current customer state
        match model.CustomerType with
        | Some customer ->
            Bulma.box [
                Bulma.content [
                    Html.h3 "Current Customer:"
                    Html.p [
                        prop.text (
                            match customer with
                            | Customer.Registered r ->
                                match r.Tier with
                                | Some CustomerTier.Standard -> 
                                    $"Registered Standard: {r.Id}"
                                | Some CustomerTier.VIP -> 
                                    $"Registered VIP: {r.Id}"
                                | None -> 
                                    $"Registered (no tier): {r.Id}"
                            | Customer.Guest g -> 
                                $"Guest: {g.Id}"
                        )
                    ]
                ]
                Bulma.button.button [
                    color.isDanger
                    prop.text "Clear"
                    prop.onClick (fun _ -> dispatch ClearCustomer)
                ]
            ]
        | None ->
            Bulma.notification [
                color.isInfo
                prop.text "No customer created yet. Enter an ID and click a button above."
            ]
    ]
```

**[DEV 1]**  
Whoa. That's a lot of code.

**[DEV A]**  
But notice: it's just a function. Given a model, return HTML elements.

**[DEV 1]**  
And `dispatch`?

**[DEV A]**  
That's how you send messages back to the update loop. User types → `dispatch (IdChanged newValue)` → update function runs → new model → view re-renders.

**[DEV 1]**  
So it's a cycle. View → Message → Update → View.

**[DEV A]**  
Exactly. And notice how we pattern match on the customer type to display it?

**[Points to the match expression in the view]**

**[DEV 1]**  
You're handling all the cases: Registered with Standard, Registered with VIP, Registered with no tier, Guest.

**[DEV A]**  
Right. The compiler ensures I don't miss any. Let's run it.

---

## Scene 5: Live Demo (2-3 min)

**[DEV A]** _(splits screen: code left, browser right)_

**[Browser shows the running app]**

**[DEV A]**  
Alright, the app is running. Let me create a customer.

**[Types "Alice" in the ID field]**

**[DEV 1]**  
The model's `CustomerId` is updating as you type.

**[DEV A]**  
Yep. Every keystroke sends `IdChanged "Alice"`.

**[Clicks "Create Standard" button]**

**[Browser shows:]**
```
Current Customer:
Registered Standard: Alice

[Clear]
```

**[DEV 1]**  
That dispatched `CreateStandard`, which called the update function, which created a new model with `CustomerType = Some (Customer.Registered ...)`.

**[DEV A]**  
Right. And the view re-rendered automatically.

**[Clicks "Clear"]**

**[Notification appears: "No customer created yet..."]**

**[DEV A]**  
Let me try VIP now.

**[Types "Bob", clicks "Create VIP"]**

**[Browser shows:]**
```
Current Customer:
Registered VIP: Bob
```

**[DEV 1]**  
Same flow. Message → Update → View.

**[DEV A]**  
And it's all type-safe. If I change the `Customer` type in Shared, this UI code will break at compile time.

**[DEV 1]**  
So refactoring the domain model automatically propagates everywhere.

**[DEV A]**  
Everywhere. Server, client, database layer if you have one. All compile-time checked.

---

## Scene 6: The Payoff (1 min)

**[DEV 1]** _(to audience)_  
So just to recap: we defined our domain types once, in the Shared project.

**[DEV A]**  
The server uses them for APIs.

**[DEV 1]**  
The client uses them for UI state.

**[DEV A]**  
And the MVU pattern gives us predictable, testable state management.

**[DEV 1]**  
With full compiler verification at every step.

**[DEV A]**  
That's the SAFE Stack. Full-stack F#, end-to-end type safety.

**[DEV 1]**  
This is pretty powerful.

**[DEV A]**  
It really is. Questions?

---

## Exit Options

### If continuing to Copilot add-on:
**[DEV 1]**  
Can we make this even faster? Like, could we generate UI code from the types?

**[DEV A]**  
Funny you mention that. Let me show you something with Copilot...

**[Continue to Copilot add-on]**

### If wrapping up here:
**[DEV A]** _(to audience)_  
That's the full MVU cycle. Model, Message, Update, View. All type-safe, all testable, all F#.

**[DEV 1]**  
Any questions about Elmish or the SAFE Stack?

**[Open floor for Q&A]**

---

## Production Notes

### Key Moments
- **Update Function:** When they see pattern matching ensures all cases handled
- **Live Demo:** Watching the cycle in action (keystroke → update → render)
- **Cross-cutting Safety:** Emphasizing how domain changes propagate everywhere

### If Running Behind
- Skip the detailed View code walkthrough (just show it briefly)
- Pre-record the browser demo and play it back
- Focus on Model and Update, show View as "here's how we render it"

### Common Questions to Expect
- "What about side effects?" → Cmd<Msg> handles them, keeps update pure
- "Performance?" → Virtual DOM diffing like React, very fast
- "Debugging?" → Time-travel debugging via Elmish debugger
- "Compared to React?" → Similar concepts, more type safety

### Physical Staging
- DEV A stays at keyboard for this segment
- DEV 1 can point at browser during demo
- Keep split-screen visible: code + running app

### Code Preparation
- Have the app already running before this segment
- Ensure hot reload works for any live edits
- Pre-test that all buttons work correctly

### Graceful Exit (if audience disengaged)
**[DEV A]**  
Actually, the details aren't as important as the concept. The point is: types everywhere, compiler checks everything.

**[Skip to wrap-up]**

---

**END OF SAFE ELMISH ADD-ON**

**Next possible modules:**
- → Copilot add-on (4 min) - AI-assisted UI generation
- → Wrap-up and Q&A
