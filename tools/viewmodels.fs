module ViewModels

open Domain
open Navbar
open System.Collections

type ArticleViewModel = {
    Article: Article<string>
    ArticlesLanguages: Article<string> list
    BlogPosts: Article<string> list
    Navbar: NavbarItem list
    Translations: IDictionary
}

type TagViewModel = {
    Tag: Tag
    Article: Article<string>
    BlogPosts: Article<string> list
    Navbar: NavbarItem list
    Translations: IDictionary
}
and Tag = {
    Title: string
    Language: string
}
