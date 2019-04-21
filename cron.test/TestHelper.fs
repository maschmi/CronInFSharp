namespace Inw.Cron.Test

open System
open Xunit
open Inw.Cron

module CronTestHelper =

    let testExecute (dt:DateTime) c =
            CronDomain.intervalDue dt c

    let getTestDetail dt c =
            sprintf "command %A at %A" c dt

    let assertSomeWithMessage m c =
        match c with
        | Some(x) -> Assert.True(true)
        | None -> Assert.True(false, "Expected Some(T) but got None " + m)

    let assertNoneWithMessage m c =
        match c with
        | Some(x) -> Assert.False(true, "Expeted None but got Some(T) " + m)
        | None -> Assert.False(false)

    let assertSome (dt:DateTime) (c:Command option)  =
        assertSomeWithMessage (getTestDetail dt c)  c

    let assertNone (dt:DateTime) (c:Command option) =
        assertNoneWithMessage  (getTestDetail dt c) c

    let testIntervals = [ 
            1
            2
            3
            6 ]

    let hoursOfDay = [0 .. 23]

    let randomHour = 
        let rnd = System.Random()
        rnd.Next(0,23)

    let createHourlyCommand h =
        { id = "test"; message = "message"; interval = Hourly h }

    let createDailyCommand h =
        { id = "daily"; message = "yay"; interval = DailyAt h}

    let createWeekdayCommand h wd =
        { id = "weekdaysAt"; message = "test"; interval = WeekdayAt (wd,h)}

    let createTestDate y m d h mm s =
        DateTime(y, m, d, h, mm, s)

    let createHourAtDay y m d h =
        createTestDate y m d h 15 0

    let createTestHours h =
        createTestDate 2019 4 13 h 15 00
        
    let createTestDayily d h =
        createTestDate 2019 04 d h 0 00

    let daysToRun d =
        d |> List.map createTestDayily

    let createTestTimes maxDay =
        daysToRun [1..maxDay] |> List.map (fun fl -> List.map fl hoursOfDay) |> Seq.concat |> List.ofSeq

    let createCommandForEveryWeekday h =
        let days = List.map enum<DayOfWeek> [0..6] 
        List.map (createWeekdayCommand h) days 
        
    let checkExecutionHrlyInterval (dt:DateTime) h c =
        match dt.Hour % h with
        | 0 -> testExecute dt c |> assertSome dt
        | _ -> testExecute dt c |> assertNone dt

    let checkExecutionAtHour (dt:DateTime) h c =
        match dt.Hour - h with
        | 0 -> testExecute dt c |> assertSome dt
        | _ -> testExecute dt c |> assertNone dt

    let checkExecutionWeekdaysAt (dt:DateTime) wd h c =  
        match dt.DayOfWeek = wd with
        | true -> checkExecutionAtHour dt h c
        | false -> testExecute dt c |> assertNone dt

    let assertCommandExecution (dt:DateTime) c =        
        match c.interval with
        | Hourly h -> checkExecutionHrlyInterval dt h c 
        | Disabled -> testExecute dt c |> assertNone dt
        | DailyAt h -> checkExecutionAtHour dt h c
        | WeekdayAt (wd,h) -> checkExecutionWeekdaysAt dt wd h c
        | MonthlyAt (n,wd,h) -> Assert.False(true)

    let listFunCrossList a funlist =
        List.map (fun f -> List.map f a) funlist

    let runAssertions afun dt c =
        List.map afun dt |> listFunCrossList c 

    let runCommandsForTest (dt:DateTime list) (c:Command list) =        
        runAssertions assertCommandExecution dt c    

    let neverExecuted (dt:DateTime) c =
        match c.interval with
        | _ -> testExecute dt c |> assertNone dt