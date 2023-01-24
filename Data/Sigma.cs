using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public enum SigmaType { Onset, Nucleus, Medial, Coda }

    public class Sigma
    {
        private int count = 0;
        private string structure = string.Empty;

        /// <summary>
        /// The starting consonant(s) of a syllable. This can be left null.
        /// </summary>
        public SigmaBlock Onset { get; set; }
        /// <summary>
        /// The middle vowel(s) of a syllable. This should not be null!
        /// </summary>
        public SigmaBlock Nucleus { get; set; }
        /// <summary>
        /// The ending consonant(s) of a syllable. This can be left null.
        /// </summary>
        public SigmaBlock Coda { get; set; }

        public SigmaPath Weight { get; set; }

        /// <summary>
        /// Returns the letter pattern structure of the syllable (sigma).
        /// This may look like "CCVC", "VC", "CVC", "CCCVCC", etc.
        /// </summary>
        /// <returns></returns>
        public string Structure()
        {
            if (string.IsNullOrEmpty(structure))
            {
                if (Onset != null)
                    structure += Onset.Template;
                if (Nucleus != null)
                    structure += Nucleus.Template;
                if (Coda != null)
                    structure += Coda.Template;
            }

            return structure;
        }
        public int Count()
        {
            if (count != 0)
            {
                if (Onset != null)
                    count += Onset.Count;
                if (Nucleus != null)
                    count += Nucleus.Count;
                if (Coda != null)
                    count += Coda.Count;
            }

            return count;
        }

        /// <summary>
        /// </summary>
        /// <param name="Onset">The first part of the sigma. For readability, set to some count of "C", "CC", etc.</param>
        /// <param name="Nucleus">The middle part of the sigma. For readability, set to some count of "VV", "VV", etc.</param>
        /// <param name="Coda">The end part of the sigma, containing consonants. For readability, set to some count of "C", "CC", etc.</param>
        public Sigma(string Onset, string Nucleus, string Coda)
        {
            this.Onset = new SigmaBlock() { Type = SigmaType.Onset };
            this.Nucleus = new SigmaBlock() { Type = SigmaType.Nucleus };
            this.Coda = new SigmaBlock() { Type = SigmaType.Coda };

            this.Onset.Template = Onset;
            this.Nucleus.Template = Nucleus;
            this.Coda.Template = Coda;
        }

        public override string ToString() { return Structure() + "[" + Count() + "]"; }
    }
}
