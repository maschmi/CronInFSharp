namespace Inw.Cron

open System

module CronDomain =         

    let hourly h n (c:Command) =
        let due = n % h
        match due with
        | 0 -> Some c
        | _ -> None

    let weekday (wd:DayOfWeek) (n:DayOfWeek) (c:Command) =
        match wd.CompareTo(n) with
        | 0 -> Some c
        | _ -> None

    let oclock oc h c =
        let difference = oc - h
        match difference with 
        | 0 -> Some c
        | _ -> None

    let convertdDay d =
        let w = d / 7
        match w with
        | 0 -> First
        | 1 -> Second
        | 2 -> Third
        | _ -> Last

    let week n (dt:DateTime) (c:Command) =
        let w = convertdDay dt.Day
        match w = n with
        | true -> Some c
        | false -> None

    
    let intervalDue (dt:DateTime) (c:Command) =        
        match c.interval with
        | Hourly h -> hourly h dt.Hour c
        | DailyAt oc -> oclock oc dt.Hour c
        | WeekdayAt (wd,h) -> c |> weekday wd dt.DayOfWeek >>= oclock h dt.Hour
        | MonthlyAt (n,wd,h) -> c |> week n dt >>= weekday wd dt.DayOfWeek >>= oclock h dt.Hour
        | Disabled -> None

    let intervalDueNow =
        intervalDue DateTime.Now

    
    let execute r e (c:Command) =
        match intervalDueNow c with
        | Some(x) -> r x
        | None -> e c