using PLGL.Data;
using PLGL.Processing;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace PLGL
{
    /// <summary>
    /// Allows deconstructed blocks to be merged, or whatever else. "Let's" gets split into three blocks, when it should be one.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="adjacentLeft"></param>
    /// <param name="adjacentRight"></param>
    public delegate void OnDeconstruct(LanguageGenerator lg, CharacterBlock current, CharacterBlock adjacentLeft, CharacterBlock adjacentRight);
    /// <summary>
    /// Processes author-defined filters, determining how each character block is processed.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="word"></param>
    public delegate void OnConstruct(LanguageGenerator lg, WordInfo word);
    public delegate void OnSyllable(LanguageGenerator lg, WordInfo word, SyllableInfo syllable);
    /// <summary>
    /// Allows manipulation of the generated word's letters.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="word"></param>
    /// <param name="letter"></param>
    public delegate void OnLetter(LanguageGenerator lg, WordInfo word, LetterInfo current);
    /// <summary>
    /// Allows manipulation of the word's prefixes. Called after word generation.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="word"></param>
    /// <param name="current"></param>
    public delegate void OnPrefix(LanguageGenerator lg, WordInfo word, AffixInfo current);
    /// <summary>
    /// Allows manipulation of the word's suffixes. Called after word generation.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="word"></param>
    /// <param name="current"></param>
    public delegate void OnSuffix(LanguageGenerator lg, WordInfo word, AffixInfo current);

    public class LanguageGenerator
    {
        private Language language;
        public Language Language
        {
            get { return language; }
            set
            {
                language = value;
                Deconstruct.Language = Language;
            }
        }

        public Deconstructor Deconstruct { get; private set; }
        public Diagnostics Diagnostics { get; private set; }

        StringBuilder sentenceBuilder = new StringBuilder();
        List<CharacterBlock> blocks = new List<CharacterBlock>();
        List<WordInfo> wordInfo = new List<WordInfo>();

        #region DECONSTRUCT_ methods
        public void DECONSTRUCT_ChangeFilter(CharacterBlock current, CharacterBlock left, CharacterBlock right,
            string currentFilter, string leftFilter, string rightFilter, string currentText, string newFilter)
        {
            if (left != null && right != null)
            {
                if (current.Filter.Name.ToUpper() == currentFilter &&
                    left.Filter.Name.ToUpper() == leftFilter &&
                    right.Filter.Name.ToUpper() == rightFilter)
                {
                    if (current.Text == currentText)
                    {
                        CharacterFilter filter = Deconstruct.GetFilter(newFilter);

                        if (filter != null)
                        {
                            current.Filter = filter;

                            if (Diagnostics.IsDeconstructLog == true)
                                Diagnostics.LogBuilder.AppendLine($"{currentFilter}[{currentText}] changed to {filter.Name} [left:{leftFilter}, current:{currentFilter}, right:{rightFilter}]");
                        }
                        else if (Diagnostics.IsDeconstructLog == true)
                            Diagnostics.LogBuilder.AppendLine($"Couldn't find {newFilter} filter to change {current.Text} of {current.Filter.Name}.");
                    }
                }
            }
        }
        public void DECONSTRUCT_MergeBlocks(CharacterBlock current, CharacterBlock left, CharacterBlock right,
            string currentFilter, string leftFilter, string rightFilter, string currentText, string newFilter)
        {
            if (left != null && right != null)
            {
                if (current.Filter.Name.ToUpper() == currentFilter &&
                    left.Filter.Name.ToUpper() == leftFilter &&
                    right.Filter.Name.ToUpper() == rightFilter)
                {
                    if (current.Text == currentText)
                    {
                        CharacterFilter filter = Deconstruct.GetFilter(newFilter);
                        if (Diagnostics.IsDeconstructEventLog == true)
                        {
                            if (filter == Deconstruct.Undefined && newFilter != Deconstruct.Undefined.Name.ToUpper())
                                Diagnostics.LogBuilder.AppendLine($"Couldn't find {newFilter} filter. Defaulting to Undefined.");
                            Diagnostics.LOG_Subheader($"DECONSTRUCT: Merged {leftFilter}[{left.Text}] and {rightFilter}[{right.Text}] to {currentFilter}[{currentText}] under the new filter: {newFilter}");
                        }

                        current.Text = left.Text + current.Text + right.Text;
                        current.Filter = filter;

                        left.IsAlive = false;
                        right.IsAlive = false;

                        LinkLeftBlock(current);
                        LinkRightBlock(current);
                    }
                }
            }
        }
        public void DECONSTRUCT_MergeBlocks(CharacterBlock current, CharacterBlock left, CharacterBlock right,
            string currentFilter, string leftFilter, string rightFilter, string newFilter)
        {
            if (left != null && right != null)
            {
                if (current.Filter.Name.ToUpper() == currentFilter &&
                    left.Filter.Name.ToUpper() == leftFilter &&
                    right.Filter.Name.ToUpper() == rightFilter)
                {
                    current.Text = left.Text + current.Text + right.Text;
                    current.Filter = Deconstruct.GetFilter(newFilter);

                    left.IsAlive = false;
                    right.IsAlive = false;

                    LinkLeftBlock(current);
                    LinkRightBlock(current);
                }
            }
        }
        public void DECONSTRUCT_ContainWithin(CharacterBlock current, CharacterBlock left, CharacterBlock right,
            string openFilter, string closeFilter, string newFilter)
        {
            string buildBlock = newFilter + "_Build";

            //Process opening filter. The filter "eats" right adjacent blocks until it encounters it's filter again.
            if (current.Filter.Name.ToUpper() == openFilter || current.Filter.Name.ToUpper() == buildBlock.ToUpper())
            {
                //It's not the end; therefore, absorb the block.
                if (right != null && right.Filter.Name.ToUpper() != closeFilter)
                {
                    right.Text = current.Text + right.Text;
                    right.Filter = new CharacterFilter() { Characters = Deconstruct.Undefined.Characters, Name = buildBlock };

                    current.IsAlive = false;

                    LinkLeftBlock(right);
                }
            }
            //Process end filter.
            if (current.Filter.Name.ToUpper() == closeFilter)
            {
                //It's the wrapperFilter, and the left block is built on. Therefore, this means it's the end of the block.
                if (left != null && left.Filter.Name.ToUpper() == buildBlock.ToUpper())
                {
                    current.Text = left.Text + current.Text;
                    current.Filter = Deconstruct.GetFilter(newFilter);

                    left.IsAlive = false;

                    LinkLeftBlock(current);
                }
            }
        }
        #endregion

        #region CONSTRUCT_ methods
        public void CONSTRUCT_Hide(WordInfo word, string filter)
        {
            if (word.Filter.Name.ToUpper() == filter && word.IsProcessed == false)
            {
                if (Diagnostics.IsConstructEventLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine($"{word.WordActual} of {word.Filter.Name} has been hidden.");

                word.WordFinal = string.Empty;
                if (word.Prefixes != null) word.Prefixes.Clear();
                if (word.Suffixes != null) word.Suffixes.Clear();
                word.IsProcessed = true;
            }
        }
        /// <summary>
        /// Applies to delimiters, undefined, etc.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="filter"></param>
        public void CONSTRUCT_KeepAsIs(WordInfo word, string filter)
        {
            if (word.Filter.Name.ToUpper() == filter && word.IsProcessed == false)
            {
                if (Diagnostics.IsConstructEventLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine($"{word.WordActual} of {word.Filter.Name} has been kept as is.");

                word.WordFinal = word.WordActual;
                word.IsProcessed = true;
            }
        }
        public void CONSTRUCT_Within(WordInfo word, string filter, int index, int subtract)
        {
            if (word.Filter.Name.ToUpper() == filter && word.IsProcessed == false)
            {
                word.WordFinal = word.WordActual.Substring(index, word.WordActual.Length - subtract);
                word.IsProcessed = true;

                if (Diagnostics.IsConstructEventLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine($"{word.WordActual} of {word.Filter.Name} has been set to substring[{index},{subtract}].");
            }
        }
        /// <summary>
        /// Almost exclusively applied to letter filters.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="filter"></param>
        public void CONSTRUCT_Generate(WordInfo word, string filter)
        {
            if (word.Filter.Name.ToUpper() == filter.ToUpper() && word.IsProcessed == false)
            {
                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_Subheader($"GENERATING: {word.WordActual}");

                ProcessLexiconInflections(word);
                ProcessLexiconRoots(word);
                ExtractAffixes(word);

                Random = SetRandom(word.WordRoot);
                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_NestLine(2, $"Seed set to {Seed}");

                if (string.IsNullOrEmpty(word.WordGenerated))
                {
                    PopulateSyllables(word);
                    PopulateLetters(word);
                }

                ProcessAffixes(word);
                AssembleAffixes(word);

                word.WordFinal = word.WordPrefixes + word.WordGenerated + word.WordSuffixes;
                word.IsProcessed = true;

                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine($"Finalized word: {word.WordActual} -> {word.WordFinal}" + Environment.NewLine);

                LexiconMemorize(word);

                SetCase(word);
            }
        }
        #endregion

        #region SYLLABLE_ methods


        #endregion

        #region LETTER_ methods
        public void LETTER_Replace(WordInfo word, LetterInfo current, LetterInfo target, char letterKey, bool condition)
        {
            if (condition && target != null)
                target.Letter = Language.Alphabet.Find(letterKey);
        }
        public void LETTER_Insert(WordInfo word, LetterInfo current, char letterKey, int indexOffset, bool condition)
        {
            if (condition)
            {
                int index = word.Letters.IndexOf(current);
                Letter letter = Language.Alphabet.Find(letterKey);
                word.Letters.Insert(index + indexOffset, new LetterInfo(letter) { IsProcessed = true });
            }
        }

        /// <summary>
        /// Returns true if the target letter is any of the keys listed.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool LETTER_Contains(LetterInfo target, params char[] keys)
        {
            if (target != null)
            {
                foreach (char k in keys)
                {
                    if (target.Letter.Key == k)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns true if the target's syllable matches the group pattern.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool LETTER_Syllable(LetterInfo target, params string[] group)
        {
            if (target != null)
            {
                foreach (string s in group)
                    if (target.Syllable.Syllable.Groups == s)
                        return true;
            }
            return false;
        }
        #endregion

        #region AFFIX_ methods
        /// <summary>
        /// Inserts the text into the current affix if the condition is true. Set condition to AFFIX_MatchLast, AFFIX_VowelFirst, etc.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <param name="affixKey"></param>
        /// <param name="insert"></param>
        /// <param name="insertIndex"></param>
        /// <param name="condition"></param>
        public void AFFIX_Insert(WordInfo word, AffixInfo current, string affixKey, string insert, int insertIndex, bool condition)
        {
            if (current.IsProcessed == false && current.Affix.Key.ToUpper() == affixKey.ToUpper())
            {
                if (condition)
                {
                    current.AffixText = current.AffixText.Insert(insertIndex, insert);
                    current.IsProcessed = true;
                }
            }
        }
        /// <summary>
        /// Removes part of the current affix if the condition is true. Set condition to AFFIX_MatchLast, AFFIX_VowelFirst, etc.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <param name="affixKey"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="condition"></param>
        public void AFFIX_Remove(WordInfo word, AffixInfo current, string affixKey, int index, int length, bool condition)
        {
            if (current.IsProcessed == false && current.Affix.Key.ToUpper() == affixKey.ToUpper())
            {
                if (condition)
                {
                    current.AffixText = current.AffixText.Remove(index, length);
                    current.IsProcessed = true;
                }
            }
        }

        //Boolean conditions
        /// <summary>
        /// Returns true is the adjacent left affix or generated word ends in the matching symbol.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool AFFIX_MatchLast(WordInfo word, AffixInfo current, char symbol)
        {
            Letter? compare = Language.Alphabet.Find(AFFIX_LastChar(word, current));

            if (compare != null)
                return symbol == compare.Case.lower || symbol == compare.Case.upper;

            return false;
        }
        /// <summary>
        /// Returns true if the adjacent left suffix or generated word ends in a vowel.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool AFFIX_VowelLast(WordInfo word, AffixInfo current)
        {
            char last = AFFIX_LastChar(word, current);

            if (Language.Alphabet.Vowels.ContainsKey(last))
                return true;

            return false;
        }
        /// <summary>
        /// Returns true if the adjacent left suffix or generated word ends in a consonant.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool AFFIX_ConsonantLast(WordInfo word, AffixInfo current)
        {
            char last = AFFIX_LastChar(word, current);

            if (Language.Alphabet.Consonants.ContainsKey(last))
                return true;

            return false;
        }

        /// <summary>
        /// Returns true is the adjacent right affix or generated word starts with the matching symbol.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool AFFIX_MatchFirst(WordInfo word, AffixInfo current, char symbol)
        {
            Letter? compare = Language.Alphabet.Find(AFFIX_FirstChar(word, current));

            if (compare != null)
                return symbol == compare.Case.lower || symbol == compare.Case.upper;

            return false;
        }
        /// <summary>
        /// Returns true if the adjacent right affix or generated word starts with a vowel.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool AFFIX_VowelFirst(WordInfo word, AffixInfo current)
        {
            char last = AFFIX_FirstChar(word, current);

            if (Language.Alphabet.Vowels.ContainsKey(last))
                return true;

            return false;
        }
        /// <summary>
        /// Returns true if the adjacent right affix or generated word starts with a consonant.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool AFFIX_ConsonantFirst(WordInfo word, AffixInfo current)
        {
            char last = AFFIX_FirstChar(word, current);

            if (Language.Alphabet.Consonants.ContainsKey(last))
                return true;

            return false;
        }

        /// <summary>
        /// Returns the adjacent left or generated word's last character. A common method used by other SUFFIX_ methods; you probably don't need this, but I left it public just in case.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public char AFFIX_LastChar(WordInfo word, AffixInfo current)
        {
            char last = char.MinValue;

            if (current.AdjacentLeft != null) last = current.AdjacentLeft.AffixText.Last();
            else last = word.WordGenerated.Last();

            return last;
        }
        /// <summary>
        /// Returns the adjacent right or generated word's first character. A common method used by other PREFIX_ methods; you probably don't need this, but I left it public just in case.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public char AFFIX_FirstChar(WordInfo word, AffixInfo current)
        {
            char last = char.MinValue;

            if (current.AdjacentRight != null) last = current.AdjacentRight.AffixText.First();
            else last = word.WordGenerated.First();

            return last;
        }
        #endregion

        #region CASE_ methods
        public Action<WordInfo> CASE_Capitalize { get; set; }
        public Action<WordInfo> CASE_Uppercase { get; set; }
        public Action<WordInfo> CASE_Random { get; set; }
        public void CASE_CapitalizeDefault(WordInfo word)
        {
            string result = word.WordFinal.Substring(1);
            word.WordFinal = Language.Alphabet.Upper(word.WordFinal[0]) + result;
        }
        public void CASE_UpperDefault(WordInfo word)
        {
            string result = string.Empty;
            for (int i = 0; i < word.WordFinal.Length; i++)
                result += Language.Alphabet.Upper(word.WordFinal[i]);
            word.WordFinal = result;
        }
        public void CASE_RandomDefault(WordInfo word)
        {
            Random random = new Random();

            string result = string.Empty;
            for (int i = 0; i < word.WordFinal.Length; i++)
            {
                if (random.Next(100) > 50)
                    result += Language.Alphabet.Upper(word.WordFinal[i]);//Language.Alphabet.Upper(w.WordFinal[i]);
                else
                    result += word.WordFinal[i];
            }
            word.WordFinal = result;
        }
        #endregion

        #region Syllable counting
        /// <summary>
        /// Sets how the generator counts a root's syllables. Default is EnglishSigmaCount (C/V border checking).
        /// </summary>
        public Func<string, int> SigmaCount { get; set; }
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
        #endregion

        #region Random management
        public int Seed { get; private set; }
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// The foundation of the generator. Initializes a new Random using the root word as its seed.
        /// This needs to be set in the letters filter in Language.Construction.ConstructFilter, before the call to generate a new word.
        /// This should be set to the root word.
        /// </summary>
        /// <param name="word"></param>
        public Random SetRandom(string word)
        {
            Seed = WordSeed(word.ToUpper());
            return new Random(Seed);
        }
        private int WordSeed(string word)
        {
            using var a = System.Security.Cryptography.SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word))) + Language.Options.SeedOffset;
        }
        public double NextDouble(double minimum, double maximum) { return Random.NextDouble() * (maximum - minimum) + minimum; }
        #endregion

        public LanguageGenerator()
        {
            SigmaCount = EnglishSigmaCount;

            CASE_Capitalize = CASE_CapitalizeDefault;
            CASE_Uppercase = CASE_UpperDefault;
            CASE_Random = CASE_RandomDefault;

            Deconstruct = new Deconstructor();
            Diagnostics = new Diagnostics();
            Deconstruct.Diagnostics = Diagnostics;
        }

        #region Generation — Main method and overloads
        /// <summary>
        /// Generates a new sentence from the input sentence according to the language's constraints. This overload returns the word list for debugging purposes.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<WordInfo> GenerateRaw(string sentence)
        {
            //Clear class-level variables for the next sentence.
            blocks.Clear();
            wordInfo.Clear();

            if (Diagnostics.IsLogging == true)
            {
                Diagnostics.LOG_Header("PROCESSING");
                Diagnostics.LogBuilder.AppendLine(sentence);
                Diagnostics.LogBuilder.AppendLine();
            }

            blocks = Deconstruct.Deconstruct(sentence);
            for (int i = 0; i < blocks.Count; i++)
            {
                CharacterBlock current = blocks[i];

                if (i > 0) LinkLeftBlock(current);
                if (i < blocks.Count - 1) LinkRightBlock(current);

                Language.OnDeconstruct(this, current, current.Left, current.Right);
            }

            blocks.RemoveAll((b) => b.IsAlive == false);

            AddWordInfo(blocks);
            LinkWords();

            //Loop through every word, applying the filters
            foreach (WordInfo word in wordInfo)
                Language.OnConstruct(this, word);

            if (Diagnostics.IsLogging == true)
            {
                Diagnostics.LOG_Header("FINISHED");
                for (int i = 0; i < wordInfo.Count; i++)
                    Diagnostics.LogBuilder.Append(wordInfo[i].WordFinal);
                Diagnostics.SaveLog();
            }

            return wordInfo;
        }
        /// <summary>
        /// Calls GenerateRaw(), compiling and returning the string.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string GenerateClean(string sentence)
        {
            sentenceBuilder.Clear();

            List<WordInfo> result = GenerateRaw(sentence);

            for (int i = 0; i < wordInfo.Count; i++)
                sentenceBuilder.Append(wordInfo[i].WordFinal);

            return sentenceBuilder.ToString();
        }
        /// <summary>
        /// Switches to the specified language and generates a new sentence.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string GenerateClean(Language language, string sentence) { Language = language; return GenerateClean(sentence); }
        /// <summary>
        /// Does the same as GenerateClean, while also returning the list of WordInfo.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GenerateDebug(string sentence, out List<WordInfo> info)
        {
            sentenceBuilder.Clear();

            List<WordInfo> result = GenerateRaw(sentence);
            info = result;

            for (int i = 0; i < wordInfo.Count; i++)
                sentenceBuilder.Append(wordInfo[i].WordFinal);

            return sentenceBuilder.ToString();
        }
        #endregion

        #region CharacterBlock — Processing and handling
        /// <summary>
        /// Loops through each word, and adds it to wordInfo.
        /// </summary>
        /// <param name="words"></param>
        private void AddWordInfo(List<CharacterBlock> blocks)
        {
            foreach (CharacterBlock b in blocks)
            {
                WordInfo word = new WordInfo();
                word.WordActual = b.Text;
                word.Filter = b.Filter;

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
        private void LinkLeftBlock(CharacterBlock block)
        {
            int index = blocks.IndexOf(block);

            for (int i = index - 1; i > 0; i--)
            {
                if (blocks[i].IsAlive == true)
                {
                    block.Left = blocks[i];
                    break;
                }
            }
        }
        private void LinkRightBlock(CharacterBlock block)
        {
            int index = blocks.IndexOf(block);

            for (int i = index + 1; i < blocks.Count; i++)
            {
                if (blocks[i].IsAlive == true)
                {
                    block.Right = blocks[i];
                    break;
                }
            }
        }

        /// <summary>
        /// Called by CONSTRUCT_Generate to determine the input word's case; then, it sets and applies the case to the finalized word.
        /// </summary>
        /// <param name="word"></param>
        public void SetCase(WordInfo word)
        {
            if (Language.Options.AllowAutomaticCasing == true)
            {
                bool firstLetter = false;
                int upper = 0;
                for (int i = 0; i < word.WordActual.Length; i++)
                {
                    if (char.IsUpper(word.WordActual[i]))
                    {
                        if (i == 0)
                            firstLetter = true;
                        upper++;
                    }
                }

                if (firstLetter == true && upper == 1)
                    word.Case = WordInfo.CaseType.Capitalize;
                if (word.WordActual.Length > 1)
                {
                    if (upper == word.WordActual.Length)
                        word.Case = WordInfo.CaseType.Uppercase;
                    if (upper > 1 && upper < word.WordActual.Length && Language.Options.AllowRandomCase == true)
                        word.Case = WordInfo.CaseType.RandomCase;
                }

                switch (word.Case)
                {
                    case WordInfo.CaseType.Capitalize: CASE_Capitalize(word); break;
                    case WordInfo.CaseType.Uppercase: CASE_Uppercase(word); break;
                    case WordInfo.CaseType.RandomCase: CASE_Random(word); break;
                }
            }
        }
        #endregion

        #region Lexicon (and inflections) — Custom words, word memorizing
        /// <summary>
        /// Checks for matching actual words in Language.Lexicon, and assigns the generated word to the value. Called by CONSTRUCT_Generate.
        /// </summary>
        /// <param name="word"></param>
        private void ProcessLexiconInflections(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true)
                Diagnostics.LOG_NestLine(2, "Checking for lexicon inflections");
            if (Language.Lexicon.Inflections.ContainsKey(word.WordActual.ToLower()))
            {
                word.WordGenerated = Language.Lexicon.Inflections[word.WordActual.ToLower()];
                if (Diagnostics.IsConstructLog == true)
                    Diagnostics.LOG_NestLine(2, $"Inflection found: {word.WordGenerated}");
            }
        }
        /// <summary>
        /// Checks for matching root words in Language.Lexicon, and assigns the generated word to the value. Called by CONSTRUCT_Generate.
        /// </summary>
        /// <param name="word"></param>
        public void ProcessLexiconRoots(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true)
                Diagnostics.LOG_NestLine(2, "Checking for lexicon roots");

            if (string.IsNullOrEmpty(word.WordRoot) == false && Language.Lexicon.Roots.ContainsKey(word.WordRoot.ToLower()))
            {
                word.WordGenerated = Language.Lexicon.Roots[word.WordRoot.ToLower()];
                if (Diagnostics.IsConstructLog == true)
                    Diagnostics.LOG_NestLine(2, $"Root found: {word.WordGenerated}");
            }
        }
        /// <summary>
        /// "Memorizes" the word if the option has been set and the word isn't in Lexicon.Inflections.
        /// </summary>
        /// <param name="word"></param>
        public void LexiconMemorize(WordInfo word)
        {
            if (Language.Options.MemorizeWords == true && Language.Lexicon.Inflections.ContainsKey(word.WordActual) == false)
                Language.Lexicon.Inflections.Add(word.WordActual, word.WordFinal);
        }
        #endregion

        #region Affixes — Root extraction, affix processing and assemblying
        private List<Affix> affixes = new List<Affix>();
        /// <summary>
        /// The lexemes are processed, stripping the actual word down to its root.
        /// </summary>
        /// <param name="word"></param>
        private void ExtractAffixes(WordInfo word)
        {
            affixes.Clear();

            //Extract affixes.
            affixes.AddRange(Language.Lexicon.GetPrefixes(word.WordActual));
            affixes.AddRange(Language.Lexicon.GetSuffixes(word.WordActual));

            if (affixes.Count > 0)
            {
                word.Prefixes = new List<AffixInfo>();
                word.Suffixes = new List<AffixInfo>();

                int suffixIndex = 0, prefixIndex = 0;
                AffixInfo current;

                if (Diagnostics.IsConstructLog == true)
                    Diagnostics.LOG_NestLine(2, "Checking and extracting affixes");

                for (int i = 0; i < affixes.Count; i++)
                {
                    if (affixes[i].ValueLocation == Affix.AffixLocation.Prefix)
                    {
                        word.Prefixes.Add(current = new AffixInfo() { Affix = affixes[i], AffixText = affixes[i].Value });

                        if (Language.Lexicon.IsCustomOrder == false)
                        {
                            prefixIndex++;
                            current.Order = prefixIndex;
                        }
                        else
                            current.Order = current.Affix.Order;

                        if (Diagnostics.IsConstructLog == true)
                            Diagnostics.LOG_NestLine(4, $"Prefix found: from {word.Prefixes.Last().Affix.Key} to {word.Prefixes.Last().Affix.Value} at {word.Prefixes.Last().Order} order.");
                    }

                    if (affixes[i].ValueLocation == Affix.AffixLocation.Suffix)
                    {
                        word.Suffixes.Add(current = new AffixInfo() { Affix = affixes[i], AffixText = affixes[i].Value });

                        if (Language.Lexicon.IsCustomOrder == false)
                        {
                            suffixIndex++;
                            current.Order = suffixIndex;
                        }
                        else
                            current.Order = current.Affix.Order;

                        if (Diagnostics.IsConstructLog == true)
                            Diagnostics.LOG_NestLine(4, $"Suffix found: from {word.Suffixes.Last().Affix.Key} to {word.Suffixes.Last().Affix.Value} at {word.Suffixes.Last().Order} order.");
                    }
                }

                if (Language.Lexicon.IsCustomOrder == false)
                {
                    word.Prefixes = word.Prefixes.OrderByDescending((p) => p.Order).ToList();
                    word.Suffixes = word.Suffixes.OrderByDescending((s) => s.Order).ToList();
                }

                //Strip word to root.
                int prefixLength = affixes.Where((a) => a.KeyLocation == Affix.AffixLocation.Prefix).Sum((a) => a.Key.Length);
                int suffixLength = affixes.Where((a) => a.KeyLocation == Affix.AffixLocation.Suffix).Sum((a) => a.Key.Length);
                word.WordRoot = word.WordActual.Substring(prefixLength, word.WordActual.Length - suffixLength);
                if (Diagnostics.IsConstructLog == true)
                    Diagnostics.LOG_NestLine(2, $"Extracted root {word.WordRoot} from {word.WordActual}.");
            }
            else
                word.WordRoot = word.WordActual;
        }
        /// <summary>
        /// The affixes are assembled and set in order.
        /// </summary>
        /// <param name="word"></param>
        private void AssembleAffixes(WordInfo word)
        {
            if (word.Prefixes != null && word.Prefixes.Count > 0)
            {
                foreach (AffixInfo p in word.Prefixes)
                    word.WordPrefixes += p.AffixText;

                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_NestLine(2, $"Prefixes set: {word.WordPrefixes}");
            }
            if (word.Suffixes != null && word.Suffixes.Count > 0)
            {
                foreach (AffixInfo s in word.Suffixes)
                    word.WordSuffixes += s.AffixText;
                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_NestLine(2, $"Suffixes set: {word.WordSuffixes}");
            }
        }
        /// <summary>
        /// Assigns all AffixInfo's left and right neighbours, and calls Language.OnPrefix/OnSuffix.
        /// </summary>
        /// <param name="word"></param>
        private void ProcessAffixes(WordInfo word)
        {
            if (word.Prefixes != null && Language.OnPrefix != null)
            {
                for (int i = 0; i < word.Prefixes.Count; i++)
                {
                    if (i > 0)
                        word.Prefixes[i].AdjacentLeft = word.Prefixes[i - 1];
                    if (i != word.Prefixes.Count - 1)
                        word.Prefixes[i].AdjacentRight = word.Prefixes[i + 1];
                }

                for (int i = 0; i < word.Prefixes.Count; i++)
                {
                    if (word.Prefixes[i].Affix.ValueLocation == Affix.AffixLocation.Prefix)
                        Language.OnPrefix(this, word, word.Prefixes[i]);
                }
            }

            if (word.Suffixes != null && Language.OnSuffix != null)
            {
                for (int i = 0; i < word.Suffixes.Count; i++)
                {
                    if (i > 0)
                        word.Suffixes[i].AdjacentLeft = word.Suffixes[i - 1];
                    if (i != word.Suffixes.Count - 1)
                        word.Suffixes[i].AdjacentRight = word.Suffixes[i + 1];
                }
                for (int i = 0; i < word.Suffixes.Count; i++)
                {
                    if (word.Suffixes[i].Affix.ValueLocation == Affix.AffixLocation.Suffix)
                        Language.OnSuffix(this, word, word.Suffixes[i]);
                }
            }
        }
        #endregion

        #region Word construction — Syllable structuring and letter population
        public void PopulateSyllables(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                Diagnostics.LOG_NestLine(2, $"Populating syllables");

            word.Syllables = new List<SyllableInfo>();

            int count = Math.Max((int)(SigmaCount(word.WordRoot) *
                NextDouble(Language.Options.SigmaSkewMin, Language.Options.SigmaSkewMax)), 1);

            for (int i = 0; i < count; i++)
            {
                SyllableInfo syllable = new SyllableInfo();
                syllable.Syllable = SelectSyllable();
                syllable.SyllableIndex = i;

                word.Syllables.Add(syllable);

                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_NestLine(4, $"Syllable {syllable.SyllableIndex} set to {syllable.Syllable.Groups}");
            }

            //Link adjacent syllables.
            for (int i = 0; i < word.Syllables.Count; i++)
            {
                if (i != 0)
                    word.Syllables[i].AdjacentLeft = word.Syllables[i - 1];
                if (i != word.Syllables.Count - 1)
                    word.Syllables[i].AdjacentRight = word.Syllables[i + 1];
            }

            if (Language.OnSyllable != null)
            {
                for (int i = 0; i < word.Syllables.Count; i++)
                {
                    if (word.Syllables[i].IsProcessed == false)
                        Language.OnSyllable(this, word, word.Syllables[i]);
                }
            }
        }
        public Syllable SelectSyllable()
        {
            double weight = Random.NextDouble() * Language.Structure.SortedSyllables.Sum(s => s.Weight);

            foreach (Syllable s in Language.Structure.SortedSyllables)
            {
                weight -= s.Weight;

                if (weight <= 0)
                    return s;
            }

            return null;
        }

        public void PopulateLetters(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                Diagnostics.LOG_NestLine(2, $"Populating letters from {word.Syllables.Count} syllables");

            word.Letters = new List<LetterInfo>();

            //Generate the letters according to the syllable structure.
            foreach (SyllableInfo s in word.Syllables)
            {
                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_Nest(4, $"Syllable [{s.SyllableIndex}, {s.Syllable.Groups}] set to ");
                for (int i = 0; i < s.Syllable.Template.Count; i++)
                {
                    double weight = Random.NextDouble() * s.Syllable.Template[i].Letters.Sum(w => w.weight);

                    foreach ((char, double) l in s.Syllable.Template[i].Letters)
                    {
                        weight -= l.Item2;

                        if (weight <= 0)
                        {
                            LetterInfo letter = new LetterInfo(Language.Alphabet.Find(l.Item1));
                            letter.Syllable = s;

                            word.Letters.Add(letter);

                            if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                                Diagnostics.LogBuilder.Append($"{letter.Letter.Key}");
                            break;
                        }
                    }
                }
                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine();
            }

            //Link adjacent letters.
                for (int i = 0; i < word.Letters.Count; i++)
            {
                if (i != 0) word.Letters[i].AdjacentLeft = word.Letters[i - 1];
                if (i < word.Letters.Count - 1) word.Letters[i].AdjacentRight = word.Letters[i + 1];
            }


            if (Language.OnLetter != null)
            {
                for (int i = 0; i < word.Letters.Count; i++)
                {
                    int currentCount = word.Letters.Count;

                    if (word.Letters[i].IsProcessed == false)
                    {
                        word.Letters[i].IsProcessed = true;

                        Language.OnLetter(this, word, word.Letters[i]);

                        LinkLeftLetter(word, word.Letters[i]);
                        LinkRightLetter(word, word.Letters[i]);
                    }

                    //If there is a difference (e.g, a letter was added), return to 0.
                    if (currentCount != word.Letters.Count)
                        i = 0;
                }
            }

            word.Letters.RemoveAll((l) => l.IsAlive == false);

            foreach (LetterInfo l in word.Letters)
                word.WordGenerated += l.Letter.Case.lower;
        }
        private void LinkLeftLetter(WordInfo word, LetterInfo letter)
        {
            int index = word.Letters.IndexOf(letter);

            for (int i = index - 1; i > 0; i--)
            {
                if (word.Letters[i].IsAlive == true)
                {
                    letter.AdjacentLeft = word.Letters[i];
                    break;
                }
            }
        }
        private void LinkRightLetter(WordInfo word, LetterInfo letter)
        {
            int index = word.Letters.IndexOf(letter);

            for (int i = index + 1; i < word.Letters.Count; i++)
            {
                if (word.Letters[i].IsAlive == true)
                {
                    letter.AdjacentRight = word.Letters[i];
                    break;
                }
            }
        }
        #endregion
    }
}
