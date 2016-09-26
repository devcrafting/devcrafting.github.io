module Navbar

open Domain

type NavbarItem = {
    NavbarLink: Article<string>
    SubItems: NavbarItem list
}

open System
open System.Collections.Generic

let private menuLink key (articles: IDictionary<String, Article<String> seq>) =
    if not (articles.ContainsKey(key)) then
        None
    else
        Some (articles.[key] |> Seq.head)

let subMenu key (items: (IDictionary<String, Article<String> seq> -> NavbarItem option) seq)  = fun articles ->
    let menuLink = menuLink key articles
    match menuLink with
    | None -> None
    | Some article -> 
        Some { NavbarLink = article
               SubItems = items |> Seq.map (fun menuItem -> menuItem articles) |> Seq.choose id |> List.ofSeq }

let menuItem key = fun articles ->
    let menuLink = menuLink key articles
    match menuLink with
    | None -> None
    | Some article -> Some { NavbarLink = article; SubItems = [] }

let generateMenu menuDefinition language (articles:Article<String> seq) =
    let articlesByKey = 
        articles
        |> Seq.filter (fun a -> a.Language = language) 
        |> Seq.groupBy (fun a -> a.UniqueKey)
        |> dict
    menuDefinition subMenu menuItem
    |> Seq.map (fun (generateMenuItem: IDictionary<String, Article<String> seq> -> NavbarItem option) -> generateMenuItem articlesByKey)
    |> Seq.choose id
