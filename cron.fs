namespace Cron

open System

type Interval = 
    | Hourly of hour: int
    | DailyAt of oClock: int
    | WeekdayAt of weekday: DayOfWeek * hour: int
    | Disabled

type Command = {
    id: string
    message: string
    interval: Interval
}

module Cron =
    let hourly h n (c:Command) =
        let due = n % h
        match due with
        | 0 -> Some c
        | _ -> None

    let weekday (c:Command) (wd:DayOfWeek) (n:DayOfWeek) =
        match wd.CompareTo(n) with
        | 0 -> Some c
        | _ -> None

    let oclock oc h c =
        let difference = oc - h
        match difference with 
        | 0 -> Some c
        | _ -> None

    let intervalDue (c:Command) =
        let now = DateTime.Now    
        match c.interval with
        | Hourly h -> hourly h now.Hour c
        | DailyAt oc -> oclock oc now.Hour c
        | WeekdayAt (wd,h) -> weekday c wd now.DayOfWeek |> Option.bind (hourly h now.Hour)
        | Disabled -> None



    let execute r e (c:Command) =
        match intervalDue c with
        | Some(x) -> r x
        | None -> e c