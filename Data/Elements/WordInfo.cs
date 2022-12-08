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
        public string WordGenerated { get; set; } = string.Empty;
        public string Delimiter { get; set; } = string.Empty;

        public List<string> Prefixes { get; set; }
        public List<string> Suffixes { get; set; }

        /// <summary>
        /// Set automatically by the generator.
        /// </summary>
        public List<SyllableInfo> Syllables { get; set; }

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
