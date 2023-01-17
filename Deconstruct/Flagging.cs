using PLGL.Construct.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Deconstruct
{
    /// <summary>
    /// Markers allow the end-user to specify custom behaviour.
    /// Mark a word in your input text, and the matching marker will apply.
    /// 
    /// Example:
    ///     - There's a name in the sentence that you want the generator to skip.
    ///     - The SkipGenerate flag has been assigned to the 's' character.
    ///     - In the input text, you'd add "**s" to the end of the name: "Trevor**s"
    ///     
    ///     - To add multiple flags to a word, add more characters. If the character key
    ///       exists in the Markers dictionary, the function will apply to the word.
    ///     - Let's add an PrependMarker with the 'a' key. This will add a fictional title "syl-" to the start of a name.
    ///     - Now add the extra character to the name: "Trevor**sa", and will output "syl-Trevor"
    /// </summary>
    public class Flagging
    {
        public Dictionary<string, Action<LanguageGenerator, WordInfo>> Actions { get; set; }
            = new Dictionary<string, Action<LanguageGenerator, WordInfo>>();
    }
}

