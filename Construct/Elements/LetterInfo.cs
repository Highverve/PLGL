using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Construct.Elements
{
    public class LetterInfo
    {
        public Letter Letter { get; set; }
        public LetterInfo AdjacentLeft { get; set; }
        public LetterInfo AdjacentRight { get; set; }

        public bool IsAlive { get; set; } = true;
        public bool IsProcessed { get; set; } = false;

        public LetterInfo(Letter letter) { this.Letter = letter; }

        public override string ToString() { return (string.IsNullOrEmpty(Letter.Name) ? Letter.Name + ": " : "" ) + Letter.Case.lower + "/" + Letter.Case.upper; }
    }
}
