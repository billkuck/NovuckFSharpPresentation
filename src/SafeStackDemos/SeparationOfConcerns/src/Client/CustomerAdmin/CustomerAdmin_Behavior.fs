module CustomerAdmin_Behavior

open CustomerAdmin_Model

open Elmish
open SAFE
open Shared
open Shared.Customer

let customerApi = Api.makeProxy<ICustomerApi> ()


let init () =
    let initialModel = {
        Customers = []
        SpendAmount = 100m
        Loading = false
        Error = None
    }
    let initialCmd = LoadCustomers |> Cmd.ofMsg

    initialModel, initialCmd

let update msg model =
    match msg with
    | LoadCustomers ->
        { model with Loading = true },
        Cmd.OfAsync.either
            customerApi.getCustomers
            ()
            CustomersLoaded
            ApiError

    | CustomersLoaded dtos ->
        let customers = dtos |> List.map dtoToCustomer
        { model with Customers = customers; Loading = false; Error = None },
        Cmd.none

    | ChangeCustomerTier (id, action) ->
        { model with Loading = true },
        Cmd.OfAsync.either
            (customerApi.changeTier id)
            action
            CustomerTierChanged
            ApiError

    | CustomerTierChanged dto ->
        // CRITICAL: Customer list is transformed, not mutated
        let updatedCustomers =
            model.Customers
            |> List.map (fun c ->
                if Customer.getId c = dto.Id
                then dtoToCustomer dto  // NEW customer instance
                else c)                  // Keep existing customer

        { model with Customers = updatedCustomers; Loading = false; Error = None },
        Cmd.none

    | UpdateSpendAmount amount ->
        { model with SpendAmount = amount }, Cmd.none

    | ApiError ex ->
        { model with Error = Some ex.Message; Loading = false },
        Cmd.none