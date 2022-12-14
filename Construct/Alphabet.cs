using System;
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

        public Consonant AddConsonant(char letter)
        {
            if (Consonants.ContainsKey(letter) == false)
                Consonants.Add(letter, new Consonant(letter));
            return Consonants[letter];
        }
        public Vowel AddVowel(char letter)
        {
            if (Vowels.ContainsKey(letter) == false)
                Vowels.Add(letter, new Vowel(letter) { IsVowel = true });
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
        public List<Letter> Letters()
        {
            List<Letter> result = new List<Letter>();
            result.AddRange(Consonants.Values);
            result.AddRange(Vowels.Values);

            return result.OrderBy(l => l.Value).ToList();
        }
        public bool IsVowel(char c) { return Find(c).IsVowel; }
    }
}
