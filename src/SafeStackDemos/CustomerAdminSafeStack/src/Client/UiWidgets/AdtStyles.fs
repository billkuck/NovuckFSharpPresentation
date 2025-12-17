module AdtStyles

type Styles =
    static member headingByLevel level =
        match level with
        | 1 -> "text-xl font-bold block m-2"
        | 2 -> "text-m font-bold block m-2"
        | 3 -> "text-sm font-bold block m-2"
        | _ -> "font-bold block m-2"
