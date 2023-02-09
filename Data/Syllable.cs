using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    //Rules gives syllables greater contextual control.
    //A rule always returns a double, which multiplies the chance of that syllable of occurring.
    //Set to zero makes the rule exclude the syllable.

    //List of rules:
    // - Left syllable doesn't end in group x
    // - Left syllable doesnt end in letter x
    // - First, middle, or end syllable check
    // - Syllable is exactly nth position

    public class Syllable
    {
        public string Groups { get; set; }
        public double Weight { get; set; }
        /// <summary>
        /// Set by OnSyllableSelect.
        /// </summary>
        public double WeightMultiplier { get; set; } = 1.0;
        /// <summary>
        /// The letter group template as set by Groups.
        /// </summary>
        public List<LetterGroup> Template { get; set; } = new List<LetterGroup>();

        public Syllable(string Keys, double Weight)
        {
            this.Groups = Keys;
            this.Weight = Weight;
        }

        internal void Initialize(Language language)
        {
            for (int i = 0; i < Groups.Length; i++)
            {
                if (language.Structure.LetterGroups.ContainsKey(Groups[i]))
                    Template.Add(language.Structure.LetterGroups[Groups[i]]);
                else
                {
                    //Output issue
                }
            }
        }
        public override string ToString() { return Groups; }
    }
}
