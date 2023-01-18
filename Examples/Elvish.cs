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
    public class Elvish
    {
        private Language lang;
        public Language Language()
        {
            lang = new();

            lang.META_Author = "Highverve";
            lang.META_Name = "Elvish";
            lang.META_Description = "An imagination of elvish written language.";

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

        private void SetLetters()
        {
            lang.Alphabet.AddConsonant('m').StartWeight = 5.0;
            lang.Alphabet.AddConsonant('n').StartWeight = 10.0;
            //lang.Alphabet.AddConsonant('ŋ').StartWeight = 0.0;

            lang.Alphabet.AddConsonant('p').StartWeight = 20.0;
            lang.Alphabet.AddConsonant('b').StartWeight = 15.0;
            lang.Alphabet.AddConsonant('t').StartWeight = 10.0;
            lang.Alphabet.AddConsonant('d').StartWeight = 5.0;
            lang.Alphabet.AddConsonant('k').StartWeight = 2.0;
            lang.Alphabet.AddConsonant('g').StartWeight = 7.0;

            lang.Alphabet.AddConsonant('f').StartWeight = 5.0;
            lang.Alphabet.AddConsonant('v').StartWeight = 10.0;
            lang.Alphabet.AddConsonant('s').StartWeight = 15.0;
            lang.Alphabet.AddConsonant('k').StartWeight = 0.0;
            lang.Alphabet.AddConsonant('h').StartWeight = 3.0;

            lang.Alphabet.AddConsonant('r').StartWeight = 10.0;

            lang.Alphabet.AddConsonant('w').StartWeight = 10.0;
            lang.Alphabet.AddConsonant('y').StartWeight = 15.0;
            lang.Alphabet.AddConsonant('l').StartWeight = 20.0;

            lang.Alphabet.AddVowel('a').StartWeight = 15.0;
            lang.Alphabet.AddVowel('e').StartWeight = 7.0;
            lang.Alphabet.AddVowel('i').StartWeight = 10.0;
            lang.Alphabet.AddVowel('o').StartWeight = 2.0;
            lang.Alphabet.AddVowel('u').StartWeight = 5.0;
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
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('w', 10.0), ('y', 10.0), ('l', 10.0));

            lang.Structure.AddLetterPath('e', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('w', 10.0), ('y', 10.0), ('l', 10.0));

            lang.Structure.AddLetterPath('i', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('w', 10.0), ('y', 10.0), ('l', 10.0));

            lang.Structure.AddLetterPath('o', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('w', 10.0), ('y', 10.0), ('l', 10.0));

            lang.Structure.AddLetterPath('u', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('m', 10.0), ('n', 10.0), ('p', 10.0), ('p', 10.0), ('b', 10.0), ('t', 10.0),
                ('d', 10.0), ('k', 10.0), ('g', 10.0), ('f', 10.0), ('v', 10.0), ('s', 10.0),
                ('k', 10.0), ('h', 10.0), ('r', 10.0), ('w', 10.0), ('y', 10.0), ('l', 10.0));


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
