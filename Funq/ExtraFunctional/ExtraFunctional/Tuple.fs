namespace ExtraFunctional
open System
open System.Runtime.CompilerServices
#nowarn"0044" //Hide 'obsolete' warning



///Provides strongly-typed operations on arbitrary tuples, e.g. (int * string * obj).
///Generally, these operations either succeed or fail at compile time.
module Tuple = 
    open ExtraFunctional.TupleImplementation
    module private Aux =
        //    ==================================================================
        //    ======         FOR GENERAL TUPLE OBJECTS OF ANY TYPE        ======
        //    ==================================================================

        let inline  staticLength (tuple : 'tuple) : 'length :> IStaticInt = 
            (^tuple : (member StaticLength : unit -> 'length) tuple)

        let inline  initial (inTuple : 'inTuple) : 'initialTuple =
            (^inTuple: (member Initial : unit -> 'initialTuple) inTuple)

        let inline  tail (inTuple : 'inTuple) : 'tailTuple =
            (^inTuple : (member Tail : unit -> 'tailTuple) inTuple)

        let inline  last (tuple : 'tuple) : 'last =
            (^tuple : (member Last : unit -> 'last) tuple)

        let inline  item (tuple : 'tuple, index : 'index) : 'item1 =
            (^tuple : (member Item : 'index -> 'item1) tuple, index)

        let inline  rev (tuple : 'tuple) : 'revTuple = 
            (^tuple: (member Rev :  unit -> 'revTuple) tuple)

        let inline  zip(tuple1 : 'tuple1, tuple2 : 'tuple2) =
            (^tuple1  : (member Zip : 'tuple2 -> 'out) tuple1, tuple2)
    
        let inline  cons(first : 'first, inTuple : 'inTuple) : 'outTuple =
            (^inTuple : (member Cons : 'first -> 'outTuple) inTuple, first)

        let inline  conj(inTuple : 'inTuple, last : 'last) : 'outTuple =
            (^inTuple : (member Conj : 'last -> 'outTuple) inTuple,last)
        
        let inline  set(inTuple : 'inTuple, index : 'index :> IStaticInt, newK : 'newK) =
            (^inTuple: (member Set : 'index * 'newK -> 'outTuple) inTuple, index, newK)

        let inline cast(inTuple : 'inTuple, typeObject : 'out) : 'outTuple = 
            (^inTuple : (member Cast<'out> : unit -> 'outTuple) inTuple)

        //    ==================================================================
        //    ======              TUPLE GENERATORS                        ======
        //    ==================================================================

        let inline  init(helper : 'helper, count : 'count :> IStaticInt, f : int -> 'element) = 
            ((^helper or ^count) : (static member Init : 'count * (int -> 'element) -> 'tuple) count, f)

        let inline  initElement(helper : 'helper, count : 'count :> IStaticInt, element : 'element) =
            ((^helper or ^count) : (static member InitElement : 'count * 'element -> 'tuple) count, element)

        //    ==================================================================
        //                     FOR TUPLES WITH SAME ELEMENT TYPES         
        //    ==================================================================
        let inline  get (tuple : 'tuple, index : int) : 'element = 
            (^tuple : (member Nth : int -> 'element) tuple, index)

        let inline  toList (tuple : 'tuple) : 'element list = 
            (^tuple : (member ToList : unit -> 'element list) tuple)

        let inline  fold (tuple : 'tuple, initial, f) =
            (^tuple : (member Fold : 'state -> ('element -> 'state -> 'state) -> 'state) tuple, initial, f)

        let inline  reduce (tuple : 'tuple, f) = 
            (^tuple : (member Reduce : ('element -> 'element -> 'element) -> 'element) tuple, f)

        let inline  map f (x : 'inTuple) = 
            (^inTuple : (member Map : ('inElement -> 'outElement) -> 'outTuple) x, f)

        let inline  toArray(inTuple : 'inTuple) : 'element array = 
            (^inTuple : (member ToArray : unit -> 'element array) inTuple)

    // ==================================================================
    // ======                 MODULE BINDINGS                      ======
    // ==================================================================

    //    ==================================================================
    //    ======         FOR GENERAL TUPLE OBJECTS OF ANY TYPE        ======
    //    ==================================================================

    ///Returns the statically determined length of the tuple, encoded as one of the strongly-typed numeric symbols, S{N} (from S0 to S10)
    let inline staticLength (tuple : '``('item1 * ... * 'itemN)``) : '``S{N}`` :> IStaticInt = Aux.staticLength(isTuple tuple)
        
    ///Returns the length of the specified tuple, as a 32-bit integer value.
    let inline length (tuple : '``('item1 * ... * 'itemN)``) : int = staticLength(tuple).Value   

    ///Returns a tuple consisting of the input tuple, without the last element. Tuple must be of size 3 or higher.
    let inline initial (tuple : '``('item1 * ... * 'itemN)``) : '``('item1 * ... * 'itemN-1)`` = Aux.initial(isTuple tuple)

    ///Returns a tuple consisting of the input tuple, without the first element. Tuple must be of size 3 or higher.
    let inline tail (tuple : '``('item1 * ... * 'itemN)``) : '``('item2 * ... * 'itemN)`` = Aux.tail(isTuple tuple)

    ///Returns the first element in the specified tuple. Identical to 'item0'.
    let inline first (tuple : '``('item1 * ... * 'itemN)`` ) : 'item1 = Aux.item(isTuple tuple,S0)

    ///Returns the last element in the specified tuple.
    let inline last (tuple : '``('item1 * ... * 'itemN)``) : 'itemN = Aux.last(isTuple tuple)

    ///Returns the element at position 'k' in the tuple, where 'k' is a strongly-typed numeric symbol, S{N} (from S0 to S10). 
    ///If the tuple has no such element, a compilation error is produced (overload resolution failure). This method works on tuples with differently typed elements.
    let inline item (k : '``S{k}`` :> IStaticInt) (tuple : '``('item1 * ... * 'itemK * ... * 'itemN)`` ) : 'itemK  = Aux.item(isTuple tuple, k)

    ///Returns the 1st element in the specified tuple. Identical to 'first'.
    let inline item0 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item0 = tuple |> item S0

    ///Returns the 2nd element in the specified tuple. The tuple must of size 2 or higher.
    let inline item1 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item1 = tuple |> item S1

    ///Returns the 3rd element in the specified tuple. The tuple must of size 3 or higher.
    let inline item2 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item2 = tuple |> item S2

    

    ///Returns the 4th element in the specified tuple. The tuple must of size 4 or higher.
    let inline item3 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item3 = tuple |> item S3

    ///Returns the 5th element in the specified tuple. The tuple must of size 5 or higher.
    let inline item4 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item4 = tuple |> item S4

    ///Returns the 6th element in the specified tuple. The tuple must of size 6 or higher.
    let inline item5 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item5 = tuple |> item S5

    ///Returns the 7th element in the specified tuple. The tuple must of size 7 or higher.
    let inline item6 (tuple : '``('item1 * ... * 'itemN)`` ) : 'item6 = tuple |> item S6

    ///Sets the item at the index 'k' to the value 'newK', returning a tuple of the results.
    let inline setItem (k : '``S{k}`` :> IStaticInt) (newK : 'newK) (tuple : '``('item1 * ... * 'itemK * ... * 'itemN)``) : '``('item1 * ... * 'newK * ... * 'itemN)`` =
        Aux.set(isTuple tuple, k, newK)

    ///Returns the specified tuple, with the 1st element set to the given value.
    let inline setItem0 (new0 : 'new0) (tuple : '``('item1 * ... * 'itemN)``) : '``('new1 * ... * 'itemN)`` = tuple |> setItem S0 new0

    ///Returns the specified tuple, with the 2nd element set to the given value. 
    let inline setItem1 (new1 : 'new1) (tuple : '``('item1 * ... * 'itemN)``) : '``('item1 * 'new2 * ... * 'itemN)`` = tuple |> setItem S1 new1

    ///Returns the specified tuple, with the 3rd element set to the given value, or adds a new value if the size is 2.
    let inline setItem2 (new2 : 'new2) (tuple : '``('item1 * ... * 'item3 * ... * 'itemN)``) : '``('item1 * ... new3 * ... * 'itemN)`` = tuple |> setItem S2 new2

    ///Returns the specified tuple, with the 4th element set to the given value, or adds a new value if the size is 3.
    let inline setItem3 (new3 : 'new3) (tuple : '``('item1 * ... * 'item4 * ... * 'itemN)``) : '``('item1 * ... * 'new4 * ... * 'itemN)`` = tuple |> setItem S3 new3

    ///Returns the specified tuple, with the 5th element set to the given value, or adds a new value if the size is 4.
    let inline setItem4 (new4 : 'new4) (tuple : '``('item1 * ... * 'item5 * ... * 'itemN)``) : '``('item1 * ... * 'new5 * ... * 'itemN)`` = tuple |> setItem S4 new4

    ///Returns the specified tuple, with the 6th element set to the given value, or adds a new value if the size is 5.
    let inline setItem5 (new5 : 'new5) (tuple : '``('item1 * ... * 'item6 * ... * 'itemN)``) : '``('item1 * ... * 'new6 * ... * 'itemN)`` = tuple |> setItem S5 new5

    ///Returns the specified tuple, with the 7th element set to the given value, or adds a new value if the size is 6.
    let inline setItem6 (new6 : 'new6) (tuple : '``('item1 * ... * 'item7 * ... * 'itemN)``) : '``('item1 * ... * 'new7 * ... * 'itemN)`` = tuple |> setItem S6 new6

    ///Applies the function on the kth element of the tuple, where k is a statically determined integer type.
    let inline mapItem (k : 'index :> IStaticInt) (f : 'itemK -> 'newK) (tuple : '``('item1 * ... * 'itemK * ... * 'itemN)``) : '``('item1 * ... * 'newK * ... * 'itemN)`` =
        let item = tuple |> item k
        let result = tuple |> setItem k (f item)
        result

    ///Applies the function on the 1st element of the tuple, returning a new tuple as a result.
    let inline mapItem0 (f : 'item1 -> 'new1) (tuple : '``('item1 * ... * 'itemN)``) : '``('new1 * ... * 'itemN)`` = 
        tuple |> mapItem S0 f

    ///Applies the function on the 2nd element of the tuple, returning a new tuple as a result.
    let inline mapItem1 (f : 'item2 -> 'new2) (tuple : '``('item1 * 'item2 * ... 'itemN)``) : '``('item1 * 'new2 * ... * 'itemN)`` = 
        tuple |> mapItem S1 f

    ///Applies the function on the 3rd element of the tuple, returning a new tuple as a result.
    let inline mapItem2 (f : 'item3 -> 'new3) (tuple : '``('item1 * 'item2 * 'item3 * ... * 'itemN)``) : '``('item1 * 'item2 * 'new3 * ... * 'itemN)`` = 
        tuple |> mapItem S2 f

    ///Applies the function on the 4th element of the tuple, returning a new tuple as a result.
    let inline mapItem3 (f : 'item4 -> 'new4) (tuple : '``('item1 * ... * 'item4 * ... * 'itemN)``) : '``('item1 * ... * 'new4 * ... * 'itemN)`` = 
        tuple |> mapItem S3 f

    ///Applies the function on the 5th element of the tuple, returning a new tuple as a result.
    let inline mapItem4 (f : 'item5 -> 'new5) (tuple : '``('item1 * ... * 'item5 * ... * 'itemN)``) : '``('item1 * ... * 'new5 * ... * 'itemN)`` = 
        tuple |> mapItem S4 f

    ///Applies the function on the 6th element of the tuple, returning a new tuple as a result.
    let inline mapItem5 (f : 'item6 -> 'new6) (tuple : '``('item1 * ... * 'item6 * ... * 'itemN)``) : '``('item1 * ... * 'new6 * ... * 'itemN)`` = 
        tuple |> mapItem S5 f

    ///Applies the function on the 7th element of the tuple, returning a new tuple as a result.
    let inline mapItem6 (f : 'item7 -> 'new7) (tuple : '``('item1 * ... * 'item7 * ... * 'itemN)``) : '``('item1 * ... * 'new7 * ... * 'itemN)`` = 
        tuple |> mapItem S6 f

    ///Returns the specified tuple, with its elements in reverse order.
    let inline rev (tuple : '``('item1 * ... * 'itemN)``) : '``('itemN * ... * 'item1)`` = 
        Aux.rev(tuple)
     
    ///Zips two tuples of equal length together, returning a tuple consisting of their elements correlated by order.
    let inline zip (tuple2 : '``(b1 * ... * bN)``) (tuple1 : '``(a1 * ... * aN)``) = 
        Aux.zip(isTuple tuple1, tuple2)

    ///Adds an element to the start of the tuple.
    let inline cons (head : 'head) (inTuple : '``('item1 * ... * 'itemN)``) : 'outTuple = 
        Aux.cons(head, isTuple inTuple) : 'outTuple

    ///Adds an element to the end of the tuple.
    let inline conj (last : 'last) (inTuple : '``('item1 * ... * 'itemN)``) : 'outTuple = 
        Aux.conj(isTuple inTuple,last)

      //    ==================================================================
    //                     FOR TUPLES WITH SAME ELEMENT TYPES         
    //    ==================================================================

    ///Maps a function over a tuple type. All tuple elements must be of the same type.
    let inline mapAll (f : '``in`` -> 'out) (tuple : '``('in * ... * 'in)``) : '``('out * ... * 'out)`` = 
        Aux.map f (isTupleSameType tuple)

    ///Iterates a function over the elements of a tuple type. All tuple elements must be of the same type.
    let inline iter (f : 'element -> unit) (tuple : '``('element * ... * 'element)``) : unit = 
        tuple |> mapAll f |> ignore

    ///Returns the nth element in a tuple consisting of identically-typed elements, or throws an exception if the element doesn't exist.
    let inline get (index : int) (tuple : '``('element * ... * 'element)``) : 'element = 
        Aux.get(isTupleSameType tuple, index)

    ///Converts an identically typed tuple to a linked list.
    let inline toList (tuple : '``('element * ... * 'element)``) : 'element list = 
        Aux.toList(isTupleSameType tuple)

    ///Folds over a tuple type. All tuple elements must be of the same type.
    let inline fold initial (f : 'state -> 'element -> 'state) (tuple : '``('element * ... * 'element)``) = 
        Aux.fold(isTuple tuple, initial, fun e s -> f s e)

    ///Reduces over a tuple type. All elements must be of the same type.
    let inline reduce (f : 'element -> 'element -> 'element) (tuple : '``(element * ... * element)``) = 
        Aux.reduce(isTupleSameType tuple, fun a b -> f b a)

    let inline apply1 (arg1 : 'arg1) (tuple : '``(('arg1 -> 'out) ... ('arg1 -> 'out))``) : '``('out * ... * 'out)`` =
        tuple |> mapAll (fun f -> f arg1)

    ///Takes as a parameter a tuple of functions, and invokes the functions with the two arguments, returning a tuple of the results.
    let inline apply2 (arg1 : 'arg1) (arg2 : 'arg2) (tuple : '``(('arg1 -> 'arg2 -> 'out) * ... * ('arg1 -> 'arg2 -> 'out))``) : '``('out * ... 'out)``= 
        tuple |> mapAll (fun f -> f arg1 arg2)

    ///Converts the tuple to an array.
    let inline toArray (tuple : '``(element * ... * element)``) : 'element array = 
        Aux.toArray(isTupleSameType tuple)

    ///Converts the specified tuple to a sequence, iterating over its elements.
    let inline toSeq (tuple : '``(element * ... * element)``) : 'element seq = 
        tuple |> toArray :> _ seq
        
    ///Converts the specified tuple to a set.
    let inline toSet (tuple : '``(element * ... * element)``) : 'element Set =
        tuple |> toArray |> Set.ofArray
        
    ///Converts a tuple of tuple (taken as a tuple of key-value pairs) to a map.
    let inline toMap (tuple : '``(('key * 'value) * ... * ('key * 'value))``) : Map<'key, 'element>=
        tuple |> toArray |> Map.ofArray

    //    ==================================================================
    //    ======              TUPLE GENERATORS                        ======
    //    ==================================================================

    ///Creates a tuple consisting of 'count' (static integer) elements by applying the generator to each index.
    let inline init (count : '``S{count}`` :> IStaticInt) (f : int -> 'element)  : '``('element * ... * 'element)`` = 
        Aux.init(helper, count, f) 

    ///Creates a tuple of the specified length, taking its elements by invoking a function several times.
    let inline initUnit (count : '``S{count}`` :> IStaticInt) (f : unit -> 'element)  : '``('element * ... * 'element)`` =
        init count (fun _ -> f()) 

    ///Creates a tuple consisting of 'count' (static integer) elements, each item being equal to the specified element.
    let inline repeat (count : '``S{count}`` :> IStaticInt) (element : 'element)  : '``('element * ... * 'element)`` =
        Aux.initElement(helper, count, element)

    ///Creates a tuple of the specified length (a static integer), taking its elements from an array, which must be of that length or longer.
    ///Throws an exception if the array is too short.
    let inline ofArray (length : '``S{n}`` :> IStaticInt) (arr : 'element array) : '``('element * ... * 'element)`` =
        if arr.Length < length.Value then invalidArg "arr" (sprintf "The collection was expected to have length '%d' or more, but it was '%d'" (length.Value) (arr.Length))
        init length (fun i -> arr.[i]) 

    ///Creates a tuple of the specified length (a static integer), taking its elements from a linked list, which must be of that length or longer.
    ///Throws an exception if the array is too short.
    let inline ofList (listLength : '``S{n}`` :> IStaticInt) (lst : 'element list) : '``('element * ... * 'element)`` =
        let arr = lst |> List.toArray
        init listLength (fun i -> arr.[i]) 

    
