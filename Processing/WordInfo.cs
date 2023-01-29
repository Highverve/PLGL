using PLGL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Processing
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
        public bool IsProcessed { get; set; } = false;

        public CharacterFilter? Filter { get; set; }
        /// <summary>
        /// This will always be null for the first word in a sentence. Account for it.
        /// </summary>
        public WordInfo? AdjacentLeft { get; set; }
        /// <summary>
        /// This will always be null for the last word in a sentence. Account for it.
        /// </summary>
        public WordInfo? AdjacentRight { get; set; }

        public List<SyllableInfo>? Syllables { get; set; }
        public List<LetterInfo>? Letters { get; set; }
        /// <summary>
        /// Populated during deconstruction lexeme discovery.
        /// </summary>
        public List<AffixInfo>? Prefixes { get; set; }
        /// <summary>
        /// Populated during deconstruction lexeme discovery.
        /// </summary>
        public List<AffixInfo>? Suffixes { get; set; }

        /// <summary>
        /// The character block, as it's been deconstructed from the sentence.
        /// </summary>
        public string? WordActual { get; set; }
        /// <summary>
        /// The word, split of all affixes.
        /// </summary>
        public string? WordRoot { get; set; }
        /// <summary>
        /// The generated word, lexemes unincluded.
        /// </summary>
        public string? WordGenerated { get; set; }
        /// <summary>
        /// The final, procedurally generated word.
        /// </summary>
        public string? WordFinal { get; set; }
        public string? WordPrefixes { get; set; }
        public string? WordSuffixes { get; set; }

        public enum CaseType { Lowercase, Capitalize, Uppercase, RandomCase }
        public CaseType Case { get; set; } = CaseType.Lowercase;

        public override string ToString() { return WordActual!; }
    }
}
