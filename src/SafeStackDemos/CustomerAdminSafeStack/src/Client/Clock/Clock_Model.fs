module Clock_Model

open System

type Seconds =
    | Seconds of int

type Model  = {
    updateFrequencyInSeconds : Seconds
    currentTime : DateTime
    running : Boolean
}

type Msg =
    | Start
    | Stop
    | Expired of DateTime
