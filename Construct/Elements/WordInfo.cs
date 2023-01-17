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
        /// <summary>
        /// Set by the generator after it's been fully processed.
        /// </summary>
        public bool IsProcessed { get; set; }

        public CharacterFilter Filter { get; set; }
        /// <summary>
        /// This will always be null for the first word in a sentence. Account for it.
        /// </summary>
        public WordInfo? AdjacentLeft { get; set; } = null;
        /// <summary>
        /// This will always be null for the last word in a sentence. Account for it.
        /// </summary>
        public WordInfo? AdjacentRight { get; set; } = null;
        /// <summary>
        /// Populated by the generator.
        /// </summary>
        public List<SigmaInfo> SigmaInfo { get; set; } = new List<SigmaInfo>();

        /// <summary>
        /// The character block, as it's been deconstructed from the sentence.
        /// </summary>
        public string WordActual { get; set; } = string.Empty;
        /// <summary>
        /// The word, split of all affixes.
        /// </summary>
        public string WordRoot { get; set; } = string.Empty;
        /// <summary>
        /// The generated word, lexemes unincluded.
        /// </summary>
        public string WordGenerated { get; set; } = string.Empty;
        /// <summary>
        /// The final, procedurally generated word.
        /// </summary>
        public string WordFinal { get; set; } = string.Empty;

        public Affix[] Prefixes { get; set; }
        public Affix[] Suffixes { get; set; }
        public string WordPrefixes { get; set; } = string.Empty;
        public string WordSuffixes { get; set; } = string.Empty;

        public enum CaseType { Lowercase, Capitalize, Uppercase, RandomCase }
        public CaseType Case { get; set; } = CaseType.Lowercase;

        public override string ToString()
        {
            return WordActual;
        }
    }
}
