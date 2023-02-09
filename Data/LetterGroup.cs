using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public class LetterGroup
    {
        public string Name { get; set; }
        public char Key { get; set; }

        internal (char letter, double weight)[] letterData { get; set; }
        public List<(Letter letter, double weight)> Letters { get; set; }

        public LetterGroup(string Name, char Key, (char letter, double weight)[] Letters)
        {
            this.Name = Name;
            this.Key = Key;
            letterData = Letters;
        }
    }
}
