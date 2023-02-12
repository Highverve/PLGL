using Microsoft.VisualBasic;
using PLGL.Data;
using PLGL.Processing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PLGL.Languages
{
    /// <summary>
    /// Tells the generator how to structure a word, starting with the syllable (sigma), and the valid lettrs inside those sigma.
    /// </summary>
    public class Structure
    {
        private Language language;
        public Structure(Language language) { this.language = language; }

        public Dictionary<char, LetterGroup> LetterGroups { get; set; } = new Dictionary<char, LetterGroup>();
        public Dictionary<string, Syllable> Syllables { get; set; } = new Dictionary<string, Syllable>();

        public List<Syllable> SortedSyllables { get; set; } = new List<Syllable>();
        private void SortList() { SortedSyllables = Syllables.Values.OrderBy((s) => s.Letters).ToList(); }

        public void AddGroup(char key, string name, params (char letter, double weight)[] letters)
        {
            if (LetterGroups.ContainsKey(key) == false)
            {
                LetterGroup group = new LetterGroup(name, key, letters);

                group.Letters = new List<(Letter l, double w)>();
                for (int i = 0; i < group.letterData.Length; i++)
                {
                    Letter l = language.Alphabet.Find(group.letterData[i].letter);
                    if (l != null)
                        group.Letters.Add(new(l, group.letterData[i].weight));
                    else
                    {
                        //Output not found issue to debug log.
                    }
                }

                LetterGroups.Add(key, group);
            }
        }
        public void AddSyllable(string letterGroups, double weight)
        {
            AddSyllable(letterGroups, weight, Array.Empty<string>());
        }
        public void AddSyllable(string letterGroups, double weight, params string[] tags)
        {
            if (Syllables.ContainsKey(letterGroups) == false)
            {
                Syllable s = new Syllable(letterGroups, weight, tags);

                for (int i = 0; i < s.Letters.Length; i++)
                {
                    if (LetterGroups.ContainsKey(s.Letters[i]))
                        s.Template.Add(LetterGroups[s.Letters[i]]);
                    else
                    {
                        //Output issue
                    }
                }

                Syllables.Add(letterGroups, s);
                SortList();
            }
        }

        internal void ResetWeights()
        {
            foreach (Syllable s in Syllables.Values)
                s.WeightMultiplier = 1.0;
        }
    }
}
