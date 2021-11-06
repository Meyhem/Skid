module Skid.Test.Templating

open Xunit

[<Fact>]
let ``Parses marks`` () =
    let marks =
        Skid.Templating.findAllMarks "{{mark1}}{{mark2}}" "{{" "}}"
        
    Assert.True(marks.Length = 2)
    Assert.Equal("mark1", marks.[0].JsonPath)
    Assert.Equal("mark2", marks.[1].JsonPath)
    
[<Fact>]
let ``Accepts custom mark start and end`` () =
    let marks =
        Skid.Templating.findAllMarks "<<mark1>>" "<<" ">>"
        
    Assert.True(marks.Length = 1)
    Assert.Equal("mark1", marks.[0].JsonPath)

[<Fact>]
let ``Accepts custom mark start and end that are regex control chars`` () =
    let marks =
        Skid.Templating.findAllMarks "[[mark1]]" "[[" "]]"
        
    Assert.True(marks.Length = 1)
    Assert.Equal("mark1", marks.[0].JsonPath)
