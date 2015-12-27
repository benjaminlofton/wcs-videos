using System;
using System.Linq;

namespace WcsVideos.Providers.AutoPopulation
{
    public class WordMatcher
    {
        private static readonly char[] Separators = new char[] { ',', '.', ' ', '!' };
        
        private readonly string text;
        
        public WordMatcher(string text)
        {
            this.text = text;
        }
        
        public bool ContainsWord(string word)
        {
            if (string.IsNullOrEmpty(this.text))
            {
                return false;
            }
            
            int index = this.text.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            
            if (index < 0)
            {
                return false;
            }
            
            char[] characters = this.text.ToCharArray();
            if (index > 0 && !WordMatcher.Separators.Contains(text[index - 1]))
            {
                return false;
            }
            
            if (index + word.Length < this.text.Length &&
                !WordMatcher.Separators.Contains(this.text[index + word.Length]))
            {
                return false;
            }
            
            return true;
        }
    }    
}