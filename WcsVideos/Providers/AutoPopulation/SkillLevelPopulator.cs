using WcsVideos.Contracts;

namespace WcsVideos.Providers.AutoPopulation
{
    public static class SkillLevelPopulator
    {
        public static string GetSkillLevel(VideoDetails details)
        {
            string result = GetSkillLevelFromString(details.Title);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            
            return GetSkillLevelFromString(details.Description);
        }
        
        private static string GetSkillLevelFromString(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }
            
            WordMatcher matcher = new WordMatcher(title);
            if (matcher.ContainsWord("Masters"))
            {
                return SkillLevel.Masters;
            }
            else if (matcher.ContainsWord("Sophisticated"))
            {
                return SkillLevel.Sophisticated;
            }
            else if (matcher.ContainsWord("Juniors") ||
                matcher.ContainsWord("Young American") ||
                matcher.ContainsWord("Young Adult"))
            {
                return SkillLevel.Juniors;
            }
            else if (matcher.ContainsWord("Pro-am") || matcher.ContainsWord("Pro Am") || matcher.ContainsWord("ProAm"))
            {
                return SkillLevel.ProAm;
            }
            else if (matcher.ContainsWord("Intermediate") ||
                matcher.ContainsWord("int"))
            {
                return SkillLevel.Intermediate;
            }
            else if (matcher.ContainsWord("Novice") ||
                matcher.ContainsWord("nov"))
            {
                return SkillLevel.Novice;
            }
            else if(matcher.ContainsWord("Newcomer"))
            {
                return SkillLevel.Newcomer;
            }
            else if (matcher.ContainsWord("Advanced") || matcher.ContainsWord("Adv"))
            {
                return SkillLevel.Advanced;
            }
            else if (matcher.ContainsWord("Allstar") ||
                matcher.ContainsWord("All-Star") ||
                matcher.ContainsWord("All Star"))
            {
                return SkillLevel.Allstar;
            }
            else if (matcher.ContainsWord("rising star") ||
                matcher.ContainsWord("showcase") ||
                matcher.ContainsWord("classic") ||
                matcher.ContainsWord("cabaret") ||
                matcher.ContainsWord("routine") ||
                matcher.ContainsWord("All-American") ||
                matcher.ContainsWord("All American") ||
                matcher.ContainsWord("All-Canadian") ||
                matcher.ContainsWord("All Canadian"))
            {
                return SkillLevel.Open;
            }
            else if (matcher.ContainsWord("Champions") ||
                matcher.ContainsWord("Champion") ||
                matcher.ContainsWord("Invitational") ||
                matcher.ContainsWord("Invitation") ||
                matcher.ContainsWord("Inspirational") ||
                matcher.ContainsWord("Demo") ||
                matcher.ContainsWord("Pro") ||
                matcher.ContainsWord("Champ") ||
                matcher.ContainsWord("Champs"))
            {
                return SkillLevel.ChampionInvitational;
            }
            else if (matcher.ContainsWord("Open"))
            {
                return SkillLevel.Open;
            }
            else
            {
                return string.Empty;
            }
        }
    }    
}