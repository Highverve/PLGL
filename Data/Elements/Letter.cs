using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data.Elements
{
    public abstract class Letter
    {
        public char Value { get; set; }
        /// <summary>
        /// The likelihood this letter will start the next word.
        /// </summary>
        public double StartWeight { get; set; }

        /// <summary>
        /// Set by the Alphabet class when added to either Dictionary.
        /// </summary>
        public bool IsVowel { get; set; } = false;

        public Letter(char Value, double StartWeight)
        {
            this.Value = char.ToLower(Value);
            this.StartWeight = StartWeight;
        }

        public char Upper() { return char.ToUpper(Value); }
    }

    public class Consonant : Letter { public Consonant(char Value, double Weight = 1) : base(Value, Weight) { } }
    public class Vowel : Letter { public Vowel(char Value, double Weight = 1) : base(Value, Weight) { } }
}
