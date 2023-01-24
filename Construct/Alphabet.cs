﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLGL.Construct.Elements;

namespace PLGL.Construct
{
    public record class Alphabet
    {
        public Dictionary<char, Consonant> Consonants { get; private set; } = new Dictionary<char, Consonant>();
        public Dictionary<char, Vowel> Vowels { get; private set; } = new Dictionary<char, Vowel>();

        public Consonant AddConsonant(char key, (char lower, char upper) cases, double startWeight)
        {
            return AddConsonant(string.Empty, key, cases, startWeight);
        }
        public Vowel AddVowel(char key, (char lower, char upper) cases, double startWeight)
        {
            return AddVowel(string.Empty, key, cases, startWeight);
        }

        public Consonant AddConsonant(string name, char key, (char lower, char upper) cases, double startWeight, string pronunciation = "")
        {
            if (Consonants.ContainsKey(key) == false)
                Consonants.Add(key, new Consonant(name, key, cases, pronunciation, startWeight));
            return Consonants[key];
        }
        public Vowel AddVowel(string name, char key, (char lower, char upper) cases, double startWeight, string pronunciation = "")
        {
            if (Vowels.ContainsKey(key) == false)
                Vowels.Add(key, new Vowel(name, key, cases, pronunciation, startWeight));
            return Vowels[key];
        }

        public Letter? Find(char letter)
        {
            if (Consonants.ContainsKey(letter))
                return Consonants[letter];
            else if (Vowels.ContainsKey(letter))
                return Vowels[letter];
            return null;
        }
        public List<Letter> Letters()
        {
            List<Letter> result = new List<Letter>();
            result.AddRange(Consonants.Values);
            result.AddRange(Vowels.Values);

            return result.OrderBy(l => l.Key).ToList();
        }

        
    }
}
