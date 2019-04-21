namespace Inw.Cron.Test

open CronTestHelper
open Inw.Cron
open System
open Xunit
module CronTest =              

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

        [<Fact>]
        let ExecuteFirstMondayInApril_ExecutesAtGivenHourAtFirstMonday () =
            let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
            let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 01)
            runCommandsForTest testTimes [command]
            
        [<Fact>]
        let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtSecondMonday () =
            let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
            let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 08)
            runCommandsForTest testTimes [command]

        [<Fact>]
        let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtThirdMonday () =
            let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
            let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 15)
            runCommandsForTest testTimes [command]

        [<Fact>]
        let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtFourhtMonday () =
            let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
            let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 22)
            runCommandsForTest testTimes [command]

        [<Fact>]
        let ExecuteFirstMondayInApril_NotExecutesAtGivenHourAtLastMonday () =
            let command = { id = "monthly"; message = "yay"; interval = MonthlyAt (First, DayOfWeek.Monday, 12)}
            let testTimes = hoursOfDay |> List.map (createHourAtDay 2019 04 29)
            runCommandsForTest testTimes [command] 