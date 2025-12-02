namespace CustomerDomain

// Final version: Explicit discount tiers as domain concept
type RegisteredCustomer = { Id: string }

type UnregisteredCustomer = { Id: string }

type Customer =
    | Standard of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer
