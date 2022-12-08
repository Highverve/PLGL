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
    public class Markings
    {
        /// <summary>
        /// An indicator written at the end of a word during the input process.
        /// 
        /// "**" is the default.
        /// </summary>
        public string Marcation { get; set; } = "**";
        
        public bool ContainsMarker(string word) { return (string.IsNullOrEmpty(word) == false) ? word.Contains(Marcation) : false; }
        public int MarkerIndex(string word) { return word.LastIndexOf(Marcation); }
        public char[] ParseMarkers(string word) { return word.Remove(0, MarkerIndex(word) - word.Length).ToCharArray(); }

        public List<Marker> GetMarkers(string word)
        {
            List<Marker> result = new List<Marker>();
            char[] chars = ParseMarkers(word);

            foreach (char f in chars)
            {
                if (Markers.ContainsKey(f))
                    result.Add(Markers[f]);
            }

            return result;
        }

        public Dictionary<char, Marker> Markers { get; set; } = new Dictionary<char, Marker>();
    }

    /// <summary>
    /// Marker ideas: SkipGenerate (useful for names), SkipLexemes, Prepend, Append
    /// </summary>
    public class Marker
    {
        public char Symbol { get; private set; }
        public Func<string, string>? Function { get; set; } = null;

        public Marker(char Symbol)
        {
            this.Symbol = Symbol;
        }
    }
}
