using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data.Elements
{
    /// <summary>
    /// A class used by generation to fill with word information when deconstructing a word.
    /// This class is particularly useful to pass to generators for additional context to their work.
    /// </summary>
    public class WordInfo
    {
        public string WordActual { get; set; } = string.Empty;
        public string WordRoot { get; set; } = string.Empty;

        public char[] Punctuation { get; set; } = new char[0];
        public char[] Flags { get; set; } = new char[0];
        public string[] Prefixes { get; set; } = new string[0];
        public string[] Suffixes { get; set; } = new string[0];

        public string WordGenerated { get; set; } = string.Empty;

        /// <summary>
        /// Set to true if the word has the SkipGeneration flag.
        /// </summary>
        public bool Skip { get; set; }
        /// <summary>
        /// Set to true if the word has the Possessive flag.
        /// </summary>
        public bool Possessive { get; set; }
        /// <summary>
        /// Set automatically by the generator.
        /// </summary>
        public List<SigmaInfo> Syllables { get; set; }

        /// <summary>
        /// This will always be null for the first word in a sentence. Account for it.
        /// </summary>
        public WordInfo? AdjacentLeft { get; set; } = null;
        /// <summary>
        /// This will always be null for the last word in a sentence. Account for it.
        /// </summary>
        public WordInfo? AdjacentRight { get; set; } = null;
    }
}
