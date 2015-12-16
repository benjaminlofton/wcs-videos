chrome.runtime.onMessage.addListener(
    function (request, sender, sendResponse) {
        if (request.method == "getVideoInfo")
        {
            var providerVideoId;
            var cannonicalUrl = document.URL;
            var index;
            
            if (cannonicalUrl)
            {
                if (cannonicalUrl.indexOf("www.youtube.com/") > -1)
                {
                    index = cannonicalUrl.indexOf("=") + 1;
                    providerVideoId = cannonicalUrl.substr(index).split('&')[0];
                }
                else if (cannonicalUrl.indexOf("vimeo.com/") > -1)
                {
                    index = cannonicalUrl.indexOf("vimeo.com/") + "vimeo.com/".length;
                    providerVideoId = cannonicalUrl.substring(index);
                }
            }
            
            sendResponse({
                providerId: 1,
                providerVideoId: providerVideoId,
                url: cannonicalUrl,
            });
        }
    }
);