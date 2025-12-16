module Client.Tests

open Fable.Mocha

open Index
open Shared
open SAFE

let client =
    testList "Client" [
        testCase "Load Customers"
        <| fun _ ->
            let model, _ = init ()
            
            Expect.equal
                model.Customers.Length
                0
                "Should start with no customers"
            
            Expect.equal
                model.SpendAmount
                100m
                "Should default to £100 spend"
    ]

let all =
    testList "All" [
        #if FABLE_COMPILER // This preprocessor directive makes editor happy
        Shared.Tests.shared
#endif
                client
    ]

[<EntryPoint>]
let main _ = Mocha.runTests all