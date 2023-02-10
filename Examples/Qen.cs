using PLGL.Data;
using PLGL.Languages;
using PLGL.Processing;
using System;

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
            lang.Options.SyllableSkewMin = 1;
            lang.Options.SyllableSkewMax = 1.3;
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
            lang.Alphabet.AddVowel("", 'a', ('a', 'A'), "/a/ (apple)");
            lang.Alphabet.AddVowel("", 'e', ('e', 'E'), "/e/ (west)");
            lang.Alphabet.AddVowel("", 'i', ('i', 'I'), "/i/ (tip)");
            lang.Alphabet.AddVowel("", 'o', ('o', 'O'), "/o/ (crop)");
            lang.Alphabet.AddVowel("", 'u', ('u', 'U'), "/u/ (sun)");
            //lang.Alphabet.AddVowel("", 'ŕ', ('ŕ', 'Ŕ'), 5, "/r/ (later)");

            //Ää, Ëë, Ïï, Öö, Üü (long)
            lang.Alphabet.AddVowel("", 'ä', ('ä', 'Ä'), "/ae/ (cake)");
            lang.Alphabet.AddVowel("", 'ë', ('ë', 'Ë'), "/ee/ beet");
            lang.Alphabet.AddVowel("", 'ï', ('ï', 'Ï'), "/ai/ (night)");
            lang.Alphabet.AddVowel("", 'ö', ('ö', 'Ö'), "/ou/ (toad)");
            lang.Alphabet.AddVowel("", 'ü', ('ü', 'Ü'), "/oo/ (tulip)");

            //Bb Pp, Dd Tt, Gg Kk
            lang.Alphabet.AddConsonant('b', ('b', 'B'));
            lang.Alphabet.AddConsonant('p', ('p', 'P'));
            lang.Alphabet.AddConsonant('d', ('d', 'D'));
            lang.Alphabet.AddConsonant('t', ('t', 'T'));
            lang.Alphabet.AddConsonant('g', ('g', 'G'));
            lang.Alphabet.AddConsonant('k', ('k', 'K'));

            //Mm, Nn, Ŋŋ, Rr, Ll
            lang.Alphabet.AddConsonant('m', ('m', 'M'));
            lang.Alphabet.AddConsonant('n', ('n', 'N'));
            lang.Alphabet.AddConsonant('ŋ', ('ŋ', 'Ŋ')); //Ng ŋ (endi-ng). Never in onset.
            lang.Alphabet.AddConsonant('r', ('r', 'R'));
            lang.Alphabet.AddConsonant('l', ('l', 'L'));

            //Ff, Vv, Ss, Zz
            lang.Alphabet.AddConsonant('f', ('f', 'F'));
            lang.Alphabet.AddConsonant('v', ('v', 'V'));
            lang.Alphabet.AddConsonant('s', ('s', 'S'));
            lang.Alphabet.AddConsonant('z', ('z', 'Z'));

            //Ww, Yy, Hh, Qq, Ŝŝ, þÞ, Żż
            lang.Alphabet.AddConsonant('w', ('w', 'W'));
            lang.Alphabet.AddConsonant('y', ('y', 'Y')); //Never as a vowel.
            lang.Alphabet.AddConsonant('h', ('h', 'H'));
            lang.Alphabet.AddConsonant('q', ('q', 'Q')); //Kw/qu, thus never in coda.
            lang.Alphabet.AddConsonant('ŝ', ('ŝ', 'Ŝ')); //Sh ŝ (ship)
            lang.Alphabet.AddConsonant('Þ', ('Þ', 'þ')); //Th þ (thatch)
            lang.Alphabet.AddConsonant('ż', ('ż', 'Ż')); //Ezh ʒ (azure)
        }
        private void SetStructure()
        {
            lang.Structure.AddGroup('V', "Vowels", ('a', 5.0), ('e', 7.0), ('i', 3.0), ('o', 5.0), ('u', 7.0),
                                               ('ä', 2.0), ('ë', 5.0), ('ï', 3.0), ('ö', 6.0), ('ü', 1.0));
            lang.Structure.AddGroup('o', "Vowels (short)", ('a', 1.0), ('e', 1.0), ('u', 1.0), ('o', 1.0), ('u', 1.0));
            lang.Structure.AddGroup('O', "Vowels (long)", ('ä', 1.0), ('ë', 1.0), ('ï', 1.0), ('ö', 1.0), ('ü', 1.0));

            lang.Structure.AddGroup('N', "Nasal", ('m', 10.0), ('n', 10.0), ('ŋ', 10.0));
            lang.Structure.AddGroup('P', "Plosive", ('b', 12.0), ('p', 4.0), ('d', 5.0), ('t', 3.0), ('g', 5.0), ('k', 1.0));
            lang.Structure.AddGroup('F', "Fricative", ('f', 10.0), ('v', 1.0), ('s', 10.0), ('z', 1.0));
            lang.Structure.AddGroup('f', "Fricative f", ('f', 10.0));
            lang.Structure.AddGroup('S', "S/SH/TH/ZH", ('s', 10.0), ('ŝ', 1.0), ('Þ', 1.0), ('ż', 0.1));
            lang.Structure.AddGroup('A', "Approximant", ('w', 1.0), ('y', 1.0), ('h', 1.0), ('q', 0.1));
            lang.Structure.AddGroup('R', "R / L", ('r', 50), ('l', 50));

            lang.Structure.AddSyllable("VN", 0.75);
            lang.Structure.AddSyllable("VP", 0.5);
            lang.Structure.AddSyllable("VR", 1.0);
            lang.Structure.AddSyllable("VS", 0.75);

            lang.Structure.AddSyllable("NV", 0.75);
            lang.Structure.AddSyllable("PV", 0.5);
            lang.Structure.AddSyllable("RV", 0.3);
            lang.Structure.AddSyllable("FV", 0.25);

            lang.Structure.AddSyllable("FVN", 1.0);
            lang.Structure.AddSyllable("FVP", 0.25);
            lang.Structure.AddSyllable("FVR", 0.85);
            lang.Structure.AddSyllable("fRVN", 0.25);
            lang.Structure.AddSyllable("fRVP", 0.25);

            lang.Structure.AddSyllable("NVP", 1.5);
            lang.Structure.AddSyllable("SPVN", 1.5);
            lang.Structure.AddSyllable("SPRVP", 0.25);
            lang.Structure.AddSyllable("SPRVN", 0.25);

            lang.Structure.AddSyllable("SVN", 0.75);
            lang.Structure.AddSyllable("SVP", 0.75);
            lang.Structure.AddSyllable("SVR", 0.75);
            lang.Structure.AddSyllable("SRVN", 0.5);
            lang.Structure.AddSyllable("SRVP", 0.5);
            lang.Structure.AddSyllable("SVN", 0.25);

            lang.Structure.AddSyllable("PVR", 0.75);
            lang.Structure.AddSyllable("PVN", 0.5);

            lang.Structure.AddSyllable("AVN", 0.75);
            lang.Structure.AddSyllable("AVP", 0.5);
            lang.Structure.AddSyllable("AVR", 0.3);
            lang.Structure.AddSyllable("AVS", 0.1);

            lang.OnLetter += (lg, word, letter) =>
                lg.LETTER_Replace(word, letter, letter.AdjacentLeft, 'ü',
                    lg.LETTER_Any(letter.AdjacentLeft, 'u') && letter.Letter.Key == 'l');

            SetExclusions();

            //Manual consonant doubling, depending on syllable condition—group type, letter type ('l'), syllable location.
            //(where "T" is th/sh letters, "V" are vowels, and "R" is r or l.
            /*lang.OnLetter += (lg, word, letter) =>
                lg.LETTER_Insert(word, letter, 'l', 0,
                lg.LETTER_Syllable(letter, "TVR") &&
                lg.LETTER_Contains(letter, 'l') &&
                (letter.Syllable.SyllableIndex < word.Syllables.Count - 1));*/
        }
        private void SetExclusions()
        {
            //Exclude 'r' if the last letter equals s or sh
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SR") && current == 1 &&
                    (last.Letter.Key == 's' || last.Letter.Key == 'ŝ'), 'r');

            //Exclude 'l' if the last letter equals th
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SR") && current == 1 &&
                    (last.Letter.Key == 'Þ'), 'l');

            //Exclude 'b', 'd', and 'g' if the last letter equals s or ŝ
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SP") && current == 1 &&
                    (last.Letter.Key == 's' || last.Letter.Key == 'ŝ'), 'b', 'd', 'g');

            //Exclude 'z' from 'S' if the next group key is 'P'
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SP") && current == 0, 'ż');

            //Exclude 'Þ' from 'S' if the next group key is 'P'
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SP") && current == 0, 'Þ');

            //Exclude 'th' from current if next group is 'P' and not last group index.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current < max && syllable.Syllable.Template[current + 1].Key == 'P', 'Þ');

            //Exclude 'ng' from 'N' group if its not the last group of the syllable.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_IsGroupLast(syllable, 'N') == false, 'ŋ');

            //Reduces the weight of 'ng' from 'n' group if its not the last syllable of the word.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_SetWeight('ŋ', 0.025, lg.SELECT_GroupContains(syllable, "N") && lg.SELECT_IsSyllableLast(syllable) == false);

            //Removes all complex syllables if not the first syllable.
            //lang.OnLetterSelection += (lg, selection, word, syllable, letter, current, max) =>
            //    lg.SELECT_Keep(current != 0, "VN", "VP", "VR", "VS");
            //lang.OnLetterSelection += (lg, selection, word, syllable, letter, current, max) =>
            //    lg.SELECT_Exclude(current == 0, "VN", "VP", "VR", "VS");

            //Double 'l' if left and right letters are vowels.


            //Last syllable ends in consonant; therefore, the current syllable *must* start with a vowel.
            lang.OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            {
                if (syllable != null)
                {
                    if (lg.SELECT_IsGroupLast(syllable, 'V', 'O', 'o') == false)
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
            
            //Last syllable ends in vowel; therefore, the current syllable *must* start with a consonant.
            lang.OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            {
                if (syllable != null)
                {
                    if (lg.SELECT_IsGroupLast(syllable, 'V', 'O', 'o') == false)
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

            //Excludes duplicate vowels, preventing two vowels from occuring in neighbouring syllables.
            lang.OnLetterSelection += (lg, selection, word, syllable, letter, current, max) =>
            {
                if (syllable.AdjacentLeft != null && lg.SELECT_Template(syllable, current)?.Key == 'V')
                {
                    LetterInfo leftVowel = lg.SELECT_Letter(syllable.AdjacentLeft, 'V');

                    if (leftVowel != null)
                    {
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'a', 'a');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'e', 'e');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'i', 'i');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'o', 'o');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'u', 'u');

                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'ä', 'ä');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'ë', 'ë');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'ï', 'ï');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'ö', 'ö');
                        lg.SELECT_Exclude(leftVowel.Letter.Key == 'ü', 'ü');
                    }
                }
            };
        }
        #endregion

        #region Lexemes
        private void SetInflections()
        {
            lang.Lexicon.AddSyllable("leave", "VR", "FVR");

            lang.Lexicon.AddSyllable("song", "VR", "AVN");
            lang.Lexicon.AddSyllable("sing", "VR", "AVN");
            lang.Lexicon.AddSyllable("sang", "VR", "AVN");
            lang.Lexicon.AddSyllable("sung", "VR", "AVN");

            lang.Lexicon.AddSyllable("ho", "AVS");

            lang.Lexicon.Inflections.Add("a", "om");
            lang.Lexicon.Inflections.Add("an", "om");
            lang.Lexicon.Inflections.Add("the", "lem");
            lang.Lexicon.Inflections.Add("as", "el");
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
            //Prefixes
            lang.Lexicon.Affixes.Add(new Affix("un", "da", Affix.AffixLocation.Prefix, Affix.AffixLocation.Prefix));

            //Suffixes
            lang.Lexicon.Affixes.Add(new Affix("'s", "'en", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("ly", "il", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("s", "", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));
            lang.Lexicon.Affixes.Add(new Affix("less", "nöl", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix));

            //Events
            lang.OnPrefix += (lg, word, current) => lg.AFFIX_Insert(word, current, "un", "l", 2, lg.AFFIX_VowelFirst(word, current));

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
