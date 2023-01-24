using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public class CharacterBlock
    {
        public CharacterFilter Filter { get; set; }
        public bool IsAlive { get; set; } = true;

        public string Text { get; set; } = string.Empty;
        public int IndexFirst { get; set; } = 0;
        public int IndexLast { get; set; } = 0;

        public CharacterBlock Left { get; set; } = null;
        public CharacterBlock Right { get; set; } = null;

        public override string ToString()
        {
            return Filter.Name.ToUpper() + "[" + IndexFirst + "," + IndexLast + "]: \"" + Text + "\"";
        }
    }
}
