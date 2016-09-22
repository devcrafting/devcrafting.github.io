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
