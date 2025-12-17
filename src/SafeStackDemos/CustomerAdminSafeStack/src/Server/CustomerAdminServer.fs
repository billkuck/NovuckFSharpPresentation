module CustomerAdminServer

open SAFE
open Saturn
open Shared
open Customer_Models

module CustomersStorage =
    let customers =
        ResizeArray [
        ]

    let addCustomer customer =
        customers.Add customer
        Ok()

let customerAdminApi ctx = {
    getCustomers = fun () -> async { return CustomersStorage.customers |> List.ofSeq }
    addCustomer =
        fun customer -> async {
            return
                match CustomersStorage.addCustomer customer with
                | Ok() -> CustomersStorage.customers |> List.ofSeq
                | Error e -> failwith e
        }
}
