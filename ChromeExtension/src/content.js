chrome.runtime.onMessage.addListener(
    function (request, sender, sendResponse) {
        if (request.method == "getVideoInfo")
        {
            var videoId;
            var canonicalUrl = document.URL;
            
            if (canonicalUrl)
            {
                var index = canonicalUrl.indexOf("=") + 1;
                videoId = canonicalUrl.substr(index).split('&')[0];
            }
            
            var videoNameNode = document.getElementById("eow-title");
            var videoName;
            if (videoNameNode)
            {
                videoName = videoNameNode.innerText;
            }
            
            sendResponse({
                videoName: videoName,
                videoId: videoId
            });
        }
    }
);