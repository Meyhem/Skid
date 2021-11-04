module Skid.Cli

open Argu

type CliArgs =
    | [<Mandatory; AltCommandLine("-f")>] File of path: string
    | [<AltCommandLine("-r"); Unique>] Recursive
    | [<Unique>] Mark_Start of markStart: string
    | [<Unique>] Mark_End of markEnd: string
    | [<MainCommand; ExactlyOnce>] Target of targetPath: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | File _ -> "Json value file path to load (can be specified multiple times)"
            | Recursive _ -> "Recurse into subdirectories if <target> is directory"
            | Target _ ->
                "Target skid file or directory to run templating on. In case of directory, it searches all '*.skid' files"
            | Mark_Start _ -> "Characters that denote start of value interpolation. Default is '{{'"
            | Mark_End _ -> "Characters that denote end of value interpolation. Default is '}}'"

type SkidExit(exitOnError : bool) =
    interface IExiter with
        member this.Exit(msg, _) =
            if exitOnError then
                printfn $"{msg}"
                exit 1
            else
                failwith $"msg"

        member this.Name = "skid"

type ApplicationConfiguration =
    { ValueFiles: string []
      Target: string
      Recursive: bool
      MarkStart: string
      MarkEnd: string }

let getApplicationConfiguration exitOnError argv =
    let parser =
        ArgumentParser.Create<CliArgs>(programName = "skid", errorHandler = SkidExit(exitOnError))

    let results = parser.Parse argv

    let config: ApplicationConfiguration =
        { ValueFiles = results.GetResults File |> List.toArray
          Target = results.GetResult Target
          Recursive =
              results.GetResults Recursive
              |> List.contains Recursive
          MarkStart = results.GetResult (Mark_Start, "{{")
          MarkEnd = results.GetResult (Mark_End, "}}") }

    config
