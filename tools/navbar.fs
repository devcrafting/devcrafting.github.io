module Navbar

open Domain

type NavbarItem = {
    NavbarLink: Article<string>
    SubItems: NavbarItem list
}

open System
open System.Collections.Generic

let private menuLink key (articles: IDictionary<String, Article<String> seq>) =
    articles.[key] |> Seq.head

let subMenu key items = fun articles ->
    { NavbarLink = menuLink key articles
      SubItems = items |> Seq.map (fun menuItem -> menuItem articles) |> List.ofSeq }

let menuItem key = fun articles ->
    { NavbarLink = menuLink key articles; SubItems = [] }

let generateMenu menuDefinition language articles =
    let articlesByKey = 
        articles
        |> Seq.filter (fun a -> a.Language = language) 
        |> Seq.groupBy (fun a -> a.UniqueKey)
        |> dict
    menuDefinition subMenu menuItem
    |> Seq.map (fun generateMenuItem -> generateMenuItem articlesByKey) 