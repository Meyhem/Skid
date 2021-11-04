module Tests

open System
open System.Linq
open Xunit

[<Fact>]
let ``Parses basic config`` () =
    let cfg =
        Skid.Cli.getApplicationConfiguration false [| "-f"; "file.json"; "target-dir" |]

    Assert.True(cfg.ValueFiles.Length = 1)
    Assert.Equal("file.json", cfg.ValueFiles.[0])

    Assert.Equal("target-dir", cfg.Target)

[<Fact>]
let ``Sets defaults`` () =
    let cfg =
        Skid.Cli.getApplicationConfiguration false [| "-f"; "file.json"; "target-dir" |]

    Assert.False(cfg.Recursive)
    Assert.Equal("{{", cfg.MarkStart)
    Assert.Equal("}}", cfg.MarkEnd)

[<Fact>]
let ``Accepts multiple value files in order`` () =
    let cfg =
        Skid.Cli.getApplicationConfiguration
            false
            [| "-f"
               "file1.json"
               "-f"
               "file2.json"
               "-f"
               "file3.json"
               "target-dir" |]

    Assert.True(cfg.ValueFiles.Length = 3)

    Assert.True(
        Enumerable.SequenceEqual(
            [| "file1.json"
               "file2.json"
               "file3.json" |],
            cfg.ValueFiles
        )
    )

[<Fact>]
let ``Fails when no value file provided`` () =
    Assert.Throws(
        Action
            (fun x ->
                Skid.Cli.getApplicationConfiguration false [| "target-dir" |]
                |> ignore)
    )
    |> ignore

[<Fact>]
let ``Fails when no target  provided`` () =
    Assert.Throws(
        Action
            (fun x ->
                Skid.Cli.getApplicationConfiguration false [| "-f"; "file1.json" |]
                |> ignore)
    )
    |> ignore
