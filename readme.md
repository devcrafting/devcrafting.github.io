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
    * Header with navbar
    * Footer
* Listing pages localized : blog home (limit nb of posts) + by tag, by date (archives) => through grouping functions on metadata
* Manage several language for a same site :
    * Switch from one lang to another if several articles with same UniqueKey exist
* Deploy to gh-pages branch automatically (or master if [username].github.io repository)
* 404 page
* Pretty code formatting based on FSharp.Formatting 
* SEO: avoid duplicate content, canonical url, nice URLs, meta title/description
    * Microformats : itemtype, itemscope, itemprop (http://www.alsacreations.com/article/lire/1509-microdata-microformats-schema-semantique.html)

# Features TODO

* SEO: avoid duplicate content, canonical url, nice URLs, meta title/description
    * Old content migration and URL conservation guidelines (redirect 301, http-equiv="refresh", rel="canonical"...)
    * http://konradpodgorski.com/blog/2013/10/21/how-i-migrated-my-blog-from-wordpress-to-octopress/#redirect-301-on-github-pages
    * http://www.davidsottimano.com/cross-domain-canonicals-blogspot-blog/
    * Google Webmaster Tools guidelines
    * XML sitemap
* Google Analytics + cookies warning
* Comments integrated with Disqus (make it configurable)
* Links to share articles on Twitter or Linked In
* Links checker : detect links that does not return HTTP Code 200
* RSS flux
* Tag cloud canvas : http://www.goat1000.com/tagcanvas-shapes.php
* Author ? Displayed on posts + page by authors
* Custom theme: layout, content files (css, js, images)

# Refactoring

* Put more code from build.fsx to tools dir
* Tags and archives are mostly the same : a listing layout and a grouping function 
* Avoid overflow on code blocks (really awful in mobile view)
