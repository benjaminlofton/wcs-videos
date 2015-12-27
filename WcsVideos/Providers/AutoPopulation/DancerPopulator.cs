using WcsVideos.Contracts;
using WcsVideos.Providers;
using System.Collections.Generic;

namespace WcsVideos.Providers.AutoPopulation
{
    public class DancerPopulator
    {
        private IDataAccess dataAccess;
        public DancerPopulator(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }
        
        public string GetDancers(VideoDetails videoDetails)
        {
            if (string.IsNullOrEmpty(videoDetails.Title))
            {
                return null;
            }
            
            WordMatcher matcher = new WordMatcher(videoDetails.Title);
            List<string> ids = new List<string>();
            
            foreach (Dancer dancer in this.dataAccess.GetAllDancers())
            {
                string[] names = dancer.Name.Split(',');
                if (names.Length == 2)
                {
                    string name = names[1].Trim() + " " + names[0].Trim();
                    
                    if (matcher.ContainsWord(name))
                    {
                        ids.Add(dancer.WsdcId);
                    }
                }
            }
            
            if (ids.Count <= 2)
            {
                return string.Join(";", ids);
            }
            
            return null;
        }
    }
}