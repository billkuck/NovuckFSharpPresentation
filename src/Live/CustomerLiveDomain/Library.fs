namespace CustomerLiveDomain

// Naive approach with booleans - allows illegal states
type Customer = {
    Id: string
    IsEligible: bool
    IsRegistered: bool
}
