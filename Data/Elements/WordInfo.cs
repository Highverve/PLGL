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
        /// <summary>
        /// The word, as it's just been split from the string.
        /// </summary>
        public string WordActual { get; set; } = string.Empty;
        /// <summary>
        /// The word, split of all punctuation.
        /// </summary>
        public string WordStripped { get; set; } = string.Empty;
        /// <summary>
        /// The word, split of all affixes.
        /// </summary>
        public string WordRoot { get; set; } = string.Empty;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// The word's stored punctuation.
        /// </summary>
        public char[] Punctuation { get; set; }
        /// <summary>
        /// The word's flags. Processed by Flagging.
        /// </summary>
        public char[] Flags { get; set; }
        /// <summary>
        /// The word's prefixes. Processed by Lexemes.
        /// </summary>
        public Affix[] Prefixes { get; set; }
        /// <summary>
        /// The word's suffixes. Processed by Lexemes.
        /// </summary>
        public Affix[] Suffixes { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string WordPrefixes { get; set; } = string.Empty;
        public string WordSuffixes { get; set; } = string.Empty;
        /// <summary>
        /// The generated word. Lexemes, punctuation, and flagging unincluded.
        /// </summary>
        public string WordGenerated { get; set; } = string.Empty;
        /// <summary>
        /// The final, procedurally generated word.
        /// </summary>
        public string WordFinal { get; set; } = string.Empty;

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
