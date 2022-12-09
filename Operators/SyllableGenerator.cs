using LanguageReimaginer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Operators
{
    /// <summary>
    /// A word has 'x' syllables (determined by word character length +/- language data offset).
    ///    A syllable has letters, either as CV, VC, CC, VV(letter structure).
    /// The word-seeded random decides how the word is generated, in two parts:
    ///	First, the letter pattern structure:
    ///		If the last syllable ended in C, the next syllable starts with V.
    ///        If it's the first syllable of the word, any letter is chosen (according to the weights).
    ///    Second, the syllable takes form: 
    ///        A letter is selected according to the randomness of the weights.
    ///        The letter is appended to the word.
    ///        [char GenerateConsonant(), char GenerateVowel() called by SyllableGenerator()]
    ///    Process one and two alternate, choosing structure then giving form, until the required syllable count is reached.
    ///    
    /// After the word, grammatical rules are applied, such as what to do with contractions or hyphens.
    /// </summary>
    public class SyllableGenerator
    {
        internal Alphabet Language { get; set; }
        internal LanguageGenerator WordGen { get; set; }
        internal RandomGenerator RanGen { get; set; }

        public void GenerateSyllable(Random random)
        {

        }
        public char GenerateConsonant()
        {
            return Language.Consonants.First().Value.Value;
        }
        public char GenerateVowel()
        {
            return Language.Vowels.First().Value.Value;
        }

        private int GenerateSymbolCount(string word)
        {
            int length = word.Length;
            RanGen.SetRandom(word);

            return 0;//RanGen.Random.Next(Math.Max(word.Length - Language.SyllableCountMin, 1), word.Length + Language.SyllableCountMax);
        }
    }
}
