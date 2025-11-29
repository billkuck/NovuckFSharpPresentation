
# F# Type-Driven Development: From C# to Correctness

## Detailed Presentation Schedule (60 minutes)

### Introduction (5 minutes)
**Minutes 0-2: Welcome & Context**
- Introduce presenters (names, background with F# and C#)
- Quick poll: "Who has written F# before?" / "Who's curious but never tried it?"
- Set expectations: This is practical, demo-heavy, and conversational ("Yes, and..." style)

**Minutes 2-3: The Promise**
- "We're going to show you how F#'s type system can catch bugs at compile time"
- "Then we'll show it's not just theoretical - works great with C# and modern web stacks"

**Minutes 3-5: Roadmap**
- Show outline on screen:
  1. Model evolution (C# → F#)
  2. C# interop demo
  3. SAFE stack web app
- "Questions are welcome throughout - interrupt us!"

---

### F# Type System & Modeling Strengths (8 minutes)

**Minutes 5-7: The Setup - The Business Rule**
- Present the scenario from Ian Russell's Essential F#:
  - "Eligible Registered Customers get 10% discount when they spend £100 or more"
- Show the naive C# approach:
  ```csharp
  class Customer {
    public string Id;
    public bool IsEligible;
    public bool IsRegistered;
  }
  ```
- Point out the problems:
  - ❌ Can be `IsEligible = true` but `IsRegistered = false` (illegal state!)
  - ❌ Nothing prevents this invalid combination
  - ❌ Easy to forget to check `IsRegistered` before checking `IsEligible`

**Minutes 7-10: F# Type System Overview**
- Quickly introduce key concepts (don't deep dive yet):
  - **Records**: Immutable by default, structural equality
  - **Discriminated Unions**: Type-safe state machines (OR types)
  - **Pattern Matching**: Compiler enforces handling all cases
- "We'll see all of these in action as we refactor"
- Show the initial F# record version (still naive):
  ```fsharp
  type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
  }
  ```
- "This is valid F#, but we're not using its strengths yet - same problems as C#"

**Minutes 10-13: Making Illegal States Unrepresentable**
- Core principle: "If it compiles, it should be correct"
- Show Ian Russell's improved version with separate record types:
  ```fsharp
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
- "Notice: Can't be Eligible without being Registered!"
- "The type itself encodes the business rules"

---

### Progressive Model Enhancement (20 minutes)

**Setup: Two Windows Side-by-Side**
- Left window: F# type definitions
- Right window: C# code consuming the types
- "Watch how the C# code changes as we improve the F# types"

**Minutes 13-15: Iteration 0 - The Naive Model**
- **Left (F#)**: Start with F# that looks like C# (from Ian Russell part1.fsx):
  ```fsharp
  type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
  }
  
  let calculateTotal customer spend =
    let discount = 
      if customer.IsEligible && spend >= 100.0M 
      then (spend * 0.1M) else 0.0M   
    spend - discount
  ```
- **Right (C#)**: Show C# consuming this:
  ```csharp
  var customer = new Customer(
    Id: "John",
    IsEligible: true,
    IsRegistered: true
  );
  
  var total = CustomerModule.calculateTotal(customer, 100.0m);
  
  // But nothing stops this illegal state:
  var badCustomer = new Customer(
    Id: "Bad",
    IsEligible: true,    // Eligible...
    IsRegistered: false  // ...but NOT registered! ❌
  );
  ```
- "Nothing stops us from creating this illegal state in either language!"
- "Yes, and we can do better..."

**Minutes 15-18: Iteration 1 - Introduce Discriminated Unions**
- Partner A: "What if we separate Registered from Unregistered customers?"
- Partner B: "Yes, and we can use a discriminated union!"
- **Left (F#)**: Refactor to (from Ian Russell part2.fsx):
  ```fsharp
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
  
  let calculateTotal customer spend =
    let discount = 
      match customer with
      | Registered c when c.IsEligible && spend >= 100.0M -> 
          spend * 0.1M
      | _ -> 0.0M
    spend - discount
  ```
- **Right (C#)**: Show how C# code must change:
  ```csharp
  // Now we have to pick a case - can't be both!
  var john = Customer.NewRegistered(
    new RegisteredCustomer("John", IsEligible: true)
  );
  
  var sarah = Customer.NewGuest(
    new UnregisteredCustomer("Sarah")
  );
  
  var johnTotal = CustomerModule.calculateTotal(john, 100.0m);
  // johnTotal = 90.0m
  
  // Can't create an eligible guest anymore - 
  // UnregisteredCustomer doesn't have IsEligible!
  // var bad = Customer.NewGuest(
  //   new UnregisteredCustomer("Bad", IsEligible: true)  // ❌ Doesn't compile!
  // );
  ```
- "Now you literally cannot create an Eligible Guest - in F# OR C#!"

**Minutes 18-21: Iteration 2 - Make Eligibility Explicit**
- Partner A: "But IsEligible is still just a boolean flag..."
- Partner B: "Yes, and we can make eligibility a real domain concept!"
- **Left (F#)**: Refactor to (from Ian Russell part3.fsx):
  ```fsharp
  type RegisteredCustomer = {
    Id: string
  }
  
  type UnregisteredCustomer = {
    Id: string
  }
  
  type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
  
  let calculateTotal customer spend =
    let discount = 
      match customer with
      | Eligible _ when spend >= 100.0M -> spend * 0.1M
      | _ -> 0.0M
    spend - discount
  ```
- **Right (C#)**: Show how the C# code becomes even clearer:
  ```csharp
  // The type TELLS you what kind of customer this is
  var john = Customer.NewEligible(
    new RegisteredCustomer("John")
  );
  
  var mary = Customer.NewEligible(
    new RegisteredCustomer("Mary")
  );
  
  var richard = Customer.NewRegistered(
    new RegisteredCustomer("Richard")
  );
  
  var sarah = Customer.NewGuest(
    new UnregisteredCustomer("Sarah")
  );
  
  // Logic in C# is now simpler too
  var johnTotal = CustomerModule.calculateTotal(john, 100.0m);
  // 90.0m - gets discount
  
  var richardTotal = CustomerModule.calculateTotal(richard, 100.0m);
  // 100.0m - no discount (not eligible)
  ```
- Show the benefits:
  - No more boolean flag to check or forget
  - Domain language is explicit: "Eligible", "Registered", "Guest"
  - Impossible states are... impossible!

**Minutes 21-25: Test All Cases Side-by-Side**
- **Left (F#)**: Show how we verify in F# Interactive:
  ```fsharp
  let john = Eligible { Id = "John" }
  let mary = Eligible { Id = "Mary" }
  let richard = Registered { Id = "Richard" }
  let sarah = Guest { Id = "Sarah" }
  
  let assertJohn = calculateTotal john 100.0M = 90.0M      // true
  let assertMary = calculateTotal mary 99.0M = 99.0M       // true
  let assertRichard = calculateTotal richard 100.0M = 100.0M // true
  let assertSarah = calculateTotal sarah 100.0M = 100.0M    // true
  ```
- **Right (C#)**: Show the same tests in C#:
  ```csharp
  var john = Customer.NewEligible(new RegisteredCustomer("John"));
  var mary = Customer.NewEligible(new RegisteredCustomer("Mary"));
  var richard = Customer.NewRegistered(new RegisteredCustomer("Richard"));
  var sarah = Customer.NewGuest(new UnregisteredCustomer("Sarah"));
  
  Assert.Equal(90.0m, CustomerModule.calculateTotal(john, 100.0m));
  Assert.Equal(99.0m, CustomerModule.calculateTotal(mary, 99.0m));
  Assert.Equal(100.0m, CustomerModule.calculateTotal(richard, 100.0m));
  Assert.Equal(100.0m, CustomerModule.calculateTotal(sarah, 100.0m));
  ```
- "Same business logic, same safety, your choice of language!"

**Minutes 25-28: Benefits Discussion**
- Quickly enumerate what we've achieved:
  - ✅ Can't create an Eligible customer who isn't Registered (impossible in both languages!)
  - ✅ Can't forget to check registration status (type enforces it)
  - ✅ Logic is simpler and clearer in both F# and C#
  - ✅ Domain language is explicit in the code
- "All of this is enforced at compile time, not runtime"
- "This is what 'making illegal states unrepresentable' means"

**Minutes 28-33: Live Refactoring Demo (Both Windows)**
- "Let's see the compiler help us in BOTH languages"
- **Left (F#)**: Add a new customer type to the DU:
  ```fsharp
  type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
    | VIP of RegisteredCustomer  // NEW!
  ```
- Show the red squiggly in F# `calculateTotal` - incomplete pattern match!
- **Right (C#)**: Show red squiggly in C# switch expression too!
  ```csharp
  var message = customer switch {
    Customer.Eligible c => $"Eligible: {c.Item.Id}",
    Customer.Registered c => $"Regular: {c.Item.Id}",
    Customer.Guest c => $"Guest: {c.Item.Id}",
    // ❌ Warning: Customer.VIP not handled!
  };
  ```
- Fix both:
  - **Left (F#)**: Add `| VIP _ when spend >= 100.0M -> spend * 0.15M`
  - **Right (C#)**: Add `Customer.VIP c => $"VIP: {c.Item.Id}",`
- "The compiler guided us to update all the places in both languages!"
- "This is what we mean by refactoring safety"

---

### C# Interop Demo (7 minutes)

**Setup: Keep Both Windows Open**
- "You've seen them side-by-side - let's go deeper on the C# side"
- "This is for teams thinking: 'Can we actually use this in our C# projects?'"

**Minutes 33-35: C# Consuming F# Types - The Basics**
- **Left (F#)**: Final model on display
  ```fsharp
  type RegisteredCustomer = { Id: string }
  type UnregisteredCustomer = { Id: string }
  
  type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
  
  let calculateTotal customer spend =
    let discount = 
      match customer with
      | Eligible _ when spend >= 100.0M -> spend * 0.1M
      | _ -> 0.0M
    spend - discount
  ```
- **Right (C#)**: Show project reference and usage
  ```csharp
  // Add project reference to F# class library
  using CustomerDomain;
  using static CustomerDomain.Customer;
  
  // Creating customers - constructors are generated automatically
  var john = NewEligible(new RegisteredCustomer("John"));
  var sarah = NewGuest(new UnregisteredCustomer("Sarah"));
  
  // Calling F# functions - just works!
  var total = CustomerModule.calculateTotal(john, 100.0m);
  ```

**Minutes 35-37: Pattern Matching in C#**
- **Right (C#)**: Show C# switch expressions (C# 8.0+)
  ```csharp
  // Pattern matching over F# discriminated unions
  string GetCustomerType(Customer customer) => customer switch
  {
    Eligible c => $"VIP Customer: {c.Item.Id}",
    Registered c => $"Regular Customer: {c.Item.Id}",
    Guest c => $"Guest: {c.Item.Id}",
    _ => throw new ArgumentException("Unknown customer type")
  };
  
  // Or access the properties directly
  decimal CalculateWithBonus(Customer customer, decimal spend)
  {
    var baseTotal = CustomerModule.calculateTotal(customer, spend);
    
    return customer switch
    {
      Eligible c => baseTotal * 0.95m,  // Extra 5% off for eligible
      _ => baseTotal
    };
  }
  ```
- "C# 8.0+ switch expressions work great with F# DUs"
- "The compiler warns you if you miss a case!"

**Minutes 37-40: Making It Feel More C#-Like (Optional Wrappers)**
- **Right (C#)**: Show how to create C#-friendly wrappers if desired
  ```csharp
  // Optional: Create a C#-friendly facade
  public static class CustomerService
  {
    public static Customer CreateEligible(string id) =>
      Customer.NewEligible(new RegisteredCustomer(id));
    
    public static Customer CreateRegistered(string id) =>
      Customer.NewRegistered(new RegisteredCustomer(id));
    
    public static Customer CreateGuest(string id) =>
      Customer.NewGuest(new UnregisteredCustomer(id));
    
    public static decimal CalculateTotal(Customer customer, decimal spend) =>
      CustomerModule.calculateTotal(customer, spend);
    
    // Can also add C#-style extension methods
    public static bool IsEligible(this Customer customer) =>
      customer.IsEligible;
    
    public static string GetId(this Customer customer) => customer switch
    {
      Customer.Eligible c => c.Item.Id,
      Customer.Registered c => c.Item.Id,
      Customer.Guest c => c.Item.Id,
      _ => throw new ArgumentException()
    };
  }
  
  // Usage becomes more C#-idiomatic
  var customer = CustomerService.CreateEligible("John");
  var total = CustomerService.CalculateTotal(customer, 100m);
  var id = customer.GetId();
  ```
- "You can wrap the F# types to make them feel more natural in C#"
- "Or use them directly - your team's choice!"
- "Best practice: Keep F# as the domain core, C# as the application shell"

---

### SAFE Stack Deep Dive (18 minutes)

**Minutes 40-41: SAFE Stack Introduction**
- Quick explanation: "Saturn (server), Azure (hosting), Fable (F#→JS), Elmish (MVU architecture)"
- "Same language client and server, share types between them"
- Show the project structure on screen

**Minutes 41-43: Copy Model to Shared Project**
- Live: Copy the domain model to `Shared/Domain.fs`
- Show it's referenced by both Server and Client projects
- "Now both sides understand the same types, no DTO mapping"

**Minutes 43-46: Server Boilerplate**
- Create a simple API endpoint in Saturn
- Show the handler function:
  ```fsharp
  let getApplications = fun next ctx -> task {
    let apps = [ /* sample data */ ]
    return! json apps next ctx
  }
  ```
- Wire it up in the application router
- Test it quickly with Postman or browser
- "That's all the server we need for this demo"

**Minutes 46-51: Client - Simple Customer Type Editor**
- Focus on Elmish MVU pattern
- **Model**: Current state
  ```fsharp
  type Model = {
    CustomerId: string
    CustomerType: Customer option
    ErrorMessage: string option
  }
  ```
- **Message**: Events that can happen
  ```fsharp
  type Msg =
    | IdChanged of string
    | MakeEligible
    | MakeRegistered
    | MakeGuest
    | CustomerLoaded of Customer
  ```
- **Update**: Pure function, state transition
  ```fsharp
  let update msg model =
    match msg with
    | IdChanged newId ->
      { model with CustomerId = newId }, Cmd.none
    | MakeEligible ->
      let customer = Eligible { Id = model.CustomerId }
      { model with CustomerType = Some customer }, Cmd.none
    | MakeRegistered ->
      let customer = Registered { Id = model.CustomerId }
      { model with CustomerType = Some customer }, Cmd.none
    | MakeGuest ->
      let customer = Guest { Id = model.CustomerId }
      { model with CustomerType = Some customer }, Cmd.none
  ```
- **View**: Render UI from current model
  ```fsharp
  let view model dispatch =
    div [] [
      input [ 
        Placeholder "Customer ID"
        OnChange (fun e -> dispatch (IdChanged e.Value)) 
      ]
      button [ OnClick (fun _ -> dispatch MakeEligible) ] [ str "Make Eligible" ]
      button [ OnClick (fun _ -> dispatch MakeRegistered) ] [ str "Make Registered" ]
      button [ OnClick (fun _ -> dispatch MakeGuest) ] [ str "Make Guest" ]
      
      // Display current customer
      match model.CustomerType with
      | Some (Eligible c) -> p [] [ str $"Eligible: {c.Id}" ]
      | Some (Registered c) -> p [] [ str $"Registered: {c.Id}" ]
      | Some (Guest c) -> p [] [ str $"Guest: {c.Id}" ]
      | None -> p [] [ str "No customer selected" ]
    ]
  ```
- Walk through one complete cycle: User clicks "Make Eligible" → Msg → Update → new Model → View

**Minutes 51-54: MVU Benefits**
- "Everything is predictable and testable"
- "Update function is pure - easy to test"
- "Time-travel debugging becomes possible"
- Show the Elmish debugger if time permits (can see all messages and state changes)

**Minutes 54-58: [OPTIONAL] Copilot-Generated Rich UI**
- "We've hand-coded one field... let's let Copilot do the rest"
- Use Copilot to generate form fields for all properties
- Show the generated code
- Quick test: "Look, it just works because the types are all there"
- "This is the power of strong typing meeting modern tooling"
- *[If running short on time, skip this and mention it as homework]*

---

### Wrap-up & Q&A (2 minutes)

**Minutes 58-59: Key Takeaways**
- Rapid-fire summary:
  1. F# types can eliminate entire classes of runtime bugs
  2. Works seamlessly with C# in the same solution
  3. SAFE stack shows F# is production-ready for web
  4. Elmish MVU gives you predictable state management
  5. Start small: one service, one module, one background job

**Minutes 59-60: Resources & Next Steps**
- Share repo link with all code from today
- Point to learning resources:
  - F# for Fun and Profit (fsharpforfunandprofit.com)
  - SAFE Stack documentation (safe-stack.github.io)
  - F# Slack community
- "Try it Monday: pick one small feature and rewrite it in F#"
- Open the floor for questions (extend beyond 60 min if needed)

---

## Topic Description for Group Leader

**Title:** F# Type-Driven Development: From C# to Correctness

**Duration:** 1 hour

**Description:**

Join us for an interactive exploration of F#'s powerful type system and how it enables building more correct software through domain modeling. In this "Yes, and..." conversational-style presentation, we'll take you on a journey from familiar C# patterns to leveraging F#'s unique strengths.

We'll start with a practical scenario using a naive C#-style model and progressively enhance it by incorporating F# features like discriminated unions, option types, and the principle of "making illegal states unrepresentable." You'll see how each refinement moves us closer to a model where many bugs become compile-time errors rather than runtime surprises.

The session includes two practical demonstrations: first, consuming our refined F# model from C# code to show real-world interoperability; and second, a deep dive into building an interactive web application using the SAFE Stack, focusing on the Elmish MVU (Model-View-Update) pattern for predictable state management.

Whether you're curious about functional programming, looking to improve your domain modeling skills, or interested in the SAFE Stack for web development, this session will provide practical insights and hands-on examples.

**Prerequisites:** Basic familiarity with C# or similar object-oriented languages. No prior F# experience required.

**Key Takeaways:**
- How F#'s type system helps eliminate entire classes of bugs
- Practical patterns for progressive model refinement
- F#/C# interoperability in real-world scenarios
- Introduction to Elmish MVU architecture for web applications

---

## Alternative Topic Description (From ChatGPT Discussion)

**Title:** From C# Classes to F# Types: Practical Domain Modeling on .NET (with SAFE)

**Description:**

In this session we'll take a very pragmatic look at what F# can bring to a typical C# shop. We'll start with a familiar, naive, C#-style domain model and progressively refactor it into a more expressive F# model, using records, discriminated unions, and options to make illegal states unrepresentable and push more correctness into the type system.

Once the model is "good enough," we'll briefly show how to consume it from C#, so you can see how F# fits naturally into an existing .NET solution. From there, we'll drop the same model into a SAFE stack application (in the Shared project), wire up minimal server boilerplate, and hand-build a simple one-field editing workflow using Elmish's Model–View–Update pattern. Time permitting, we'll also let Copilot generate a richer UI over the full model—just to show how far you can go with strong types and modern tooling.

If you're a C# developer who's curious about F#, or you've ever wondered "could my types be doing more of the work for me?", this talk is for you.

