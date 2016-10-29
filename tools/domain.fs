module Domain

open System
open System.Collections.Generic

type GenerationOptions = {
    OutputDir: string
    SourceDir: string
    LayoutsDir: string
    Root: string
    OutputGitRemote: string
    Prefix: string option
    Comments: CommentSystem option
    FileToUrlConvertionPatterns: FileToUrlConvertionPattern list * Convertion
}
and CommentSystem = Disqus of IDictionary<string, string>
and Disqus = { 
    PageUrl: string
    DisqusInstance: string
}
and FileToUrlConvertionPattern = {
    FilePattern: string
    Convertion: Convertion
}
and Convertion = DirectoryWithIndexHtml | HtmlFile

type Article<'T> = 
    {
        UniqueKey: string
        Language: string
        Url: string
        CompleteUrl: string
        Title: string
        ShortTitle: string
        Abstract: 'T
        Date: DateTime
        Body: 'T
        Type: string option
        Layout: string
        Tags: string list
        Hidden: bool
        RedirectFrom: string list
    }
    member x.With(body, abs) =
        { UniqueKey = x.UniqueKey; Url = x.Url; CompleteUrl = x.CompleteUrl; 
          Title = x.Title; ShortTitle = x.ShortTitle
          Abstract = abs; Date = x.Date; Body = body; Tags = x.Tags; Language = x.Language
          Type = x.Type; Layout = x.Layout; Hidden = x.Hidden; RedirectFrom = x.RedirectFrom }

type File =
| Content of string
| Article of string * Article<string>