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
        public Language Language { get; set; }

        public void SetRandom(string word)
        {
            Seed = WordSeed(word.ToUpper());
            Random = new Random(Seed);
        }
        private int WordSeed(string word)
        {
            using var a = SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word))) + Language.Options.SeedOffset;
        }
        public double NextDouble(double minimum, double maximum) { return Random.NextDouble() * (maximum - minimum) + minimum; }
    }
}
