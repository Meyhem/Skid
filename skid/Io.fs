module Skid.Io

open System.IO
open System.Text.RegularExpressions

let resolvePath path = Path.GetFullPath path
let pathExists path = FileInfo(path).Exists

let isDir path =
    (File.GetAttributes(path)
     &&& FileAttributes.Directory) = FileAttributes.Directory

let isFile path = path |> isDir |> not
let loadFile path = File.ReadAllText path
let writeFile path content = File.WriteAllText(path, content)
let trimExtension (path: string) = Regex.Replace(path, "\.\w+$", "")

let listSkidFiles path recursive =
    Directory.EnumerateFiles(
        path,
        "*.skid",
        if recursive then
            SearchOption.AllDirectories
        else
            SearchOption.TopDirectoryOnly
    )
    |> Seq.map resolvePath
    |> Seq.toArray

let discoverSkidFiles path recursive =
    if isFile path then
        [| resolvePath path |]
    else
        listSkidFiles path recursive
