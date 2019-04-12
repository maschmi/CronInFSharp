// Learn more about F# at http://fsharp.org

open System

type Interval = 
    | Hourly of int    
    | Disabled

type Command = {
    id: string
    message: string
    interval: Interval
}
    
let hourly h n (c:Command) =
    let due = n % h
    match due with
    | 0 -> Some c
    | _ -> None

let intervalDue (c:Command) =
    let now = DateTime.Now
    match c.interval with
    | Hourly h -> hourly h now.Hour c
    | Disabled -> None

let printMessage c =
    printf "command %s says %s" c.id c.message 

let logNone c =
    printf "%s was not executed" c.id

let execute how (c:Command) =
    match intervalDue c with
    | Some(x) -> how x
    | None -> logNone c

let log =
    execute printMessage 

let test = { id ="test";  message = "log me"; interval = Hourly 9 }

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    log test
    0 // return an integer exit code
