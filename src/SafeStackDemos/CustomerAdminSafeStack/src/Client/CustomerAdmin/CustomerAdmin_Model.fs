module CustomerAdmin_Model

open SAFE
open Shared
open Customer_Models
open System

type Model = {
    Customers: RemoteData<Customer list>
}

type Msg =
    | CreateUnregisteredCustomer
    | RegisterCustomer of Guid * RegistrationInformation
    | PromoteCustomer of RegisteredCustomer
    | DemoteCustomer of RegisteredCustomer
    | LoadCustomers of ApiCall<unit, Customer list>
    | SaveCustomers of ApiCall<string, Customer list>
