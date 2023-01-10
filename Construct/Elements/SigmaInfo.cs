using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Construct.Elements
{
    /// <summary>
    /// A class used by generation to fill with syllable information when constructing a word.
    /// This class is particularly useful to pass to generators for additional context to their work.
    /// </summary>
    public class SigmaInfo
    {
        /// <summary>
        /// This reference is set by the word generator. It should never be null.
        /// </summary>
        public Sigma Sigma { get; set; }
        public string Onset { get; set; } = string.Empty;
        public string Nucleus { get; set; } = string.Empty;
        public string Coda { get; set; } = string.Empty;

        public string Structure() { return Sigma.Structure(); }
        /// <summary>
        /// The position of the sigma in the word. Set by the generator.
        /// </summary>
        public WordPosition Position { get; set; }
        public SigmaBlock Last()
        {
            if (Sigma.Coda != null && Sigma.Coda.Count > 0) return Sigma.Coda;
            if (Sigma.Medial != null && Sigma.Medial.Count > 0) return Sigma.Medial;
            if (Sigma.Nucleus != null && Sigma.Nucleus.Count > 0) return Sigma.Nucleus;
            if (Sigma.Onset != null && Sigma.Onset.Count > 0) return Sigma.Onset;

            //If this is ever reached, the syllable wasn't set correctly!
            return null;
        }
        public SigmaBlock First()
        {
            if (Sigma.Onset != null && Sigma.Onset.Count > 0) return Sigma.Onset;
            if (Sigma.Nucleus != null && Sigma.Nucleus.Count > 0) return Sigma.Nucleus;
            if (Sigma.Medial != null && Sigma.Medial.Count > 0) return Sigma.Medial;
            if (Sigma.Coda != null && Sigma.Coda.Count > 0) return Sigma.Coda;

            //If this is ever reached, the syllable wasn't set correctly!
            return null;
        }

        /// <summary>
        /// This will always be null for the first word in a sentence. Account for it.
        /// </summary>
        public SigmaInfo? AdjacentLeft { get; set; } = null;
        /// <summary>
        /// This will always be null for the last word in a sentence. Account for it.
        /// </summary>
        public SigmaInfo? AdjacentRight { get; set; } = null;
    }
}
