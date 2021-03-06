using WcsVideos.Contracts;

namespace WcsVideos.Providers.AutoPopulation
{
    public static class DanceCategoryPopulator
    {
        public static string GetDanceCategory(VideoDetails details)
        {
            if (details == null)
            {
                return string.Empty;
            }
            
            string result = GetCategoryFromString(details.Title);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            
            return GetCategoryFromString(details.Description);
        }
        
        private static string GetCategoryFromString(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }
            
            WordMatcher matcher = new WordMatcher(title);
            if (matcher.ContainsWord("Demo") ||
                matcher.ContainsWord("Pro Show"))
            {
                return DanceCategory.Demo;
            }
            else if(matcher.ContainsWord("Showcase"))
            {
                return DanceCategory.Showcase;
            }
            else if (matcher.ContainsWord("Rising Star") ||
                matcher.ContainsWord("Rising-Star") ||
                matcher.ContainsWord("RisingStar"))
            {
                return DanceCategory.RisingStar;
            }
            else if (matcher.ContainsWord("Three for all") ||
                matcher.ContainsWord("three-for-all"))
            {
                return DanceCategory.ThreeForAll;
            }
            else if (matcher.ContainsWord("Jill And Jack") ||
                matcher.ContainsWord("jill-and-jack") ||
                matcher.ContainsWord("Jill & Jack"))
            {
                return DanceCategory.JillAndJack;
            }
            else if (matcher.ContainsWord("Strictly") ||
                matcher.ContainsWord("SS"))
            {
                return DanceCategory.Strictly;
            }
            else if (matcher.ContainsWord("JnJ") ||
                matcher.ContainsWord("Jack and Jill") ||
                matcher.ContainsWord("JJ") ||
                matcher.ContainsWord("Jack & Jill") ||
                matcher.ContainsWord("J&J"))
            {
                return DanceCategory.JackAndJill;
            }
            else if (matcher.ContainsWord("Classic"))
            {
                return DanceCategory.Classic;
            }
            else if (matcher.ContainsWord("Routine") ||
                matcher.ContainsWord("Routines"))
            {
                return DanceCategory.Routine;
            }
            else if (matcher.ContainsWord("Social") ||
                matcher.ContainsWord("late night"))
            {
                return DanceCategory.Social;
            }
            else
            {
                return string.Empty;
            }
        }
    }    
}