module Document

open FSharp.Literate
open FSharp.Markdown
open System
open System.Collections.Generic
open System.IO
open System.Text.RegularExpressions

open Domain

(* Following code comes from https://github.com/tpetricek/tomasp.net/blob/master/tools/document.fs *)
type DisposableFile(file, deletes) =
    static member Create(file) =
        new DisposableFile(file, [file])
    static member CreateTemp(?extension) =
        let temp = Path.GetTempFileName()
        let file = match extension with Some ext -> temp + ext | _ -> temp
        new DisposableFile(file, [temp; file])
    member x.FileName = file
    interface System.IDisposable with
        member x.Dispose() =
            for delete in deletes do
            if File.Exists(delete) then File.Delete(delete)

open FSharp.Markdown.Html

let private (|ColonSeparatedSpans|_|) spans =
    let rec loop before spans = 
        match spans with
        | Literal(text=s)::rest when s.Contains(":") ->
            let s1, s2 = s.Substring(0, s.IndexOf(':')).Trim(), s.Substring(s.IndexOf(':')+1)
            let before = List.rev before
            let before = if String.IsNullOrWhiteSpace(s1) then before else Literal(s1, None)::before
            let rest = if String.IsNullOrWhiteSpace(s2) then rest else Literal(s2, None)::rest
            Some(before, rest)
        | [] -> None
        | x::xs -> loop (x::before) xs
    loop [] spans
    
let private createFormattingContext writer = 
    { Writer = writer
      Links = dict []
      Newline = "\n"
      LineBreak = ignore
      WrapCodeSnippets = false
      GenerateHeaderAnchors = true
      UniqueNameGenerator = new UniqueNameGenerator()
      ParagraphIndent = ignore }

let private formatSpans spans = 
    let sb = System.Text.StringBuilder()
    ( use wr = new StringWriter(sb)
      let fc = createFormattingContext wr
      Html.formatSpans fc spans )
    sb.ToString()

let private formatPlainSpans spans = 
    let sb = Text.StringBuilder()
    let rec loop spans = 
        for span in spans do
        match span with
        | DirectLink(body=body) -> loop body
        | Literal(text=t) -> sb.Append(t) |> ignore
        | _ -> failwithf "Unsupported span: %A" span
    loop spans
    sb.ToString()

let private readProperty = function
    | [Span(body=ColonSeparatedSpans(before, after))] ->
        match formatPlainSpans before with
        | "description" -> "description", formatSpans after
        | s -> s, (formatPlainSpans after).Trim()
    | p -> failwithf "Failed to read property: %A" p
    
let private (|Properties|) = function
    | ListBlock(kind=MarkdownListKind.Unordered; items=props)::rest ->
        props |> List.map readProperty |> dict, rest
    | rest -> dict [], rest

let private (|Let|) p v = p, v

let private (|Abstract|) = function
  | HorizontalRule(_)::ListBlock(kind=MarkdownListKind.Unordered; items=props)::rest 
  | HorizontalRule(_)::Let [] (props, rest) ->
      let rec split acc = function
        | HorizontalRule _ :: rest -> List.rev acc, rest
        | p :: rest -> split (p::acc) rest
        | _ -> failwith "Parsing abstract failed"
      let standalone = props |> Seq.exists(function [Span(body=[Literal(text="standalone")])] -> true | _ -> false)
      let abs, rest = split [] rest
      Some(standalone, abs), rest
  | rest -> None, rest

let private readMetadata (paragraphs:MarkdownParagraphs) =
    match paragraphs with 
    | Heading(size=1; body=title)::Properties(props, Abstract(abs, rest)) -> title, props, abs, rest
    | f -> failwithf "No metadata %A" f

let private tryFind k (props:IDictionary<string, string>) = 
    if props.ContainsKey k then Some(props.[k]) else None

(* Adapted to support multi lang *)
let private parseMetadata (cfg:GenerationOptions) (file:string) (title, props, abstractOpt, body) =
    let abs, body =
        match abstractOpt with
        | Some(true, abs) -> abs, body
        | Some(false, abs) -> abs, (abs @ body)
        | None -> [], body
    let normalizedRelativeFileName = file.Substring(cfg.SourceDir.Length).Replace('\\', '/')
    let extractFromFileName level =
        let splittedFileName = normalizedRelativeFileName.Split([| '/' |], StringSplitOptions.RemoveEmptyEntries)
        if splittedFileName.Length < level + 1 then
            None
        else
            splittedFileName |> Seq.skip (level - 1) |> Seq.head |> Some
    let articleType = extractFromFileName 2
    let fileToUrlConvertion = 
        let convertionPatterns, defaultConvertion = cfg.FileToUrlConvertionPatterns
        let firstMatch =
            convertionPatterns
            |> List.filter (fun p -> Regex.IsMatch(normalizedRelativeFileName, p.FilePattern))
            |> List.map (fun p -> p.Convertion)
            |> List.tryHead
        defaultArg firstMatch defaultConvertion 
    let url = 
        match fileToUrlConvertion with
        | DirectoryWithIndexHtml ->
            (Path.ChangeExtension(normalizedRelativeFileName, "")
                .TrimEnd('.')
                .Replace("/index", "")) + "/"
        | HtmlFile -> Path.ChangeExtension(normalizedRelativeFileName, "html")
    let titleString = formatSpans title
    { 
        UniqueKey = defaultArg (tryFind "uniquekey" props) ""
        Language = defaultArg (extractFromFileName 1) "en"
        Title = titleString
        ShortTitle = defaultArg (tryFind "shortTitle" props) titleString
        Abstract = abs
        Date = defaultArg (tryFind "date" props |> Option.map DateTime.Parse) DateTime.MinValue
        Url = url
        CompleteUrl = cfg.Root + url
        Body = body
        Type = articleType
        Layout = defaultArg (tryFind "layout" props) (defaultArg articleType "default")
        Tags = (defaultArg (tryFind "tags" props) "").Split([| ',' |], StringSplitOptions.RemoveEmptyEntries) 
                |> Seq.map (fun s -> s.Trim()) |> List.ofSeq
        Hidden = match tryFind "hidden" props with Some hidden -> bool.Parse(hidden) | None -> false
        RedirectFrom = (defaultArg (tryFind "redirectfrom" props) "").Split([| ',' |], StringSplitOptions.RemoveEmptyEntries) 
                |> Seq.map (fun s -> s.Trim()) |> List.ofSeq
    }

let transform withOptions file =
    match Path.GetExtension(file).ToLower() with
    | (".fsx" | ".md") as extension ->
        use html = DisposableFile.CreateTemp(".html")
        let document =
            if extension = ".fsx" then
                Literate.ParseScriptFile(file)
            else
                Literate.ParseMarkdownFile(file)
        let article = parseMetadata withOptions file (readMetadata document.Paragraphs)

        use tmpBody = DisposableFile.CreateTemp(".html")
        Literate.ProcessDocument(document.With(article.Body), tmpBody.FileName) 
        use tmpAbstract = DisposableFile.CreateTemp(".html")
        Literate.ProcessDocument(document.With(article.Abstract), tmpAbstract.FileName) 

        Article (file, article.With(File.ReadAllText(tmpBody.FileName), File.ReadAllText(tmpAbstract.FileName)))
    | ".html" -> 
        Article (file, { UniqueKey = "index"; Language = "fr"; Url = "/"; 
                         CompleteUrl = withOptions.Root + "/"; Title = ""; ShortTitle = ""
                         Abstract = ""; Date = DateTime.MinValue; Body = File.ReadAllText(file);
                         Type = None; Layout = "raw"; Tags = []; Hidden = false; RedirectFrom = [] })
    | _ -> Content file

