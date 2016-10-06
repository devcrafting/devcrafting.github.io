module Domain

open System

type GenerationOptions = {
    OutputDir: string
    SourceDir: string
    LayoutsDir: string
    Root: string
    OutputGitRemote: string
    Prefix: string option
    FileToUrlConvertionPatterns: FileToUrlConvertionPattern list * Convertion
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
        { UniqueKey = x.UniqueKey; Url = x.Url; Title = x.Title; ShortTitle = x.ShortTitle
          Abstract = abs; Date = x.Date; Body = body; Tags = x.Tags; Language = x.Language
          Type = x.Type; Layout = x.Layout; Hidden = x.Hidden; RedirectFrom = x.RedirectFrom }

type File =
| Content of string
| Article of string * Article<string>