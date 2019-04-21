namespace Inw.Cron

[<AutoOpen>]
module Operators =
    
    //binds output to 1-track function
    let (>>=) input outfun = 
        input |> Option.bind outfun

    //binds function to create a even bigger switch
    let (>=>) switchFunction1 switchFunction2  =        
        switchFunction1 >> (Option.bind switchFunction2)

    