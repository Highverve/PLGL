using LanguageReimaginer.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    /// <summary>
    /// An optional class, used to build a reference guide for pronunciation.
    /// Work-in-progress, and low-priority.
    /// </summary>
    public class Phonemes
    {
        
    }
    public class Phoneme
    {
        public string IPA { get; set; } = string.Empty;

        public Letter? AdjacentLeft { get; set; } = null;
        public Letter? AdjacentRight { get; set; } = null;
    }
}
