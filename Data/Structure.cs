using LanguageReimaginer.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    /// <summary>
    /// Tells the generator how to structure a word, starting with the syllable (sigma), and the valid lettrs inside those sigma.
    /// </summary>
    public class Structure
    {
        //1. Merge with pathways?!
        //2. Either create methods for adding whole sigmas, or methods for generating max lengths of each block type.
        //   Both of these will use some form of weight distribution to decide commonality of blocks in syllables, and syllables in words.

        // Generator input factors: expected syllable count
        //
        // Set sigma factors: (List) int length, SigmaFactor class
        // Length selection potential multiplying factors:
        //      - Expected syllable count of the word.
        //      - First/last syllable length skew. 1.0 would be no change, whereas .5 would skew the generator to choose a
        //        shorter initial syllable template. E.g, 1.5 could be "CCCVC" compared to .5 "CVC", or "VC".
        //      - Previous/next syllable length skew. Higher value means better chance of the syllable to be opposite length compared to it's predecessor.
        //        In a 1.5 skew, this would make the word follow a long-short-long-short pattern, whereas with 0.5 the word's syllable lengths are likely to be similar.
        //      - Increasing/decreasing length skew. A higher value means the next syllable is more likely to be longer than the last (trending upward).
        //      - So if a word starts with a short syllable, it could end with a longer one. If it starts with a long syllable, the following syllables may stay as long.
        //      -
        // Letter formation will follow similar rules, especially taking syllable position into account.

        /// <summary>
        /// The possible syllable pattern that the generator may choose.
        /// </summary>
        public List<Sigma> SigmaTemplates { get; private set; } = new List<Sigma>();

        /// <summary>
        /// Any of these strings can be empty without issue (except the nucleus).
        /// </summary>
        /// <param name="onset"></param>
        /// <param name="nucleus"></param>
        /// <param name="coda"></param>
        /// <param name="weights"></param>
        public void AddSigma(string onset, string nucleus, string coda, SigmaWeight weights) { SigmaTemplates.Add(new Sigma(onset, nucleus, coda) { Weight = weights }); }
        public void AddSigmaVC(string nucleus, string coda, SigmaWeight weights) { SigmaTemplates.Add(new Sigma(string.Empty, nucleus, coda) { Weight = weights }); }
        public void AddSigmaCV(string onset, string nucleus, SigmaWeight weights) { SigmaTemplates.Add(new Sigma(onset, nucleus, string.Empty) { Weight = weights }); }
        public void AddSigmaV(string nucleus, SigmaWeight weights) { SigmaTemplates.Add(new Sigma(string.Empty, nucleus, string.Empty) { Weight = weights }); }

        public List<LetterPath> Pathways { get; private set; } = new List<LetterPath>();
        /// <summary>
        /// Returns the possible paths based on the generator's current word position, sigma position, and the last letter generated.
        /// Final candidate is selected by distributed weight in the generator's method.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="wordPos"></param>
        /// <param name="sigmaPos"></param>
        /// <returns></returns>
        public LetterPath[] GetPotentialPaths(Letter previous, WordPosition wordPos, SigmaPosition sigmaPos)
        {
            return Pathways.Where<LetterPath>((l) => (l.WordPosition == wordPos || l.WordPosition == WordPosition.Any))
                .Where<LetterPath>((l) => (l.SigmaPosition == sigmaPos || l.SigmaPosition == SigmaPosition.Any))
                .Where<LetterPath>(l => l.Operator == previous).ToArray();
        }
    }

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
    /// chance of the generator will select another 's'.
    /// 
    /// Weight is always relative to the letter's other paths.
    ///     
    /// </summary>
    public class LetterPath
    {
        /// <summary>
        /// The current generated letter. This tells the generator the next letter it would prefer.
        /// </summary>
        public Letter Operator { get; set; }
        /// <summary>
        /// The next potential letter.
        /// </summary>
        public Letter[] Next { get; set; }

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

        /// <summary>
        /// The likelihood of this letter occurring, relative to the other weighted letters under this rule by this letter.
        /// </summary>
        public double Weight { get; set; } = 1.0;
    }
}
