namespace Inw.Cron

open System

[<AutoOpen>]
module DomainTypes =
    type OccurenceInMonth =
        | First
        | Second
        | Third
        | Last


    type Interval = 
        | Hourly of hour: int
        | DailyAt of oClock: int
        | WeekdayAt of weekday: DayOfWeek * oClock: int
        | MonthlyAt of nth: OccurenceInMonth * weekday: DayOfWeek * oClock: int
        | Disabled

    type Command = {
        id: string
        message: string
        interval: Interval
    }