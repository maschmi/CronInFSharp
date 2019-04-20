module Tests

open System
open Xunit
open Cron

let testExecute (dt:DateTime) c =
    Cron.intervalDue dt c

let getTestDetail dt c =
        sprintf "command %A at %A" c dt

let assertSomeWithMessage c m =
    match c with
    | Some(x) -> Assert.True(true)
    | None -> Assert.True(false, "Expected Some(T) but got None " + m)

let assertNoneWithMessage c m =
    match c with
    | Some(x) -> Assert.False(true, "Expeted None but got Some(T) " + m)
    | None -> Assert.False(false)

let assertSome (dt:DateTime) (c:Command option)  =
    getTestDetail dt c |> assertSomeWithMessage c

let assertNone (dt:DateTime) (c:Command option) =
    getTestDetail dt c |> assertNoneWithMessage c

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
//    | MonthlyAt (n,wd,h) -> ignore

let listFunCrossList a funlist =
    List.map (fun f -> List.map f a) funlist

let runCommandsForTest (dt:DateTime list) (c:Command list) =        
    List.map assertCommandExecution dt |> listFunCrossList c

[<Fact>]
let ExecuteHourly_ExecutesEveryInterval () =
    let commands = testIntervals |> List.map createHourlyCommand
    let testTimes = hoursOfDay |> List.map createTestHours
    runCommandsForTest testTimes commands       

[<Fact>]
let ExecuteDisabled_DoesNotExecute () =
    let commands = [{ id="test"; message="disabled"; interval=Disabled }]
    let testTimes = hoursOfDay |> List.map createTestHours
    runCommandsForTest testTimes commands

[<Fact>]
let ExecuteDaily_ExecutesAtGivenHour () =
    let commands = hoursOfDay |> List.map createDailyCommand
    let testTimes = createTestTimes 30
    runCommandsForTest testTimes commands

[<Fact>]
let ExecuteEveryWeekdayAt_ExecutesAtGivenHour () =
    let commands = createCommandForEveryWeekday randomHour
    let testTimes = createTestTimes 30
    runCommandsForTest testTimes commands

// [<Fact>]
// let ExecuteFirstMondayInApril_ExecutesAtGivenHourAtFirstMonday () =
//     let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
//     let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 01)
//     runCommandsForTest testTimes [command]

// [<Fact>]
// let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtSecondMonday () =
//     let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
//     let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 08)
//     runCommandsForTest testTimes [command]

// [<Fact>]
// let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtThirdMonday () =
//     let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
//     let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 15)
//     runCommandsForTest testTimes [command]

// [<Fact>]
// let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtFourhtMonday () =
//     let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
//     let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 22)
//     runCommandsForTest testTimes [command]

// [<Fact>]
// let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtLastMonday () =
//     let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
//     let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 29)
//     runCommandsForTest testTimes [command] 