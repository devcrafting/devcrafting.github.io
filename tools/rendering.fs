module Rendering

open System
open System.Collections.Generic
open System.IO
open System.Web
open Fake

open Domain
open Navbar
open ViewModels

let rec listFiles root = seq {
  if not (File.Exists(root </> ".ignore")) then
    yield! Directory.GetFiles(root)
    for d in Directory.GetDirectories(root) do
      yield! listFiles d }

let processFile cfg (file:String) (articleViewModel:ArticleViewModel) =
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
    |> Seq.groupBy (fun t -> (fst t).Title.ToLower(), (fst t).Language)
    |> Seq.sortBy fst
    |> Seq.map (fun tagWithArticles ->
        let tag = snd tagWithArticles |> Seq.head |> fst
        let otherTags = 
            snd tagWithArticles
            |> Seq.map fst
            |> Seq.except [ tag ]
            |> Seq.map (fun t -> sprintf "/%s/tag/%s/" t.Language t.Title)
            |> Seq.distinct
            |> Seq.toList
        let tagViewModel = { 
            Tag = tag
            Article = { Article.Empty with RedirectFrom = otherTags }
            BlogPosts = snd tagWithArticles |> Seq.map snd |> Seq.sortByDescending (fun a -> a.Date) |> List.ofSeq
            Navbar = navbar.[tag.Language]
            Translations = translations.[tag.Language] }
        printfn "Generate tag page for: /%s/%s (+ %A)" tagViewModel.Tag.Language tagViewModel.Tag.Title tagViewModel.Article.RedirectFrom
        let outFile = cfg.OutputDir </> tagViewModel.Tag.Language </> "tag" </> tagViewModel.Tag.Title </> "index.html"
        ensureDirectory (Path.GetDirectoryName outFile)
        DotLiquid.transform outFile (cfg.LayoutsDir </> "tag.html") tagViewModel
        tagViewModel
    )

let generateRedirectPages cfg articles =
    // on Windows, it will only handle case insensitive URL (not the standard for URL)
    // ex: if you want redirect from /blog and /Blog, it is not possible on Windows
    // workaround is to rely on other mecanism: Jekyll redirect_from or web server/proxy... 
    articles
    |> Seq.collect (fun a -> a.RedirectFrom |> List.map (fun r -> a, r))
    |> Seq.sortByDescending (snd >> String.length) // Allow to generate longer path first and then take decision when sub path have to be generated
    |> Seq.iter (fun (a, r) -> 
        printfn "Generate redirect page %s for %s" r a.Url
        let encodedPath = 
            r.Split([| '/' |], StringSplitOptions.RemoveEmptyEntries) 
            |> Seq.fold (fun fullPath path -> fullPath </> HttpUtility.UrlEncode(HttpUtility.UrlDecode(path))) String.Empty
        let mutable outFile = cfg.OutputDir </> encodedPath
        if Directory.Exists(outFile) then
            outFile <- outFile </> "index.html"
        printfn "%s" outFile
        ensureDirectory(Path.GetDirectoryName outFile)
        DotLiquid.transform outFile (cfg.LayoutsDir </> "redirect.html") a
    )

let getComments cfg forArticle translations =
    match cfg.CommentsSystem with
    | NoComments -> { CountWidget = ""; DisplayWidget = ""; ScriptWidget = "" }
    | Disqus config -> 
        let model = { PageUrl = forArticle.CompleteUrl; DisqusInstance = config.[forArticle.Language]; Translations = translations }
        { 
            CountWidget = DotLiquid.render (cfg.LayoutsDir </> "disqusCount.html") model
            DisplayWidget = DotLiquid.render (cfg.LayoutsDir </> "disqus.html") model
            ScriptWidget = DotLiquid.render (cfg.LayoutsDir </> "disqusScript.html") model
        }
