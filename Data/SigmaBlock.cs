using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public class SigmaBlock
    {
        public SigmaType Type { get; internal set; }

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
            get { return Type == SigmaType.Nucleus ? new string('V', Count) : new string('C', Count); }
            set { Count = value.Length; }
        }
    }
}
