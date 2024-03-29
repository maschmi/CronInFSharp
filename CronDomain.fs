namespace Inw.Cron

open System

module CronDomain =         

    let CheckHourly h n (c:Command) =
        let due = n % h
        match due with
        | 0 -> Some c
        | _ -> None

    let CheckWeekday (wd:DayOfWeek) (n:DayOfWeek) (c:Command) =
        match wd.CompareTo(n) with
        | 0 -> Some c
        | _ -> None

    let CheckOClock oc h c =
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

    let CheckWeekdDayAt wd h (dt:DateTime) =
        CheckWeekday wd dt.DayOfWeek
        &&& CheckOClock h dt.Hour

    let CheckMonthlyAt n wd h (dt:DateTime) =
        week n dt 
        &&& CheckWeekday wd dt.DayOfWeek 
        &&& CheckOClock h dt.Hour

    let intervalDue (dt:DateTime) (c:Command) =        
        match c.interval with
        | Hourly h -> c |> CheckHourly h dt.Hour
        | DailyAt oc -> c |> CheckOClock oc dt.Hour
        | WeekdayAt (wd,h) -> c |> CheckWeekdDayAt wd h dt
        | MonthlyAt (n,wd,h) -> c |> CheckMonthlyAt n wd h dt
        | Disabled -> None

    let intervalDueNow =
        intervalDue DateTime.Now
    
    let execute r e (c:Command) =
        match intervalDueNow c with
        | Some(x) -> r x
        | None -> e c