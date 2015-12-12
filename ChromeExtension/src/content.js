chrome.runtime.onMessage.addListener(
    function (request, sender, sendResponse) {
        if (request.method == "getVideoInfo")
        {
            var providerVideoId;
            var cannonicalUrl = document.URL;
            
            if (cannonicalUrl)
            {
                var index = cannonicalUrl.indexOf("=") + 1;
                providerVideoId = cannonicalUrl.substr(index).split('&')[0];
            }
            
            sendResponse({
                providerId: 1,
                providerVideoId: providerVideoId,
                url: cannonicalUrl,
            });
        }
    }
);