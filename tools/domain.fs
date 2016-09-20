module Domain

open System

type GenerationOptions = {
    OutputDir: string
    SourceDir: string
    LayoutsDir: string
    Root: string
    Prefix: string option
}

type Article<'T> = 
    {
        UniqueKey: string
        Language: string option
        Url: string
        Title: string
        Date: DateTime
        Body: 'T
        Type: string option
        Layout: string
        Tags: string list
    }
    member x.With(body) =
        { UniqueKey = x.UniqueKey; Url = x.Url; Title = x.Title; Date = x.Date; Body = body
          Tags = x.Tags; Language = x.Language; Type = x.Type; Layout = x.Layout }
