namespace CustomerDomain

// Final version: Explicit eligibility as domain concept
type RegisteredCustomer = { Id: string }

type UnregisteredCustomer = { Id: string }

type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
