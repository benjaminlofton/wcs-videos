chrome.runtime.onMessage.addListener(
    function (request, sender, sendResponse) {
        if (request.method == "getVideoInfo")
        {
            var links = document.getElementsByTagName("link");
            var canonicalUrl;
            var videoId;
            
            for (var i = 0; i < links.length; i++)
            {
                if (links[i].rel == "canonical")
                {
                    canonicalUrl = links[i].href;
                }
            }
            
            if (canonicalUrl)
            {
                var index = canonicalUrl.lastIndexOf("=") + 1;
                videoId = canonicalUrl.substr(index);
            }
            
            var videoName = document.getElementById("eow-title").innerText;
            
            sendResponse({
                videoName: videoName,
                videoId: videoId
            });
        }
    }
);