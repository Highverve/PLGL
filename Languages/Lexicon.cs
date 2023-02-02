using PLGL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Languages
{
    /// <summary>
    /// A constraining class, that aims to answer a tricky question: "How do we handle lexemes?"
    /// 
    /// Scenario:
    ///     1. The generator is processing a sentence, when a word ("running") matches a Dictionary key.
    ///     2. The usual generation process is circumvented, and the word undergoes a strictly defined process.
    ///     3. The root word is split from it's modifying suffix ("runn" and "ing").
    ///     4. The word ("runn", or "run" if double consonant is removed) is processed through the generator.
    ///     5. An affix and/or punctuation mark is added, according to the style preferences of the language author.
    /// 
    /// If the root word has multiple affixes (on either end), the process loops through each one and
    /// applies the style preference accordingly.
    /// </summary>
    public class Lexicon
    {
        //public List<(string[], string)> Lexemes { get; private set; } = new List<(string[], string)>();
        public Dictionary<string, string> Inflections { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Roots { get; private set; } = new Dictionary<string, string>();

        public List<Affix> Affixes { get; private set; } = new List<Affix>();

        public List<Affix> GetPrefixes(string word)
        {
            List<Affix> results = new List<Affix>();
            //Double-check: If it's not from longest to shortest: ordered.Reverse();
            List<Affix> prefixes = Affixes.Where(s => s.KeyLocation == Affix.AffixLocation.Prefix)
                                   .OrderBy(s1 => s1.Key.Length).ToList();
            prefixes.Reverse();

            for (int i = 0; i < prefixes.Count; i++)
            {
                if (word.ToLower().StartsWith(prefixes[i].Key.ToLower()))
                {
                    results.Add(prefixes[i]);
                    word = word.Remove(0, prefixes[i].Key.Length - 1); //-1? or no
                    i = 0; //Restart loop.
                }
            }
            return results;
        }
        public List<Affix> GetSuffixes(string word)
        {
            List<Affix> results = new List<Affix>();
            //Double-check: If it's not from longest to shortest: ordered.Reverse();
            List<Affix> suffixes = Affixes.Where(s => s.KeyLocation == Affix.AffixLocation.Suffix)
                                   .OrderBy(s1 => s1.Key.Length).ToList();
            suffixes.Reverse();

            for (int i = 0; i < suffixes.Count; i++)
            {
                if (word.ToLower().EndsWith(suffixes[i].Key.ToLower()))
                {
                    results.Add(suffixes[i]);
                    word = word.Remove(word.Length - suffixes[i].Key.Length, suffixes[i].Key.Length); //-1? or no
                    i = -1; //Restart loop.
                }
            }
            return results;
        }
    }
}
