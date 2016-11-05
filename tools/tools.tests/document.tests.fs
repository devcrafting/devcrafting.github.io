module Document.Tests

open Domain
open Document

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

open System

let basicCfg cfg = 
            { cfg with
                SourceDir = "source/" 
                FileToUrlConvertionPatterns = ([], snd cfg.FileToUrlConvertionPatterns) }

let (!=!) article testExpr = 
    match article with
    | Article (_, article) -> testExpr article
    | _ -> test <@ false @>

module ``Simplest file transformations use case: `` =
    [<Property>]
    let ``Any file with extension other than .fsx or .md are transformed in a Content file`` cfg =
        transform cfg "anyfile" = Content "anyfile"

    [<Property>]
    let ``Minimal .md file (i.e at least a title) is transformed in an Article file with default properties`` cfg =
        let cfg = basicCfg cfg
        transform cfg "source/minimal.md" !=!
            fun article ->
                article.UniqueKey =! ""
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

    [<Property>]
    let ``Transformation of a .md file without at least a title in first line throws Exception`` cfg =
        raisesWith <@ transform cfg "source/nometadata.md" @> (fun e -> <@ e.Message.StartsWith("No metadata") @>)

module ``Transformation of 'basic' metadata: `` =
    [<Property>]
    let ``ShortTitle metadata is parsed`` cfg =
        let cfg = basicCfg cfg
        transform cfg "source/shortTitle.md" !=!
            fun article -> article.ShortTitle =! "shortTitle"

module ``Detect hidden file in transform`` =
    [<Property>]
    let ``When it contains hidden metadata`` cfg =
        let cfg = basicCfg cfg
        transform cfg "source/hidden.md" !=!
            fun article -> article.Hidden =! true

    [<Property>]
    let ``When they are in a drafts directory given in config`` cfg =
        let cfg = { basicCfg cfg with DraftsFolderOrFilePrefix = [ "drafts" ] }
        transform cfg "source/drafts/test.md" !=!
            fun article -> article.Hidden =! true