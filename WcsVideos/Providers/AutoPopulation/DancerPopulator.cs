using WcsVideos.Contracts;
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
            string result = GetDancersFromString(videoDetails.Title);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            
            return GetDancersFromString(videoDetails.Description);
        }
        
        private string GetDancersFromString(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }
            
            WordMatcher matcher = new WordMatcher(title);
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