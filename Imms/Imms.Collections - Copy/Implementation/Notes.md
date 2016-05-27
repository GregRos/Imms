# Beware all ye who enter here
This part of the library contains aggressively optimized data structure code that discards all semblance of good OOP design (or any other form of good design, or indeed the very concept of 'good' itself) in order to reduce overhead.
 
Every allocation is seen as a sin and every method call as a potential hurdle. 

All sorts of weird things are done, largely to encourage the JITter to inline method calls.

This is why reviewing the code with an eye towards such frippery as DRY, single responsibility principles, design patterns, and so forth, is wrong.

This library's whole point is high performance. If high performance is sacrificed for good design, we have a well-designed piece of junk.

Here are some examples of the kinds of optimizations that are done. Note that no object in the Implementation namespace is meant to be user-visible.

1. Fields are always used instead of property accessors. This is to avoid unnecessary method calls. It is true that the JITter is supposed to inline simple method calls, but the process is totally opaque and properties have no advantage over fields in this case. It sounds like a bigger headache to track down method calls that fail to be inlined for whatever reason.

2. Fields are public, just to make sure some kind of accessibility/security related issue don't discourage the JITter from inlining method calls when it should.

3. #If directives are usually used instead of the Conditional attribute because it's easier to control which parts get excluded