#load "../document.fsx"

open FSharp.Markdown

let result = Document.Document.transform 
                (__SOURCE_DIRECTORY__ + "/source/fr/index.md")
                { SourceDir = __SOURCE_DIRECTORY__ + "/source"; OutputDir = ""; Root = "http://www.devcrafting.com"; Prefix = Some "" }
{ result with Body = MarkdownParagraphs.Empty } = { 
    UniqueKey = "home"
    Url = "http://www.devcrafting.com/fr"
    Title = "Accueil"
    Date = new System.DateTime(2016, 4, 15, 4, 0, 0)
    Body = MarkdownParagraphs.Empty }
