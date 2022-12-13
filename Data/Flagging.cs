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
    ///
    ///     - Append. Prepend or append text. Useful for adding flavor to words. Custom character for custom result.
    ///         1. Example: (**e)Append. Re-adds the removed "e" that was removed by the suffix to the root word. Communal -> commun|al -> commune|al
    ///     - Remove. Removes a set amount of characters from the start or the end.
    ///         1. Example: (**d)Removes double "n" from "running" after the root is extracted.
    /// </summary>
    public class Flag
    {
        public char Symbol { get; set; }
        /// <summary>
        /// Called by the generator. String parameter is the word (actual, root), and sets the word to something else.
        /// </summary>
        public Func<string, string>? Function { get; set; } = null;

        public enum TextPosition { BeforePrefix, AfterPrefix, BeforeSuffix, AfterSuffix }
        public TextPosition Position { get; set; } = TextPosition.AfterSuffix;
        public bool TargetGenerated { get; set; } = true;

        public Flag(char Symbol)
        {
            this.Symbol = Symbol;
        }
    }
    public class SkipGenerate : Flag { public SkipGenerate() : base('X') { } }
    public class SkipLexemes : Flag { public SkipLexemes() : base('x') { } }
    public class Add : Flag
    {
        public Add(char Symbol, string ToStart, string ToEnd) : base(Symbol)
        {
            Function = (s) => { return ToStart + s + ToEnd; };
        }
        public Add(char Symbol, string ToStart) : this(Symbol, ToStart, "") { }
        public Add(char Symbol, string ToEnd, int ignore = 0) : this(Symbol, "", ToEnd) { }
    }
    public class Remove : Flag
    {
        public Remove(char Symbol, int Start, int End) : base(Symbol)
        {
            Function = (s) => { return s.Remove(Start, (s.Length - End) - Start); };
        }
    }
}

