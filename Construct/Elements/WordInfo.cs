using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLGL.Deconstruct;
using PLGL.Construct;

namespace PLGL.Construct.Elements
{
    //public enum Capitalization { First, all }

    /// <summary>
    /// A class used by generation to fill with word information when deconstructing a word.
    /// This class is particularly useful to pass to generators for additional context to their work.
    /// </summary>
    public class WordInfo
    {
        public CharacterFilter Filter { get; set; }

        /// <summary>
        /// The word, as it's just been split from the string.
        /// </summary>
        public string WordActual { get; set; } = string.Empty;
        /// <summary>
        /// The word, split of all affixes.
        /// </summary>
        public string WordRoot { get; set; } = string.Empty;

        /// <summary>
        /// The word's prefixes. Processed by Lexemes.
        /// </summary>
        public Affix[] Prefixes { get; set; }
        /// <summary>
        /// The word's suffixes. Processed by Lexemes.
        /// </summary>
        public Affix[] Suffixes { get; set; }
        public string WordPrefixes { get; set; } = string.Empty;
        public string WordSuffixes { get; set; } = string.Empty;

        /// <summary>
        /// The generated word, lexemes unincluded.
        /// </summary>
        public string WordGenerated { get; set; } = string.Empty;
        /// <summary>
        /// The final, procedurally generated word.
        /// </summary>
        public string WordFinal { get; set; } = string.Empty;

        /// <summary>
        /// Set automatically by the generator.
        /// </summary>
        public List<SigmaInfo> SigmaInfo { get; set; } = new List<SigmaInfo>();

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
