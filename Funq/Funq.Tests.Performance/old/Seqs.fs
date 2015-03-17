namespace Funq.Benchmarking
open System.Collections


type NamedSeq<'v> = 
    {
        Seq : 'v seq
        Meta : Meta list
    }
    interface IDataGenerator<'v> with
        member x.GetEnumerator () = x.Seq.GetEnumerator()
        member x.GetEnumerator() : IEnumerator= x.Seq.GetEnumerator() :>_
        member x.Metadata = x.Meta

module Seqs = 
    module Strings = 
        let letters(min,max)= 
            {
                Seq  = seq {while true do yield Rand.String.word_like(min,max)} |> Seq.cache          
                Meta = [{Name = "Strings of Letters by Length"; Range=(min,max)}]
            }
    module Numbers = 
        let length(min,max) = 
            {
               Seq = seq {while true do yield Rand.Int.of_length(min,max)} |> Seq.cache
               Meta = [{Name = "Numbers by Length"; Range=(min,max)}]
            }
        let float(min,max) = 
            {
                Seq = seq {while true do yield Rand.Float.in_range(min,max)} |> Seq.cache
                Meta = [{Name = "Floating Point in Range"; Range=(min,max)}]
            }



    module Lists = 
        let of_seq (min,max) (s : NamedSeq<_>) = 
            {
                Seq = seq {for slice in Rand.Seq(s).random_slices(min,max) do yield slice |> List.ofSeq} |> Seq.cache
                Meta = [{Name = "Lists by Length"; Range=(min,max)}] @ s.Meta
            }
    module Arrays = 
        let of_seq (min,max) (s : NamedSeq<_>) = 
            {
                Seq = seq {for slice in Rand.Seq(s).random_slices(min,max) do yield slice |> Array.ofSeq} |> Seq.cache
                Meta = [{Name = "Arrays by Length"; Range=(min,max)}] @ s.Meta
            }