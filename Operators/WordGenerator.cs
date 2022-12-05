using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Operators
{
    internal class WordGenerator
    {
        private int syllableOffsetMin, syllableOffsetMax;
        public void SetSyllableOffset(int min, int max)
        {
            syllableOffsetMin = min;
            syllableOffsetMax = max;
        }
        private int GenerateSymbolCount(string word)
        {
            int length = word.Length;
            SetRandom(word);

            return random.Next(Math.Max(word.Length - syllableOffsetMin, 1), word.Length + syllableOffsetMax);
        }

        #region Random variables/methods
        private int Seed { get; set; }
        private Random random { get; set; }
        private void SetRandom(string word)
        {
            Seed = SetSeed(word);
            random = new Random(Seed);
        }
        private int SetSeed(string word)
        {
            using var a = SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word)));
        }
        #endregion

        public WordGenerator() { }

        StringBuilder sentenceBuilder = new StringBuilder();
        public string GenerateSentence(string sentence)
        {
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

            SetRandom(word);
            int length = GenerateSymbolCount(word);

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
