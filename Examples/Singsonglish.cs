using PLGL.Data;
using PLGL.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Examples
{
    public class Singsonglish
    {
        public static string PlayerName { get; set; } = "Puppycat";

        private Language lang;
        public Language Language()
        {
            lang = new();

            lang.META_Author = "Highverve";
            lang.META_Name = "Singsonglish";
            lang.META_Description = "The songsinging language of the gnomes.";

            SetOptions();

            //Processing
            SetFilters();
            SetDeconstructEvents();
            SetConstructEvents();

            //Structural
            SetLetters();
            SetSigma();
            SetPaths();

            //Lexemes
            SetInflections();
            SetAffixes();

            return lang;
        }

        private void SetOptions()
        {
            lang.Options.Pathing = LanguageOptions.LetterPathing.Inclusion;
            lang.Options.MemorizeWords = false;
            lang.Options.SigmaSkewMin = 1.0;
            lang.Options.SigmaSkewMax = 2.5;

            lang.Options.AllowAutomaticCasing = true;
            lang.Options.AllowRandomCase = true;
        }

        #region Processing
        private void SetFilters()
        {
            lang.AddFilter("Delimiter", " ");
            lang.AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            lang.AddFilter("Numbers", "1234567890");

            lang.AddFilter("Punctuation", ".,?!;:'\"-#$%()");

            lang.AddFilter("Escape", "[]");
            lang.AddFilter("Flags", "");
            lang.AddFilter("FlagsOpen", "{");
            lang.AddFilter("FlagsClose", "}");
        }
        private void SetDeconstructEvents()
        {
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_ContainWithin(current, left, right, "FLAGSOPEN", "FLAGSCLOSE", "FLAGS");
        }
        private void SetConstructEvents()
        {
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
            lang.Construct += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "NUMBERS")
                {
                    double number = double.Parse(word.WordActual.Replace(",", ""));

                    number /= 10;

                    word.WordFinal = number.ToString();
                    word.IsProcessed = true;
                }
            };
            lang.Construct += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");

            SetPunctuation();
            SetFlagging();

            lang.Construct += (lg, word) => lg.CONSTRUCT_Within(word, "ESCAPE", 1, 2);
        }

        #endregion

        #region Structural
        private void SetLetters()
        {
            lang.Alphabet.AddConsonant('l', ('l', 'L'), 20);
            lang.Alphabet.AddConsonant('d', ('d', 'D'), 10);
            lang.Alphabet.AddConsonant('s', ('s', 'S'), 2);

            lang.Alphabet.AddVowel('a', ('a', 'A'), 8);
            lang.Alphabet.AddVowel('e', ('e', 'E'), 0);
            lang.Alphabet.AddVowel('i', ('i', 'I'), 0);
            lang.Alphabet.AddVowel('o', ('o', 'O'), 0);
            lang.Alphabet.AddVowel('u', ('u', 'U'), 0);

            lang.Generate += (lg, w, current, left, right) =>
            {
                if (left != null && (left.Letter.Key == 'l' && current.Letter.Key != 'l') && w.Letters.IndexOf(current) > 1)
                {
                    //lg.GENERATE_InsertLetter(w, current, 'l', 0);
                }
            };
        }
        private void SetSigma()
        {
            lang.Structure.AddSigma("C", "V", "", new SigmaPath() { SelectionWeight = 20.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("", "V", "", new SigmaPath() { SelectionWeight = 1.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
        }
        private void SetPaths()
        {
            //Vowels
            lang.Structure.AddLetterPath('a', WordPosition.Any, SigmaPosition.Any,
                ('a', 30.0), ('i', 2.0), ('o', 0.25), ('u', 1.0), ('l', 3.0), ('d', 5.0));
            lang.Structure.AddLetterPath('e', WordPosition.Any, SigmaPosition.Any,
                ('e', 30.0), ('i', 5.0), ('l', 2.0), ('d', 10.0));
            lang.Structure.AddLetterPath('i', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.5), ('i', 30.0), ('e', 0.5), ('u', 1.0), ('l', 2.0), ('d', 10.0));
            lang.Structure.AddLetterPath('o', WordPosition.Any, SigmaPosition.Any,
                ('a', 1.0), ('i', 2.5), ('e', 3.0), ('u', 5.0), ('o', 30.0), ('l', 2.0), ('d', 6.0));
            lang.Structure.AddLetterPath('u', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.5), ('e', 0.5), ('u', 1.0), ('l', 2.0), ('d', 8.0));

            //Consonants
            lang.Structure.AddLetterPath('l', WordPosition.Any, SigmaPosition.Any,
                ('a', 15.0), ('i', 5.0), ('e', 10.0), ('u', 3.0), ('o', 5.0), ('l', 1.0));
            lang.Structure.AddLetterPath('d', WordPosition.Any, SigmaPosition.Any,
                ('a', 7.0), ('i', 10.0), ('e', 2.0), ('u', 2.0), ('o', 5.0), ('d', 1.0));
            lang.Structure.AddLetterPath('s', WordPosition.Any, SigmaPosition.Any,
                ('a', 10.0), ('i', 15.0), ('e', 5.0), ('u', 1.0), ('o', 3.0), ('s', 1.0));
        }
        #endregion

        #region Lexemes
        private void SetInflections()
        {
            lang.Lexicon.Roots.Add("gnom", "laloodi");
            lang.Lexicon.Roots.Add("gnome", "laloodi");

            lang.Lexicon.Inflections.Add("sing", "ladadee");
            lang.Lexicon.Inflections.Add("sang", "ladali");
            lang.Lexicon.Inflections.Add("sung", "ladadoo");
            lang.Lexicon.Inflections.Add("song", "lala");
        }
        private void SetAffixes()
        {
            lang.Lexicon.Affixes.Add(new Affix("'s", "-doo", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, 0));
            lang.Lexicon.Affixes.Add(new Affix("s", "-da", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, 0));
            lang.Lexicon.Affixes.Add(new Affix("ly", "-dee", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, 0));
            lang.Lexicon.Affixes.Add(new Affix("ish", "-dei", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, 0));
        }
        #endregion

        #region Numbers
        public void SetNumbers()
        {

        }
        #endregion

        #region Punctuation
        private void SetPunctuation()
        {
            lang.Construct += (lg, word) => lang.Punctuation.Process(lg, word, "PUNCTUATION");

            lang.Punctuation.Add(".", (w) => { return " ha·"; });
            lang.Punctuation.Add("...", (w) => { return " haaa·"; });
            lang.Punctuation.Add(",", (w) => { return " sa"; });
            lang.Punctuation.Add(";", (w) => { return " sa·"; });
            lang.Punctuation.Add("!", (w) => { return " daaa·"; });
        }
        #endregion

        #region Flagging
        public void SetFlagging()
        {
            lang.Construct += (lg, word) => lang.Flags.Process(lg, word, "FLAGS");

            lang.Flags.Add("<HIDE", lang.Flags.ACTION_HideLeft);
            lang.Flags.Add("HIDE>", lang.Flags.ACTION_HideRight);
            lang.Flags.Add("<HIDE>", lang.Flags.ACTION_HideAdjacents);
            lang.Flags.Add("NOGEN", lang.Flags.ACTION_NoGenerate);
            lang.Flags.Add("PLAYER", (lg, word) => lang.Flags.ACTION_ReplaceCurrent(lg, word, () => { return PlayerName; }));
        }
        #endregion
    }
}
