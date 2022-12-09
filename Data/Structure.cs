using LanguageReimaginer.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    /// <summary>
    /// Tells the generator how to structure a word.
    /// </summary>
    public class Structure
    {
        //1. Merge with pathways?!
        //2. Either create methods for adding whole sigmas, or methods for generating max lengths of each block type.
        //   Both of these will use some form of weight distribution to decide commonality of blocks in syllables, and syllables in words.

        /*public Dictionary<string, Sigma> LetterForms { get; private set; } = new Dictionary<string, Sigma>();
        public void AddForm(params Sigma[] syllables)
        {
            for (int i = 0; i < syllables.Length; i++)
            {
                if (LetterForms.ContainsKey(syllables[i].Display) == false)
                    LetterForms.Add(syllables[i].Display, syllables[i]);
            }

            //Example
            //AddForm(new Form(10.0, 'C', 'V'), new Form(10.0, 'V', 'C'), new Form(5.0, 'C', 'C'),
            //        new Form(2.0, 'V', 'V'), new Form(1.0, 'C', 'V', 'C'), new Form(1.0, 'V', 'C', 'V'));
        }*/
    }
}
