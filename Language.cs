using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLGL.Data;
using PLGL.Examples;
using PLGL.Languages;

namespace PLGL
{
    public class Language
    {
        /// <summary>
        /// The name of the language.
        /// </summary>
        public string META_Name { get; set; } = string.Empty;
        /// <summary>
        /// The language's nickname. LanguageGenerator sets this value to the Languages<string, Language> Dictionary key.
        /// </summary>
        public string META_Nickname { get; set; } = string.Empty;
        /// <summary>
        /// The description of the language.
        /// </summary>
        public string META_Description { get; set; } = string.Empty;
        /// <summary>
        /// The author of the language.
        /// </summary>
        public string META_Author { get; set; } = string.Empty;

        public LanguageOptions Options { get; set; } = new LanguageOptions();

        public Alphabet Alphabet { get; private set; }
        public Structure Structure { get; private set; }

        public Lexicon Lexicon { get; private set; }
        public Punctuation Punctuation { get; private set; }
        public Flagging Flags { get; private set; }
        public Numbers Numbers { get; private set; }

        public List<CharacterFilter> Filters { get; set; } = new List<CharacterFilter>();
        public void AddFilter(string name, params char[] characters)
        {
            Filters.Add(new CharacterFilter()
            {
                Name = name,
                Characters = characters
            });
        }
        public void AddFilter(string name, string characters)
        {
            Filters.Add(new CharacterFilter()
            {
                Name = name,
                Characters = characters.ToCharArray()
            });
        }

        public OnDeconstruct OnDeconstruct { get; set; }
        public OnConstruct OnConstruct { get; set; }

        public OnSyllableSelect OnSyllableSelection { get; set; }
        public OnLetterSelect OnLetterSelection { get; set; }

        public OnPrefix OnPrefix { get; set; }
        public OnSuffix OnSuffix { get; set; }

        public Language()
        {
            Alphabet = new Alphabet();
            Structure = new Structure(this);

            Lexicon = new Lexicon(this);
            Punctuation = new Punctuation();
            Flags = new Flagging();
            Numbers = new Numbers();
        }
    }
    public class LanguageOptions
    {
        /// <summary>
        /// A global seed used to offset the seed generated for each word. Default is 0.
        /// Increasing or decreasing will change every word of the generated language.
        /// </summary>
        public int SeedOffset { get; set; } = 0;

        /// <summary>
        /// Attempts to match the generated word's case to the original word (lowercase, uppercase, capitalize, etc).
        /// Default is true.
        /// </summary>
        public bool AllowAutomaticCasing { get; set; } = true;
        /// <summary>
        /// If the case of the original word doesn't follow a clear pattern (word, Word, WORD—lowercase, capitalized, uppercase), it will be randomized (wOrD). Default is true.
        /// </summary>
        public bool AllowRandomCase { get; set; } = true;
        /// <summary>
        /// If true, affixes are sorted by the language author's order. Otherwise, sorted to match the order the affixes are read. Default is false.
        /// </summary>
        public bool AffixCustomOrder { get; set; } = false;
        /// <summary>
        /// If true, all inflections are added to the Lexicon class, skipping the generation stage for previously processed words.
        /// </summary>
        public bool MemorizeWords { get; set; } = true;

        public enum PathSelection
        {
            /// <summary>For language author's with a solid grip on their language. This will error if there isn't a next path.</summary>
            Exclusion,
            /// <summary>With no valid path forward, the generator will default to selecting a letter by StartWeight that fits the sigma template.</summary>
            Inclusion,
            /// <summary>The generator will simply end the word. While this is the safest option, too many empy pathways will result in a lot of short words.</summary>
            EndWord
        }
        /// <summary>
        /// Determines how the generator behaves if it encounters a letter path with no way forward, yet has more letters to generate.
        /// </summary>
        public PathSelection Syllables { get; set; } = PathSelection.EndWord;
        public PathSelection LetterPathing { get; set; } = PathSelection.EndWord;

        public LanguageOptions()
        {
            SyllableSkewMin = (count) => { return 0.8; };
            SyllableSkewMax = (count
                ) => { return 1.2; };
        }

        #region Syllable counting
        /// <summary>
        /// All lowercase consonants in your input language. This is for estimating syllable count, and defaults to english.
        /// </summary>
        public char[] InputConsonants { get; set; } = "bcdfghjklmnpqrstvwxyz".ToArray();
        /// <summary>
        /// All lowercase vowels in your input language. This is for estimating syllable count, and defaults to english.
        /// </summary>
        public char[] InputVowels { get; set; } = "aeiou".ToArray();
        /// <summary>
        /// Tells the generator how much lower the word's syllable count could be from the syllable estimate. Default is 0.8
        /// </summary>
        public Func<int, double> SyllableSkewMin { get; set; }
        /// <summary>
        /// Tells the generator how much higher the word's syllable count could be from the syllable estimate. Default is 1.2.
        /// </summary>
        public Func<int, double> SyllableSkewMax { get; set; }

        /// <summary>
        /// Sets how the generator counts a root's syllables. Default is EnglishSigmaCount (C/V border checking).
        /// </summary>
        public Func<string, int> CountSyllables { get; set; }

        /// <summary>
        /// Roughly estimates a word's syllables. It transforms the word into c/v, and counts where a consonant shares a border with a vowel. Only misses where a consonant could also be a vowel (such as "y")).
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int EnglishSyllableCount(string word)
        {
            if (word.ToLower().EndsWith("es"))
                word = word.Substring(0, word.Length - 2);
            if (word.ToLower().EndsWith("ed"))
                word = word.Substring(0, word.Length - 2);

            string cv = string.Empty;

            foreach (char c in word)
            {
                if (InputConsonants.Contains(c)) cv += 'c';
                if (InputVowels.Contains(c)) cv += 'v';
            }

            int result = 0;
            while (cv.Length > 1)
            {

                //Check for consonant-vowel border.
                if (cv[0] == 'c' && cv[1] == 'v' ||
                    cv[0] == 'v' && cv[1] == 'c')
                {
                    result++;
                    cv = cv.Remove(0, Math.Min(cv.Length, 2));
                }

                if (cv.Length <= 1)
                    break;

                //If double consonant or vowel, remove one.
                if (cv[0] == 'c' && cv[1] == 'c' ||
                    cv[0] == 'v' && cv[1] == 'v')
                {
                    cv = cv.Remove(0, 1);
                }
            }

            if (word.Length > 0 && result == 0)
                result = 1;

            return result;
        }
        /// <summary>
        /// Counts each valid character found in Language.Options.InputConsonants and InputVowels. More ideal for languages where each letter may be a syllable.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int CharacterSyllableCount(string word)
        {
            int result = 0;

            foreach (char c in word)
            {
                if (InputConsonants.Contains(c)) result++;
                if (InputVowels.Contains(c)) result++;
            }

            return result;
        }
        #endregion
    }
}
