#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/DotLiquid/lib/NET45/DotLiquid.dll"
#r "packages/Suave/lib/net40/Suave.dll"

#load "packages/FSharp.Formatting/FSharp.Formatting.fsx"
#load "tools/domain.fs"
#load "tools/document.fs"
#load "tools/navbar.fs"
#load "tools/localization.fs"
#load "tools/viewmodels.fs"
#load "tools/dotliquid.fs"
#load "tools/rendering.fs"

#load "config.fsx"

open System
open System.IO
open Fake
open Fake.Git

open Document
open Domain
open Navbar
open Config
open Rendering

let cfg = { 
    SourceDir = __SOURCE_DIRECTORY__ </> "source"
    OutputDir = __SOURCE_DIRECTORY__ </> "output"
    LayoutsDir = __SOURCE_DIRECTORY__ </> "layouts"
    OutputGitRemote = "https://github.com/devcrafting/devcrafting.com"
    Root = "http://www.devcrafting.com"
    Prefix = Some ""
    CommentsSystem = Disqus (dict [ ("fr", "devcrafting-2"); ("en", "devcrafting-1") ] )
    FileToUrlConvertionPatterns = [ { FilePattern = "/404.md"; Convertion = HtmlFile } ], DirectoryWithIndexHtml
    DraftsFolderOrFilePrefix = [ "drafts" ]
}


open System.Collections
open System.Collections.Generic


open System.Web

let generateSite cfg changes =
    let filesWithoutComments =
        listFiles cfg.SourceDir |> List.ofSeq
        |> Seq.map (fun file -> transform cfg file)
    let languagesUsed =
        filesWithoutComments
        |> Seq.choose (function | Article (_, article) -> Some article.Language | _ -> None ) |> Seq.distinct
    let translations = Localization.getTranslations languagesUsed translationByLanguage traceImportant 
    let files =
        filesWithoutComments
        |> Seq.map (function 
                    | Article(file, article) -> 
                        Article(file, { article with Comments = getComments cfg article translations.[article.Language] })
                    | other -> other)
    let articles = 
        files
        |> Seq.choose (function 
            | Article (file, article) -> Some article
            | _ -> None)
        |> Seq.filter (fun a -> not a.Hidden)
    let menuByLanguage = 
        languagesUsed
        |> Seq.map (fun l -> l, generateMenu menu l articles |> List.ofSeq)
        |> dict
    let articlesByKey = articles |> Seq.groupBy (fun a -> a.UniqueKey) |> dict
    let blogPosts = 
        articles
        |> Seq.filter (fun a -> a.Type = Some "blog" && a.Layout <> "bloglisting")
        |> Seq.sortByDescending (fun a -> a.Date)
        |> Seq.groupBy (fun a -> a.Language)
        |> dict

    trace "Generating tag pages..."
    let tags = generateTagPages cfg articles menuByLanguage translations |> List.ofSeq

    trace "Generating redirect pages..."
    generateRedirectPages cfg articles

    trace "Processing files..."
    files
    |> Seq.iter (function 
        | Article (file, article) 
            when changes = Set.empty || Set.contains file changes || article.Layout = "bloglisting" ->
            
            let blogPosts = 
                if blogPosts.ContainsKey(article.Language) then
                    blogPosts.[article.Language] |> List.ofSeq
                else 
                    []

            let articlesLanguages =
                if articlesByKey.ContainsKey(article.UniqueKey) then
                    articlesByKey.[article.UniqueKey] |> List.ofSeq
                else
                    []

            processFile cfg file 
                { Article = article
                  BlogPosts = blogPosts
                  ArticlesLanguages = articlesLanguages
                  Navbar = menuByLanguage.[article.Language]
                  Translations = translations.[article.Language] }
        | Content file
            when changes = Set.empty || Set.contains file changes -> copyFile cfg file
        | _ -> ())
    
    trace "Ignore drafts when publishing"
    File.WriteAllLines(
        cfg.OutputDir </> ".gitignore", 
        cfg.DraftsFolderOrFilePrefix |> Seq.map (fun x -> x + "/"))

let regenerateSite () = 
    trace "Regenerating site from scratch"
    if Directory.Exists(cfg.OutputDir) then
        for dir in Directory.GetDirectories(cfg.OutputDir) do
            if not (dir.EndsWith(".git")) then 
                CleanDir dir; Directory.Delete dir
        for f in Directory.GetFiles(cfg.OutputDir) do File.Delete f
    generateSite cfg Set.empty

DotLiquid.initialize cfg 

// Suave Web server for debugging
open Suave
open Suave.Filters
open Suave.Operators
open Suave.WebSocket

let port = 11111
let refreshEvent = new Event<unit>()

let wsRefresh = """
  <script language="javascript" type="text/javascript">
    function init() {
      try {
        websocket = new WebSocket("ws://" + window.location.hostname + ":" + window.location.port + "/websocket");
        websocket.onmessage = function(evt) { location.reload(); };
      } catch (e) { /* silently ignore lack of websockets */ }
    }
    window.addEventListener("load", init, false);
  </script>"""

let handleDir dir = 
  let html = File.ReadAllText(cfg.OutputDir </> dir </> "index.html")
  html.Replace(cfg.Root, sprintf "http://localhost:%d" port)
      .Replace("</body", wsRefresh + "</body")
      .Replace("---", "")
  |> Successful.OK

let app = 
    choose [
        path "/websocket" >=> handShake (fun ws ctx -> async {
            let msg = System.Text.Encoding.UTF8.GetBytes "refreshed"
            while true do
                do! refreshEvent.Publish |> Control.Async.AwaitEvent
                do! ws.send Text msg true |> Async.Ignore
            return Choice1Of2 () })
        path "/" >=> request (fun _ -> handleDir "")
        pathScan "/%s/" handleDir
        Files.browseHome
        pathScan "/%s" (fun path -> 
            if Directory.Exists(cfg.OutputDir </> path) then
                Redirection.moved_permanently ("/" + path + "/") // Same behavior as Jekyll
            else
                File.ReadAllText(cfg.OutputDir </> path) |> Successful.OK) ]

let serverConfig =
  { Web.defaultConfig with
      homeFolder = Some cfg.OutputDir
      logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Warn
      bindings = [ HttpBinding.mkSimple HTTP "127.0.0.1" port ] }

let startServer () =
  let _, start = Web.startWebServerAsync serverConfig app
  let cts = new System.Threading.CancellationTokenSource()
  Async.Start(start, cts.Token)

// FAKE
Target "run" (fun () ->
    regenerateSite ()
    use watcher = 
        !! (cfg.SourceDir </> "**/*.*") ++ (cfg.LayoutsDir </> "*.*")
        |> WatchChanges (fun e ->
            trace "Changed files"
            e |> Seq.iter (fun f -> printfn " - %s" f.Name)
            try
                if e |> Seq.exists (fun f -> f.FullPath.StartsWith(cfg.LayoutsDir)) then
                    trace "Layout changed, regenerating all files..."
                    generateSite cfg Set.empty
                else
                    generateSite cfg (set [ for f in e -> f.FullPath ])
                refreshEvent.Trigger ()
                trace "Site updated successfully..."
            with e ->
                traceError "Updating site failed!"
                traceException e )
    startServer ()
    Diagnostics.Process.Start(sprintf "http://localhost:%d" port) |> ignore
    trace "Waiting for changes, press Enter to stop...."
    Console.ReadLine () |> ignore 
)

Target "publish" (fun () ->
    runGitCommand __SOURCE_DIRECTORY__ "add ." |> ignore
    runGitCommand __SOURCE_DIRECTORY__ (sprintf "commit -a -m \"Updating site (%s)\"" (DateTime.Now.ToString("f"))) |> ignore
    Git.Branches.push __SOURCE_DIRECTORY__

    if not (Directory.Exists(cfg.OutputDir)) then
        trace "Cloning remote repository with "
        let cloneLocation = DirectoryInfo(cfg.OutputDir).Parent.FullName
        let cloneDirectory = DirectoryInfo(cfg.OutputDir).Name
        runGitCommand cloneLocation (sprintf "clone -b master %s %s" cfg.OutputGitRemote cloneDirectory) |> ignore
    
    regenerateSite ()
    runGitCommand cfg.OutputDir "add ." |> ignore
    runGitCommand cfg.OutputDir (sprintf "commit -a -m \"Publish site (%s)\"" (DateTime.Now.ToString("f"))) |> ignore
    Git.Branches.push cfg.OutputDir
)

Target "generate" (fun () ->
    regenerateSite ()
)

let buildDir  = "./build/"

let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

Target "build" (fun _ ->
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

open Fake.Testing

Target "test" (fun () -> 
    !! "build/*.tests.dll"
    |> xUnit2 (fun p -> { p with ToolPath = "packages/xunit.runner.console/tools/xunit.console.exe" })
)

"build" ==> "test" 

RunTargetOrDefault "run"