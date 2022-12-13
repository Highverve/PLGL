using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageReimaginer.Data.Elements;

namespace LanguageReimaginer.Data
{
    public record class Alphabet
    {
        public Dictionary<char, Consonant> Consonants { get; private set; } = new Dictionary<char, Consonant>();
        public Dictionary<char, Vowel> Vowels { get; private set; } = new Dictionary<char, Vowel>();

        public Consonant AddConsonant(char letter)
        {
            if (Consonants.ContainsKey(letter) == false)
                Consonants.Add(letter, new Consonant(letter));
            return Consonants[letter];
        }
        public Vowel AddVowel(char letter)
        {
            if (Vowels.ContainsKey(letter) == false)
                Vowels.Add(letter, new Vowel(letter));
            return Vowels[letter];
        }

        public Letter? Find(char letter)
        {
            if (Consonants.ContainsKey(letter))
                return Consonants[letter];
            else if (Vowels.ContainsKey(letter))
                return Vowels[letter];
            return null;
        }
    }
}
