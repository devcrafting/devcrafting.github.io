module Document.Tests

open Domain
open Document

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

open System

[<Property>]
let ``Any file with extension other than .fsx or .md are transformed in a Content file`` cfg =
    transform cfg "anyfile" = Content "anyfile"

[<Property>]
let ``File with .md extension are transformed in an Article file`` cfg =
    let cfg = 
        { cfg with
            SourceDir = "source/" 
            FileToUrlConvertionPatterns = ([], snd cfg.FileToUrlConvertionPatterns) }

    match transform cfg "source/minimal.md" with
    | Article (_, article) ->
        article.Title =! "minimal"
        article.Language =! "en"
        article.ShortTitle =! "minimal"
        article.Abstract =! "\n\n"
        article.Body =! "\n\n"
        article.Date =! DateTime.MinValue
        test <@ not article.Hidden @>
        article.Tags =! []
        article.Type =! None
        article.Layout =! "default"
        article.RedirectFrom =! []
    | _ -> test <@ false @>

[<Property>]
let ``File with .md extension transformation throws Exception if no metadata`` cfg =
    raisesWith <@ transform cfg "source/nometadata.md" @> (fun e -> <@ e.Message.StartsWith("No metadata") @>)
