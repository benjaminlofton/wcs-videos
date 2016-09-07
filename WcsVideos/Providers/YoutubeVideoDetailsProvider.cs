using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace WcsVideos.Providers
{
    public class YoutubeVideoDetailsProvider : IVideoDetailsProvider
    {
        public static string GoogleApiKey { get; set; } 
        
        private const string SnippetUrlTemplate = "youtube/v3/videos?part=snippet&id={0}&key={1}";
        private const string BaseUrl = "https://www.googleapis.com/";
        
        private readonly string providerVideoId;
        
        public YoutubeVideoDetailsProvider(Uri url)
        {
            if (string.Equals(url.Host, "www.youtube.com", StringComparison.Ordinal))
            {
                string[] parameters = url.Query.TrimStart('?').Split('&');
                
                string videoParameter = parameters.FirstOrDefault(x => x.StartsWith("v="));
                
                if (!string.IsNullOrEmpty(videoParameter))
                {
                    this.providerVideoId = videoParameter.Substring(2);
                }
            }
            else if (string.Equals(url.Host, "youtu.be", StringComparison.Ordinal))
            {
                this.providerVideoId = url.AbsolutePath.TrimStart('/');
            }
        }
        
        public VideoDetails GetVideoDetails()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(YoutubeVideoDetailsProvider.BaseUrl);
                string relativeUrl = string.Format(
                    YoutubeVideoDetailsProvider.SnippetUrlTemplate,
                    this.providerVideoId,
                    YoutubeVideoDetailsProvider.GoogleApiKey);

                HttpResponseMessage response = client.GetAsync(relativeUrl).Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                
                string serialized = response.Content.ReadAsStringAsync().Result;
  
                JObject parsed = JObject.Parse(serialized);
                JArray items = (JArray)parsed["items"];
                
                if (items == null || items.Count == 0)
                {
                    return null;
                }
                
                string title = (string)items[0]["snippet"]["title"];
                string description = (string)items[0]["snippet"]["description"];
                
                return new VideoDetails
                {
                    ProviderVideoId = this.providerVideoId,
                    ProviderId = 1,
                    Title = title,    
                    Description = description,
                };
            }
        }
    }
}