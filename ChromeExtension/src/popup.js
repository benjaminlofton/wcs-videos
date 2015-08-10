/* global chrome */
var host = "localhost:5000";

console.log("Extension loaded");
document.addEventListener('DOMContentLoaded', function() {
    console.log("DOM content loaded");
    chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
        console.log("Tab query executed")
        chrome.tabs.executeScript(tabs[0].id, { file: "content.js" }, function (result) {
            console.log("Content script executed");
            chrome.tabs.sendMessage(tabs[0].id, {method: "getVideoInfo"}, function(response) {
                console.log("Response received");
                document.getElementById("videoName").textContent = response.videoName;
                document.getElementById("videoId").textContent = response.videoId;
                var url = "http://" + host + "/Videos/Add" +
                    "?providerVideoId=" + response.videoId +
                    "&title=" + encodeURIComponent(response.videoName);
                
                chrome.tabs.create({
                    url: url,
                    index: tabs[0].index + 1,
                    active: true
                });
            });              
        });
    });
});
