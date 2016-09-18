# Features

* Generate static website from Markdown & F# Script files as content.
* Blog posts & single pages "article model" => metadata can be from file name, folder structure, or "encoded" file content
    * Title
    * Date
    * Abstract (=> separator in file) VS extracted first words used in listing => standalone abstract 
    * URL from file name
    * Lang given by first level dir in sources => make it a function ?
    * Type of article: blog post, page...
    * Tags to classify articles
    * UniqueKey
* Common layout : 1 for blog post and 1 for single pages
* Listing pages : by tag, by date => through grouping functions on metadata
* Manage several language for a same site :
    * Switch from one lang to another if several article with same UniqueKey exist 
    * Tags pages by language
* Deploy to gh-pages branch automatically