using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    public class Lexicon
    {
        public Dictionary<string, string> InflectedWords { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> RootWords { get; private set; } = new Dictionary<string, string>();
    }
}
