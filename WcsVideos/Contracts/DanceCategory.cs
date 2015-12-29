using System.Collections.Generic;

namespace WcsVideos.Contracts
{
    public static class DanceCategory
    {
        public static readonly Dictionary<string, string> DisplayNames = new Dictionary<string, string>
        {
            { DanceCategory.Classic, "Classic" },
            { DanceCategory.Showcase, "Showcase" },
            { DanceCategory.RisingStar, "Rising Star" },
            { DanceCategory.Routine, "Routine" },
            { DanceCategory.JackAndJill, "Jack and Jill" },
            { DanceCategory.Strictly, "Strictly Swing" },
            { DanceCategory.ThreeForAll, "Three for All" },
            { DanceCategory.JillAndJack, "Jill and Jack" },
            { DanceCategory.Social, "Social" },
            { DanceCategory.Demo, "Demo" },
        };
        
        private static readonly Dictionary<string, int> OrderMap = new Dictionary<string, int>
        {
            { string.Empty, 10 },
            { DanceCategory.Routine, 9 },
            { DanceCategory.RisingStar, 8 },
            { DanceCategory.Showcase, 7 },
            { DanceCategory.Classic, 6 },
            { DanceCategory.ThreeForAll, 5 },
            { DanceCategory.JillAndJack, 4 },
            { DanceCategory.Strictly, 3 },
            { DanceCategory.JackAndJill, 2 },
            { DanceCategory.Social, 1 },
            { DanceCategory.Demo, 0 },
        };
        
        public static string GetValidatedDanceCategory(string danceCategoryId)
        {
            if (string.IsNullOrEmpty(danceCategoryId) || !DanceCategory.DisplayNames.ContainsKey(danceCategoryId))
            {
                return null;
            }
            
            return danceCategoryId;
        }
        
        public static string GetDanceCategoryDisplayName(string danceCategoryId)
        {
            string ret;
            if (string.IsNullOrEmpty(danceCategoryId) || !DanceCategory.DisplayNames.TryGetValue(danceCategoryId, out ret))
            {
                ret = "(None)";
            }
            
            return ret;
        }
        
        public static int GetOrder(string danceCategoryId)
        {
            int ret;
            if (!DanceCategory.OrderMap.TryGetValue(danceCategoryId, out ret))
            {
                ret = DanceCategory.OrderMap[string.Empty];
            }
            
            return ret;
        }
        
        public const string Classic = "CLASSIC";
        public const string Showcase = "SHOWCASE";
        public const string Routine = "ROUTINE";
        public const string RisingStar = "RISING_STAR";
        public const string JackAndJill = "JACK_AND_JILL";
        public const string Strictly = "STRICTLY";
        public const string ThreeForAll = "THREE_FOR_ALL";
        public const string JillAndJack = "JILL_AND_JACK";
        public const string Social = "SOCIAL";
        public const string Demo = "DEMO";
    }
}
