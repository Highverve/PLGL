using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
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
        /// <summary>
        /// An indicator written at the end of a word during the input process.
        /// 
        /// "**" is the default.
        /// </summary>
        public string Marcation { get; set; } = "**";
        
        public bool ContainsFlag(string word) { return (string.IsNullOrEmpty(word) == false) ? word.Contains(Marcation) : false; }
        public int FlagIndex(string word) { return word.LastIndexOf(Marcation); }
        public char[] ParseFlags(string word) { return word.Remove(0, FlagIndex(word) - word.Length).ToCharArray(); }

        public List<Flag> GetFlags(string word)
        {
            List<Flag> result = new List<Flag>();
            char[] chars = ParseFlags(word);

            foreach (char f in chars)
            {
                if (Flags.ContainsKey(f))
                    result.Add(Flags[f]);
            }

            return result;
        }

        public Dictionary<char, Flag> Flags { get; set; } = new Dictionary<char, Flag>();
    }


    /// <summary>
    /// Marker ideas:
    ///     - (**X)SkipGenerate. Skips the entire word, making output = input. This is useful for names, places, etc.
    ///     - (**x)SkipLexemes. Processes the entire word, avoiding any lexeme processing.
    ///     - (**p)Possessive. Processes the "'s" possessive as a possessive, instead of a default contraction.
    ///
    ///     - Prepend/Append. Prepend or append text. Useful for adding flavor to words. Custom character for custom result.
    ///     - (**e)Append. Re-adds the removed "e" that was removed by the suffix to the root word. Communal -> commun|al -> commune|al
    /// </summary>
    public class Flag
    {
        public char Symbol { get; private set; }
        public Func<string, string>? Function { get; set; } = null;

        public enum TextPosition { BeforePrefix, AfterPrefix, BeforeSuffix, AfterSuffix }
        public TextPosition Position { get; set; } = TextPosition.AfterSuffix;
        public bool TargetGenerated { get; set; } = true;

        public Flag(char Symbol)
        {
            this.Symbol = Symbol;
        }
    }
}
