﻿using LanguageReimaginer.Data;
using LanguageReimaginer.Data.Elements;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageReimaginer.Operators
{
    /// <summary>
    /// Generator breakdown:
    /// 
    /// 1. Flagging. All valid flags are read, and changes are applied.
    /// 2. Lexemes. All valid affixes are grabbed and separated. The root word is retrieved.
    /// 3. Preparation. The generated word's foundation is prepared.
    ///     - The syllable count of the root word is estimated, by checking the boundaries of "VC" and "CV".
    ///       Example: in·ter·est·ing·ly -> VC·CVC·VC(C)·V(C)C·CV. Double consonants/vowels are stripped (ignored).
    ///     - The syllable structure is generated, according to the language author's specifications. "CCV·VC·VC·CV"
    ///     - The first letter is chosen, according to the first sigma's C/V, and the language's letter weight distribution.
    ///     - The letter pathways are generated by the language author's constraints.
    /// 4. The sigmas are merged together, with any punctuation in between preserved. Affixes are prepended and appended in order.
    /// 5. The word is complete! Repeat for the next word until the sentence is processed and returned.
    /// 
    ///     Lexemes in action: "interestingly"
    ///         |est| is handled entirely by the generator.
    ///         |inter|, |ing|, and |ly| are all affixes. They are handles by the lexeme rules.
    ///     
    ///     Flags in action: "Bateman's**xp" and "dog's**p"
    ///         |Bateman's| is skipped entirely. **x skips generation. **p skips contraction rules, however, **x also superceeds it.
    ///         |'s| is processed according to the language's possessive rules. |dog| is processed as normal.
    ///         
    ///     
    /// 
    /// </summary>
    public class LanguageGenerator
    {
        internal RandomGenerator RanGen { get; set; }
        //internal SyllableGenerator SyllableGen { get; set; }

        List<WordInfo> wordInfo = new List<WordInfo>();
        List<SigmaInfo> sigmaInfo = new List<SigmaInfo>();
        StringBuilder sentenceBuilder = new StringBuilder();

        public Language Language { get; set; }

        public LanguageGenerator() { }
        public LanguageGenerator(Language Language) { this.Language = Language; }

        public string Generate(string sentence, out List<WordInfo> info)
        {
            sentenceBuilder.Clear();
            wordInfo.Clear();

            //Pre: Split words by delimiter and add to WordInfo list.

            //Deconstructing a word.
            //1. Punctuation marks (1 of 2). Loop through WordInfo list, isolate all punctuation marks (e.g., comma, or quote/comma),
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
            //9. The new affixes are retrieve and placed in order.
            //10. The new punctuation marks are adding to the end of the word.


            string[] words = Regex.Split(sentence, "@\"(?<=[" + string.Join("", Language.Options.Delimiters) + "])\"");

            //Loop through split words and add to wordInfo list.
            foreach (string s in words)
            {
                WordInfo word = new WordInfo();
                word.WordActual = s;

                wordInfo.Add(word);
            }

            //Link adjacent words.
            for (int i = 0; i < wordInfo.Count; i++)
            {
                if (i != 0)
                    wordInfo[i].AdjacentLeft = wordInfo[i - 1];
                if (i != wordInfo.Count)
                    wordInfo[i].AdjacentRight = wordInfo[i - 1];
            }

            foreach (WordInfo part in wordInfo)
            {
                //Check for flags
                //Check for punctuation marks. If the sentence contains any, then: isolate (Add before or after
                //Check for and process lexemes.

                //sentenceBuilder.Append(NextWord(s) + " ");
            }

            info = wordInfo;
            return sentenceBuilder.ToString();
        }
    }
}
