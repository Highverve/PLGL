using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data.Elements
{
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
        /// The semivowel which links the boundary between the nucleus's vowels and the coda's consonants.
        /// This is optional, and will be unimplemented for awhile.
        /// </summary>
        public SigmaBlock Medial { get; set; }
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
                if (Medial != null)
                    structure += Medial.Template;
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
                if (Medial != null)
                    count += Medial.Count;
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
            this.Onset = new SigmaBlock() { Type = BlockType.Onset };
            this.Nucleus = new SigmaBlock() { Type = BlockType.Nucleus };
            this.Medial = new SigmaBlock() { Type = BlockType.Medial };
            this.Coda = new SigmaBlock() { Type = BlockType.Coda };

            this.Onset.Template = Onset;
            this.Nucleus.Template = Nucleus;
            this.Coda.Template = Coda;
        }

        public override string ToString() { return Structure() +"[" + Count() + "]"; }
    }

    public enum BlockType { Onset, Nucleus, Medial, Coda }
    public class SigmaBlock
    {
        public BlockType Type { get; internal set; }

        /// <summary>
        /// Determines how many consonants (Block Type.Onset or Type.Coda) or vowels (Block Type.Nucleus) the generator creates in this part of the sigma.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// How the block is laid out, such as "CC" if onset, or "V" if nucleus.
        /// 
        /// Get returns the length of Count as the block type (e.g, 'C' * Count, returning 'CC' if Count is 2).
        /// Set equals the Count to the length of the string.
        /// </summary>
        public string Template
        {
            get { return (Type == BlockType.Nucleus) ? new string('V', Count) : new string('C', Count); }
            set { Count = value.Length; }
        }
    }

    public class SigmaPath
    {
        /// <summary>
        /// How likely the generator will choose this sigma. Default is 1.0.
        /// </summary>
        public double SelectionWeight { get; set; } = 1.0;

        /// <summary>
        /// How likely this sigma will start a word. Default is 1.0.
        /// </summary>
        public double StartingWeight { get; set; } = 1.0;
        /// <summary>
        /// How likely this sigma will end a word. Default is 1.0.
        /// </summary>
        public double EndingWeight { get; set; } = 1.0;

        /// <summary>
        /// How likely this sigma will follow a sigma that ends with a vowel. Default is 1.0.
        /// </summary>
        public double LastVowelWeight { get; set; } = 1.0;
        /// <summary>
        /// How likely this sigma will follow a sigma that ends with a consonant. Default is 1.0.
        /// </summary>
        public double LastConsonantWeight { get; set; } = 1.0;
    }
}
