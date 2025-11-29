namespace CustomerDomain.Iteration0

// Iteration 0: Naive model - just like C#
// Problems: Can have IsEligible=true with IsRegistered=false (illegal state!)
type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
}
