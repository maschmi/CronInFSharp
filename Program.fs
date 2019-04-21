// Learn more about F# at http://fsharp.org


open System
open Inw.Cron

[<EntryPoint>]
let main argv =
        printfn "There should be some kind of user interaction ;)"
        let c = [{ id = "test"; message="something to say"; interval = WeekdayAt (DayOfWeek.Monday , 5 )}]
        let error = "{\"id\":\"test\",\"message\":\"something to say\",\"interval\":{\"Case\":\"WeekdayAt\",\"Fields\":[5]}}"
        printfn "Original %A" c
        let ser = c |> Json.serialize
        printfn "%s" ser
        let deserialize s  =
                match (Json.deserialize s) with
                | Ok (o:Command list) -> sprintf "Ok %A" o
                | Error e -> sprintf "NotOk %A" e
        deserialize ser |> printfn "%A"
        // deserialize error |> printfn "%A"
        0 // return an integer exit code
