#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/DotLiquid/lib/NET45/DotLiquid.dll"

#load "packages/FSharp.Formatting/FSharp.Formatting.fsx"
#load "tools/domain.fs"
#load "tools/document.fs"
#load "tools/dotliquid.fs"

open System
open System.IO
open Fake

open Document
open Domain

let cfg = { 
    SourceDir = __SOURCE_DIRECTORY__ </> "source"
    OutputDir = __SOURCE_DIRECTORY__ </> "output"
    LayoutsDir = __SOURCE_DIRECTORY__ </> "layouts"
    Root = "http://www.devcrafting.com"
    Prefix = Some "" }

let rec listFiles root = seq {
  if not (File.Exists(root </> ".ignore")) then
    yield! Directory.GetFiles(root)
    for d in Directory.GetDirectories(root) do
      yield! listFiles d }

let processFile cfg (file:String) =
    printfn "Processing file: %s" (file.Replace(cfg.SourceDir, ""))
    let article = transform file cfg
    let outFile = article.Url.Replace(cfg.Root, cfg.OutputDir) </> "index.html"
    ensureDirectory (Path.GetDirectoryName outFile)
    let layout = defaultArg article.Type "default"
    DotLiquid.transform outFile (layout + ".html") article

let generateSite cfg =
    listFiles cfg.SourceDir |> List.ofSeq
    |> Seq.iter (processFile cfg)

DotLiquid.initialize cfg 

generateSite cfg
