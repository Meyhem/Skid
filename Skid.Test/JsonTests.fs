module Skid.Test.JsonTests

open System
open Xunit

[<Fact>]
let ``Parses json into JObject`` () =
    let result =
        Skid.Json.parse "{ \"prop\": \"value\" }"

    Assert.NotNull(result)
    Assert.Equal("value", result.SelectToken("prop").ToString())

[<Fact>]
let ``Fails on invalid json`` () =
    Assert.Throws(
        typeof<Newtonsoft.Json.JsonReaderException>,
        Action(fun x -> Skid.Json.parse "{ \"prop\": }" |> ignore)
    )
    |> ignore

[<Fact>]
let ``Selects merged prop from multiple JObjects`` () =
    let json1 = Skid.Json.parse "{ \"prop\": 123 }"
    let json2 = Skid.Json.parse "{ \"prop\": 456 }"
    
    Assert.Equal(Some("456"), Skid.Json.selectMergedProp "prop" [| json1; json2 |])

[<Fact>]
let ``Selects None if not exist`` () =
    let json1 = Skid.Json.parse "{ \"prop\": 123 }"
    let json2 = Skid.Json.parse "{ \"prop\": 456 }"
    
    Assert.Equal(None, Skid.Json.selectMergedProp "badprop" [| json1; json2 |])
