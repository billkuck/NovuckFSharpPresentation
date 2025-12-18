module CustomerAdmin_View

open Feliz
open Shared
open CustomerAdmin_Model

module ViewComponents =

    let tierBadge tier =
        let (bgColor, textColor) =
            match tier with
            | "VIP" -> "bg-yellow-400", "text-yellow-900"
            | "Standard" -> "bg-blue-500", "text-white"
            | "Registered" -> "bg-gray-400", "text-gray-900"
            | "Guest" -> "bg-gray-200", "text-gray-600"
            | _ -> "bg-gray-200", "text-gray-600"

        Html.span [
            prop.className $"px-3 py-1 rounded-full text-sm font-semibold {bgColor} {textColor}"
            prop.text tier
        ]

    let buttonAttributes tierAction =
        match tierAction with
        | Customer.Promote -> ("⬆ Promote", "bg-green-500", "bg-green-600", "bg-gray-200")
        | Customer.Demote -> ("⬇ Demote", "bg-orange-500", "bg-orange-600", "bg-gray-200")
        | Customer.Register -> ("→ Register", "bg-blue-500", "bg-blue-600", "bg-gray-200")
        | Customer.Unregister -> ("✕ Unregister", "bg-red-500", "bg-red-600", "bg-gray-200")

    let buttonClass isEnabled tierAction =
        let (label, activeColor, hoverColor, disabledColor) = buttonAttributes tierAction
        if isEnabled then
            $"px-3 py-1 {activeColor} hover:{hoverColor} text-white rounded text-sm font-medium"
        else
            $"px-3 py-1 {disabledColor} text-gray-400 rounded text-sm font-medium cursor-not-allowed opacity-50"

    let actionButton customer tierAction dispatch =
        let customerId = Customer.getId customer
        let (label, activeColor, hoverColor, disabledColor) = buttonAttributes tierAction
        let isEnabled = Customer.canTransition tierAction customer
        Html.button [
            prop.className (buttonClass isEnabled tierAction)
            prop.disabled (not isEnabled)
            prop.onClick (fun _ -> if isEnabled then  dispatch (ChangeCustomerTier (customerId, string tierAction)))
            prop.text label
        ]

    let registerUnregisterButton customer dispatch =
        if Customer.canTransition Customer.Register customer then
            actionButton customer Customer.Register dispatch
        elif Customer.canTransition Customer.Unregister customer then
            actionButton customer Customer.Unregister dispatch
        else
            Html.div [ prop.className "px-3 py-1 min-w-[110px]" ]

    let actionButtons customer dispatch =
        Html.div [
            prop.className "flex gap-2"
            prop.children [
                actionButton customer Customer.Promote dispatch
                actionButton customer Customer.Demote dispatch
                registerUnregisterButton customer dispatch
            ]
        ]

    let customerNameDisplay customer =
        Html.span [
            prop.className "text-lg font-medium text-gray-800"
            prop.text (Customer.getName customer)
        ]

    let customerRow customer dispatch =
        Html.div [
            prop.className "flex items-center justify-between p-4 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow"
            prop.children [
                Html.div [
                    prop.className "flex items-center gap-4 flex-1"
                    prop.children [
                        customerNameDisplay customer
                        tierBadge (Customer.getTier customer)
                    ]
                ]
                actionButtons customer dispatch
            ]
        ]

    let customerList customers dispatch =
        Html.div [
            prop.className "space-y-3"
            prop.children [
                for customer in customers do
                    customerRow customer dispatch
            ]
        ]

    let discountCalculator customers spendAmount dispatch =
        Html.div [
            prop.className "bg-white rounded-lg shadow-md p-6 mt-6"
            prop.children [
                Html.h2 [
                    prop.className "text-2xl font-bold mb-4 text-gray-800"
                    prop.text "Discount Calculator"
                ]

                Html.div [
                    prop.className "mb-4"
                    prop.children [
                        Html.label [
                            prop.className "block text-sm font-medium text-gray-700 mb-2"
                            prop.text "Spend Amount (£):"
                        ]
                        Html.input [
                            prop.type' "number"
                            prop.className "w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            prop.value (string spendAmount)
                            prop.step 0.01
                            prop.min 0
                            prop.onChange (fun (value: string) -> 
                                match System.Decimal.TryParse(value) with
                                | true, amount -> dispatch (UpdateSpendAmount amount)
                                | false, _ -> ())
                        ]
                    ]
                ]

                Html.div [
                    prop.className "space-y-2"
                    prop.children [
                        for customer in customers do
                            let discount = Customer.calculateDiscount customer spendAmount
                            let total = Customer.calculateTotal customer spendAmount
                            let name = Customer.getName customer
                            let tier = Customer.getTier customer
                            let discountPercent = Customer.getDiscountPercent customer

                            Html.div [
                                prop.className "flex items-center justify-between p-3 bg-gray-50 rounded"
                                prop.children [
                                    Html.span [
                                        prop.className "font-medium text-gray-700"
                                        prop.text $"{name} ({tier}):"
                                    ]
                                    Html.div [
                                        prop.className "text-right"
                                        prop.children [
                                            if discountPercent > 0 then
                                                Html.span [
                                                    prop.className "text-sm text-green-600 mr-2"
                                                    prop.text (sprintf "(-%d%%)" discountPercent)
                                                ]
                                            Html.span [
                                                prop.className "font-semibold text-gray-800"
                                                prop.text (sprintf "£%.2f → £%.2f" spendAmount total)
                                            ]
                                            if discount > 0m then
                                                Html.span [
                                                    prop.className "ml-2"
                                                    prop.text "💰"
                                                ]
                                        ]
                                    ]
                                ]
                            ]
                    ]
                ]
            ]
        ]

let view model dispatch =
    Html.div [
        prop.className "min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 p-8"
        prop.children [
            Html.div [
                prop.className "max-w-4xl mx-auto"
                prop.children [
                    Html.h1 [
                        prop.className "text-4xl font-bold text-center mb-8 text-gray-800"
                        prop.text "Customer Tier Management"
                    ]

                    if model.Loading then
                        Html.div [
                            prop.className "text-center py-8"
                            prop.children [
                                Html.div [
                                    prop.className "inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"
                                ]
                            ]
                        ]

                    match model.Error with
                    | Some err ->
                        Html.div [
                            prop.className "bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4"
                            prop.text err
                        ]
                    | None -> Html.none

                    ViewComponents.customerList model.Customers dispatch

                    ViewComponents.discountCalculator model.Customers model.SpendAmount dispatch
                ]
            ]
        ]
    ]
        