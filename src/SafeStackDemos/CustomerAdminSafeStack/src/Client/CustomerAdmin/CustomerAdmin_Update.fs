module CustomerAdmin_Update

open Elmish
open SAFE
open Shared

open CustomerAdmin_Model
open Customer_Models

let customerAdminApi = Api.makeProxy<ICustomerAdminApi> ()

let init () =
    let initialModel = { Customers = NotStarted }
    let initialCmd = LoadCustomers(Start()) |> Cmd.ofMsg

    initialModel, initialCmd

let update msg model =
    match msg with
    | LoadCustomers msg ->
        match msg with
        | Start() ->
            let loadCustomersCmd = Cmd.OfAsync.perform customerAdminApi.getCustomers () (Finished >> LoadCustomers)

            { model with Customers = model.Customers.StartLoading() }, loadCustomersCmd
        | Finished customers -> { model with Customers = Loaded customers }, Cmd.none
    | SaveCustomers msg ->
        match msg with
        | Start customerInfo ->
            let saveCustomersCmd =
                let customer = Customers.create() |> Guest
                Cmd.OfAsync.perform customerAdminApi.addCustomer customer (Finished >> SaveCustomers)

            model, saveCustomersCmd
        | Finished customers ->
            {
                model with
                    Customers = RemoteData.Loaded customers
            },
            Cmd.none
