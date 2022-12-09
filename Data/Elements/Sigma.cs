using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data.Elements
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

        public Sigma()
        {
            Onset = new SigmaBlock() { Type = BlockType.Onset };
            Nucleus = new SigmaBlock() { Type = BlockType.Nucleus };
            Medial = new SigmaBlock() { Type = BlockType.Medial };
            Coda = new SigmaBlock() { Type = BlockType.Coda };
        }

        /*public Clustering Cluster { get; private set; }

        public Type Select(int index) { return (Char.ToUpper(Order[index]) == 'C') ? typeof(Consonant) : typeof(Vowel); }
        public Type First() { return Select(0); }
        public Type Last() { return Select(Count()); } //-1? Double check

        public bool IsConsonant(int index) { return Select(index) == typeof(Consonant); }
        public bool IsVowel(int index) { return Select(index) == typeof(Vowel); }

        private Clustering SetCluster()
        {
            if (DoubleForm('C')) return Clustering.Consonants;
            if (DoubleForm('V')) return Clustering.Vowels;

            return Clustering.None;
        }
        private bool DoubleForm(char c) { return Display.Contains((c + "" + c), StringComparison.OrdinalIgnoreCase); }

        /// <summary>
        /// The structure a syllable will take.
        /// </summary>
        /// <param name="Weight">The chance for </param>
        /// <param name="Order">Must be either C (for 'consonant') or V (for 'vowel'). Limited to three characters.</param>
        public Sigma(double Weight, params char[] Order)
        {
            this.Weight = Weight;
            this.Order = Order;

            for (int i = 0; i < Order.Length; i++)
                Display += Order[i];
            Cluster = SetCluster();
        }*/
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
}
