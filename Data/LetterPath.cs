using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public enum WordPosition { First, Middle, Last, Any }
    public enum SigmaPosition { Onset, Nucleus, Medial, Coda, Any }

    /// <summary>
    /// A class that informs the generator the next best letter to select, based on the language author's constraints.
    ///
    /// Scenario: You want the letter 's' to have a good chance to double up only at the end and middle of a word, and never at the beginning.
    ///     1. First, the end: Operator is 's', Next is 's', WordPosition is "Last", Weight is 10.0.
    ///     2. And the middle: Operator is 's', Next is 's', WordPosition is "Middle", Weight is 10.0.
    ///     3. Now, don't set anything for WordPosition at "First". Letter paths are inherently exclusive.
    /// 
    /// So if a middle or ending sigma's onset or coda has a double consonant, and one of the letters selected is 's', there's a good
    /// chance the generator will select another 's'.
    /// 
    /// Weight is always relative to the letter's other paths.
    ///     
    /// </summary>
    public class LetterPath
    {
        /// <summary>
        /// The current generated letter. This tells the generator the next letter it would prefer.
        /// </summary>
        public char Previous { get; set; }
        /// <summary>
        /// The next potential letter.
        /// </summary>
        public List<(char, double)> Next { get; set; } = new List<(char, double)>();

        /// <summary>
        /// Does this path rule apply only to a certain part of the word?
        /// If it applies to the first and last, but not the middle, then simply add two rules.
        /// </summary>
        public WordPosition WordPosition { get; set; } = WordPosition.Any;
        /// <summary>
        /// Does this path rule apply only to a certain part of the syllable?
        /// If it applies to the onset and coda, but not the medial, then simply add two rules.
        /// </summary>
        public SigmaPosition SigmaPosition { get; set; } = SigmaPosition.Any;
    }
}
