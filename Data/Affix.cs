using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public class Affix
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public enum AffixLocation { Prefix, Suffix }
        public int Order { get; set; }
        public string LetterGroups { get; set; }

        /// <summary>
        /// What type of affix is it? This tells the generator where to look for the affix.
        /// </summary>
        public AffixLocation KeyLocation { get; set; }
        /// <summary>
        /// This tells the generator where the Value should be added at the end of the generation process.
        /// </summary>
        public AffixLocation ValueLocation { get; set; }

        public Affix(string Key, string Value, AffixLocation KeyLocation, AffixLocation ValueLocation,
            int Order = 0, string LetterGroups = "")
        {
            this.Key = Key;
            this.Value = Value;
            this.KeyLocation = KeyLocation;
            this.ValueLocation = ValueLocation;
            this.Order = Order;
            this.LetterGroups = LetterGroups;
        }
    }
}
