namespace CustomerLiveDomain

type RegisteredCustomer = { 
    Id: string
}

type UnregisteredCustomer = { 
    Id: string 
}

type Customer = 
    | Standard of RegisteredCustomer    // Eligible tier explicit
    | Registered of RegisteredCustomer  // Registered but not eligible
    | Guest of UnregisteredCustomer
