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

let private readMetadata (paragraphs:MarkdownParagraphs) =
    match paragraphs with 
    | Heading(size=1; body=title)::Properties(props, rest) -> title, props, rest
    | f -> failwithf "No metadata %A" f

let private tryFind k (props:IDictionary<string, string>) = 
    if props.ContainsKey k then Some(props.[k]) else None

(* Adapted to support multi lang *)
let private parseMetadata (cfg:GenerationOptions) (file:string) (title, props, body) =
    let normalizedRelativeFileName = file.Substring(cfg.SourceDir.Length).Replace('\\', '/')
    let extractFromFileName level =
        let splittedFileName = normalizedRelativeFileName.Split([| '/' |], StringSplitOptions.RemoveEmptyEntries)
        if splittedFileName.Length < level + 1 then
            None
        else
            splittedFileName |> Seq.skip (level - 1) |> Seq.head |> Some
    { 
        UniqueKey = defaultArg (tryFind "uniquekey" props) ""
        Language = extractFromFileName 1
        Title = formatSpans title
        Date = defaultArg (tryFind "date" props |> Option.map DateTime.Parse) DateTime.MinValue
        Url = cfg.Root + (Path.ChangeExtension(normalizedRelativeFileName, "")
                            .TrimEnd('.')
                            .Replace("/index", "")) + "/"
        Body = body
        Type = extractFromFileName 2
        Tags = (defaultArg (tryFind "tags" props) "").Split([| ',' |], StringSplitOptions.RemoveEmptyEntries) 
                |> Seq.map (fun s -> s.Trim()) |> List.ofSeq
    }

let transform file withOptions =
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

        article.With(File.ReadAllText(tmpBody.FileName))
    | _ -> failwith "Not supported file!"

