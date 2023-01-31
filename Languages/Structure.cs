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
        private void SortList() { SortedSyllables = Syllables.Values.OrderBy((s) => s.Groups).ToList(); }

        public void AddGroup(char key, string name, params (char letter, double weight)[] letters)
        {
            if (LetterGroups.ContainsKey(key) == false)
            {
                LetterGroup group = new LetterGroup(name, key, letters);

                List<(Letter l, double w)> list = new List<(Letter l, double w)>();
                for (int i = 0; i < group.Letters.Length; i++)
                {
                    Letter l = language.Alphabet.Find(group.Letters[i].letter);
                    if (l != null)
                        list.Add(new(l, group.Letters[i].weight));
                    else
                    {
                        //Output not found issue to debug log.
                    }
                }

                LetterGroups.Add(key, group);
            }
        }
        public void AddSyllable(string groups, double weight)
        {
            if (Syllables.ContainsKey(groups) == false)
            {
                Syllable s = new Syllable(groups, weight);

                for (int i = 0; i < s.Groups.Length; i++)
                {
                    if (LetterGroups.ContainsKey(s.Groups[i]))
                        s.Template.Add(LetterGroups[s.Groups[i]]);
                    else
                    {
                        //Output issue
                    }
                }

                Syllables.Add(groups, s);
                SortList();
            }
        }

        public void AddSyllablePath(Syllable previous, WordPosition position, params (Syllable, double)[] sigmaWeights)
        {

        }
        public (double, Syllable)[] GetPotentialSyllables(Syllable lastSigma)
        {
            List<(double, Syllable)> results = new List<(double, Syllable)>();
            //List<Sigma> filter = ;


            return results.ToArray();
        }
    }
}
