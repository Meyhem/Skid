module Skid.Json

open System.Text
open Newtonsoft.Json.Linq

let parse (jsonString: string) : JObject = JObject.Parse(jsonString)
let selectProp (path: string) (root: JObject) = root.SelectToken(path).ToString()

let selectMergedProp (path: string) (root: JObject []) =
    root
    |> Array.rev
    |> Array.choose
        (fun jObject ->
            match jObject.SelectToken(path) with
            | null -> None
            | value -> Some(value.ToString()))
    |> (fun values ->
        match values with
        | v when Array.length v = 0 -> None
        | v -> Some(Array.head v))
