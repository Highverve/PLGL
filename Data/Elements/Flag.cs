using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data.Elements
{
    /// <summary>
    /// Flags allow the end-user to specify custom behaviour.
    /// Mark a word in your input text with an unlisted symbol, and the matching flag will apply.
    /// </summary>
    public class Flag
    {
        public char Symbol { get; private set; }
        public Func<string, string>? Function { get; set; } = null;

        public Flag(char Symbol)
        {
            this.Symbol = Symbol;
        }
    }

    /// <summary>
    /// Replaces a procedurally generated word with a custom word.
    /// </summary>
    public class FlagAppend : Flag
    {
        public string Suffix { get; private set; }
        public FlagAppend(char Symbol, string Suffix) : base(Symbol)
        {
            this.Suffix = Suffix;
        }
    }
}
