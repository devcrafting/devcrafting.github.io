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
    CommentsSystem: CommentsSystem
    FileToUrlConvertionPatterns: FileToUrlConvertionPattern list * Convertion
    DraftsFolderOrFilePrefix: string list
    RssTitle: string
    RssDescription: string
}
and CommentsSystem = NoComments | Disqus of IDictionary<string, string>
and CommentsWidgets = { CountWidget: string; DisplayWidget: string; ScriptWidget: string }
and Disqus = { 
    PageUrl: string
    DisqusInstance: string
    Translations: IDictionary<string, obj>
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
        Comments: CommentsWidgets
    }
    member x.With(body, abs) =
        { UniqueKey = x.UniqueKey; Url = x.Url; CompleteUrl = x.CompleteUrl; 
          Title = x.Title; ShortTitle = x.ShortTitle; Comments = x.Comments
          Abstract = abs; Date = x.Date; Body = body; Tags = x.Tags; Language = x.Language
          Type = x.Type; Layout = x.Layout; Hidden = x.Hidden; RedirectFrom = x.RedirectFrom }
    static member Empty =
        { UniqueKey = ""; Url = ""; CompleteUrl = ""; 
          Title = ""; ShortTitle = ""; Comments = { CountWidget = ""; DisplayWidget = ""; ScriptWidget = "" }
          Abstract = ""; Date = DateTime.Now; Body = ""; Tags = []; Language = ""
          Type = None; Layout = ""; Hidden = false; RedirectFrom = [] }

type File =
| Content of string
| Article of string * Article<string>