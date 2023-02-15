using PLGL.Data;
using PLGL.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

namespace PLGL
{
    /// <summary>
    /// Allows deconstructed blocks to be merged, or whatever else. "Let's" gets split into three blocks, when it should be one.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="adjacentLeft"></param>
    /// <param name="adjacentRight"></param>
    public delegate void OnDeconstruct(LanguageGenerator lg, CharacterBlock current);
    /// <summary>
    /// Processes author-defined filters, determining how each character block is processed.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="word"></param>
    public delegate void OnConstruct(LanguageGenerator lg, WordInfo word);
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

    /// <summary>
    /// Called before the letter is chosen.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="selection"></param>
    /// <param name="word"></param>
    /// <param name="lastSyllable"></param>
    /// <param name="lastLetter"></param>
    public delegate void OnLetterSelect(LanguageGenerator lg, List<(Letter letter, double weight)> selection, WordInfo word,
        SyllableInfo syllable, LetterInfo lastLetter, int currentLetterIndex, int maxLetters);
    /// <summary>
    /// Called before the syllable is chosen. This allows the language author to manipulate the possible syllables in the list.
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="selection"></param>
    /// <param name="word"></param>
    /// <param name="last"></param>
    /// <param name="max"></param>
    public delegate void OnSyllableSelect(LanguageGenerator lg, List<Syllable> selection, WordInfo word,
        SyllableInfo lastSyllable, int currentSyllableIndex, int maxSyllables);

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
        public void DECONSTRUCT_ChangeFilter(CharacterBlock current, string currentFilter, string leftFilter,
            string rightFilter, string currentText, string newFilter)
        {
            if (current.Left != null && current.Right != null)
            {
                if (current.Filter.Name.ToUpper() == currentFilter &&
                    current.Left.Filter.Name.ToUpper() == leftFilter &&
                    current.Right.Filter.Name.ToUpper() == rightFilter)
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
        public void DECONSTRUCT_MergeBlocks(CharacterBlock current,
            string currentFilter, string leftFilter, string rightFilter, string currentText, string newFilter)
        {
            if (current.Left != null && current.Right != null)
            {
                if (current.Filter.Name.ToUpper() == currentFilter &&
                    current.Left.Filter.Name.ToUpper() == leftFilter &&
                    current.Right.Filter.Name.ToUpper() == rightFilter)
                {
                    if (current.Text == currentText)
                    {
                        CharacterFilter filter = Deconstruct.GetFilter(newFilter);
                        if (Diagnostics.IsDeconstructEventLog == true)
                        {
                            if (filter == Deconstruct.Undefined && newFilter != Deconstruct.Undefined.Name.ToUpper())
                                Diagnostics.LogBuilder.AppendLine($"Couldn't find {newFilter} filter. Defaulting to Undefined.");
                            Diagnostics.LOG_Subheader($"DECONSTRUCT: Merged {leftFilter}[{current.Left.Text}] and {rightFilter}[{current.Right.Text}] to {currentFilter}[{currentText}] under the new filter: {newFilter}");
                        }

                        current.Text = current.Left.Text + current.Text + current.Right.Text;
                        current.Filter = filter;

                        current.Left.IsAlive = false;
                        current.Right.IsAlive = false;

                        LinkLeftBlock(current);
                        LinkRightBlock(current);
                    }
                }
            }
        }
        public void DECONSTRUCT_MergeBlocks(CharacterBlock current,
            string currentFilter, string leftFilter, string rightFilter, string newFilter)
        {
            if (current.Left != null && current.Right != null)
            {
                if (current.Filter.Name.ToUpper() == currentFilter &&
                    current.Left.Filter.Name.ToUpper() == leftFilter &&
                    current.Right.Filter.Name.ToUpper() == rightFilter)
                {
                    current.Text = current.Left.Text + current.Text + current.Right.Text;
                    current.Filter = Deconstruct.GetFilter(newFilter);

                    current.Left.IsAlive = false;
                    current.Right.IsAlive = false;

                    LinkLeftBlock(current);
                    LinkRightBlock(current);
                }
            }
        }
        public void DECONSTRUCT_ContainWithin(CharacterBlock current,
            string openFilter, string closeFilter, string newFilter)
        {
            string buildBlock = newFilter + "_Build";

            //Process opening filter. The filter "eats" right adjacent blocks until it encounters it's filter again.
            if (current.Filter.Name.ToUpper() == openFilter || current.Filter.Name.ToUpper() == buildBlock.ToUpper())
            {
                //It's not the end; therefore, absorb the block.
                if (current.Right != null && current.Right.Filter.Name.ToUpper() != closeFilter)
                {
                    current.Right.Text = current.Text + current.Right.Text;
                    current.Right.Filter = new CharacterFilter() { Characters = Deconstruct.Undefined.Characters, Name = buildBlock };

                    current.IsAlive = false;

                    LinkLeftBlock(current.Right);
                }
            }
            //Process end filter.
            if (current.Filter.Name.ToUpper() == closeFilter)
            {
                //It's the wrapperFilter, and the left block is built on. Therefore, this means it's the end of the block.
                if (current.Left != null && current.Left.Filter.Name.ToUpper() == buildBlock.ToUpper())
                {
                    current.Text = current.Left.Text + current.Text;
                    current.Filter = Deconstruct.GetFilter(newFilter);

                    current.Left.IsAlive = false;

                    LinkLeftBlock(current);
                }
            }
        }
        #endregion

        #region CONSTRUCT_ methods
        /// <summary>
        /// Sets the final part of the word to empty, effectively excluding the result from the output.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="filter"></param>
        public void CONSTRUCT_Hide(WordInfo word, string filter)
        {
            if (word.Filter.Name.ToUpper() == filter.ToUpper() && word.IsProcessed == false)
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
        /// Replaces all input characters found in WordInfo.WordActual with their respective output characters.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="filter"></param>
        /// <param name="keyValues"></param>
        public void CONSTRUCT_Replace(WordInfo word, string filter, params (char input, char output)[] keyValues)
        {
            if (word.Filter.Name.ToUpper() == filter.ToUpper() && word.IsProcessed == false)
            {
                word.WordFinal = word.WordActual;

                foreach ((char input, char output) kvp in keyValues)
                    word.WordFinal = word.WordFinal.Replace(kvp.input, kvp.output);

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
            if (word.Filter.Name.ToUpper() == filter.ToUpper() && word.IsProcessed == false)
            {
                if (Diagnostics.IsConstructEventLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine($"{word.WordActual} of {word.Filter.Name} has been kept as is.");

                word.WordFinal = word.WordActual;
                word.IsProcessed = true;
            }
        }
        /// <summary>
        /// Sets the final word to a substring of the actual word. This is useful for [Escape] filters, where you need the brackets removed.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="filter"></param>
        /// <param name="index"></param>
        /// <param name="subtract"></param>
        public void CONSTRUCT_Within(WordInfo word, string filter, int index, int subtract)
        {
            if (word.Filter.Name.ToUpper() == filter.ToUpper() && word.IsProcessed == false)
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

                ProcessLexiconVocabulary(word);
                if (word.SkipLexemes == false)
                    ExtractAffixes(word);
                else
                    word.WordRoot = word.WordActual;
                ProcessLexiconRoots(word);

                Random = SEED_SetRandom(word.WordRoot);
                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_NestLine(2, $"Seed set to {Seed}");

                if (word.IsRootMatch == false && word.IsVocabMatch == false)
                {
                    PopulateSyllables(word);
                    PopulateLetters(word);
                }

                ProcessAffixes(word);
                AssembleAffixes(word);

                if (word.IsVocabMatch == false)
                    word.WordFinal = word.WordPrefixes + word.WordGenerated + word.WordSuffixes;

                word.IsProcessed = true;

                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LogBuilder.AppendLine($"Finalized word: {word.WordActual} -> {word.WordFinal}" + Environment.NewLine);

                LexiconMemorize(word);

                SetCase(word);
            }
        }
        #endregion

        #region WORD_ methods
        /// <summary>
        /// If the filter matches, this returns the last occurence of the word, relative to the current word.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public WordInfo? WORD_LastByFilter(WordInfo current, string filter)
        {
            for (int i = wordInfo.IndexOf(current) - 1; i >= 0; i--)
            {
                if (wordInfo[i].Filter.Name.ToUpper() == filter.ToUpper())
                    return wordInfo[i];
            }
            return null;
        }
        /// <summary>
        /// If the filter matches, this returns the next occurence of the word, relative to the current word.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public WordInfo? WORD_NextByFilter(WordInfo current, string filter)
        {
            for (int i = wordInfo.IndexOf(current) + 1; i < wordInfo.Count; i++)
            {
                if (wordInfo[i].Filter.Name.ToUpper() == filter.ToUpper())
                    return wordInfo[i];
            }
            return null;
        }
        #endregion

        #region SELECT_ methods

        /// <summary>
        /// Excludes the letter from the selection process if the condition is true.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="condition"></param>
        public void SELECT_Exclude(bool condition, params char[] letters)
        {
            if (condition == true)
            {
                if (Diagnostics.IsSelectEventLog == true)
                    Diagnostics.LOG_NestLine(6, $"Excluding {letters.Length} keys from group selection: {string.Join('/', letters)}");

                foreach (char letter in letters)
                    letterSelection.RemoveFirst(l => l.letter.Key == letter);
            }
        }
        /// <summary>
        /// Excludes the syllable group from the selection process if the condition is true.
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="condition"></param>
        public void SELECT_Exclude(bool condition, params string[] groups)
        {
            if (condition == true)
            {
                if (Diagnostics.IsSelectEventLog == true)
                    Diagnostics.LOG_NestLine(6, $"Excluding {groups.Length} groups from syllable selection: {string.Join('/', groups)}");

                foreach (string group in groups)
                    syllableSelection.RemoveFirst(s => s.Letters == group);
            }
        }
        /// <summary>
        /// Removes all syllables from the list, keeping only the groups specified.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="groups"></param>
        public void SELECT_Keep(bool condition, params string[] groups)
        {
            if (condition == true)
            {
                syllableSelection.Clear();
                foreach (string s in groups)
                    if (Language.Structure.Syllables.ContainsKey(s))
                        syllableSelection.Add(Language.Structure.Syllables[s]);
            }
        }

        /// <summary>
        /// Sets the weight multiplier of the syllable, if the group is found and the condition is met.
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="multiplier"></param>
        /// <param name="condition"></param>
        public void SELECT_SetWeight(string groups, double multiplier, bool condition)
        {
            if (condition == true)
            {
                Syllable s = syllableSelection.Where(s => s.Letters == groups).FirstOrDefault();

                if (s != null)
                    s.WeightMultiplier = multiplier;
            }
        }
        /// <summary>
        /// Sets the weight multiplier of the letter, if the letter is found and the condition is met.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="multiplier"></param>
        /// <param name="condition"></param>
        public void SELECT_SetWeight(char letter, double multiplier, bool condition)
        {
            if (condition == true)
            {
                Letter l = Language.Alphabet.Find(letter);

                if (l != null)
                    l.WeightMultiplier *= multiplier;
            }
        }

        /// <summary>
        /// Returns true if the target syllable's group contains any parts specified in string group.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool SELECT_GroupContains(SyllableInfo target, string group)
        {
            return target.Syllable.Letters.Contains(group);
        }
        /// <summary>
        /// Returns true if the target syllable's group matches any of the groups.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public bool SELECT_GroupAny(SyllableInfo target, params string[] groups)
        {
            foreach (string g in groups)
                if (target.Syllable.Letters == g) return true;

            return false;
        }
        /// <summary>
        /// Returns true if the syllable index is 0.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool SELECT_IsSyllableFirst(SyllableInfo target)
        {
            return target.SyllableIndex == 0;
        }
        /// <summary>
        /// Returns true if the syllable index matches the count of syllables in it's parent word.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool SELECT_IsSyllableLast(SyllableInfo target)
        {
            return target.SyllableIndex == target.Word.Syllables.Count - 1;
        }
        /// <summary>
        /// Returns true if IsSyllableFirst and IsSyllableLast are both false.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool SELECT_IsSyllableMiddle(SyllableInfo target)
        {
            return (SELECT_IsSyllableFirst(target) && SELECT_IsSyllableLast(target)) == false;
        }
        /// <summary>
        /// Returns true if the first group in the target syllable matches any of the parameter keys.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public bool SELECT_IsGroupFirst(SyllableInfo target, params char[] groupKeys)
        {
            foreach (char g in groupKeys)
                if (target.Syllable.Letters.StartsWith(g))
                    return true;
            return false;
        }
        /// <summary>
        /// Returns true if the last group in the target syllable matches any of the parameter keys.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public bool SELECT_IsGroupLast(SyllableInfo target, params char[] groupKeys)
        {
            foreach (char g in groupKeys)
                if (target.Syllable.Letters.EndsWith(g))
                    return true;
            return false;
        }
        /// <summary>
        /// Returns true if IsGroupFirst and IsGroupLast are both false.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public bool SELECT_IsGroupMiddle(SyllableInfo target, params char[] groupKey)
        {
            return (SELECT_IsGroupFirst(target, groupKey) &&
                    SELECT_IsGroupLast(target, groupKey)) == false;
        }

        /// <summary>
        /// Returns true if the syllable contains any of the tags. Case-sensitive.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool SELECT_TagsAny(Syllable target, params string[] tags)
        {
            foreach (string s in tags)
            {
                if (target.Tags.Contains(s))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Returns true if the syllable contains all of the tags. Case-sensitive.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool SELECT_TagsAll(Syllable target, params string[] tags)
        {
            int matched = 0;
            foreach (string s in tags)
            {
                if (target.Tags.Contains(s))
                    matched++;
            }
            return matched == tags.Length;
        }

        public LetterGroup SELECT_Template(SyllableInfo target, int position)
        {
            return target.Syllable.Template[position];
        }
        public LetterInfo SELECT_Letter(SyllableInfo target, char groupKey)
        {
            return target.Letters.Where(l => l.Group.Key == groupKey).FirstOrDefault();
        }
        public char[] SELECT_GroupExcept(LetterGroup target, params char[] keep)
        {
            List<char> result = new();

            if (keep.Length > 0)
            {
                for (int i = 0; i < target.Letters.Count; i++)
                {
                    if (keep.Contains(target.Letters[i].letter.Key) == false)
                        result.Add(target.Letters[i].letter.Key);
                }
            }

            return result.ToArray();
        }
        #endregion

        #region SYLLABLE_ methods
        /// <summary>
        /// If the condition is true (and the group is valid), replaces the target syllable with the new syllable group.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="syllable"></param>
        /// <param name="target"></param>
        /// <param name="group"></param>
        /// <param name="condition"></param>
        public void SYLLABLE_Replace(WordInfo word, SyllableInfo syllable, SyllableInfo target, string group, bool condition)
        {
            if (condition && target != null)
            {
                if (Language.Structure.Syllables.ContainsKey(group))
                    target.Syllable = Language.Structure.Syllables[group];
            }
        }

        /// <summary>
        /// Returns true if the syllable matches the group pattern.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool SYLLABLE_Any(SyllableInfo target, params string[] group)
        {
            if (target != null)
            {
                foreach (string s in group)
                    if (target.Syllable.Letters == s)
                        return true;
            }
            return false;
        }
        /// <summary>
        /// Returns true if the target syllable starts with any of the LetterGroup keys.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="groups">Set to any defined letter group keys.</param>
        /// <returns></returns>
        public bool SYLLABLE_Starts(SyllableInfo target, params char[] groups)
        {
            if (target != null)
            {
                foreach (char g in groups)
                {
                    if (target.Syllable.Template.First().Key == g)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns true if the target syllable ends with any of the LetterGroup keys.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public bool SYLLABLE_Ends(SyllableInfo target, params char[] groups)
        {
            if (target != null)
            {
                foreach (char g in groups)
                {
                    if (target.Syllable.Template.Last().Key == g)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region LETTER_ methods
        public void LETTER_Replace(LetterInfo target, char letterKey, bool condition)
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
        public bool LETTER_Any(LetterInfo target, params char[] keys)
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
        /// <param name="letterGroups"></param>
        public void AFFIX_Insert(WordInfo word, AffixInfo current, string affixKey, string insert,
            int insertIndex, bool condition)
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
        /// Returns true if the adjacent left affix or generated word ends in any of the matching symbols.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public bool AFFIX_MatchLast(WordInfo word, AffixInfo current, params char[] symbols)
        {
            Letter? compare = Language.Alphabet.Find(AFFIX_LastChar(word, current));

            if (compare != null)
            {
                foreach (char s in symbols)
                if (s == compare.Case.lower || s == compare.Case.upper)
                    return true;
            }

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
        /// Returns true if the adjacent right affix or generated word ends in any of the matching symbols.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="current"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public bool AFFIX_MatchFirst(WordInfo word, AffixInfo current, params char[] symbols)
        {
            Letter? compare = Language.Alphabet.Find(AFFIX_FirstChar(word, current));

            if (compare != null)
            {
                foreach (char s in symbols)
                    if (s == compare.Case.lower || s == compare.Case.upper)
                        return true;
            }

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
            else
            {
                if (string.IsNullOrEmpty(word.WordGenerated) == false)
                    last = word.WordGenerated.Last();
                else
                    last = word.WordFinal.Last();
            }

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

        #region Random management
        public int Seed { get; set; }
        public bool SEED_EndsAny(params int[] numbers)
        {
            foreach (int n in numbers)
            {
                if (Seed.ToString().EndsWith(n.ToString()))
                    return true;
            }
            return false;
        }

        public Random Random { get; set; } = new Random();
        /// <summary>
        /// The foundation of the generator. Initializes a new Random using the root word as its seed.
        /// This needs to be set in the letters filter in Language.Construction.ConstructFilter,
        /// before the call to generate a new word, and should be set to the root word.
        /// </summary>
        /// <param name="word"></param>
        public Random SEED_SetRandom(string word)
        {
            Seed = SEED_Generate(word.ToUpper());
            return new Random(Seed);
        }
        public Random SEED_SetRandom(int seed)
        {
            Seed = seed;
            return new Random(Seed);
        }
        public int SEED_Generate(string word)
        {
            using var a = System.Security.Cryptography.SHA1.Create();
            return BitConverter.ToInt32(a.ComputeHash(Encoding.UTF8.GetBytes(word))) + Language.Options.SeedOffset;
        }
        public double NextDouble(double minimum, double maximum) { return Random.NextDouble() * (maximum - minimum) + minimum; }
        #endregion

        public LanguageGenerator()
        {
            CASE_Capitalize = CASE_CapitalizeDefault;
            CASE_Uppercase = CASE_UpperDefault;
            CASE_Random = CASE_RandomDefault;

            Deconstruct = new Deconstructor();
            Diagnostics = new Diagnostics(this);
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

                Language.OnDeconstruct(this, current);
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

        #region Lexicon — Custom words, word memorizing
        /// <summary>
        /// Checks for matching actual words in Language.Lexicon, and assigns the generated word to the value. Called by CONSTRUCT_Generate.
        /// </summary>
        /// <param name="word"></param>
        private void ProcessLexiconVocabulary(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true)
                Diagnostics.LOG_NestLine(2, "Checking for lexicon vocabulary");

            if (Language.Lexicon.Vocabulary.ContainsKey(word.WordActual))
            {
                word.WordFinal = Language.Lexicon.Vocabulary[word.WordActual];
                word.IsVocabMatch = true;

                if (Diagnostics.IsConstructLog == true)
                    Diagnostics.LOG_NestLine(2, $"Vocabulary found: {word.WordGenerated}");
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

            if (string.IsNullOrEmpty(word.WordRoot) == false && Language.Lexicon.Roots.ContainsKey(word.WordRoot))
            {
                word.WordGenerated = Language.Lexicon.Roots[word.WordRoot];
                word.IsRootMatch = true;

                if (Diagnostics.IsConstructLog == true)
                    Diagnostics.LOG_NestLine(2, $"Root found: {word.WordGenerated}");
            }
        }
        /// <summary>
        /// "Memorizes" the word if the option has been set and the word isn't in Lexicon.Vocabulary
        /// </summary>
        /// <param name="word"></param>
        public void LexiconMemorize(WordInfo word)
        {
            if (Language.Options.MemorizeWords == true && Language.Lexicon.Vocabulary.ContainsKey(word.WordActual) == false)
                Language.Lexicon.Vocabulary.Add(word.WordActual, word.WordFinal);
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
                        word.Prefixes.Add(current = new AffixInfo()
                        {
                            Affix = affixes[i],
                            AffixText = affixes[i].Value,
                            LetterGroups = affixes[i].LetterGroups
                        });

                        if (Language.Options.AffixCustomOrder == false)
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
                        word.Suffixes.Add(current = new AffixInfo()
                        {
                            Affix = affixes[i],
                            AffixText = affixes[i].Value,
                            LetterGroups = affixes[i].LetterGroups
                        });
                        if (Language.Options.AffixCustomOrder == false)
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

                if (Language.Options.AffixCustomOrder == false)
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

        #region Word construction — Syllable and letter populating
        public void PopulateSyllables(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                Diagnostics.LOG_NestLine(2, $"Populating syllables");

            word.Syllables = new List<SyllableInfo>();

            if (Language.Lexicon.Syllables.ContainsKey(word.WordRoot))
            {
                Syllable[] syllables = Language.Lexicon.Syllables[word.WordRoot];

                for (int i = 0; i < syllables.Length; i++)
                {
                    SyllableInfo syllable = new SyllableInfo();
                    syllable.Syllable = syllables[i];
                    syllable.SyllableIndex = i;
                    syllable.Word = word;

                    word.Syllables.Add(syllable);

                    if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                        Diagnostics.LOG_NestLine(4, $"Syllable {syllable.SyllableIndex} found in Lexicon: {syllable.Syllable.Letters}");
                }
            }
            else
            {
                //Estimate syllable count.
                int syllables = Language.Options.CountSyllables(word.WordRoot);
                //Choose multiplying factor between the min and max functions, with the word-seeded random.
                double modifier = NextDouble(Language.Options.SyllableSkewMin(syllables), Language.Options.SyllableSkewMax(syllables));
                //Make sure the final count is never below 1.
                int count = (int)Math.Max(syllables * modifier, 1);

                for (int i = 0; i < count; i++)
                {
                    SyllableInfo syllable = new SyllableInfo();
                    syllable.Syllable = SelectSyllable(word, i, count);
                    syllable.SyllableIndex = i;
                    syllable.Word = word;

                    word.Syllables.Add(syllable);

                    if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                        Diagnostics.LOG_NestLine(4, $"Syllable {syllable.SyllableIndex} set to {syllable.Syllable.Letters}");
                }
            }

            //Link adjacent syllables.
            for (int i = 0; i < word.Syllables.Count; i++)
            {
                if (i != 0)
                    word.Syllables[i].AdjacentLeft = word.Syllables[i - 1];
                if (i != word.Syllables.Count - 1)
                    word.Syllables[i].AdjacentRight = word.Syllables[i + 1];
            }
        }
        public void PopulateLetters(WordInfo word)
        {
            if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                Diagnostics.LOG_NestLine(2, $"Populating letters from {word.Syllables.Count} syllables");

            word.Letters = new List<LetterInfo>();

            //Generate the letters according to the syllable structure.
            for (int s = 0; s < word.Syllables.Count; s++)
            {
                word.Syllables[s].Letters = new List<LetterInfo>();

                for (int i = 0; i < word.Syllables[s].Syllable.Template.Count; i++)
                {
                    Letter selection = SelectLetter(word, word.Syllables[s], word.Syllables[s].Syllable.Template[i], i, word.Syllables[s].Syllable.Template.Count - 1);
                    LetterInfo letter = new LetterInfo(selection);
                    letter.Syllable = word.Syllables[s];
                    letter.Group = word.Syllables[s].Syllable.Template[i];

                    word.Letters.Add(letter);
                    word.Syllables[s].Letters.Add(letter);
                }

                if (Diagnostics.IsConstructLog == true && Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    Diagnostics.LOG_NestLine(4, $"Syllable {word.Syllables[s].Syllable.Letters} set to {string.Concat(word.Syllables[s].Letters)}");
            }

            //Link adjacent letters.
            for (int i = 0; i < word.Letters.Count; i++)
            {
                if (i != 0) word.Letters[i].AdjacentLeft = word.Letters[i - 1];
                if (i < word.Letters.Count - 1) word.Letters[i].AdjacentRight = word.Letters[i + 1];
            }

            foreach (LetterInfo l in word.Letters)
                word.WordGenerated += l.Letter.Case.lower;
        }
        private void AssembleWord(WordInfo word)
        {
            foreach (SyllableInfo syllable in word.Syllables)
            {
                foreach (LetterInfo letter in syllable.Letters)
                    syllable.Result += letter.Letter.Case.lower;
            }
        }

        private List<Syllable> syllableSelection = new List<Syllable>();
        public Syllable SelectSyllable(WordInfo word, int current, int max)
        {
            Language.Structure.ResetWeights();
            syllableSelection = Language.Structure.SortedSyllables.ToList();
            Language.OnSyllableSelection?.Invoke(this, syllableSelection, word,
                word.Syllables.LastOrDefault(), current, max);

            double weight = Random.NextDouble() * syllableSelection.Sum(s => s.Weight * s.WeightMultiplier);

            foreach (Syllable s in syllableSelection)
            {
                weight -= s.Weight * s.WeightMultiplier;

                if (weight <= 0)
                    return s;
            }

            return null;
        }

        private List<(Letter letter, double weight)> letterSelection = new List<(Letter letter, double weight)>();
        public Letter SelectLetter(WordInfo word, SyllableInfo syllable, LetterGroup group, int current, int max)
        {
            Language.Alphabet.ResetWeights();
            letterSelection = group.Letters.ToList();
            Language.OnLetterSelection?.Invoke(this, letterSelection, word,
                syllable, word.Letters.LastOrDefault(), current, max);

            double weight = Random.NextDouble() * letterSelection.Sum(w => w.weight * w.letter.WeightMultiplier);

            foreach ((Letter letter, double weight) l in letterSelection)
            {
                weight -= (l.weight * l.letter.WeightMultiplier);

                if (weight <= 0)
                    return l.letter;
            }
            return null;
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
