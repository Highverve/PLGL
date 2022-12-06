﻿using LanguageReimaginer.Data;
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
        public Language Language { get; set; }

        public int Seed { get; private set; }
        public Random Random { get; set; }

        public void SetRandom(string word)
        {
            Seed = SetSeed(word);
            Random = new Random(Seed);
        }
        private int SetSeed(string word)
        {
            using var a = SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word)));
        }

        public double WeightSum(Letter[] letters) { return letters.Sum(x => x.Weight); }

        public Letter GenerateLetter(Letter[] letters)
        {
            double numericValue = Random.NextDouble() * WeightSum(letters);

            foreach (Letter l in letters)
            {
                numericValue -= l.Weight;

                if (numericValue <= 0)
                    return l;
            }
            //This code should never be reached
            return null;
        }
    }
}
