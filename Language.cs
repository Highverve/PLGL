using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLGL.Construct.Elements;
using PLGL.Languages;

namespace PLGL
{
    /// <summary>
    /// Language showcasing:
    ///     - Fauxglish. A language designed to mimic the patterns of English.
    ///     - Elvish.
    ///     - Orcish.
    ///     - Singing. A silly language. Uses L, D, A, E, O, no double consonants, yet triple vowel potential.
    /// </summary>

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

        public OnDeconstruct Deconstruct { get; set; }
        public OnConstruct Construct { get; set; }
        public OnGenerate Generate { get; set; }
        public OnPrefix OnPrefix { get; set; }
        public OnSuffix OnSuffix { get; set; }

        public Language()
        {
            Alphabet = new Alphabet();
            Structure = new Structure();

            Lexicon = new Lexicon();
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
        /// If true, all inflections are added to the Lexicon class, skipping the generation stage for previously processed words.
        /// </summary>
        public bool MemorizeWords { get; set; } = true;

        public enum LetterPathing
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
        public LetterPathing Pathing { get; set; } = LetterPathing.EndWord;

        /// <summary>
        /// All lowercase consonants in your input language. This is for estimating syllable count, and defaults to english.
        /// </summary>
        public char[] InputConsonants { get; set; } = "bcdfghjklmnpqrstvwxyz".ToArray();
        /// <summary>
        /// All lowercase vowels in your input language. This is for estimating syllable count, and defaults to english.
        /// </summary>
        public char[] InputVowels { get; set; } = "aeiou".ToArray();
        /// <summary>
        /// Tells the generator how much lower the word's syllable count could be from the syllable estimate. Default is 0.6
        /// </summary>
        public double SigmaSkewMin { get; set; } = 0.6;
        /// <summary>
        /// Tells the generator how much higher the word's syllable count could be from the syllable estimate. Default is 1.2.
        /// </summary>
        public double SigmaSkewMax { get; set; } = 1.2;
    }
}
