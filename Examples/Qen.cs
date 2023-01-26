using PLGL.Data;
using PLGL.Languages;

namespace PLGL.Examples
{
    public class Qen
    {
        private Language lang;
        public Language Language()
        {
            lang = new();

            lang.META_Author = "Highverve";
            lang.META_Name = "Qen";
            lang.META_Nickname = "Qen";
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
            lang.Options.SigmaSkewMin = 0.8;
            lang.Options.SigmaSkewMax = 2;

            lang.Options.AllowAutomaticCasing = true;
            lang.Options.AllowRandomCase = true;
        }

        #region Processing
        private void SetFilters()
        {
            lang.AddFilter("Delimiter", " ");
            lang.AddFilter("Compound", "");

            lang.AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            lang.AddFilter("Numbers", "1234567890");

            lang.AddFilter("Punctuation", ".,?!;:'\"-#$%()");

            lang.AddFilter("Escape", "[]"); //The word inside is skipped.
            lang.AddFilter("Flags", "");
            lang.AddFilter("FlagsOpen", "{");
            lang.AddFilter("FlagsClose", "}");

        }
        private void SetDeconstructEvents()
        {
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_ChangeFilter(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "-", "COMPOUND");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_MergeBlocks(current, left, right, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");
            lang.Deconstruct += (lg, current, left, right) => lg.DECONSTRUCT_ContainWithin(current, left, right, "FLAGSOPEN", "FLAGSCLOSE", "FLAGS");
        }
        private void SetConstructEvents()
        {
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
            lang.Construct += (lg, word) => lg.CONSTRUCT_Hide(word, "COMPOUND");

            lang.Construct += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");

            SetPunctuation();
            SetFlagging();
            SetNumbers();

            lang.Construct += (lg, word) => lg.CONSTRUCT_Within(word, "ESCAPE", 1, 2);
        }
        #endregion

        #region Structural
        /// <summary>
        /// Aa, Ee, Ii, Oo, Uu (short)
        /// Ää, Ëë, Ïï, Öö, Üü (long)
        /// Bb Pp, Dd Tt, Gg Kk
        /// Mm Nn, Ŋŋ, Rr Ll
        /// Ff, Vv, Ss, Zz
        /// Ww, Yy, Hh, Qq, Ŝŝ, þÞ, Żż
        /// </summary>
        private void SetLetters()
        {
            //Aa, Ee, Ii, Oo, Uu (short)
            lang.Alphabet.AddVowel("", 'a', ('a', 'A'), 15, "/a/ (apple)");
            lang.Alphabet.AddVowel("", 'e', ('e', 'E'), 7, "/e/ (west)");
            lang.Alphabet.AddVowel("", 'i', ('i', 'I'), 10, "/i/ (tip)");
            lang.Alphabet.AddVowel("", 'o', ('o', 'O'), 2, "/o/ (crop)");
            lang.Alphabet.AddVowel("", 'u', ('u', 'U'), 5, "/u/ (sun)");
            //lang.Alphabet.AddVowel("", 'ŕ', ('ŕ', 'Ŕ'), 5, "/r/ (later)");

            //Ää, Ëë, Ïï, Öö, Üü (long)
            lang.Alphabet.AddVowel("", 'ä', ('ä', 'Ä'), 7, "/ae/ (cake)");
            lang.Alphabet.AddVowel("", 'ë', ('ë', 'Ë'), 3.5, "/ee/ beet");
            lang.Alphabet.AddVowel("", 'ï', ('ï', 'Ï'), 5, "/ai/ (night)");
            lang.Alphabet.AddVowel("", 'ö', ('ö', 'Ö'), 1, "/ou/ (toad)");
            lang.Alphabet.AddVowel("", 'ü', ('ü', 'Ü'), 2.5, "/oo/ (tulip)");

            //Bb Pp, Dd Tt, Gg Kk
            lang.Alphabet.AddConsonant('b', ('b', 'B'), 20);
            lang.Alphabet.AddConsonant('p', ('p', 'P'), 15);
            lang.Alphabet.AddConsonant('d', ('d', 'D'), 10);
            lang.Alphabet.AddConsonant('t', ('t', 'T'), 5);
            lang.Alphabet.AddConsonant('g', ('g', 'G'), 2);
            lang.Alphabet.AddConsonant('k', ('k', 'K'), 1);

            //Mm, Nn, Ŋŋ, Rr, Ll
            lang.Alphabet.AddConsonant('m', ('m', 'M'), 5);
            lang.Alphabet.AddConsonant('n', ('n', 'N'), 5);
            lang.Alphabet.AddConsonant('ŋ', ('ŋ', 'Ŋ'), 0); //Ng ŋ (endi-ng). Never in onset.
            lang.Alphabet.AddConsonant('r', ('r', 'R'), 10);
            lang.Alphabet.AddConsonant('l', ('l', 'L'), 20);

            //Ff, Vv, Ss, Zz
            lang.Alphabet.AddConsonant('f', ('f', 'F'), 5);
            lang.Alphabet.AddConsonant('v', ('v', 'V'), 10);
            lang.Alphabet.AddConsonant('s', ('s', 'S'), 15);
            lang.Alphabet.AddConsonant('z', ('z', 'Z'), 5);

            //Ww, Yy, Hh, Qq, Ŝŝ, þÞ, Żż
            lang.Alphabet.AddConsonant('w', ('w', 'W'), 3);
            lang.Alphabet.AddConsonant('y', ('y', 'Y'), 5); //Never as a vowel.
            lang.Alphabet.AddConsonant('h', ('h', 'H'), 3);
            lang.Alphabet.AddConsonant('q', ('q', 'Q'), 1); //Kw/qu, thus never in coda.
            lang.Alphabet.AddConsonant('ŝ', ('ŝ', 'Ŝ'), 10); //Sh ŝ (ship)
            lang.Alphabet.AddConsonant('Þ', ('Þ', 'þ'), 7); //Th þ (thatch)
            lang.Alphabet.AddConsonant('ż', ('ż', 'Ż'), 1); //Ezh ʒ (azure)
        }
        private void SetSigma()
        {
            lang.Structure.AddSigma("C", "V", "", new SigmaPath() { SelectionWeight = 10.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("", "V", "", new SigmaPath() { SelectionWeight = 3.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("CC", "V", "", new SigmaPath() { SelectionWeight = 2.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
            lang.Structure.AddSigma("C", "V", "C", new SigmaPath() { SelectionWeight = 10.0, StartingWeight = 1.0, EndingWeight = 1.0, LastConsonantWeight = 1.5 });
        }

        /// <summary>
        /// Aa, Ee, Ii, Oo, Uu (short)
        /// Ää, Ëë, Ïï, Öö, Üü (long)
        /// Bb Pp, Dd Tt, Gg Kk
        /// Mm Nn, Ŋŋ, Rr Ll
        /// Ff, Vv, Ss, Zz
        /// Ww, Yy, Hh, Qq, Ŝŝ, þÞ, Żż
        /// </summary>
        private void SetPaths()
        {
            //Idea: Add leter group class, and make sigmas match letter grouping instead of standard C/V.
            //Imagine these letter groups: "short vowels" (v), "long vowels" (V), "fricatives" (f), plosives (p), nasal (n), r / l (r)
            //Instead of CVC, it could be frvp (possible letters generated: flik, vrep,), 

            #region Vowels (short)
            lang.Structure.AddLetterPath('a', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('e', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('i', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('o', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('u', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));

            lang.Structure.AddLetterPath('ä', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('ë', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('ï', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('ö', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));
            lang.Structure.AddLetterPath('ü', WordPosition.Last, SigmaPosition.Coda, ('ŋ', 1.0), ('ż', 1.0));

            lang.Structure.AddLetterPath('a', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('e', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('i', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('o', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('u', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));
            #endregion

            #region Vowels (long)
            lang.Structure.AddLetterPath('ä', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('ë', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('ï', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('ö', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('ü', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));
            #endregion

            #region Plosive (b, p, d, t, g, k)
            lang.Structure.AddLetterPath('b', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 10.0), ('p', 0.0), ('d', 0.0), ('t', 0.0), ('g', 0.0), ('k', 0.0),
                ('m', 0.0), ('n', 0.0), ('ŋ', 0.0), ('r', 5.0), ('l', 3.0),
                ('f', 0.0), ('v', 0.0), ('s', 5.0), ('ŝ', 2.0), ('Þ', 0.0), ('z', 0.0),
                ('w', 0.0), ('y', 0.0), ('h', 0.0), ('q', 0.0));

            lang.Structure.AddLetterPath('p', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('d', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 0.0), ('p', 0.0), ('d', 5.0), ('t', 0.0), ('g', 0.0), ('k', 0.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 1.0), ('v', 1.0), ('s', 8.0), ('ŝ', 0.0), ('Þ', 0.0), ('z', 2.0),
                ('w', 0.0), ('y', 0.0), ('h', 0.0), ('q', 0.0));

            lang.Structure.AddLetterPath('t', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('g', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('k', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));
            #endregion

            #region Nasal (m, n, ŋ, r, l)
            lang.Structure.AddLetterPath('m', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('n', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('ŋ', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('r', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('l', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));
            #endregion

            #region Fricatives (f, v, s, z)
            lang.Structure.AddLetterPath('f', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('v', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('s', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('z', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));
            #endregion

            #region Approximants / Fricatives (w, y, h, q, ŝ, Þ, ż)
            lang.Structure.AddLetterPath('w', WordPosition.Any, SigmaPosition.Any,
                ('a', 10.0), ('e', 5.0), ('i', 2.0), ('o', 2.0), ('u', 1.0),
                ('ä', 8.0), ('ë', 0.5), ('ï', 5.0), ('ö', 1.0), ('ü', 0.5),
                ('b', 0.0), ('p', 0.0), ('d', 0.0), ('t', 0.0), ('g', 0.0), ('k', 0.0),
                ('m', 0.0), ('n', 0.0), ('ŋ', 0.0), ('r', 1.0), ('l', 0.0),
                ('f', 0.0), ('v', 0.0), ('s', 0.0), ('ŝ', 0.0), ('Þ', 0.0), ('z', 0.0),
                ('w', 0.0), ('y', 0.0), ('h', 0.0), ('q', 0.0));

            lang.Structure.AddLetterPath('y', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0),
                ('w', 7.0), ('y', 7.0), ('h', 7.0), ('q', 7.0));

            lang.Structure.AddLetterPath('h', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0));

            lang.Structure.AddLetterPath('q', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0));

            lang.Structure.AddLetterPath('ŝ', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0));

            lang.Structure.AddLetterPath('Þ', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0));

            lang.Structure.AddLetterPath('ż', WordPosition.Any, SigmaPosition.Any,
                ('a', 2.0), ('e', 1.0), ('i', 10.0), ('o', 0.25), ('u', 7.0),
                ('ä', 7.0), ('ë', 7.0), ('ï', 7.0), ('ö', 7.0), ('ü', 7.0),
                ('b', 7.0), ('p', 7.0), ('d', 7.0), ('t', 7.0), ('g', 7.0), ('k', 7.0),
                ('m', 7.0), ('n', 7.0), ('ŋ', 0.0), ('r', 7.0), ('l', 7.0),
                ('f', 7.0), ('v', 7.0), ('s', 7.0), ('ŝ', 7.0), ('Þ', 7.0), ('z', 7.0));
            #endregion

        }
        #endregion

        #region Lexemes
        private void SetInflections()
        {
        }
        private void SetAffixes()
        {
            lang.Lexicon.Affixes.Add(new Affix("'s", "'en", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("ly", "ila", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("s", "im", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("less", "nöl", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));

            lang.OnSuffix += (lg, word, current) => lg.AFFIX_Remove(word, current, "ly", 0, 1, lg.AFFIX_VowelLast(word, current));
            lang.OnSuffix += (lg, word, current) => lg.AFFIX_Insert(word, current, "s", "l", 0, lg.AFFIX_VowelLast(word, current));
            lang.OnSuffix += (lg, word, current) => lg.AFFIX_Remove(word, current, "less", 0, 1, lg.AFFIX_ConsonantLast(word, current));
        }
        #endregion

        #region Punctuation
        private void SetPunctuation()
        {
            lang.Construct += (lg, word) => lang.Punctuation.Process(lg, word, "PUNCTUATION");

            lang.Punctuation.Add(".", (w) => { return "৹"; });
            lang.Punctuation.Add(",", (w) => { return ","; });
            lang.Punctuation.Add(";", (w) => { return ";"; });
            lang.Punctuation.Add("!", (w) => { return "!"; });
            lang.Punctuation.Add("$", (w) => { return "$"; });
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
            lang.Flags.Add("NOPLURAL", lang.Flags.ACTION_NoAffixes);
        }
        #endregion

        public void SetNumbers()
        {
            lang.Construct += (lg, word) => lang.Numbers.Process(lg, word, "NUMBERS", '-', Char.MinValue);
            lang.Numbers.Add(('0', '○', ""),
                ('1', '•', ""), ('2', '›', ""), ('3', '△', ""),
                ('4', '◇', ""), ('5', '⨰', ""), ('6', '∓', ""),
                ('7', '⪦', ""), ('8', '⋇', ""), ('9', '⨳', ""),
                ('A', 'A', ""), ('B', 'B', ""));
        }
    }
}
