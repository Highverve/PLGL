using PLGL.Construct.Elements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Deconstruct
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
        public SortedDictionary<string, Func<WordInfo, string>> Marks { get; set; }
            = new SortedDictionary<string, Func<WordInfo, string>>(new LengthComparer());

        public void Process(LanguageGenerator lg, WordInfo word, string filterName)
        {
            if (word.Filter.Name.ToUpper() == filterName)
            {
                foreach (string s in Marks.Keys)
                {
                    if (word.WordActual.Contains(s))
                        word.WordFinal = word.WordActual.Replace(s, Marks[s](word));
                }
            }
        }

        public void Add(string key, Func<WordInfo, string> value)
        {
            if (Marks.ContainsKey(key) == false)
            {
                Marks.Add(key, value);
            }
        }
    }

    /// <summary>
    /// Code borrowed from: https://stackoverflow.com/questions/20993877/how-to-sort-a-dictionary-by-key-string-length
    /// Code author: Boluc Papuccuoglu
    /// </summary>
    internal class LengthComparer : IComparer<String>
    {
        public int Compare(string x, string y)
        {
            int lengthComparison = x.Length.CompareTo(y.Length);
            if (lengthComparison == 0) return x.CompareTo(y);
            else return lengthComparison;
        }
    }
}
