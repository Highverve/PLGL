using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    /// <summary>
    /// This class answers the question: "What do we do with punctuation marks?"
    /// 
    /// Here are several examples:
    ///     1. For commas, the language author may wish to leave it there. This mark still needs to be added,
    ///        otherwise it would get included into the word-seed.
    ///     2. For question marks, the language author may wish to turn it into a word (in Japanese, the particle "ka" is used).
    ///        If making use of non-punctuated particles (Japanese's "wa" subject particle), a flag addition is recommended.
    ///     3. 
    /// </summary>
    public class Punctuation
    {
    }
    public enum MarkLocation { InsideWord, OutsideWord }
    public class PunctuationMark
    {
    }
}
