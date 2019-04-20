namespace Cron

open System
open System.Runtime.ExceptionServices

type OccurenceInMonht =
    | First
    | Second
    | Thrid
    | Last


type Interval = 
    | Hourly of hour: int
    | DailyAt of oClock: int
    | WeekdayAt of weekday: DayOfWeek * oClock: int
    | MonthlyAt of nth: OccurenceInMonht * weekday: DayOfWeek * oClock: int
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

    let intervalDue (dt:DateTime) (c:Command) =        
        match c.interval with
        | Hourly h -> hourly h dt.Hour c
        | DailyAt oc -> oclock oc dt.Hour c
        | WeekdayAt (wd,h) -> weekday c wd dt.DayOfWeek |> Option.bind (oclock h dt.Hour)
        | MonthlyAt (n,wd,h) -> None
        | Disabled -> None

    let intervalDueNow =
        intervalDue DateTime.Now

    
    let execute r e (c:Command) =
        match intervalDueNow c with
        | Some(x) -> r x
        | None -> e c