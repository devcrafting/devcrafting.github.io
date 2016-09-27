#load "tools/navbar.fs"

open Navbar

let menu submenu menuItem =
    [
        subMenu "services" [
            menuItem "architecture-and-conception"
            menuItem "software-craftsmanship-practices"
            menuItem "agile-approaches"
        ]
        menuItem "blog"
        menuItem "contact"
        menuItem "about"
    ]

let translationByLanguage =
    dict [ ("dateFormat", 
            dict [ ("fr", "dddd d MMMM yyyy"); ("en", "dddd, MMMM d, yyyy") ])
           ("continueReading",
            dict [ ("fr", "Lire la suite..."); ("en", "continue reading...") ]) ]
