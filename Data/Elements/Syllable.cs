using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data.Elements
{
    internal class Syllable
    {
        public List<Letter> Letters { get; private set; } = new List<Letter>();

    }
}
