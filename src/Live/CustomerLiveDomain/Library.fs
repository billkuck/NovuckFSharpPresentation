namespace CustomerLiveDomain

// First discriminated union - makes registration explicit
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
