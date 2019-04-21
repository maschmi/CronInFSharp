namespace Inw.Cron

[<AutoOpen>]
module Operators =
    
    //binds output to 1-track function
    let (>>=) input outfun = 
        input |> Option.bind outfun

    //binds function to create a even bigger switch
    let (>=>) switchFunction1 switchFunction2  =        
        switchFunction1 >> (Option.bind switchFunction2)

    //paralellize switches
    let plusOption addSome switch1 switch2 x =
        match (switch1 x),(switch2 x) with
        | Some s1,Some s2 -> Some (addSome s1 s2)
        | None,Some _ -> None
        | Some _,None -> None
        | None,None -> None

    let (&&&) s1 s2 =
        let addSome c1 c2 = c1
        plusOption addSome s1 s2