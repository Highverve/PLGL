using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Deconstruct
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
        public List<(string[], string)> Lexemes { get; private set; } = new List<(string[], string)>();
        public Dictionary<string, string> Inflections { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Roots { get; private set; } = new Dictionary<string, string>();

        public List<Affix> Affixes { get; private set; } = new List<Affix>();

        public List<Affix> GetPrefixes(string word)
        {
            List<Affix> results = new List<Affix>();
            //Double-check: If it's not from longest to shortest: ordered.Reverse();
            List<Affix> prefixes = Affixes.Where<Affix>(s => s.Affixation == Affix.AffixType.Prefix)
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
            List<Affix> suffixes = Affixes.Where<Affix>(s => s.Affixation == Affix.AffixType.Suffix)
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
    public class Affix
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public enum AffixType { Prefix, Suffix }
        public enum LocationType { Start, End }
        public int Order { get; set; }
        public bool IsGenerated { get; set; } = false;

        /// <summary>
        /// What type of affix is it? This tells the generator where to look for the affix.
        /// </summary>
        public AffixType Affixation { get; set; }
        /// <summary>
        /// This tells the generator where the Value should be added at the end of the generation process.
        /// </summary>
        public LocationType Location { get; set; }

        public Affix(string Key, string Value, AffixType Affixation, LocationType Location, int Order)
        {
            this.Key = Key;
            this.Value = Value;
            this.Affixation = Affixation;
            this.Location = Location;
            this.Order = Order;
        }
        /// <summary>
        /// With this constructor, the affix's value will be procedurally generated, like a regular word.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Affixation"></param>
        /// <param name="Location"></param>
        public Affix(string Key, AffixType Affixation, LocationType Location, int Order) : this(Key, "", Affixation, Location, Order)
        {
            IsGenerated = true;
        }
    }
}
