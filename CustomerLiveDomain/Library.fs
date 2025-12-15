namespace CustomerLiveDomain

// Naive F# record - same problem as C# class
type Customer = {
    Id: string
    IsEligible: bool // Eligible for discount
    IsRegistered: bool
}
