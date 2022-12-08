using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data.Elements
{
    /// <summary>
    /// A class used by generation to fill with syllable information when constructing a word.
    /// This class is particularly useful to pass to generators for additional context to their work.
    /// </summary>
    public class SyllableInfo
    {
        public string Syllable { get; set; } = string.Empty;
        /// <summary>
        /// This reference is set by the word generator. It should never be null.
        /// </summary>
        public Form? Form { get; set; } = null;

        /// <summary>
        /// This will always be null for the first word in a sentence. Account for it.
        /// </summary>
        public SyllableInfo? AdjacentLeft { get; set; } = null;
        /// <summary>
        /// This will always be null for the last word in a sentence. Account for it.
        /// </summary>
        public SyllableInfo? AdjacentRight { get; set; } = null;
    }
}
