module Rss

open Domain
open System.Xml.Linq
open System.IO
open Fake

let (!) name = XName.Get(name)
       
let private generateFeedForLanguage cfg language (articles:Article<string> seq) =
    let rssItems =
        [| for article in articles ->
            XElement(!"item", 
                XElement(!"title", article.Title),
                XElement(!"link", article.CompleteUrl),
                XElement(!"guid", article.CompleteUrl),
                XElement(!"pubDate", article.Date.ToUniversalTime().ToString("r")),
                XElement(!"description", article.Abstract))|]
    let rssChannel =
        XElement(!"channel",
            XElement(!"title", cfg.RssTitle),
            XElement(!"link", cfg.Root),
            XElement(!"description", cfg.RssDescription),
            rssItems)
    let rssDoc = XDocument(XElement(!"rss", XAttribute(!"version", "2.0"), rssChannel))
    let outputFile = cfg.OutputDir </> language </> "rss.xml"
    File.WriteAllText(outputFile, rssDoc.ToString())

let generateFeeds (cfg:GenerationOptions) (articles:Article<_> seq) =
    articles
    |> Seq.filter (fun a -> a.Date > System.DateTime.MinValue)
    |> Seq.groupBy (fun a -> a.Language)
    |> Seq.iter (fun (lang, articles) ->
                    let lastTwentyArticlesForLanguage = 
                        articles
                        |> Seq.sortByDescending (fun a -> a.Date)
                        |> Seq.take (min 20 <| Seq.length articles)
                    generateFeedForLanguage cfg lang lastTwentyArticlesForLanguage)
 