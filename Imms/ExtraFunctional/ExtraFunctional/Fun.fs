namespace ExtraFunctional

module Fun = 
    let apply1 a f = f a
    let apply2 a b f = f a b

    let compose1 g f = f >> g
    let compose2 g f = fun a b -> (f a b) |> g
    let compose3 g f = fun a b c -> (f a b c) |> g
    let compose4 g f = fun a b c d-> (f a b c d) |> g
    let compose5 g f = fun a b c d e -> (f a b c d e) |> g

    let pipe1 = (|>)
    let pipe2 f x = fun a ->  f a x
    let pipe3 f x = fun a b -> f a b x
    let pipe4 f x = fun a b c -> f a b c x

    let compose1_2nd g f = fun g_1 f_1 -> g g_1 (f f_1)
    let compose2_2nd g f = fun f_1 f_2 g_1 -> g g_1 (f f_1 f_2)
    let compose3_2nd g f = fun f_1 f_2 f_3 g_1 -> g g_1 (f f_1 f_2 f_3)

    let compose1_3rd g f = fun f_1 g_1 g_2 -> g g_1 g_2 (f f_1)
    let compose2_3rd g f = fun f_1 f_2 g_1 g_2 -> g g_1 g_2 (f f_1 f_2)
    let compose3_3rd g f = fun f_1 f_2 f_3 g_1 g_2 -> g g_1 g_2 (f f_1 f_2 f_3)
     

    ///Takes a function taking an integer (assumed to be an index) and a value, and returns a function without the integer.
    ///The integer value that is supplied is the number of times that function instance is called.
    let indexed (f : int -> 'v -> 'out) : 'v -> 'out = 
            let i = ref 0
            fun x -> 
                let r=  f !i x
                i := !i + 1
                r
[<AutoOpen>]
module Operators = 
    ///The "placeholder" operator. Partially invokes a function, skipping the first parameter and letting you specify the second one.
    let (@?) (f : 'a -> 'b -> 'c) (b : 'b) = b |> Fun.pipe2 f

    ///The double "placeholder" operator. Partially invokes a function, skipping the first two parameters and letting you specify the third one.
    let (@?@?) (f : 'a -> 'b -> 'c -> 'd) (c : 'c) = c |> Fun.pipe3 f

    ///The triple "placeholder" operator. Partially invokes a function, skipping the first three parameters and letting you specify the fourth one.
    let (@?@?@?) (f : 'a -> 'b -> 'c -> 'd -> 'e) (d : 'd) = d |> Fun.pipe4 f

    let (+>>) f g = fun x -> (f x) |> g x 

    ///The 2-composition operator. Composes a curried function taking two arguments (left operand) with another function (right operand).
    let (.>>) f g = f |> Fun.compose2 g

    ///The 3-composition operator. Composes a curried function taking three arguments (left operand) with another function (right operand).
    let (..>>) f g = f |> Fun.compose3 g

    ///The 4-composition operator. Composes a curried function taking four arguments (left operand) with another function (right operand).
    let (...>>) f g = f |> Fun.compose4 g

    ///The 5-composition operator. Composes a curried function taking five arguments (left operand) with another function (right operand).
    let (....>>) f g = f |> Fun.compose5 g

    let (>>.) f g = f |> Fun.compose1_2nd g

    let (.>>.) f g = f |> Fun.compose2_2nd g

    let (..>>.) f g = f |> Fun.compose3_2nd g

    let (>>..) f g = f |> Fun.compose1_3rd g
   
    let (.>>..) f g = f |> Fun.compose2_3rd g

    let (..>>..) f g = f |> Fun.compose3_3rd g


    


