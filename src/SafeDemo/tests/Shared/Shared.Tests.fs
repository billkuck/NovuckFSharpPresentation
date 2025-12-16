module Shared.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

let shared =
    testList "Shared" [
        testCase "Customer Tier"
        <| fun _ ->
            let customer = VIP { Id = "1"; Name = "Alice" }
            let tier = Customer.getTier customer
            Expect.equal tier "VIP" "Should be VIP tier"
        
        testCase "Calculate Discount"
        <| fun _ ->
            let customer = VIP { Id = "1"; Name = "Alice" }
            let discount = Customer.calculateDiscount customer 100m
            Expect.equal discount 15m "VIP should get 15% discount on £100"
    ]