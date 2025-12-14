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

// F# pattern matching module
module CustomerOperations =
    let calculateTotal (customer: Customer) (spend: decimal) =
        let discount =
            match customer with
            | Registered c when c.IsEligible && spend >= 100m -> spend * 0.1m
            | _ -> 0.0m
        spend - discount
