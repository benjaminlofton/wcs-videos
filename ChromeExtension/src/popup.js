/* global chrome */
var host = "localhost:5000";

document.addEventListener('DOMContentLoaded', function() {
    chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
        chrome.tabs.executeScript(tabs[0].id, { file: "content.js" }, function (result) {
            chrome.tabs.sendMessage(tabs[0].id, {method: "getVideoInfo"}, function (response) {
                onReceiveResponse(tabs[0], response);
            });              
        });
    });
});


function onReceiveResponse(tab, response)
{
    var statusElement = document.getElementById("status");
    if (response.videoId && response.videoName)
    {
        statusElement.textContent = "Checking to see if video exists...";
        statusElement.className = "";
        var xhr = new XMLHttpRequest();
        var url = "http://" + host + "/Videos/CheckVideo?providerVideoId=" + response.videoId;
        xhr.open("GET", url);
        xhr.onreadystatechange = function() {
            if (xhr.readyState == 4)
            {
                if (xhr.status == 200)
                {
                    var result = JSON.parse(xhr.responseText);
                    if (result.exists)
                    {
                        statusElement.textContent = "Oops, this video already exists!";
                        statusElement.className = "error";
                    }
                    else
                    {
                        statusElement.textContent = "Taking you there!"
                        statusElement.className = "success";
                        
                        var url = "http://" + host + "/Videos/Add" +
                            "?providerVideoId=" + encodeURIComponent(response.videoId) +
                            "&title=" + encodeURIComponent(response.videoName);
                        
                        chrome.tabs.create({
                            url: url,
                            index: tab.index + 1,
                            active: true
                        });
                    }
                }
                else
                {
                    statusElement.textContent = "Oops, unable to communicate with the website!";
                    statusElement.className = "error";
                }
            }
        }
        
        xhr.send();
    }
    else
    {
        statusElement.textContent = "Oops, this page doesn't contain a video!";
        statusElement.className = "error";
    }
}