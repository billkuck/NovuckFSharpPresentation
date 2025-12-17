module CustomerAdmin_Model

open Shared

type Model = {
    Customers: Customer list
    SpendAmount: decimal
    Loading: bool
    Error: string option
}

type Msg =
    | LoadCustomers
    | CustomersLoaded of CustomerDto list
    | ChangeCustomerTier of customerId: string * action: string
    | CustomerTierChanged of CustomerDto
    | UpdateSpendAmount of decimal
    | ApiError of exn
