namespace Inw.Cron

[<AutoOpen>]
module Operators =
    
    let (>>=) input outfun = 
        input |> Option.bind outfun
    
    let (>=>) switchFunction1 switchFunction2  =        
        switchFunction1 >> (Option.bind switchFunction2)