using System.Collections.Generic;

namespace WcsVideos.Contracts
{
    public static class SkillLevel
    {
        public static readonly Dictionary<string, string> DisplayNames = new Dictionary<string, string>
        {
            { SkillLevel.Masters, "Masters" },
            { SkillLevel.Sophisticated, "Sophisticated" },
            { SkillLevel.Juniors, "Juniors" },
            { SkillLevel.Newcomer, "Newcomer" },
            { SkillLevel.Novice, "Novice" },
            { SkillLevel.Intermediate, "Intermediate" },
            { SkillLevel.Advanced, "Advance" },
            { SkillLevel.Allstar, "Allstar" },
            { SkillLevel.ChampionInvitational, "Champion / Invitational" },
            { SkillLevel.Open, "Open" },
        };
        
        public static string GetValidatedSkillLevel(string skillLevelId)
        {
            if (skillLevelId == null || !SkillLevel.DisplayNames.ContainsKey(skillLevelId))
            {
                return null;
            }
            
            return skillLevelId;
        }
        
        public static string GetSkillLevelDisplayName(string skillLevelId)
        {
            string ret;
            if (string.IsNullOrEmpty(skillLevelId) || !SkillLevel.DisplayNames.TryGetValue(skillLevelId, out ret))
            {
                ret = "(None)";
            }
            
            return ret;
        }
        
        public const string Masters = "MASTERS";
        public const string Sophisticated = "SOPHISTICATED";
        public const string Juniors = "JUNIORS";
        public const string Newcomer = "NEWCOMER";
        public const string Novice = "NOVICE";
        public const string Intermediate = "INTERMEDIATE";
        public const string Advanced = "ADVANCED";
        public const string Allstar = "ALLSTAR";
        public const string ChampionInvitational = "CHAMPION_INVITATIONAL";
        public const string Open = "OPEN";
    }
}
