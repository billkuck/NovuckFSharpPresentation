module NkWidgets

open Feliz
open Feliz.DaisyUI

type AdtTab = {
    Label: string
    ContentFun: unit -> Fable.React.ReactElement list
    Active: bool
}

type NkWidget =

    static member text
        (content : string)
        =
        Html.text content

    static member text
        (content : int)
        =
        Html.text content

    static member text
        (content : string option)
        =
        if content.IsSome then
            NkWidget.text content.Value
        else
            NkWidget.text ""

    static member text
        (content : int option)
        =
        if content.IsSome then
            NkWidget.text content.Value
        else
            NkWidget.text ""

     static member text
        (content : unit -> string)
        =
        Html.text (content())

    static member h1 (content : string) =  Html.h1 [ prop.className (NkStyles.Styles.headingByLevel 1); prop.text content ]
    static member h2 (content : string) =  Html.h2 [ prop.className (NkStyles.Styles.headingByLevel 2); prop.text content ]
    static member h3 (content : string) =  Html.h3 [ prop.className (NkStyles.Styles.headingByLevel 3); prop.text content ]

    static member button
        (label: string)
        (clickHandler: Browser.Types.MouseEvent -> unit) 
        =
        Daisy.button.button [
            button.outline
            prop.text label
            button.xs
            prop.onClick clickHandler
        ]

    static member nkTable<'T>
        (items: 'T list)
        (headings: string list)
        (renderItemRow: 'T -> Fable.React.ReactElement list)
        =
        Daisy.table [ table.zebra; table.sm; prop.children [
            Html.thead [ Html.tr [
                for heading in headings do
                    Html.th heading
            ]]
            Html.tbody [
                for item in items do
                    Html.tr (List.map (fun (columnContent: Fable.React.ReactElement) -> (Html.td columnContent)) (renderItemRow item))
        ]]]

    static member nkActionsContainer
        (contents : Fable.React.ReactElement list)
        =
        Daisy.cardActions 
            contents

    static member nkContainer
        (title: string)
        (content: Fable.React.ReactElement list)
        =
        Daisy.card [ prop.className "shadow-lg";  card.border;
            prop.children [
                Daisy.cardBody
                    (Daisy.cardTitle title :: (content))
        ]]

    static member textInput
        (prompt: string)
        (initialValue: string)
        (onChange: string -> unit)
        =
        Html.input [
            prop.className "shadow appearance-none border rounded w-full py-2 px-3 outline-none focus:ring-2 ring-teal-300 text-grey-darker"
            prop.value initialValue
            prop.placeholder prompt
            prop.onChange (onChange)
        ]

    static member selectList<'ITEM>
        (items: 'ITEM list)
        (selectedItemName: string option)
        (getNameAndId: ('ITEM -> (string * string)))
        (onChangeMethod: (string -> unit))
        =
        Daisy.select [
            prop.onChange (onChangeMethod)
            select.accent
            prop.children [
                for item in items do
                    let (name, id) = getNameAndId(item)
                    let selected = (selectedItemName.IsSome && selectedItemName.Value = name)
                    Html.option [
                        prop.text name;
                        prop.value id
                        prop.defaultValue selected
                    ]
        ]]

    static member selectListWithFilter<'ITEM>
        (prompt: string)
        (items: 'ITEM list option)
        (selectedItemName: string option)
        (getNameAndId: ('ITEM -> (string * string)))
        (onChangeMethod: (string -> unit))
        (filter: string)
        =
        Daisy.select [
            prop.onChange (onChangeMethod)
            select.accent
            prop.children [
                Html.option [
                    prop.text prompt;
                ]
                if items.IsSome then
                    for item in items.Value do
                        let (name, id) = getNameAndId(item)
                        if items.Value.Length < 3 then
                            printfn "DEBUG - select list item - name:%A, id:%A" name id

                        let selected = (selectedItemName.IsSome && selectedItemName.Value = name)
                        if name.ToLower().Contains(filter.ToLower()) then
                            Html.option [
                                prop.text name;
                                prop.value id
                                prop.defaultValue selected
                            ]
                        else
                            Html.none
        ]]

    static member optionalContent<'T>
        (dataOption: 'T option)
        (renderSome: 'T -> Fable.React.ReactElement)
        =
        match dataOption with
        | None ->  Html.none 
        | Some x -> (renderSome x)

    static member optionalContentList<'T>
        (dataOption: 'T option)
        (renderSome: 'T -> Fable.React.ReactElement list)
        =
        match dataOption with
        | None ->  Html.none
        | Some x ->
            Html.span
                (renderSome x)

    static member collapsable
        (title: string)
        (expanded: bool)
        (renderContent: unit -> Fable.React.ReactElement)
        =
        Daisy.collapse [
            collapse.plus;
            prop.children [
                Html.input [
                    prop.type' "checkbox"
                    prop.defaultChecked expanded
                ]
                Daisy.collapseTitle [ 
                    prop.className "text-base font-bold text-right bg-neutral-400"
                    prop.text title
                ]
                Daisy.collapseContent [
                    NkWidget.h1 title
                    (renderContent ())
                ]
            ]
        ]

    static member ThemeSelector
        (selectedTheme: string)
        (allThemeNames: string list option)
        (onChangeMethod: (string -> unit))
        =
        let Equal first second : bool =
            first = second

        NkWidget.optionalContent allThemeNames (fun (allThemeNames) ->
            Html.span [
                prop.onChange (onChangeMethod)
                prop.children [
                    for themeName in allThemeNames do
                    Daisy.join [ prop.className "space-1 p-1"; prop.children [ Daisy.join [ prop.className "border space-1 p-1";
                        prop.children [
                            Daisy.label [ prop.className "pr-1";
                                prop.children [ Html.text themeName ] ]
                            Daisy.radio [ theme.controller;  prop.value themeName;
                                prop.name "theme" ;
                                prop.defaultChecked (Equal themeName selectedTheme) ]
            ]]]]]]
        )

