#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/DotLiquid/lib/NET45/DotLiquid.dll"
#r "packages/Suave/lib/net40/Suave.dll"

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
    let article = transform file cfg
    printfn "Processing file: %s, layout %s" (file.Replace(cfg.SourceDir, "")) article.Layout
    let outFile = article.Url.Replace(cfg.Root, cfg.OutputDir) </> "index.html"
    ensureDirectory (Path.GetDirectoryName outFile)
    DotLiquid.transform outFile (article.Layout + ".html") article

let generateSite cfg =
    listFiles cfg.SourceDir |> List.ofSeq
    |> Seq.iter (processFile cfg)

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
      //.Replace("</head", "<link href='/custom/bootstrap.css' rel='stylesheet'></head")
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
        Files.browseHome ]

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
    generateSite cfg
    let all = __SOURCE_DIRECTORY__ |> Path.GetFullPath
    use watcher = 
        !! (all </> "**/*.*") -- (all </> ".git") -- (all </> ".git/**/*.*") 
        |> WatchChanges (fun e ->
            printfn "Changed files"
            e |> Seq.iter (fun f -> printfn " - %s" f.Name)
            try
                generateSite cfg 
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

RunTargetOrDefault "run"