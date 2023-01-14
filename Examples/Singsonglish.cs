using PLGL.Construct;
using PLGL.Construct.Elements;
using PLGL.Deconstruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Examples
{
    public class Singsonglish
    {
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

            lang.Options.MatchCase = true;
            lang.Options.AllowRandomCase = true;
        }

        #region Processing
        private void SetFilters()
        {
            lang.AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            lang.AddFilter("Numbers", "1234567890");
            lang.AddFilter("Delimiter", " ");
            lang.AddFilter("Punctuation", ".,?!;:'\"-#$%()");
            lang.AddFilter("Flags", "{}"); //[NOLEXEMES]
            lang.AddFilter("Escape", "[]"); //The word inside is skipped.
        }
        private void SetDeconstructEvents()
        {
            lang.Deconstruct += (lg, current, left, right) => lg.EVENT_MergeBlocks(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
            lang.Deconstruct += (lg, current, left, right) => lg.EVENT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.EVENT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.EVENT_MergeBlocks(current, left, right, "LETTERS", "FLAGS", "FLAGS", "FLAGS");
            lang.Deconstruct += (lg, current, left, right) => lg.EVENT_MergeBlocks(current, left, right, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");
        }
        private void SetConstructEvents()
        {
            lang.ConstructFilter += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            lang.ConstructFilter += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
            lang.ConstructFilter += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "NUMBERS")
                {
                    double number = double.Parse(word.WordActual.Replace(",", ""));

                    number /= 10;

                    word.WordFinal = number.ToString();
                    word.IsProcessed = true;
                }
            };
            lang.ConstructFilter += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");
            lang.ConstructFilter += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "PUNCTUATION")
                {
                    word.WordFinal = word.WordActual;
                    if (word.WordActual.Contains(";"))
                        word.WordFinal = word.WordActual.Replace(";", " ha·");
                    if (word.WordActual.Contains("."))
                        word.WordFinal = word.WordActual.Replace(".", " sa·");
                    if (word.WordActual.Contains("!"))
                        word.WordFinal = word.WordActual.Replace("!", " daaa·");

                    word.IsProcessed = true;
                }
            };
            lang.ConstructFilter += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "FLAGS")
                {
                    string command = word.WordActual.Substring(1, word.WordActual.Length - 2);

                    if (word.AdjacentLeft != null && command.Contains("TACOFY"))
                    {
                        word.AdjacentLeft.WordFinal = "taco'd";
                    }

                    word.IsProcessed = true;
                }
            };
            lang.ConstructFilter += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "ESCAPE")
                {
                    word.WordFinal = word.WordActual.Substring(1, word.WordActual.Length - 2);
                    word.IsProcessed = true;
                }
            };
        }
        #endregion

        #region Structural
        private void SetLetters()
        {
            lang.Alphabet.AddConsonant('l').StartWeight = 20.0;
            lang.Alphabet.AddConsonant('d').StartWeight = 10.0;
            lang.Alphabet.AddConsonant('s').StartWeight = 2.0;

            lang.Alphabet.AddVowel('a').StartWeight = 8.0;
            lang.Alphabet.AddVowel('e').StartWeight = 0.0;
            lang.Alphabet.AddVowel('i').StartWeight = 0.0;
            lang.Alphabet.AddVowel('o').StartWeight = 0.0;
            lang.Alphabet.AddVowel('u').StartWeight = 0.0;
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
            lang.Lexicon.Affixes.Add(new Affix("'s", "-doo", Affix.AffixType.Suffix, Affix.LocationType.End, 0));
            lang.Lexicon.Affixes.Add(new Affix("s", "-da", Affix.AffixType.Suffix, Affix.LocationType.End, 0));
            lang.Lexicon.Affixes.Add(new Affix("ly", "-dee", Affix.AffixType.Suffix, Affix.LocationType.End, 0));
            lang.Lexicon.Affixes.Add(new Affix("ish", "-dei", Affix.AffixType.Suffix, Affix.LocationType.End, 0));
        }
        #endregion

        
    }
}
