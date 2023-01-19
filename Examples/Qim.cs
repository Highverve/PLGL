using PLGL.Construct.Elements;
using PLGL.Construct;
using PLGL.Deconstruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Examples
{
    public class Qim
    {
        private Language lang;
        public Language Language()
        {
            lang = new();

            lang.META_Author = "Highverve";
            lang.META_Name = "Qim";
            lang.META_Description = "The common language of the Rootfolk from Bälore and Boffer.";

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
            lang.Options.SigmaSkewMax = 2.0;

            lang.Options.AllowAutomaticCasing = true;
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
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "LETTERS", "FLAGS", "FLAGS", "FLAGS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");
        }
        private void SetConstructEvents()
        {
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "NUMBERS");
            lang.Construct += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");
            lang.Construct += (lg, word) =>
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
            lang.Construct += (lg, word) =>
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
            lang.Construct += (lg, word) =>
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


        /// <summary>
        /// Aa, Ee, Ii, Oo, Uu (short)
        /// Ää, Ëë, Ïï, Öö, Üü (long)
        /// Bb Pp, Dd Tt, Gg Kk
        /// Mm Nn, Ŋŋ, Rr Ll
        /// Ff, Vv, Ss, Ŝŝ, þÞ, Zz Żż
        /// Ww, Yy, Hh, Qq
        /// </summary>
        private void SetLetters()
        {
            lang.Alphabet.AddVowel('a', ('a', 'A'), 15);
            lang.Alphabet.AddVowel('e', ('e', 'E'), 7);
            lang.Alphabet.AddVowel('i', ('i', 'I'), 10);
            lang.Alphabet.AddVowel('o', ('o', 'O'), 2);
            lang.Alphabet.AddVowel('u', ('u', 'U'), 5);

            lang.Alphabet.AddVowel('ä', ('ä', 'Ä'), 7);
            lang.Alphabet.AddVowel('ë', ('ë', 'Ë'), 3.5);
            lang.Alphabet.AddVowel('ï', ('ï', 'Ï'), 5);
            lang.Alphabet.AddVowel('ö', ('ö', 'Ö'), 1);
            lang.Alphabet.AddVowel('ü', ('ü', 'Ü'), 2.5);

            lang.Alphabet.AddConsonant('b', ('b', 'B'), 20);
            lang.Alphabet.AddConsonant('p', ('p', 'P'), 15);
            lang.Alphabet.AddConsonant('d', ('d', 'D'), 10);
            lang.Alphabet.AddConsonant('t', ('t', 'T'), 5);
            lang.Alphabet.AddConsonant('g', ('g', 'G'), 2);
            lang.Alphabet.AddConsonant('k', ('k', 'K'), 2);

            lang.Alphabet.AddConsonant('m', ('m', 'M'), 5);
            lang.Alphabet.AddConsonant('n', ('n', 'N'), 5);
            lang.Alphabet.AddConsonant('ŋ', ('ŋ', 'Ŋ'), 0); //Ng ŋ (endi-ng)
            lang.Alphabet.AddConsonant('r', ('r', 'R'), 10);
            lang.Alphabet.AddConsonant('l', ('l', 'L'), 20);

            lang.Alphabet.AddConsonant('f', ('f', 'F'), 5);
            lang.Alphabet.AddConsonant('v', ('v', 'V'), 10);
            lang.Alphabet.AddConsonant('s', ('s', 'S'), 15);
            lang.Alphabet.AddConsonant('ŝ', ('ŝ', 'Ŝ'), 10); //Sh ʃ (ship)
            lang.Alphabet.AddConsonant('þ', ('þ', 'Þ'), 5); //Th þ (thatch)
            lang.Alphabet.AddConsonant('z', ('z', 'Z'), 5); //Th þ (thatch)
            lang.Alphabet.AddConsonant('ż', ('ż', 'Ż'), 5); //Th þ (thatch)

            lang.Alphabet.AddConsonant('w', ('w', 'W'), 10); //Th þ (thatch)
            lang.Alphabet.AddConsonant('y', ('y', 'Y'), 15); //Th þ (thatch)
            lang.Alphabet.AddConsonant('h', ('h', 'H'), 3); //Th þ (thatch)
            lang.Alphabet.AddConsonant('q', ('q', 'Q'), 0.1); //Th þ (thatch)
        }
        private void SetSigma()
        {
            lang.Structure.AddSigma("C", "V", "", new SigmaPath() { SelectionWeight = 20.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("C", "VV", "", new SigmaPath() { SelectionWeight = 20.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("", "V", "", new SigmaPath() { SelectionWeight = 1.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("CC", "V", "", new SigmaPath() { SelectionWeight = 3.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
        }
        private void SetPaths()
        {
            //Vowels
            lang.Structure.AddLetterPath('a', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('l', 10.0), ('w', 10.0), ('y', 10.0));

            lang.Structure.AddLetterPath('e', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('l', 10.0), ('w', 10.0), ('y', 10.0));

            lang.Structure.AddLetterPath('i', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('l', 10.0), ('w', 10.0), ('y', 10.0));

            lang.Structure.AddLetterPath('o', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('l', 10.0), ('w', 10.0), ('y', 10.0));

            lang.Structure.AddLetterPath('u', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('l', 10.0), ('w', 10.0), ('y', 10.0));


            //Consonants
            lang.Structure.AddLetterPath('l', WordPosition.Any, SigmaPosition.Any,
                ('a', 15.0), ('i', 5.0), ('e', 10.0), ('u', 3.0), ('o', 5.0), ('l', 1.0));
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
