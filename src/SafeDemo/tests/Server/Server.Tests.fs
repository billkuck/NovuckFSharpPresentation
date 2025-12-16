module Server.Tests

open Expecto

open Shared
open Server

let server =
    testList "Server" [
        testCase "Get Customers"
        <| fun _ ->
            let customers = Storage.customers
            Expect.isNonEmpty customers "Should have customers"
            
        testCase "Customer DTO Conversion"
        <| fun _ ->
            let customer = VIP { Id = "1"; Name = "Alice" }
            let dto = Storage.customerToDto customer
            Expect.equal dto.Tier "VIP" "Should convert to VIP tier"
    ]

let all = testList "All" [ Shared.Tests.shared; server ]

[<EntryPoint>]
let main _ = runTestsWithCLIArgs [] [||] all