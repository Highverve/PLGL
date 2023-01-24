using PLGL.Deconstruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Construct.Elements
{
    public class AffixInfo
    {
        public Affix Affix { get; set; }
        /// <summary>
        /// Set to the Affix.Value. Can be changed by OnPrefix or OnSuffix depending on affix context.
        /// </summary>
        public string AffixText { get; set; }

        public AffixInfo AdjacentLeft { get; set; }
        public AffixInfo AdjacentRight { get; set; }

        public int Order { get; set; }
        public bool IsProcessed { get; set; } = false;
    }
}
