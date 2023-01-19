using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Construct.Elements
{
    public abstract class Letter
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Pronunciation { get; private set; } = string.Empty;

        public char Key { get; private set; }
        public (char lower, char upper) Case { get; private set; }

        /// <summary>
        /// The likelihood this letter will start the next word.
        /// </summary>
        public double StartWeight { get; private set; }

        public Letter(string Name, char Key, (char lower, char upper) Case, string Pronunciation, double StartWeight)
        {
            this.Name = Name;
            this.Key = Key;
            this.Case = Case;
            this.StartWeight = StartWeight;
            this.Pronunciation = Pronunciation;
        }
    }

    public class Consonant : Letter
    {
        public Consonant(string Name, char Key, (char lower, char upper) Case,
            string Pronunciation, double Weight = 1)
            : base(Name, Key, Case, Pronunciation, Weight) { }
    }
    public class Vowel : Letter
    {
        public Vowel(string Name, char Key, (char lower, char upper) Case,
            string Pronunciation, double Weight = 1)
            : base(Name, Key, Case, Pronunciation, Weight) { }
    }
}
