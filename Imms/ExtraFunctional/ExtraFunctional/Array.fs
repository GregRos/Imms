module ExtraFunctional.Array

let binSearchBy (f : 'v -> 'k when 'k : comparison) (k : 'k) (arr : 'v array) =
    let rec bin mn mx =
        let count = mx - mn
        if count = 0 then mn
        else
            let md = mn + (count / 2)
            if arr.[md] |> f >  k then
                bin mn md
            elif arr.[md] |> f < k then
                bin (md + 1) mx
            else
                md
    bin 0 (arr.Length - 1) 
                     

