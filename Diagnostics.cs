using PLGL.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL
{
    public class Diagnostics
    {
        public string LogName { get; set; } = Environment.CurrentDirectory + "//" + "log";
        public StringBuilder LogBuilder { get; set; } = new StringBuilder();
        public char NestSymbol { get; set; } = '-';
        public string[] DeconstructExclusion { get; set; } = new string[] { "Delimiter" };
        public string[] FilterEventExclusion { get; set; } = new string[] { "Delimiter" };

        private LanguageGenerator generator;

        public Diagnostics(LanguageGenerator generator) { this.generator = generator; }

        public bool IsLogging { get; set; } = true;
        public bool IsDeconstructLog { get; set; } = true;
        public bool IsDeconstructEventLog { get; set; } = true;
        public bool IsConstructLog { get; set; } = true;
        public bool IsConstructEventLog { get; set; } = true;
        public bool IsSelectEventLog { get; set; } = true;

        public void LOG_Header(string text)
        {
            LogBuilder.AppendLine($"---------------|    {text}    |---------------{Environment.NewLine}");
        }
        public void LOG_Subheader(string text)
        {
            LogBuilder.AppendLine($"-----|    {text}    |-----");
        }
        public void LOG_NestLine(int depth, string text) { LogBuilder.AppendLine(new string(NestSymbol, depth) + " " + text); }
        public void LOG_Nest(int depth, string text) { LogBuilder.Append(new string(NestSymbol, depth) + " " + text); }
        public void LOG_Space() { LogBuilder.Append(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine); }

        public void SaveLog()
        {
            if (File.Exists(LogName + ".txt"))
                File.Delete(LogName + ".txt");

            File.AppendAllText(LogName + ".txt", LogBuilder.ToString());
            LogBuilder.Clear();
        }

        public Dictionary<string, (int, string)> Uniques { get; set; } = new Dictionary<string, (int, string)>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// This method uses data from Diagnostics.Uniques, thus only works if run after TEST_UniqueGeneration.
        /// </summary>
        /// <returns></returns>
        public List<(string generated, (int count, string seeds))> TESTS_Duplicates()
        {
            List<(string, (int, string))> results = Uniques.Select(x => (x.Key, x.Value)).Where((x) => x.Value.Item1 > 0).ToList();

            results.Sort((a, b) => b.Item2.Item2.CompareTo(a.Item2.Item2));
            //results.RemoveRange(count, results.Count - (count + 1));

            return results;
        }

        /// <summary>
        /// Tests your language to determine how many unique words are generated.
        /// </summary>
        /// <param name="syllableCount">I recommend a syllable count between 3 and 4.</param>
        /// <param name="seedCount">How many seeds the generator tests.</param>
        /// <param name="seedOffset">Increase to shift the range that the seedCount tests at.</param>
        /// <returns>Returns a percentage value between 0.0 and 1.0. This is the duplicate percentage.</returns>
        public double TEST_UniqueGeneration(int syllableCount, int seedCount, int seedOffset = 0)
        {
            Uniques.Clear();

            Func<int, double> syllableMin = generator.Language.Options.SyllableSkewMin, syllableMax = generator.Language.Options.SyllableSkewMax;
            var syllableFunc = generator.Language.Options.CountSyllables;

            for (int i = 0; i < seedCount; i++)
            {
                generator.SEED_SetRandom(i);
                generator.Language.Options.CountSyllables = (word) => { return syllableCount; };
                generator.Language.Options.SyllableSkewMin = (count) => { return 1; };
                generator.Language.Options.SyllableSkewMax = (count) => { return 1; };

                WordInfo word = new WordInfo();
                word.Filter = generator.Deconstruct.Undefined;
                word.WordRoot = string.Empty;

                generator.PopulateSyllables(word);
                generator.PopulateLetters(word);

                if (Uniques.ContainsKey(word.WordGenerated) == false)
                    Uniques.Add(word.WordGenerated, (0, i.ToString()));
                else
                    Uniques[word.WordGenerated] = (Uniques[word.WordGenerated].Item1 + 1, Uniques[word.WordGenerated].Item2 + ";" + i);

                generator.Language.Options.SyllableSkewMin = syllableMin;
                generator.Language.Options.SyllableSkewMax = syllableMax;
                generator.Language.Options.CountSyllables = syllableFunc;
            }

            return (double)Uniques.Where((x) => x.Value.Item1 > 0).Count() / (double)Uniques.Count;
        }
    }
}
