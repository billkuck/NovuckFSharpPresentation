namespace CustomerDomain.Iteration1

// Iteration 1: Discriminated Union separating Registered vs Guest
// Better: Can't be eligible without being registered
// But: Still using boolean flag for eligibility
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
