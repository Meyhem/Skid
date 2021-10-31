let die msg =
    printf $"Error: {msg}"
    exit 1

let printpass str =
    printf $"{str}"
    str

[<EntryPoint>]
let main argv =
    let config =
        Skid.Cli.getApplicationConfiguration [| "--value-file"
                                                "D:/dev/skidtest/base.json"
                                                "--recursive"
                                                "D:/dev/skidtest" |]

    for path in config.ValueFiles do
        if not (Skid.Io.pathExists path) then
            die $"Value file '{path}' does not exist"

    let valueDocuments =
        config.ValueFiles
        |> Array.map Skid.Io.loadFile
        |> Array.map Skid.Json.parse

    let skidFiles =
        Skid.Io.discoverSkidFiles config.Target config.Recursive

    for skidFile in skidFiles do
        let content = Skid.Io.loadFile skidFile
        let marks = Skid.Templating.findAllMarks content

        let values =
            marks
            |> List.map (fun mark -> mark.JsonPath, Skid.Json.selectMergedProp mark.JsonPath valueDocuments)
            |> List.choose
                (fun (path, optValue) ->
                    match optValue with
                    | Some v -> Some(path, v)
                    | None -> None)
            |> dict

        printfn $"Templating {skidFile} ({List.length marks} marks)"
        
        Skid.Templating.replaceAllMarks skidFile content marks values
        |> Skid.Io.writeFile (Skid.Io.trimExtension skidFile)

    0
