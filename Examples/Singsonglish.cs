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
            SetStructure();

            //Lexemes
            SetInflections();
            SetAffixes();

            return lang;
        }

        private void SetOptions()
        {
            //lang.Options.Pathing = LanguageOptions.PathSelection.Inclusion;
            lang.Options.MemorizeWords = false;
            //lang.Options.SyllableSkewMin = 1.0;
            //lang.Options.SyllableSkewMax = 2.5;

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
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_ContainWithin(current, "FLAGSOPEN", "FLAGSCLOSE", "FLAGS");
        }
        private void SetConstructEvents()
        {
            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
            lang.OnConstruct += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "NUMBERS")
                {
                    double number = double.Parse(word.WordActual.Replace(",", ""));

                    number /= 10;

                    word.WordFinal = number.ToString();
                    word.IsProcessed = true;
                }
            };
            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");

            SetPunctuation();
            SetFlagging();

            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_Within(word, "ESCAPE", 1, 2);
        }

        #endregion

        #region Structural
        private void SetLetters()
        {
            lang.Alphabet.AddConsonant('l', ('l', 'L'));
            lang.Alphabet.AddConsonant('d', ('d', 'D'));
            lang.Alphabet.AddConsonant('s', ('s', 'S'));

            lang.Alphabet.AddVowel('a', ('a', 'A'));
            lang.Alphabet.AddVowel('e', ('e', 'E'));
            lang.Alphabet.AddVowel('i', ('i', 'I'));
            lang.Alphabet.AddVowel('o', ('o', 'O'));
            lang.Alphabet.AddVowel('u', ('u', 'U'));
        }
        public void SetStructure()
        {
            lang.Structure.AddGroup('V', "Vowels", ('a', 1.0), ('e', 1.0), ('i', 1.0), ('o', 1.0), ('u', 1.0));
            lang.Structure.AddGroup('C', "Consonants", ('l', 5.0), ('d', 3.0), ('s', 0.5));

            lang.Structure.AddSyllable("CV", 10.0);
            lang.Structure.AddSyllable("CVV", 2.0);
            lang.Structure.AddSyllable("VC", 0.25);
        }
        #endregion

        #region Lexemes
        private void SetInflections()
        {
            lang.Lexicon.Roots.Add("gnom", "laloodi");
            lang.Lexicon.Roots.Add("gnome", "laloodi");

            lang.Lexicon.Vocabulary.Add("sing", "ladadee");
            lang.Lexicon.Vocabulary.Add("sang", "ladali");
            lang.Lexicon.Vocabulary.Add("sung", "ladadoo");
            lang.Lexicon.Vocabulary.Add("song", "lala");
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
            lang.OnConstruct += (lg, word) => lang.Punctuation.Process(lg, word, "PUNCTUATION");

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
            lang.OnConstruct += (lg, word) => lang.Flags.Process(lg, word, "FLAGS");

            lang.Flags.Add("<HIDE", lang.Flags.ACTION_HideLeft);
            lang.Flags.Add("HIDE>", lang.Flags.ACTION_HideRight);
            lang.Flags.Add("<HIDE>", lang.Flags.ACTION_HideAdjacents);
            lang.Flags.Add("NOGEN", lang.Flags.ACTION_NoGenerate);
            lang.Flags.Add("PLAYER", (lg, word) => lang.Flags.ACTION_ReplaceCurrent(lg, word, () => { return PlayerName; }));
        }
        #endregion
    }
}
