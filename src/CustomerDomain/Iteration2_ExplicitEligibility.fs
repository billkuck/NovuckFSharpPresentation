namespace CustomerDomain.Iteration2

// Iteration 2: Explicit eligibility as domain concept
// Best: Eligibility is part of the type, not a boolean flag
// Domain language is explicit: Eligible, Registered, Guest
type RegisteredCustomer = { Id: string }

type UnregisteredCustomer = { Id: string }

type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
