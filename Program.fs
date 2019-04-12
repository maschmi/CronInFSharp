// Learn more about F# at http://fsharp.org


open System
open Cron

let printMessage c =
    printf "command %s says %s" c.id c.message 
    
let logNone c =
    printf "%s was not executed" c.id

let log =
    Cron.execute printMessage logNone

let test = { id ="test";  message = "log me"; interval = DailyAt 20 }

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    log test
    0 // return an integer exit code
