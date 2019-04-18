module Tests

open System
open Xunit
open Cron
open Microsoft.VisualStudio.TestPlatform.CommunicationUtilities

let testExecute (dt:DateTime) c =
    Cron.intervalDue dt c

let assertSomeWithMessage m c =
    match c with
    | Some(x) -> Assert.True(true)
    | None -> Assert.True(false, "Expected Some(T) but got None" + m)

let assertNoneWithMessage m c =
    match c with
    | Some(x) -> Assert.True(false, "Expeted None but got Some(T)" + m)
    | None -> Assert.True(true)

let testIntervals = [ 
        1
        2
        3
        6 ]

let hoursOfDay = [0 .. 23]

let createHourlyCommand h =
    { id = "test"; message = "message"; interval = Hourly h }
 
let createTestDate y m d h mm s =
    DateTime(y, m, d, h, mm, s)

let createTestHours h =
    createTestDate 2019 4 13 h 15 00
    
let createTestDayily d h =
    createTestDate 2019 04 d h 0 00

let getTestDetail dt c =
        sprintf "command %A at %A" c dt

let checkExecutionHrly (dt:DateTime) h c =
    let assertSome = getTestDetail dt c |> assertSomeWithMessage
    let assertNone = getTestDetail dt c |> assertNoneWithMessage
    match dt.Hour % h with
    | 0 -> testExecute dt c |> assertSome
    | _ -> testExecute dt c |> assertNone

let checkExecutionDailyAt (dt:DateTime) h c =
    let assertSome = getTestDetail dt c |> assertSomeWithMessage
    let assertNone = getTestDetail dt c |> assertNoneWithMessage
    match dt.Hour - h with
    | 0 -> testExecute dt c |> assertSome
    | _ -> testExecute dt c |> assertNone

let assertCommandExecution (dt:DateTime) c =        
    match c.interval with
    | Hourly h -> checkExecutionHrly dt h c 
    | Disabled -> testExecute dt c |> assertNoneWithMessage "disabled"
    | DailyAt h -> checkExecutionDailyAt dt h c
    | _ -> Assert.False(true, "untested interval")

let prepareTestTimeFunction dt =
       List.map assertCommandExecution dt

let runCommandsOnPreparedFunctions c f =
       List.map(fun fl -> List.map fl c) f

let runCommandsForTest (dt:DateTime list) (c:Command list) =        
        prepareTestTimeFunction dt
        |> runCommandsOnPreparedFunctions c

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

let createDailyCommand h =
    { id = "daily"; message = "yay"; interval = DailyAt h}

let daysToRun d =
    d |> List.map createTestDayily

let createTestTimes maxDay =
    daysToRun [1..maxDay] |> List.map (fun fl -> List.map fl hoursOfDay) |> Seq.concat |> List.ofSeq

[<Fact>]
let ExecuteDaily_ExecutesAtGivenHour () =
    let commands = hoursOfDay |> List.map createDailyCommand
    let testTimes = createTestTimes 30
    runCommandsForTest testTimes commands
   
let createWeekdayCommand wd h =
    { id = "weekdaysAt"; message = "test"; interval = WeekdayAt (wd,h)}

let createCommandForEveryWeekday h =
    [0..6] |> List.map enum<DayOfWeek> |> List.map createWeekdayCommand

let randomHour = 
    let rnd = System.Random()
    rnd.Next(0,23)
    

[<Fact>]
let ExecuteEveryWeekdayAt_ExecutesAtGivenHour () =
    let commands = randomHour |> createCommandForEveryWeekday
    let testTimes = createTestTimes 30
    runCommandsForTest testTimes commands