using PLGL.Data;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// The possible syllable pattern that the generator may choose.
        /// </summary>
        public List<Sigma> Templates { get; private set; } = new List<Sigma>();

        /// <summary>
        /// Any of these strings can be empty without issue (except the nucleus).
        /// </summary>
        /// <param name="onset"></param>
        /// <param name="nucleus"></param>
        /// <param name="coda"></param>
        /// <param name="weights"></param>
        public void AddSigma(string onset, string nucleus, string coda, SigmaPath weights) { Templates.Add(new Sigma(onset, nucleus, coda) { Weight = weights }); }
        public void AddSigmaVC(string nucleus, string coda, SigmaPath weights) { Templates.Add(new Sigma(string.Empty, nucleus, coda) { Weight = weights }); }
        public void AddSigmaCV(string onset, string nucleus, SigmaPath weights) { Templates.Add(new Sigma(onset, nucleus, string.Empty) { Weight = weights }); }
        public void AddSigmaV(string nucleus, SigmaPath weights) { Templates.Add(new Sigma(string.Empty, nucleus, string.Empty) { Weight = weights }); }

        public void AddSigmaPath(Sigma previous, WordPosition position, params (Sigma, double)[] sigmaWeights)
        {

        }
        public (double, Sigma)[] GetPotentialSigma(Sigma lastSigma)
        {
            List<(double, Sigma)> results = new List<(double, Sigma)>();
            //List<Sigma> filter = ;


            return results.ToArray();
        }

        public List<LetterPath> LetterPaths { get; private set; } = new List<LetterPath>();

        public void AddLetterPath(char letter, WordPosition wordPos, SigmaPosition sigmaPos, params (char, double)[] letterWeights)
        {
            LetterPath path = new LetterPath();

            path.Previous = letter;
            path.WordPosition = wordPos;
            path.SigmaPosition = sigmaPos;
            path.Next.AddRange(letterWeights);

            LetterPaths.Add(path);
        }
        /// <summary>
        /// Returns the possible paths based on the generator's last generated letter, word position, and then sigma position.
        /// Finally, it's sorted by word and sigma position.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="wordPos"></param>
        /// <param name="sigmaPos"></param>
        /// <returns></returns>
        public LetterPath[] GetPotentialPaths(char previous, WordPosition wordPos, SigmaPosition sigmaPos)
        {
            //Filters the list by:
            //  1. The operating letter ("previous").
            //  2. The word positon matches, or is any.
            //  3. The sigma position matches, or is any.
            //Then, it orders it by:
            //  1. Word position.
            //  2. Sigma position.

            return LetterPaths.Where(l => l.Previous == previous)
                .Where((l) => l.WordPosition == wordPos || l.WordPosition == WordPosition.Any)
                .Where((l) => l.SigmaPosition == sigmaPos || l.SigmaPosition == SigmaPosition.Any)
                .OrderBy(l => l.WordPosition.CompareTo(wordPos)).ThenBy(l => l.SigmaPosition.CompareTo(sigmaPos)).ToArray();
        }

        public LetterPath[] GetPotentialPaths(char previous, WordPosition wordPos)
        {
            //Filters the list by:
            //  1. The operating letter ("previous").
            //  2. The word positon matches, or is any.
            //  3. The sigma position matches, or is any.
            //Then, it orders it by:
            //  1. Word position.
            //  2. Sigma position.

            return LetterPaths.Where(l => l.Previous == previous)
                .Where((l) => l.WordPosition == wordPos || l.WordPosition == WordPosition.Any)
                .OrderBy(l => l.WordPosition.CompareTo(wordPos)).ToArray();
        }

        //New code

        private Language language;
        public Structure(Language language) { this.language = language; }

        public Dictionary<char, LetterGroup> LetterGroups { get; set; } = new Dictionary<char, LetterGroup>();
        public Dictionary<string, Syllable> Syllables { get; set; } = new Dictionary<string, Syllable>();

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
            }
        }
    }
}
