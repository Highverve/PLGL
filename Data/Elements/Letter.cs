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
        public double Weight { get; set; }
        //public bool ProhibitDouble { get; set; } = false; //May not be necessary.

        public Letter(char Value, double Weight)
        {
            this.Value = char.ToLower(Value);
            this.Weight = Weight;
        }

        public char Upper() { return char.ToUpper(Value); }
    }

    public class Consonant : Letter { public Consonant(char Value, double Weight = 1) : base(Value, Weight) { } }
    public class Vowel : Letter { public Vowel(char Value, double Weight = 1) : base(Value, Weight) { } }
}
