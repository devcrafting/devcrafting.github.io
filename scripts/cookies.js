var tagAnalyticsCNIL = {}

tagAnalyticsCNIL.CookieConsentBuilder = function (gaProperty) {
    var disableStr = 'ga-disable-' + gaProperty;
    var firstCall = false;

    function getCookieExpireDate() {
        var cookieTimeout = 33696000000; // 13 months of 30 days each
        var date = new Date();
        date.setTime(date.getTime() + cookieTimeout);
        var expires = "; expires=" + date.toGMTString();
        return expires;
    }

    function showBanner() {
        var div = document.getElementById("cookie-banner");
        div.style.display = "";
    }

    function getCookie(nameOfCookie) {
        if (document.cookie.length > 0) {
            begin = document.cookie.indexOf(nameOfCookie + "=");
            if (begin != -1) {
                begin += nameOfCookie.length + 1;
                end = document.cookie.indexOf(";", begin);
                if (end == -1) end = document.cookie.length;
                return unescape(document.cookie.substring(begin, end));
            }
        }
        return null;
    }

    function notToTrack() {
        if ((navigator.doNotTrack && (navigator.doNotTrack == 'yes' || navigator.doNotTrack == '1'))
            || (navigator.msDoNotTrack && navigator.msDoNotTrack == '1')) {
            return true;
        }
        return false;
    }

    function isTrackAccepted() {
        if (navigator.doNotTrack && (navigator.doNotTrack == 'no' || navigator.doNotTrack == 0)) {
            return true;
        }
        return false;
    }

    function deleteCookie(name) {
        var path = ";path=" + "/";
        var hostname = document.location.hostname;
        if (hostname.indexOf("www.") === 0)
            hostname = hostname.substring(4);
        var domain = ";domain=" + "." + hostname;
        var expiration = "Thu, 01-Jan-1970 00:00:01 GMT";
        document.cookie = name + "=" + path + domain + ";expires=" + expiration;
    }

    function deleteAnalyticsCookies() {
        var cookieNames = ["__utma", "__utmb", "__utmc", "__utmt", "__utmv", "__utmz", "_ga", "_gat"]
        for (var i = 0; i < cookieNames.length; i++)
            deleteCookie(cookieNames[i])
    }

    function isClickOnOptOut(evt) {
        return (evt.target.parentNode.id == 'cookie-banner'
            || evt.target.parentNode.parentNode.id == 'cookie-banner'
            || evt.target.id == 'optout-button')
    }

    function consent(evt) {
        if (!clickprocessed && !isClickOnOptOut(evt)) {
            evt.preventDefault();
            document.cookie = 'hasConsent=true;' + getCookieExpireDate() + ' ; path=/';
            callGoogleAnalytics();
            clickprocessed = true;
            window.setTimeout(function () { evt.target.click(); }, 1000)
        }
    }

    function callGoogleAnalytics() {
        if (firstCall) return;
        else firstCall = true;

        (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
        (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
        m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
        })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

        ga('create', gaProperty, 'auto');
        ga('send', 'pageview');
    }

    return {   
        gaOptout: function () {
            var div = document.getElementById("aknowledge-optout");
            div.style.display = "";
            var div = document.getElementById("inform-and-consent");
            div.style.display = "none";
            document.cookie = disableStr + '=true;' + getCookieExpireDate() + ' ; path=/';
            document.cookie = 'hasConsent=false;' + getCookieExpireDate() + ' ; path=/';
            window[disableStr] = true;
            clickprocessed = true;
            deleteAnalyticsCookies();
            setTimeout(tagAnalyticsCNIL.CookieConsent.hideInform, 5000);
        },

        showInform: function () {
            var div = document.getElementById("inform-and-ask");
            div.style.display = "";
        },

        hideInform: function () {
            var div = document.getElementById("inform-and-ask");
            div.style.display = "none";
            var div = document.getElementById("cookie-banner");
            div.style.display = "none";
        },

        start: function () {
            var consentCookie = getCookie('hasConsent');
            clickprocessed = false;
            if (consentCookie) {
                if (document.cookie.indexOf('hasConsent=false') > -1)
                    window[disableStr] = true;
                else
                    callGoogleAnalytics();
            } else if (notToTrack()) {
                tagAnalyticsCNIL.CookieConsent.gaOptout()
            } else if (isTrackAccepted()) {
                consent();
            } else {
                // Any action on the site is considered as cookie acceptance 
                if (window.addEventListener) {
                    window.addEventListener("load", showBanner, false);
                    document.addEventListener("click", consent, false);
                } else {
                    window.attachEvent("onload", showBanner);
                    document.attachEvent("onclick", consent);
                }
            }
        }
    }
};
