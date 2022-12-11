using LanguageReimaginer.Data;
using LanguageReimaginer.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Operators
{
    public class RandomGenerator
    {
        public int Seed { get; private set; }
        public Random Random { get; set; } = new Random();

        public void SetRandom(string word)
        {
            Seed = WordSeed(word.ToUpper());
            Random = new Random(Seed);
        }
        private int WordSeed(string word)
        {
            using var a = SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word)));
        }


        //Remove this code; or, if possible, make it anonymous?
        public double WeightSum(Letter[] letters) { return letters.Sum(x => x.StartWeight); }
        public Letter GenerateLetter(Letter[] letters)
        {
            double weight = Random.NextDouble() * WeightSum(letters);

            foreach (Letter l in letters)
            {
                weight -= l.StartWeight;

                if (weight <= 0)
                    return l;
            }

            //This code should never be reached
            return null;
        }
    }
}
