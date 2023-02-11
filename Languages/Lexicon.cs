using PLGL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Languages
{
    public class Lexicon
    {
        public Dictionary<string, string> Vocabulary { get; set; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        public Dictionary<string, string> Roots { get; set; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        public Dictionary<string, Syllable[]> Syllables { get; set; } = new Dictionary<string, Syllable[]>(StringComparer.InvariantCultureIgnoreCase);

        private Language language;
        public Lexicon(Language language) { this.language = language; }

        /// <summary>
        /// This allows a language author to set the syllable structure of a word, while maintaining the letter population process. Perfect for inflections like "sing", "sang", "sung", or for unappealing words.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="syllables"></param>
        public void AddSyllable(string root, params string[] syllables)
        {
            if (Syllables.ContainsKey(root) == false)
            {
                List<Syllable> result = new List<Syllable>();

                foreach (string s in syllables)
                {
                    if (language.Structure.Syllables.ContainsKey(s))
                        result.Add(language.Structure.Syllables[s]);
                }

                Syllables.Add(root, result.ToArray());
            }
        }
        /// <summary>
        /// This allows the author to set the root of a word, overriding the usual procedural generation, yet keeping any affixes intact.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="word"></param>
        public void AddRoot(string root, string word)
        {
            if (Roots.ContainsKey(root) == false)
                Roots.Add(root, word);
        }
        /// <summary>
        /// This allows the author to set the entire word, overriding procedural generation AND affixes.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddVocabulary(string input, string output)
        {
            if (Vocabulary.ContainsKey(input) == false)
                Vocabulary.Add(input, output);
        }

        public List<Affix> Affixes { get; private set; } = new List<Affix>();
        public List<Affix> GetPrefixes(string word)
        {
            List<Affix> results = new List<Affix>();
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

        public bool ContainsPrefixes(string word)
        {
            List<Affix> suffixes = Affixes.Where(s => s.KeyLocation == Affix.AffixLocation.Suffix)
                                   .OrderBy(s1 => s1.Key.Length).ToList();
            suffixes.Reverse();

            for (int i = 0; i < suffixes.Count; i++)
            {
                if (word.ToLower().EndsWith(suffixes[i].Key.ToLower()))
                    return true;
            }
            return false;
        }
        public bool ContainsSuffixes(string word)
        {
            List<Affix> suffixes = Affixes.Where(s => s.KeyLocation == Affix.AffixLocation.Suffix)
                                   .OrderBy(s1 => s1.Key.Length).ToList();
            suffixes.Reverse();

            for (int i = 0; i < suffixes.Count; i++)
            {
                if (word.ToLower().EndsWith(suffixes[i].Key.ToLower()))
                    return true;
            }
            return false;
        }
    }
}
