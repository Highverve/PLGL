using PLGL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Processing
{
    public class SyllableInfo
    {
        public Syllable? Syllable { get; set; }

        public SyllableInfo? AdjacentLeft { get; set; }
        public SyllableInfo? AdjacentRight { get; set; }

        public bool IsProcessed { get; set; } = false;
        public int SyllableIndex { get; set; }

        public WordInfo Word { get; set; }
        public List<LetterInfo> Letters { get; set; }
        public string Result { get; set; }
    }
}
