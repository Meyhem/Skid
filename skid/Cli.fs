module Skid.Cli

open Argu

type CliArgs =
    | [<Mandatory; AltCommandLine("-f")>] File of path: string
    | [<AltCommandLine("-r"); Unique>] Recursive
    | [<MainCommand; ExactlyOnce>] Target of targetPath: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | File _ -> "Value file path to use"
            | Recursive _ -> "Recurse into subdirectories if Target is directory"
            | Target _ -> "Target skid file or directory to run transformation. In case of directory, it searches all '*.skid' files"

type ApplicationConfiguration =
    { ValueFiles: string []
      Target: string
      Recursive: bool }

let getApplicationConfiguration argv =
    let parser =
        ArgumentParser.Create<CliArgs>(programName = "skid")

    let results = parser.Parse argv

    let config: ApplicationConfiguration =
        {
          ValueFiles = results.GetResults File |> List.toArray //argsToValueFiles (List.toSeq args)
          Target = results.GetResult Target // argsToTarget args
          Recursive =
              results.GetResults Recursive
              |> List.contains Recursive
        }

    config
