module Skid.Templating

open System.Collections.Generic
open System.Text
open System.Text.RegularExpressions

type TemplateMark = { MatchedExpression: string; Required: bool; JsonPath: string }

let extractCaptures (groups: GroupCollection) =
    if groups.Count = 3 then
        Some({
          MatchedExpression = groups.[0].Value 
          Required = groups.[1].Value = "!" 
          JsonPath = groups.[2].Value
        })
    elif groups.Count = 2 then
        Some({
          MatchedExpression = groups.[0].Value 
          Required = false
          JsonPath = groups.[1].Value
        })
    else
        None

let findAllMarks templateString =
    Regex.Matches(templateString, "{{\s*(!)?\s*(.*)\s*}}")
    |> Seq.cast
    |> Seq.toList
    |> List.map (fun (m: Match) -> extractCaptures m.Groups)
    |> List.choose id

let replaceAllMarks (path : string) (templateString : string) (marks: TemplateMark list) (values: IDictionary<string, string>)  =
    let builder = StringBuilder(templateString) 
    for mark in marks do
        let hasValueForJsonPath, value = values.TryGetValue mark.JsonPath
        
        if mark.Required && not(hasValueForJsonPath) then
            printfn $"<!> missing required value '{mark.JsonPath}' used in '{path}'"
        
        let replacement = if hasValueForJsonPath then value else ""
        
        builder.Replace(mark.MatchedExpression, replacement) |> ignore

    builder.ToString()