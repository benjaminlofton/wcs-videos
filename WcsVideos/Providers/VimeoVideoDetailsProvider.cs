using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace WcsVideos.Providers
{
    public class VimeoVideoDetailsProvider : IVideoDetailsProvider
    {
        private const string EmbedUrlTemplate = "api/oembed.json?url={0}";
        private const string BaseUrl = "https://vimeo.com/";
        
        private readonly string url;
        
        public VimeoVideoDetailsProvider(Uri url)
        {
            this.url = url.AbsoluteUri.ToString();
        }
        
        public VideoDetails GetVideoDetails()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(VimeoVideoDetailsProvider.BaseUrl);
                string relativeUrl = string.Format(
                    VimeoVideoDetailsProvider.EmbedUrlTemplate,
                    this.url);

                HttpResponseMessage response = client.GetAsync(relativeUrl).Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                
                string serialized = response.Content.ReadAsStringAsync().Result;
  
                JObject parsed = JObject.Parse(serialized);

                string title = (string)parsed["title"];
                string providerVideoId = (string)parsed["video_id"];
                string description = (string)parsed["description"];
                
                return new VideoDetails
                {
                    ProviderVideoId = providerVideoId,
                    ProviderId = 2,
                    Title = title,
                    Description = description,    
                };
            }
        }
    }
}