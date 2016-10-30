module Document.Tests

open Domain
open Document

open Xunit
open FsCheck
open FsCheck.Xunit

open System

[<Property>]
let ``Any file with extension other than .fsx or .md are transformed in a Content file`` cfg =
    transform cfg "anyfile" = Content "anyfile"

[<Property>]
let ``File with .md extension are transformed in an Article`` cfg =
    let cfg = 
        { cfg with
            SourceDir = "source/" 
            FileToUrlConvertionPatterns = ([], snd cfg.FileToUrlConvertionPatterns) }
    match transform cfg "source/minimal.md" with
    | Article (_, _) -> true
    | _ -> false

[<Property>]
let ``File with .md extension transformation throws Exception if no metadata`` cfg =
    let e = Assert.Throws<Exception>(fun () -> transform cfg "source/nometadata.md" |> ignore)
    e.Message.StartsWith("No metadata")