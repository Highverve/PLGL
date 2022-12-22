using PLGL.Data;
using PLGL.Data.Elements;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PLGL
{
    public class LanguageGenerator
    {
        StringBuilder sentenceBuilder = new StringBuilder();
        List<WordInfo> wordInfo = new List<WordInfo>();

        /// <summary>
        /// Sets how the generator count's a root's syllables. Default is EnglishSigmaCount (C/V border checking).
        /// </summary>
        public Func<string, int> SigmaCount { get; set; }
        private bool skipGeneration, skipLexemes;

        #region Language management
        public Language Language { get; set; }
        public Dictionary<string, Language> Languages { get; private set; } = new Dictionary<string, Language>();

        public void AddLanguage(string key, Language language)
        {
            if (Languages.ContainsKey(key) == false)
                Languages.Add(key, language);
        }
        public void AddLanguage(Language language) { AddLanguage(language.META_Nickname, language); }
        public void SelectLanguage(string nickname)
        {
            if (Languages.ContainsKey(nickname))
                Language = Languages[nickname];
        }
        public void SelectLanguage(Language language) { Language = language; }
        #endregion

        #region Random management
        public int Seed { get; private set; }
        public Random Random { get; set; } = new Random();

        private void SetRandom(string word)
        {
            Seed = WordSeed(word.ToUpper());
            Random = new Random(Seed);
        }
        private int WordSeed(string word)
        {
            using var a = System.Security.Cryptography.SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word))) + Language.Options.SeedOffset;
        }
        private double NextDouble(double minimum, double maximum) { return Random.NextDouble() * (maximum - minimum) + minimum; }
        #endregion

        public LanguageGenerator() { SigmaCount = (s) => EnglishSigmaCount(s); }
        public LanguageGenerator(Language Language) : base() { this.Language = Language; }

        #region Generation — Main method and overloads
        /// <summary>
        /// Generates a new sentence from the input sentence according to the language's constraints. This overload returns the word list for debugging purposes.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public string Generate(string sentence, out List<WordInfo> info)
        {
            //Clear class-level variables for the next sentence.
            sentenceBuilder.Clear();
            wordInfo.Clear();

            /* Pre: Split words by delimiter and add to WordInfo list.

            //Deconstructing a word.
            //1. Punctuation marks. Loop through WordInfo list, isolate all punctuation marks (e.g., comma, or quote/comma),
            //   add punctuation to WordInfo.
            //2. Flagging. Check for flag marcation, add flags to WordInfo, process flags.
            //3. Lexemes. Check for affixes, add prefixes and suffixes to WordInfo.

            //Constructing a new word.
            //4. Estimate sigma (syllable) count by checking the boundaries of "VC" and "CV".
            //5. Generate sigma structure; by length of actual sigma count * sigma weight. "CCV·VC·VC·CV"
            //6. The first letter of the word is chosen; by first sigma's C/V, then by start letter weight.
            //7. The next letters are chosen, according to the pathways set by the language author.

            //Combining together.
            //8. The sigmas of the root word are combined in order.
            //9. The new affixes are retrieved and placed in order.
            //10. The new punctuation marks are adding to the end of the word.*/

            //This should split a word by the delimiters, and then leave the delimiter at the end.
            string[] words = sentence.Split(Language.Options.Delimiters);//Regex.Split(sentence, "@\"(?<=[" + string.Join("", Language.Options.Delimiters) + "])\"");
            AddWordInfo(words);
            LinkWords();

            foreach (WordInfo word in wordInfo)
            {
                //Added in case no flags or punctuation marks are found.
                word.WordStripped = word.WordActual;

                skipGeneration = false;
                skipLexemes = false;

                //The word is checked for flags. If any, the flags are processed and stripped.
                ParseFlags(word);
                CheckFlags(word);

                if (skipGeneration == false && skipLexemes == false)
                {
                    CheckLexiconInflection(word);

                    ProcessLexemes(word);
                    AssembleLexemes(word);
                }
                else word.WordRoot = word.WordStripped;

                CheckLexiconRoot(word);

                //The foundation of the generator. Initializes a new Random using the root word as its seed.
                SetRandom(word.WordRoot);

                if (skipGeneration == false)
                {
                    SelectSigmaStructures(word);
                    PopulateWord(word);
                }

                //Put the word together.
                word.WordFinal = word.WordPrefixes + word.WordGenerated + word.WordSuffixes;

                LexiconMemorize(word);
            }

            //Compile the sentence.
            foreach(WordInfo word in wordInfo)
                sentenceBuilder.Append(word.WordFinal + ' ');

            //Return the result.
            info = wordInfo;
            return sentenceBuilder.ToString();
        }
        /// <summary>
        /// Generates a new sentence from the input sentence according to the language's constraints.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string Generate(string sentence) { return Generate(sentence, out _); }
        /// <summary>
        /// Switches to the specified language and generates a new sentence.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string Generate(Language language, string sentence) { SelectLanguage(language); return Generate(sentence); }
        /// <summary>
        /// Switches to the specified language and generates a new sentence.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string Generate(string language, string sentence) { SelectLanguage(language); return Generate(sentence); }
        #endregion

        #region Deconstruction — Adding and linking words
        /// <summary>
        /// Loops through each word, and adds it to wordInfo.
        /// </summary>
        /// <param name="words"></param>
        private void AddWordInfo(string[] words)
        {
            foreach (string s in words)
            {
                WordInfo word = new WordInfo();
                word.WordActual = s;

                wordInfo.Add(word);
            }
        }
        /// <summary>
        /// Links adjacent words for additional generation context.
        /// </summary>
        private void LinkWords()
        {
            for (int i = 0; i < wordInfo.Count; i++)
            {
                if (i != 0)
                    wordInfo[i].AdjacentLeft = wordInfo[i - 1];
                if (i != wordInfo.Count - 1)
                    wordInfo[i].AdjacentRight = wordInfo[i + 1];
            }
        }
        #endregion

        #region Flagging
        /// <summary>
        /// Checks if a word contains the flag marcation, parses all characters after it, and sets word.Flags to gathered flags.
        /// </summary>
        /// <param name="word"></param>
        private void ParseFlags(WordInfo word)
        {
            if (Language.Flagging.ContainsFlag(word.WordActual))
            {
                //Add any flags as char array to WordInfo.
                int marcation = Language.Flagging.FlagIndex(word.WordActual);
                string flags = word.WordActual.Substring(marcation + Language.Flagging.Marcation.Length,
                    word.WordActual.Length - (marcation + Language.Flagging.Marcation.Length)); //May need + Flagging.Marcation.Length to startIndex.
                string flagsFinal = string.Empty;

                foreach (char c in flags)
                     flagsFinal += c;

                word.Flags = flagsFinal.ToCharArray();
                word.WordStripped = word.WordActual.Substring(0, word.WordActual.Length - (Language.Flagging.Marcation.Length + word.Flags.Length));
            }
        }
        /// <summary>
        /// Checks if a word has any two flags: (X)Skip Generation, (x)Skip Lexemes
        /// </summary>
        /// <param name="word"></param>
        private void CheckFlags(WordInfo word)
        {
            //Set generator-level flags.
            if (word.Flags?.Any() == true && word.Flags.Contains('X') == true)
            {
                skipGeneration = true;
                word.WordGenerated = word.WordStripped;
            }
            if (word.Flags?.Any() == true && word.Flags.Contains('x') == true) skipLexemes = true;
        }
        #endregion

        #region Punctuation
        #endregion

        #region Lexicon (and inflections) — Affixes, root extraction, custom words
        /// <summary>
        /// Checks inflection-level lexicon for matches. If there's a match, skip procedural generation and lexeme processing.
        /// </summary>
        /// <param name="word"></param>
        private void CheckLexiconInflection(WordInfo word)
        {
            if (Language.Lexicon.Inflections.ContainsKey(word.WordStripped))
                skipGeneration = skipLexemes = ProcessLexiconInflections(word);
        }
        /// <summary>
        /// Checks root-level lexicon for matches. If there's a match, skip procedural generation.
        /// </summary>
        /// <param name="word"></param>
        private void CheckLexiconRoot(WordInfo word)
        {
            if (Language.Lexicon.Roots.ContainsKey(word.WordRoot))
                skipGeneration = ProcessLexiconRoots(word);
        }
        private bool ProcessLexiconInflections(WordInfo word)
        {
            word.WordGenerated = Language.Lexicon.Inflections[word.WordActual];
            return true;
        }
        private bool ProcessLexiconRoots(WordInfo word)
        {
            word.WordGenerated = Language.Lexicon.Roots[word.WordRoot];
            return true;
        }
        
        /// <summary>
        /// The lexemes are processed, stripping the actual word down to its root.
        /// </summary>
        /// <param name="word"></param>
        private void ProcessLexemes(WordInfo word)
        {
            //Extract affixes.
            word.Prefixes = Language.Lexicon.GetPrefixes(word.WordStripped).ToArray();
            word.Suffixes = Language.Lexicon.GetSuffixes(word.WordStripped).ToArray();

            //Strip word to root.
            int prefixLength = word.Prefixes.Sum((a) => a.Key.Length);
            int suffixLength = word.Suffixes.Sum((a) => a.Key.Length);
            word.WordRoot = word.WordStripped.Substring(prefixLength, word.WordStripped.Length - suffixLength);
        }
        /// <summary>
        /// The affixes are assembled and set in order.
        /// </summary>
        /// <param name="word"></param>
        private void AssembleLexemes(WordInfo word)
        {
            foreach (Affix p in word.Prefixes)
                word.WordPrefixes += p.Value;
            foreach (Affix s in word.Suffixes)
                word.WordSuffixes += s.Value;
        }

        /// <summary>
        /// "Memorizes" the word if the option has been set and the word isn't in Lexicon.Inflections.
        /// </summary>
        /// <param name="word"></param>
        private void LexiconMemorize(WordInfo word)
        {
            if (Language.Options.MemorizeWords == true && Language.Lexicon.Inflections.ContainsKey(word.WordActual) == false)
                Language.Lexicon.Inflections.Add(word.WordActual, word.WordFinal);
        }
        #endregion

        #region Word construction — Sigma structuring and word population
        /// <summary>
        /// Estimate sigma count and generate sigma structure.
        /// </summary>
        /// <param name="word"></param>
        private void SelectSigmaStructures(WordInfo word)
        {
            int sigmaCount = (int)(SigmaCount(word.WordRoot) *
                    NextDouble(Language.Options.SigmaSkewMin, Language.Options.SigmaSkewMax));

            //Generate sigma structure.
            for (int i = 0; i < sigmaCount; i++)
            {
                //5.1 Select sigma by sigma's weights and the language's sigma options.
                SigmaInfo info = new SigmaInfo();

                Sigma last = word.SigmaInfo.LastOrDefault() != null ? word.SigmaInfo.LastOrDefault().Sigma : null;
                Sigma sigma = SelectSigma(i, last);

                if (i == 0) info.Position = WordPosition.First;
                else if (i == sigmaCount - 1) info.Position = WordPosition.Last;
                else info.Position = WordPosition.Middle;

                info.Sigma = sigma;
                word.SigmaInfo.Add(info);
            }

            //Link adjacent sigma.
            for (int i = 0; i < word.SigmaInfo.Count; i++)
            {
                if (i != 0)
                    word.SigmaInfo[i].AdjacentLeft = word.SigmaInfo[i - 1];
                if (i != word.SigmaInfo.Count - 1)
                    word.SigmaInfo[i].AdjacentRight = word.SigmaInfo[i + 1];
            }
        }
        private Sigma SelectSigma(int sigmaPosition, Sigma lastSigma)
        {
            double weight = 0;

            //if (lastSigma.Coda != null || lastSigma.Coda.Count > 0)


            weight = Random.NextDouble() * Language.Structure.Templates.Sum(s => s.Weight.SelectionWeight);

            foreach (Sigma s in Language.Structure.Templates)
            {
                weight -= s.Weight.SelectionWeight;

                if (weight <= 0)
                    return s;
            }

            return null;
        }
        /// <summary>
        /// Roughly estimates a word's syllables. It transforms the word into c/v, and counts where a consonant shares a border with a vowel. Only misses where a consonant could also be a vowel (such as "y")).
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int EnglishSigmaCount(string word)
        {
            string cv = string.Empty;

            foreach (char c in word)
            {
                if (Language.Options.InputConsonants.Contains(c)) cv += 'c';
                if (Language.Options.InputVowels.Contains(c)) cv += 'v';
            }

            int result = 0;
            while (cv.Length > 1)
            {

                //Check for consonant-vowel border.
                if (cv[0] == 'c' && cv[1] == 'v' ||
                    cv[0] == 'v' && cv[1] == 'c')
                {
                    result++;
                    cv = cv.Remove(0, Math.Min(cv.Length, 2));
                }

                if (cv.Length <= 1)
                    break;

                //If double consonant or vowel, remove one.
                if (cv[0] == 'c' && cv[1] == 'c' ||
                    cv[0] == 'v' && cv[1] == 'v')
                {
                    cv = cv.Remove(0, 1);
                }
            }

            if (word.Length > 0 && result == 0)
                result = 1;

            return result;
        }
        /// <summary>
        /// Counts each valid character found in Language.Options.InputConsonants and InputVowels. More ideal for languages where each letter may be a syllable.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int CharacterSigmaCount(string word)
        {
            int result = 0;

            foreach (char c in word)
            {
                if (Language.Options.InputConsonants.Contains(c)) result++;
                if (Language.Options.InputVowels.Contains(c)) result++;
            }

            return result;
        }

        /// <summary>
        /// Populates the sigma structures according to the language's letter pathing.
        /// </summary>
        /// <param name="word"></param>
        private void PopulateWord(WordInfo word)
        {
            //Select starting letter according to letter weights.
            if (word.SigmaInfo[0].First().Type == BlockType.Nucleus)
                word.WordGenerated += SelectFirstVowel();
            else
                word.WordGenerated += SelectFirstConsonant();

            int onsetOffset = 0, nucleusOffset = 0;
            if (word.SigmaInfo.First().First().Type == BlockType.Onset)
                onsetOffset = word.WordGenerated.Length;
            if (word.SigmaInfo.First().First().Type == BlockType.Nucleus)
                nucleusOffset = word.WordGenerated.Length;

            foreach (SigmaInfo s in word.SigmaInfo)
            {
                for (int i = 0; i < s.Sigma.Onset.Count - onsetOffset; i++)
                    s.Onset += SelectLetter(word.WordGenerated.LastOrDefault(), s.Position, SigmaPosition.Onset, false);
                word.WordGenerated += s.Onset;

                for (int i = 0; i < s.Sigma.Nucleus.Count - nucleusOffset; i++)
                    s.Nucleus += SelectLetter(word.WordGenerated.LastOrDefault(), s.Position, SigmaPosition.Nucleus, true);
                word.WordGenerated += s.Nucleus;

                for (int i = 0; i < s.Sigma.Coda.Count; i++)
                    s.Coda += SelectLetter(word.WordGenerated.LastOrDefault(), s.Position, SigmaPosition.Coda, false);
                word.WordGenerated += s.Coda;

                onsetOffset = 0;
                nucleusOffset = 0;
            }
        }
        private char SelectFirstVowel()
        {
            double weight = Random.NextDouble() * Language.Alphabet.Vowels.Values.Sum(w => w.StartWeight);

            foreach (Vowel v in Language.Alphabet.Vowels.Values)
            {
                weight -= v.StartWeight;

                if (weight <= 0)
                    return v.Value;
            }

            return '_';
        }
        private char SelectFirstConsonant()
        {
            double weight = Random.NextDouble() * Language.Alphabet.Consonants.Values.Sum(w => w.StartWeight);

            foreach (Consonant c in Language.Alphabet.Consonants.Values)
            {
                weight -= c.StartWeight;

                if (weight <= 0)
                    return c.Value;
            }

            return '_';
        }
        private char SelectLetter(char last, WordPosition wordPos, SigmaPosition sigmaPos, bool isVowel)
        {
            LetterPath[] potentials = Language.Structure.GetPotentialPaths(last, wordPos, sigmaPos);
            LetterPath chosen = potentials[0]; //Add failsafes for errors. See Language.Pathing for guidelines.

            List<(char, double)> filter = isVowel ?
                chosen.Next.Where(x => Language.Alphabet.Vowels.ContainsKey(x.Item1)).ToList() :
                chosen.Next.Where(x => Language.Alphabet.Consonants.ContainsKey(x.Item1)).ToList();

            double weight = Random.NextDouble() * filter.Sum(w => w.Item2);

            foreach ((char, double) l in filter)
            {
                weight -= l.Item2;

                if (weight <= 0)
                    return l.Item1;
            }

            throw new Exception(string.Format("Letter pathing match not found: {0}, {1}, {2}, {3}", last, wordPos, sigmaPos));
        }
        #endregion
    }
}
