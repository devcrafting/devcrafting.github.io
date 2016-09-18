#load "../document.fsx"

open FSharp.Markdown
open Document.Document

let file =  __SOURCE_DIRECTORY__ + "/source/fr/index.md"
let cfg = { SourceDir = __SOURCE_DIRECTORY__ + "/source"; OutputDir = ""; Root = "http://www.devcrafting.com"; Prefix = Some "" }

let result = transform file cfg
{ result with Body = "" } = { 
    UniqueKey = "home"
    Language = Some "fr"
    Url = "http://www.devcrafting.com/fr/"
    Title = "Accueil"
    Date = new System.DateTime(2016, 4, 15, 4, 0, 0)
    Body = ""
    Type = None
    Tags = [ "foo"; "bar" ] }
