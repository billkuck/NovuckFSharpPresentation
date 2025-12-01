module Customer_Models

open System

type RegistrationInformation = {
    Name: string
    EmailAddress: string
}

type RegisteredCustomer = {
    Id: Guid
    Registration: RegistrationInformation
}

type UnregisteredCustomer = { Id: Guid }

type Customer =
    | Eligible of RegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer


module Customers =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create () : UnregisteredCustomer = {
        Id = Guid.NewGuid()
    }

type ICustomerAdminApi = {
    getCustomers: unit -> Async<Customer list>
    addCustomer: Customer -> Async<Customer list>
}