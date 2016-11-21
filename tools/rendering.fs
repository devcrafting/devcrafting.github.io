module Rendering

open System
open System.Collections.Generic
open System.IO
open Fake

open Domain
open Navbar
open ViewModels

let rec listFiles root = seq {
  if not (File.Exists(root </> ".ignore")) then
    yield! Directory.GetFiles(root)
    for d in Directory.GetDirectories(root) do
      yield! listFiles d }

let processFile cfg (file:String) articleViewModel =
    printfn "Processing file: %s, layout %s" (file.Replace(cfg.SourceDir, "")) articleViewModel.Article.Layout
    let outFile = 
        let outFileTmp = cfg.OutputDir </> articleViewModel.Article.Url.Substring(1)
        if outFileTmp.EndsWith(".html") then outFileTmp 
        else outFileTmp </> "index.html"
    ensureDirectory (Path.GetDirectoryName outFile)
    DotLiquid.transform outFile (articleViewModel.Article.Layout + ".html") articleViewModel

let copyFile cfg (file:String) =
    printfn "Copying content file: %s" (file.Replace(cfg.SourceDir, ""))
    let outFile = file.Replace(cfg.SourceDir, cfg.OutputDir)
    ensureDirectory (Path.GetDirectoryName outFile)
    File.Copy(file, outFile, true)

let generateTagPages cfg articles (navbar: IDictionary<string, NavbarItem list>) (translations: IDictionary<string, Dictionary<string, obj>>) = 
    articles
    |> Seq.collect (fun a -> a.Tags |> Seq.map (fun t -> { Title = t; Language = a.Language }, a))
    |> Seq.groupBy fst
    |> Seq.map (fun tagWithArticles ->
        let tag = fst tagWithArticles
        let tagViewModel = { 
            Tag = tag
            BlogPosts = snd tagWithArticles |> Seq.map snd |> Seq.sortByDescending (fun a -> a.Date) |> List.ofSeq
            Navbar = navbar.[tag.Language]
            Translations = translations.[tag.Language] }
        printfn "Generate tag page for: %s" tagViewModel.Tag.Title
        let outFile = cfg.OutputDir </> tagViewModel.Tag.Language </> "tag" </> tagViewModel.Tag.Title </> "index.html"
        ensureDirectory (Path.GetDirectoryName outFile)
        DotLiquid.transform outFile (cfg.LayoutsDir </> "tag.html") tagViewModel
        tagViewModel
    )
