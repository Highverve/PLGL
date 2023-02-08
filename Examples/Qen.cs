using PLGL.Data;
using PLGL.Languages;
using PLGL.Processing;

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
            SetStructure();

            //Lexemes
            SetInflections();
            SetAffixes();

            return lang;
        }

        private void SetOptions()
        {
            lang.Options.Pathing = LanguageOptions.LetterPathing.Inclusion;
            lang.Options.MemorizeWords = false;
            lang.Options.SyllableSkewMin = 0.8;
            lang.Options.SyllableSkewMax = 1.5;
            lang.Options.SeedOffset = 2;

            lang.Options.AllowAutomaticCasing = true;
            lang.Options.AllowRandomCase = true;
        }

        #region Processing
        private void SetFilters()
        {
            lang.AddFilter("Delimiter", " ");
            lang.AddFilter("Compound", "");//-

            lang.AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            lang.AddFilter("Numbers", "1234567890");

            lang.AddFilter("Punctuation", ".,?!;:'\"-#$%()");

            lang.AddFilter("Escape", "[]"); //The word inside is skipped.
            lang.AddFilter("Flags", "");
            lang.AddFilter("FlagsOpen", "{");
            lang.AddFilter("FlagsClose", "}");
            lang.AddFilter("Mark", "");

        }
        private void SetDeconstructEvents()
        {
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_ChangeFilter(current, "PUNCTUATION", "LETTERS", "LETTERS", "-", "COMPOUND");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_MergeBlocks(current, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");
            lang.OnDeconstruct += (lg, current) => lg.DECONSTRUCT_ContainWithin(current, "FLAGSOPEN", "FLAGSCLOSE", "FLAGS");
        }
        private void SetConstructEvents()
        {
            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");
            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_Replace(word, "COMPOUND", ('-', ' '));

            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_Generate(word, "LETTERS");

            SetPunctuation();
            SetFlagging();
            SetNumbers();

            lang.OnConstruct += (lg, word) => lg.CONSTRUCT_Within(word, "ESCAPE", 1, 2);

            lang.OnConstruct += (lg, word) =>
            {
                if (word.Filter.Name.ToUpper() == "MARK")
                {
                    WordInfo last = lg.WORD_LastByFilter(word, "MARK");
                    if (last != null)
                    {
                        Console.WriteLine($"Found MARK right of {last.AdjacentLeft.WordActual}/{last.AdjacentLeft.WordFinal}!");
                    }
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
        private void SetStructure()
        {
            lang.Structure.AddGroup('V', "Vowels", ('a', 5.0), ('e', 7.0), ('i', 3.0), ('o', 5.0), ('u', 7.0),
                                               ('ä', 2.0), ('ë', 5.0), ('ï', 3.0), ('ö', 6.0), ('ü', 1.0));
            lang.Structure.AddGroup('o', "Vowels (short)", ('a', 1.0), ('e', 1.0), ('u', 1.0), ('o', 1.0), ('u', 1.0));
            lang.Structure.AddGroup('O', "Vowels (long)", ('ä', 1.0), ('ë', 1.0), ('ï', 1.0), ('ö', 1.0), ('ü', 1.0));

            lang.Structure.AddGroup('N', "Nasal", ('m', 10.0), ('n', 10.0));
            lang.Structure.AddGroup('n', "Nasal with ng", ('m', 5.0), ('n', 5.0), ('ŋ', 3.0));
            lang.Structure.AddGroup('P', "Plosive", ('b', 12.0), ('p', 4.0), ('d', 5.0), ('t', 3.0), ('g', 5.0), ('k', 1.0));
            lang.Structure.AddGroup('p', "Plosive higher", ('p', 4.0), ('t', 1.0), ('k', 2.0));
            lang.Structure.AddGroup('F', "Fricative", ('f', 10.0), ('v', 1.0), ('s', 10.0), ('z', 1.0));
            lang.Structure.AddGroup('f', "Fricative f", ('f', 10.0));
            lang.Structure.AddGroup('S', "S/SH", ('s', 30.0), ('ŝ', 1.0));
            lang.Structure.AddGroup('A', "Approximant", ('w', 1.0), ('y', 1.0), ('h', 1.0));
            lang.Structure.AddGroup('R', "R / L", ('r', 50), ('l', 50));
            lang.Structure.AddGroup('r', "R > L", ('r', 90), ('l', 10));
            lang.Structure.AddGroup('l', "L > R", ('r', 10), ('l', 90));
            lang.Structure.AddGroup('T', "TH/SH", ('Þ', 1.0), ('ŝ', 1.0));

            lang.Structure.AddSyllable("VN", 2.5);
            lang.Structure.AddSyllable("VP", 2.0);
            lang.Structure.AddSyllable("VR", 1.5);
            lang.Structure.AddSyllable("VS", 1.25);

            lang.Structure.AddSyllable("FVN", 1.5);
            lang.Structure.AddSyllable("FVP", 1.0);
            lang.Structure.AddSyllable("FVR", 1.0);
            lang.Structure.AddSyllable("fRVN", 0.25);
            lang.Structure.AddSyllable("fRVP", 0.25);

            lang.Structure.AddSyllable("NVP", 1.5);
            lang.Structure.AddSyllable("SpVN", 1.5);
            lang.Structure.AddSyllable("SpRVP", 0.25);
            lang.Structure.AddSyllable("SpRVN", 0.25);

            lang.Structure.AddSyllable("TVN", 1.75);
            lang.Structure.AddSyllable("TVP", 1.75);
            lang.Structure.AddSyllable("TVR", 1.75);
            lang.Structure.AddSyllable("TRVN", 0.5);
            lang.Structure.AddSyllable("TRVP", 0.5);
            lang.Structure.AddSyllable("TVn", 0.25);

            lang.Structure.AddSyllable("PV", 1.0);
            lang.Structure.AddSyllable("PVR", 1.25);
            lang.Structure.AddSyllable("PVN", 1.5);

            lang.Structure.AddSyllable("AVR", 1.0);
            //lang.Structure.AddSyllable("ARV", 1.0);
            lang.Structure.AddSyllable("on", 0.5);

            lang.Lexicon.AddSyllable(lang, "leaves", "VR", "FVR");

            lang.OnLetter += (lg, word, letter) =>
                lg.LETTER_Replace(word, letter, letter.AdjacentLeft, 'ü',
                    lg.LETTER_Any(letter.AdjacentLeft, 'u') && letter.Letter.Key == 'l');

            lang.OnSyllableSelection += (lg, selection, word, last, current, max) =>
            {
                if (last != null)
                {
                    //Last syllable ends in consonant; therefore, the current syllable *must* start with a vowel.
                    if (last.Syllable.Template.Last().Key != 'V' &&
                        last.Syllable.Template.Last().Key != 'o' &&
                        last.Syllable.Template.Last().Key != 'O')
                    {
                        for (int i = 0; i < selection.Count; i++)
                        {
                            if (selection[i].Template.First().Key != 'V' &&
                                selection[i].Template.First().Key != 'o' &&
                                selection[i].Template.First().Key != 'O')
                            {
                                selection.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            };

            lang.OnSyllableSelection += (lg, selection, word, last, current, max) =>
            {
                if (last != null)
                {
                    //Last syllable ends in vowel; therefore, the current syllable *must* start with a consonant.
                    if (last.Syllable.Template.Last().Key == 'V' &&
                        last.Syllable.Template.Last().Key == 'o' &&
                        last.Syllable.Template.Last().Key == 'O')
                    {
                        for (int i = 0; i < selection.Count; i++)
                        {
                            if (selection[i].Template.First().Key == 'V' &&
                                selection[i].Template.First().Key == 'o' &&
                                selection[i].Template.First().Key == 'O')
                            {
                                selection.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            };

            //Manual consonant doubling, depending on syllable condition—group type, letter type ('l'), syllable location.
            //(where "T" is th/sh letters, "V" are vowels, and "R" is r or l.
            /*lang.OnLetter += (lg, word, letter) =>
                lg.LETTER_Insert(word, letter, 'l', 0,
                lg.LETTER_Syllable(letter, "TVR") &&
                lg.LETTER_Contains(letter, 'l') &&
                (letter.Syllable.SyllableIndex < word.Syllables.Count - 1));*/
        }
        #endregion

        #region Lexemes
        private void SetInflections()
        {
            lang.Lexicon.Inflections.Add("a", "om");
            lang.Lexicon.Inflections.Add("an", "om");
            lang.Lexicon.Inflections.Add("the", "bem");
            lang.Lexicon.Inflections.Add("as", "en");
            lang.Lexicon.Inflections.Add("is", "ha");
            lang.Lexicon.Inflections.Add("was", "hel");
            lang.Lexicon.Inflections.Add("were", "hend");

            /*lang.Lexicon.Inflections.Add("in", "ul");
            lang.Lexicon.Inflections.Add("on", "el");
            lang.Lexicon.Inflections.Add("at", "ön");

            lang.Lexicon.Inflections.Add("he", "bem");
            lang.Lexicon.Inflections.Add("him", "bem");
            lang.Lexicon.Inflections.Add("himself", "bem");
            lang.Lexicon.Inflections.Add("she", "bem");
            lang.Lexicon.Inflections.Add("her", "bem");
            lang.Lexicon.Inflections.Add("herself", "bem");
            lang.Lexicon.Inflections.Add("it", "bem");
            lang.Lexicon.Inflections.Add("itself", "bem");

            lang.Lexicon.Inflections.Add("i", "rende");
            lang.Lexicon.Inflections.Add("me", "remet");
            lang.Lexicon.Inflections.Add("myself", "rölden");
            lang.Lexicon.Inflections.Add("we", "ken");
            lang.Lexicon.Inflections.Add("us", "könden");
            lang.Lexicon.Inflections.Add("them", "felk");
            lang.Lexicon.Inflections.Add("themselves", "felken");


            lang.Lexicon.Inflections.Add("this", "bën");
            lang.Lexicon.Inflections.Add("that", "bön");

            lang.Lexicon.Inflections.Add("here", "ŝën");
            lang.Lexicon.Inflections.Add("there", "ŝen");
            lang.Lexicon.Inflections.Add("where", "ŝun");

            lang.Lexicon.Inflections.Add("then", "sten");
            lang.Lexicon.Inflections.Add("when", "stul");

            lang.Lexicon.Inflections.Add("who", "yun");
            lang.Lexicon.Inflections.Add("what", "yöp");
            lang.Lexicon.Inflections.Add("why", "Þal");
            lang.Lexicon.Inflections.Add("how", "hïn");*/

            lang.Lexicon.Inflections.Add("ah", "oh");
            lang.Lexicon.Inflections.Add("ahh", "ohh");
            lang.Lexicon.Inflections.Add("ahhh", "ohhh");

            lang.Lexicon.Inflections.Add("word", "qen");
            lang.Lexicon.Inflections.Add("language", "qendin");
            lang.Lexicon.Inflections.Add("voice", "elis");
            lang.Lexicon.Inflections.Add("speak", "qenelis");
            lang.Lexicon.Inflections.Add("speech", "qeneli");
        }
        private void SetAffixes()
        {
            lang.Lexicon.Affixes.Add(new Affix("'s", "'en", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("ly", "il", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("s", "s", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("less", "nöl", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));

            lang.OnSuffix += (lg, word, current) => lg.AFFIX_Remove(word, current, "ly", 0, 1, lg.AFFIX_VowelLast(word, current));
            lang.OnSuffix += (lg, word, current) => lg.AFFIX_Insert(word, current, "s", "l", 0, lg.AFFIX_VowelLast(word, current));
            lang.OnSuffix += (lg, word, current) => lg.AFFIX_Remove(word, current, "less", 0, 1, lg.AFFIX_ConsonantLast(word, current));
        }
        #endregion

        #region Punctuation
        private void SetPunctuation()
        {
            lang.OnConstruct += (lg, word) => lang.Punctuation.Process(lg, word, "PUNCTUATION");

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
            lang.OnConstruct += (lg, word) => lang.Flags.Process(lg, word, "FLAGS");

            lang.Flags.Add("<HIDE", lang.Flags.ACTION_HideLeft);
            lang.Flags.Add("HIDE>", lang.Flags.ACTION_HideRight);
            lang.Flags.Add("<HIDE>", lang.Flags.ACTION_HideAdjacents);
            lang.Flags.Add("NOGEN", lang.Flags.ACTION_NoGenerate);
            lang.Flags.Add("NOAFFIX", lang.Flags.ACTION_NoAffixes);
            lang.Flags.Add("MARK", (lg, word) =>
            {
                word.Filter = lg.Deconstruct.GetFilter("MARK");
                word.WordFinal = string.Empty;
                word.IsProcessed = true;
            });
        }
        #endregion

        public void SetNumbers()
        {
            lang.OnConstruct += (lg, word) => lang.Numbers.Process(lg, word, "NUMBERS", '-', Char.MinValue);
            lang.Numbers.Add(('0', '○', ""),
                ('1', '•', ""), ('2', '›', ""), ('3', '△', ""),
                ('4', '◇', ""), ('5', '⨰', ""), ('6', '∓', ""),
                ('7', '⪦', ""), ('8', '⋇', ""), ('9', '⨳', ""));
        }
    }
}
