module Localization

open System
open System.Collections.Generic

let getTranslations languages (translationByLanguage : IDictionary<string, IDictionary<string, string>>) warn = 
    let translations = new Dictionary<string, Dictionary<string, obj>>()
    languages
    |> Seq.iter (fun lang -> 
        let translationsForLang = new Dictionary<string, obj>()
        translations.Add(lang, translationsForLang)
        if not (String.IsNullOrWhiteSpace(lang)) then
            translationByLanguage
            |> Seq.iter (fun kvp ->
                if kvp.Value.ContainsKey(lang) then
                    translationsForLang.Add(kvp.Key, kvp.Value.[lang])
                else
                    warn (sprintf "Translation missing for %s in %s" kvp.Key lang))
    )
    translations
