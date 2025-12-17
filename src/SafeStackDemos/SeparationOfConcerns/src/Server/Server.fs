module Server

open SAFE
open Saturn
open Shared

module Storage =
    let mutable customers = [
        VIP { Id = "1"; Name = "Alice" }
        Standard { Id = "2"; Name = "Bob" }
        Registered { Id = "3"; Name = "Carol" }
        Guest { Id = "4"; Name = "Dave" }
    ]

    let customerToDto customer : CustomerDto = {
        Id = Customer.getId customer
        Name = Customer.getName customer
        Tier = Customer.getTier customer
    }

    let dtoToCustomer (dto: CustomerDto) : Customer =
        match dto.Tier with
        | "VIP" -> VIP { Id = dto.Id; Name = dto.Name }
        | "Standard" -> Standard { Id = dto.Id; Name = dto.Name }
        | "Registered" -> Registered { Id = dto.Id; Name = dto.Name }
        | "Guest" -> Guest { Id = dto.Id; Name = dto.Name }
        | _ -> Guest { Id = dto.Id; Name = dto.Name }

let customerApi ctx = {
    getCustomers = fun () -> async {
        return Storage.customers |> List.map Storage.customerToDto
    }

    changeTier = fun customerId actionStr -> async {
        let customer =
            Storage.customers
            |> List.tryFind (fun c -> Customer.getId c = customerId)

        match customer with
        | Some cust ->
            // Parse action string to TierAction
            let action =
                match actionStr with
                | "Promote" -> Some Customer.Promote
                | "Demote" -> Some Customer.Demote
                | "Register" -> Some Customer.Register
                | "Unregister" -> Some Customer.Unregister
                | _ -> None

            // ⭐ Use shared validation and transition logic
            let updated =
                match action with
                | Some act when Customer.canTransition act cust ->
                    Customer.applyTransition act cust
                | _ -> cust  // Invalid action or transition

            Storage.customers <- Storage.customers |> List.map (fun c ->
                if Customer.getId c = customerId then updated else c)

            return Storage.customerToDto updated
        | None ->
            return failwith $"Customer {customerId} not found"
    }
}

let webApp = Api.make customerApi

let app = application {
    use_router webApp
    memory_cache
    use_static "public"
    use_gzip
}

[<EntryPoint>]
let main _ =
    run app
    0