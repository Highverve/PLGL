using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLGL.Data;

namespace PLGL.Processing
{
    public class LetterInfo
    {
        public Letter Letter { get; set; }
        public SyllableInfo Syllable { get; set; }
        public LetterGroup Group { get; set; }

        public LetterInfo AdjacentLeft { get; set; }
        public LetterInfo AdjacentRight { get; set; }

        public bool IsAlive { get; set; } = true;
        public bool IsProcessed { get; set; } = false;

        public LetterInfo(Letter letter) { Letter = letter; }

        public override string ToString() { return Letter.Case.lower.ToString(); }
    }
}
