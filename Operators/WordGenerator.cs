using LanguageReimaginer.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Operators
{
    public class WordGenerator
    {
        internal RandomGenerator RanGen { get; set; }
        internal SyllableGenerator SyllableGen { get; set; }

        public WordGenerator() { }

        StringBuilder sentenceBuilder = new StringBuilder();
        public string GenerateSentence(string sentence)
        {

            Vowel v = new Vowel('a');

            //GUIDELINES:
            //  1. Splits string by delimiter(s).
            //  2. Apply grammatical rules (e.g, replacing punctuation).
            //  3. Iterate each word into Next(), and add the result into the StringBuilder.
            //  4. Return the StringBuilder's result.

            sentenceBuilder.Clear();

            string[] words = sentence.Split(' '); //Temporary ' ' char. Replace with customizable option(s).

            foreach (string s in words)
            {
                sentenceBuilder.Append(NextWord(s) + " ");
            }

            return sentenceBuilder.ToString();
        }

        StringBuilder wordBuilder = new StringBuilder();
        private string NextWord(string word)
        {
            //  1. SetRandom to word.
            //  2. Get syllable count to generate.
            //  

            wordBuilder.Clear();

            RanGen.SetRandom(word);
            int length = 3;

            while (length > 0)
            {

                wordBuilder.Append(NextSyllable());
                length--;
            }

            return wordBuilder.ToString();
        }

        private string NextSyllable()
        {
            //TO-DO: Generate syllable from weighted data syllables.
            return string.Empty;
        }
    }
}
