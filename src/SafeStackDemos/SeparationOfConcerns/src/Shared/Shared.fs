namespace Shared

open System

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

