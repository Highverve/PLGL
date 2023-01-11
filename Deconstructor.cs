using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL
{
    public class Deconstructor
    {
        private CharacterFilter undefined = new CharacterFilter() { Characters = new char[0], Name = "Undefined" };
        public CharacterFilter Undefined { get { return undefined; } }
        public Language Language { get; set; }

        public Deconstructor() { }

        public CharacterFilter CheckFilter(char character)
        {
            foreach (CharacterFilter filter in Language.Deconstruction.Filters)
            {
                if (filter.Characters != null && filter.Characters.Contains(character))
                    return filter;
            }
            return Undefined;
        }

        /// <summary>
        /// Breaks a sentence down with the required character filters, returning a list of character blocks.
        /// This is the "first pass". Further functionality (that may require surrounding context-dependent action) should be in the "second pass".
        /// E.g, "let's" is one word, yet three character blocks: "let", "'", and "s". The second pass code will handle this behavior, but is outside the scope of this class.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public List<CharacterBlock> Deconstruct(string sentence)
        {
            List<CharacterBlock> result = new List<CharacterBlock>();

            CharacterFilter lastFilter = null, filter = null;
            result.Add(new CharacterBlock());
            CharacterBlock current = result.Last();

            for (int c = 0; c < sentence.Length; c++)
            {
                filter = CheckFilter(sentence[c]);
                if (c == 0) lastFilter = filter;

                if (filter != lastFilter)
                {
                    //Filter type has changed! Add block to list, and make new block.
                    result.Add(new CharacterBlock());
                    current = result.Last();
                    current.IndexFirst = c;
                }

                if (filter != null)
                {
                    current.Filter = filter;
                    current.Text += sentence[c];
                    current.IndexLast = c;
                }

                lastFilter = filter;
            }

            //Link words to each other for context-dependent processing.
            for (int i = 0; i < result.Count; i++)
            {
                if (i != 0)
                    result[i].Left = result[i - 1];
                if (i != result.Count - 1)
                    result[i].Right = result[i + 1];
            }

            return result;
        }
    }
    public class CharacterFilter
    {
        public string Name { get; set; } = string.Empty;
        public char[] Characters { get; set; }
    }
    public class CharacterBlock
    {
        public CharacterFilter Filter { get; set; }
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
