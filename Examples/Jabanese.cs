using PLGL.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Examples
{
    public class Jabanese : Language
    {
        //Resources:
        // - https://en.wikipedia.org/wiki/Hiragana
        // 

        public Jabanese()
        {
            META_Author = "Highverve";
            META_Name = "Jabanese";
            META_Nickname = "Jab";
            META_Description = "A fictional language that matches certain Japanese rules.";

            SetOptions();

            //Processing
            SetFilters();
            SetDeconstructEvents();
            SetConstructEvents();

            //Structural
            SetLetters();
            SetSyllables();

            //Lexemes
            SetLexicon();
            SetAffixes();
        }

        private void SetOptions()
        {
            Options.LetterPathing = LanguageOptions.PathSelection.Inclusion;
            Options.MemorizeWords = true;
            Options.SyllableSkewMin = (count) =>
            {
                if (count == 1) return 1.5;
                if (count == 2) return 1.5;
                if (count == 3) return 0.8;
                return 1.0;
            };
            Options.SyllableSkewMax = (count) =>
            {
                if (count == 1) return 2.5;
                if (count == 2) return 1.8;
                if (count == 3) return 1.5;

                return 1.5;               
            };
            Options.SeedOffset = 6;

            Options.AllowAutomaticCasing = false;
            Options.AllowRandomCase = false;

            Options.CountSyllables = Options.EnglishSyllableCount;
        }

        private void SetFilters()
        {
            AddFilter("Delimiter", " ");
            AddFilter("Compound", "");//-

            //The "content" filters.
            AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            AddFilter("Numbers", "1234567890");
            AddFilter("Punctuation", ".,?!;:'\"-#$%()");

            //These filters manipulate other filters.
            AddFilter("Escape", "[]");
            AddFilter("Flags", "");
            AddFilter("FlagsOpen", "{");
            AddFilter("FlagsClose", "}");
        }
        private void SetDeconstructEvents()
        {
            //If the filter is PUNCTUATION, its text equals a single apostrophe,
            //and its neighbours are LETTERS, merge with neighbours and set to LETTERS.
            OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");

            //If the filter is PUNCTUATION, its text equals a period,
            //and its neighbours are NUMBERS, merge with neighbours and set to NUMBERS.
            OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");

            //Does the same as above, except checks for a comma.
            OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");

            //If the filter if PUNCTUATION, its text equals a hyphen,
            //and its neighbours are LETTERS, change the filter into COMPOUND.
            OnDeconstruct += (lg, current) => lg.DECONSTRUCT_ChangeFilter(current, "PUNCTUATION", "LETTERS", "LETTERS", "-", "COMPOUND");

            //If the filter is LETTERS, and its neighbours are ESCAPE, merge with neighbours and set to ESCAPE.
            OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");

            //Merges all blocks contained within FLAGSOPEN and FLAGSCLOSED into a single FLAGS filter block.
            OnDeconstruct += (lg, current) => lg.DECONSTRUCT_ContainWithin(current, "FLAGSOPEN", "FLAGSCLOSE", "FLAGS");
        }
        private void SetConstructEvents()
        {
            //Keeps UNDEFINED and DELIMITER as is. The final result will equal the initial value (Word.WordActual).
            OnConstruct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            OnConstruct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");

            //Removes the hyphen in the COMPOUND filter. A purely visual choice.
            OnConstruct += (lg, word) => lg.CONSTRUCT_Replace(word, "COMPOUND", ('-', ' '));

            //Generates each LETTERS block to a new word, according to the constraints of the language.
            OnConstruct += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");

            //Sets the final result to the substring of the intial value, thus removing and open and closing brackets.
            OnConstruct += (lg, word) => lg.CONSTRUCT_Within(word, "ESCAPE", 1, 2);

            SetPunctuation();
            SetFlagging();
            SetNumbers();
        }
        private void SetPunctuation()
        {
            OnConstruct += (lg, word) => Punctuation.Process(lg, word, "PUNCTUATION");

            Punctuation.Add(".", (w) => { return "。"; });
            Punctuation.Add(",", (w) => { return "、"; });
            Punctuation.Add(";", (w) => { return "〜"; });
            Punctuation.Add("/", (w) => { return "・"; });
            Punctuation.Add("?", (w) => { return "？"; });
            Punctuation.Add("!", (w) => { return "！"; });
            Punctuation.Add("$", (w) => { return "¥"; });
        }
        private void SetNumbers()
        {
        }
        private void SetFlagging()
        {
        }

        private void SetLetters()
        {
            //aiueo
            Alphabet.AddVowel("", 'a', ('a', 'A'), "/a/ (crop)");
            Alphabet.AddVowel("", 'i', ('i', 'I'), "/i/ (beet)");
            Alphabet.AddVowel("", 'u', ('u', 'U'), "/u/ (noon)");
            Alphabet.AddVowel("", 'e', ('e', 'E'), "/e/ (west)");
            Alphabet.AddVowel("", 'o', ('o', 'O'), "/o/ (yo)");

            //ksŝtċţnhmyrwgzjdpb
            Alphabet.AddConsonant('k', ('k', 'K'));
            Alphabet.AddConsonant('s', ('s', 'S')); //Exclude i, keep the rest.
            Alphabet.AddConsonant("", 'ŝ', ('ŝ', 'Ŝ'), "SH"); //Only shi, all other vowels must be excluded.
            Alphabet.AddConsonant('t', ('t', 'T')); //
            Alphabet.AddConsonant("", 'ċ', ('ċ', 'Ċ'), "CH");
            Alphabet.AddConsonant("", 'ţ', ('ţ', 'Ţ'), "TSU"); //Only tsu, all other vowels must be excluded.
            Alphabet.AddConsonant('n', ('n', 'N'));
            Alphabet.AddConsonant('h', ('h', 'H')); //Exclude u; this turns into fu.
            Alphabet.AddConsonant('f', ('f', 'F')); //Only fu.
            Alphabet.AddConsonant('m', ('m', 'M'));
            Alphabet.AddConsonant('y', ('y', 'Y')); //Only ya, yu, yo. Exclude yi and ye.
            Alphabet.AddConsonant('r', ('r', 'R'));
            Alphabet.AddConsonant('w', ('w', 'W')); //Only wa, exclude all other vowels.

            Alphabet.AddConsonant('g', ('g', 'G'));
            Alphabet.AddConsonant('z', ('z', 'Z')); //Exclude zi; this turns into ji.
            Alphabet.AddConsonant('j', ('j', 'J')); //Only ji, exclude all other vowels.
            Alphabet.AddConsonant('d', ('d', 'D'));
            Alphabet.AddConsonant('p', ('p', 'P'));
            Alphabet.AddConsonant('b', ('b', 'B'));
        }
        private void SetSyllables()
        {
            Structure.AddGroup('V', "Vowels", ('a', 5.0), ('i', 5.0), ('u', 5.0), ('e', 5.0), ('o', 5.0));
            Structure.AddGroup('C', "Consonants",
                ('k', 5.0), ('s', 4.0), ('ŝ', 1.0), ('t', 4.0), ('ċ', 1.0), ('ţ', 1.0),
                ('n', 3.0), ('h', 1.0), ('m', 2.0), ('y', 2.0), ('r', 2.0), ('w', 0.5),
                ('g', 5.0), ('z', 1.0), ('j', 0.5), ('d', 5.0), ('b', 4.0), ('p', 2.0));

            //Z and D in diagraph will get transformed to in romanize method,
            //and their distinctive kana in ToHiragana.
            Structure.AddGroup('v', "Vowels (Diagraphs)", ('a', 5.0), ('u', 5.0), ('o', 5.0));
            Structure.AddGroup('Y', "Diagraphs", ('y', 1.0));
            Structure.AddGroup('H', "Diagraphs (Sh / Ch)", ('h', 1.0));
            Structure.AddGroup('c', "Consonants (Diagraphs)",
                ('k', 5.0), ('ŝ', 1.0), ('ċ', 1.0),
                ('n', 3.0), ('h', 1.0), ('m', 2.0), ('r', 2.0),
                ('g', 5.0), ('z', 1.0), ('d', 1.0), ('b', 4.0), ('p', 2.0));

            Structure.AddGroup('N', "Standalone N", ('n', 1.0));

            //Structure.AddSyllable("V", 1.0, "Vowels"); //Likely only precede a word? Double-check
            Structure.AddSyllable("CV", 1.0, "Monograph");
            Structure.AddSyllable("cYv", 0.1, "Diagraphs (Y)");
            Structure.AddSyllable("cv", 0.1, "Diagraphs (H)");
            Structure.AddSyllable("N", 0.1, "Standalone");

            SetExclusions();
        }
        private void SetExclusions()
        {
            //Exclude 'i' from s.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 &&
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 's', 'i');

            //Exclude all except 'i' from 'sh' for 'V' group (monograph).
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 &&
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'ŝ', 'a', 'u', 'e', 'o');

            //Exclude 'i' and 'u' from 't'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 &&
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 't', 'i', 'u');

            //Exclude all except 'i' from 'ch' for 'V' group for (monograph).
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 && 
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'ċ', 'a', 'e', 'u', 'o');

            //Exclude all except 'u' from 'ts'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 && 
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'ţ', 'a', 'i', 'e', 'o');

            //Exclude all except 'u' from 'f'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 &&
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'f', 'a', 'i', 'e', 'o');

            //Exclude 'i' and 'e' from y.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 && 
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'y', 'i', 'e');

            //Exclude all vowels except 'a' from w.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 && 
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'w', 'i', 'u', 'e', 'o');
            
            //Exclude 'w' from selection if not the first letter
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0, 'w');

            //Exclude 'i' from 'z'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current != 0 &&
                    lg.SELECT_Template(syllable, current).Key == 'V' &&
                    last.Letter.Key == 'z', 'i');

            //Exclude "N" syllable from all selections unless it's the last syllable.
            OnSyllableSelection += (lg, selection, word, last, current, max) =>
                lg.SELECT_Exclude(current < max - 1 || max == 1, "N");

            //Exclude 'ts' selection if its the only syllable in the word.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(word.Syllables.Count == 1, 'ţ');

            #region Diagraph exclusion

            //Exclude "cYv" diagraph if its not the last syllable
            OnSyllableSelection += (lg, selection, word, last, current, max) =>
                lg.SELECT_Exclude(current < max - 1 || max == 1, "cYv");

            //Exclude sh /ch from "cYv"
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(syllable.Syllable.Letters == "cYv" && current == 0, 'ŝ', 'c');

            //Exclude from "cv", except sh / ch.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(syllable.Syllable.Letters == "cv" && current == 0,
                'k', 'n', 'h', 'm', 'r', 'g', 'z', 'd', 'b', 'p');

            #endregion

            //Exclude duplicate 
            OnLetterSelection += (lg, selection, word, syllable, letter, current, max) =>
            {
                if (syllable.AdjacentLeft != null &&
                    (lg.SELECT_Template(syllable, current)?.Key == 'C' ||
                    lg.SELECT_Template(syllable, current)?.Key == 'c'))
                {
                    LetterInfo leftConsonant = lg.SELECT_Letter(syllable.AdjacentLeft, 'C');

                    if (leftConsonant != null)
                        lg.SELECT_Exclude(true, leftConsonant.Letter.Key);
                }
            };

            //Excludes duplicate vowels, preventing two vowels from occuring in neighbouring syllables.
            /*OnLetterSelection += (lg, selection, word, syllable, letter, current, max) =>
            {
                if (syllable.AdjacentLeft != null && lg.SELECT_Template(syllable, current)?.Key == 'V')
                {
                    LetterInfo leftVowel = lg.SELECT_Letter(syllable.AdjacentLeft, 'V');
                    LetterInfo leftConsonant = lg.SELECT_Letter(syllable.AdjacentLeft, 'C');

                    if (leftVowel != null &&
                        leftConsonant.Letter.Key != 'y' &&
                        leftConsonant.Letter.Key != 'y' &&
                        leftConsonant.Letter.Key != 'y' &&
                        leftConsonant.Letter.Key != 'y' &&
                        leftConsonant.Letter.Key != 'y')
                    {
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'a', 'a');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'e', 'e');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'i', 'i');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'o', 'o');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'u', 'u');
                    }
                }
            };*/
        }

        private void SetLexicon()
        {
        }
        private void SetAffixes()
        {
        }

        public string ToRomaji(string sentence)
        {
            //ksŝtċţnhmyrwgzjdpb

            sentence = sentence.Replace("ŝ", "sh");
            sentence = sentence.Replace("ċ", "ch");
            sentence = sentence.Replace("ţ", "ts");

            sentence = sentence.Replace("zi", "ji");
            sentence = sentence.Replace("di", "ji");
            sentence = sentence.Replace("du", "zu");

            return sentence;
        }
        public string ToHiragana(string sentence)
        {
            //ksŝtċţnhmyrwgzjdpb
            //aiueo
            (string, string)[] symbolPair = new(string, string)[]
            {
                //Diagraphs
                ("kya", "きゃ"), ("kyu", "きゅ"), ("kyo", "きょ"),
                ("ŝa", "しゃ"), ("ŝu", "しゅ"), ("ŝo", "しょ"),
                ("ċa", "ちゃ"), ("ċu", "ちゅ"), ("ċo", "ちょ"),
                ("nya", "にゃ"), ("nyu", "にゅ"), ("nyo", "にょ"),
                ("hya", "ひゃ"), ("hyu", "ひゅ"), ("hyo", "ひょ"),
                ("mya", "みゃ"), ("myu", "みゅ"), ("myo", "みょ"),
                ("rya", "りゃ"), ("ryu", "りゅ"), ("ryo", "りょ"),
                ("gya", "ぎゃ"), ("gyu", "ぎゅ"), ("gyo", "ぎょ"),
                ("zya", "じゃ"), ("zyu", "じゅ"), ("zyo", "じょ"),
                ("dya", "ぢゃ"), ("dyu", "じゅ"), ("dyo", "じょ"),
                ("bya", "びゃ"), ("byu", "びゅ"), ("byo", "びょ"),
                ("pya", "ぴゃ"), ("pyu", "ぴゅ"), ("pyo", "ぴょ"),

                //Monographs
                ("ka", "か"), ("ki", "き"), ("ku", "く"), ("ke", "け"), ("ko", "こ"),
                ("sa", "さ"), ("ŝi", "し"), ("su", "す"), ("se", "せ"), ("so", "そ"),
                ("ta", "た"), ("ċi", "ち"), ("ţu", "つ"), ("te", "て"), ("to", "と"),
                ("na", "な"), ("ni", "に"), ("nu", "ぬ"), ("ne", "ね"), ("no", "の"),
                ("ha", "は"), ("hi", "ひ"), ("fu", "ふ"), ("he", "へ"), ("ho", "ほ"),
                ("ma", "ま"), ("mi", "み"), ("mu", "む"), ("me", "め"), ("mo", "も"),
                ("ya", "や"), ("yu", "ゆ"), ("yo", "よ"),
                ("ra", "ら"), ("ri", "り"), ("ru", "る"), ("re", "れ"), ("ro", "ろ"),
                ("wa", "わ"),
                ("ga", "が"), ("gi", "ぎ"), ("gu", "ぐ"), ("ge", "げ"), ("go", "ご"),
                ("za", "ざ"), ("zi", "じ"), ("zu", "ず"), ("ze", "ぜ"), ("zo", "ぞ"),
                ("da", "だ"), ("di", "ぢ"), ("du", "づ"), ("de", "で"), ("do", "ど"),
                ("ba", "ば"), ("bi", "び"), ("bu", "ぶ"), ("be", "べ"), ("bo", "ぼ"),
                ("pa", "ぱ"), ("pi", "ぴ"), ("pu", "ぷ"), ("pe", "ぺ"), ("po", "ぽ"),

                //Final nasal monograph
                ("n", "ん"),
            };

            foreach ((string key, string value) pair in symbolPair)
                sentence = sentence.Replace(pair.key, pair.value);

            return sentence;
        }
    }
}
