# Features

* Generate static website from Markdown & F# Script files as content.
* Blog posts & single pages "article model" => metadata can be from file name, folder structure, or "encoded" file content
    * Title
    * Date
    * Abstract (=> separator in file) VS extracted first words used in listing => standalone abstract 
    * URL from file name
    * Lang given by first level dir in sources
    * Type of article: blog post, page... => from second level dir in sources
    * Tags to classify articles
    * UniqueKey
* Configurable menu/navbar and localized strings through config.fsx file
* Layouts localized : 1 for blog post, 1 for single pages
* Listing pages localized : blog home + by tag, by date => through grouping functions on metadata
* Manage several language for a same site :
    * Switch from one lang to another if several articles with same UniqueKey exist
* Deploy to gh-pages branch automatically (or master if [username].github.io repository)

# Features TODO

* RSS flux
* SEO: no duplicate content, canonical url, nice URLs, XML sitemap
