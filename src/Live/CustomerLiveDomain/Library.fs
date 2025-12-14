namespace CustomerLiveDomain

// Final version - eligibility is explicit in the type
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

// F# pattern matching module
module CustomerOperations =
    let calculateTotal (customer: Customer) (spend: decimal) =
        let discount =
            match customer with
            | Eligible _ when spend >= 100m -> spend * 0.1m
            | _ -> 0.0m
        spend - discount
