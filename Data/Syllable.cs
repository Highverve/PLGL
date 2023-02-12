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
        /// <summary>
        /// The group this syllable belongs to. This can be anything, and is useful for excluding specific syllables in the OnSyllableSelect event.
        /// </summary>
        public string[] Tags { get; set; }
        /// <summary>
        /// The LetterGroup structure. This is Template as a string.
        /// </summary>
        public string Letters { get; set; }
        /// <summary>
        /// The LetterGroup structure. This matches Letters.
        /// </summary>
        public List<LetterGroup> Template { get; set; } = new List<LetterGroup>();

        /// <summary>
        /// Determines how frequently the syllable will appear, relative to all over syllables and their weights.
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Set by OnSyllableSelect.
        /// </summary>
        public double WeightMultiplier { get; set; } = 1.0;

        public Syllable(string Keys, double Weight, string[] Tags)
        {
            this.Letters = Keys;
            this.Weight = Weight;
            this.Tags = Tags;
        }
        public Syllable(string Keys, double Weight)
            : this(Keys, Weight, Array.Empty<string>()) { }

        public override string ToString() { return Letters; }
    }
}
