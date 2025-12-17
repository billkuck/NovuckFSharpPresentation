namespace CustomerDomain.Iteration2

// Iteration 2: Explicit discount tiers as domain concept
// Best: Discount tier is part of the type, not a boolean flag
// Domain language is explicit: Standard, Registered, Guest
type RegisteredCustomer = { Id: string }

type UnregisteredCustomer = { Id: string }

type Customer =
    | Standard of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
