using PLGL.Data;
using PLGL.Languages;
using PLGL.Processing;
using System;

namespace PLGL.Examples
{
    public class Qen : Language
    {
        public Qen()
        {
            META_Author = "Highverve";
            META_Name = "Qen";
            META_Nickname = "Qen";
            META_Description = "The common language of the Rootfolk from Bälore and Boffer.";

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
        }

        private void SetOptions()
        {
            Options.LetterPathing = LanguageOptions.PathSelection.Inclusion;
            Options.MemorizeWords = true;
            Options.SyllableSkewMin = (count) => { return 1; };
            Options.SyllableSkewMax = (count) => { return 1.3; };
            Options.SeedOffset = 15;

            Options.AllowAutomaticCasing = true;
            Options.AllowRandomCase = true;
            Options.CountSyllables = Options.EnglishSyllableCount;
        }

        #region Processing
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
            SetNumbers();
            SetFlagging();
        }

        private void SetPunctuation()
        {
            OnConstruct += (lg, word) => Punctuation.Process(lg, word, "PUNCTUATION");

            Punctuation.Add(".", (w) => { return "৹"; });
            Punctuation.Add(",", (w) => { return ","; });
            Punctuation.Add(";", (w) => { return ";"; });
            Punctuation.Add("!", (w) => { return "!"; });
            Punctuation.Add("$", (w) => { return "$"; });
        }
        public void SetNumbers()
        {
            OnConstruct += (lg, word) => Numbers.Process(lg, word, "NUMBERS", '-', Char.MinValue);
            Numbers.Add(('0', '○', ""),
                ('1', '•', ""), ('2', '›', ""), ('3', '△', ""),
                ('4', '◇', ""), ('5', '⨰', ""), ('6', '∓', ""),
                ('7', '⪦', ""), ('8', '⋇', ""), ('9', '⨳', ""));
        }
        public void SetFlagging()
        {
            OnConstruct += (lg, word) => Flags.Process(lg, word, "FLAGS");

            Flags.Add("<HIDE", Flags.ACTION_HideLeft);
            Flags.Add("HIDE>", Flags.ACTION_HideRight);
            Flags.Add("<HIDE>", Flags.ACTION_HideAdjacents);
            Flags.Add("NOGEN", Flags.ACTION_NoGenerate);
            Flags.Add("NOAFFIX", Flags.ACTION_NoAffixes);
            Flags.Add("MARK", (lg, word) =>
            {
                word.Filter = lg.Deconstruct.GetFilter("MARK");
                word.WordFinal = string.Empty;
                word.IsProcessed = true;
            });
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
            Alphabet.AddVowel("", 'a', ('a', 'A'), "/a/ (apple)");
            Alphabet.AddVowel("", 'e', ('e', 'E'), "/e/ (west)");
            Alphabet.AddVowel("", 'i', ('i', 'I'), "/i/ (tip)");
            Alphabet.AddVowel("", 'o', ('o', 'O'), "/o/ (crop)");
            Alphabet.AddVowel("", 'u', ('u', 'U'), "/u/ (sun)");
            //Alphabet.AddVowel("", 'ŕ', ('ŕ', 'Ŕ'), 5, "/r/ (later)");

            //Ää, Ëë, Ïï, Öö, Üü (long)
            Alphabet.AddVowel("", 'ä', ('ä', 'Ä'), "/ae/ (cake)");
            Alphabet.AddVowel("", 'ë', ('ë', 'Ë'), "/ee/ beet");
            Alphabet.AddVowel("", 'ï', ('ï', 'Ï'), "/ai/ (night)");
            Alphabet.AddVowel("", 'ö', ('ö', 'Ö'), "/ou/ (toad)");
            Alphabet.AddVowel("", 'ü', ('ü', 'Ü'), "/oo/ (tulip)");

            //Bb Pp, Dd Tt, Gg Kk
            Alphabet.AddConsonant('b', ('b', 'B'));
            Alphabet.AddConsonant('p', ('p', 'P'));
            Alphabet.AddConsonant('d', ('d', 'D'));
            Alphabet.AddConsonant('t', ('t', 'T'));
            Alphabet.AddConsonant('g', ('g', 'G'));
            Alphabet.AddConsonant('k', ('k', 'K'));

            //Mm, Nn, Ŋŋ, Rr, Ll
            Alphabet.AddConsonant('m', ('m', 'M'));
            Alphabet.AddConsonant('n', ('n', 'N'));
            Alphabet.AddConsonant('ŋ', ('ŋ', 'Ŋ')); //Ng ŋ (endi-ng). Never in onset.
            Alphabet.AddConsonant('r', ('r', 'R'));
            Alphabet.AddConsonant('l', ('l', 'L'));

            //Ff, Vv, Ss, Zz
            Alphabet.AddConsonant('f', ('f', 'F'));
            Alphabet.AddConsonant('v', ('v', 'V'));
            Alphabet.AddConsonant('s', ('s', 'S'));
            Alphabet.AddConsonant('z', ('z', 'Z'));

            //Ww, Yy, Hh, Qq, Ŝŝ, þÞ, Żż
            Alphabet.AddConsonant('w', ('w', 'W'));
            Alphabet.AddConsonant('y', ('y', 'Y')); //Never as a vowel.
            Alphabet.AddConsonant('h', ('h', 'H'));
            Alphabet.AddConsonant('q', ('q', 'Q')); //Kw/qu, thus never in coda.
            Alphabet.AddConsonant('ŝ', ('ŝ', 'Ŝ')); //Sh ŝ (ship)
            Alphabet.AddConsonant('Þ', ('Þ', 'þ')); //Th þ (thatch)
            Alphabet.AddConsonant('ż', ('ż', 'Ż')); //Ezh ʒ (azure)
        }
        private void SetStructure()
        {
            Structure.AddGroup('V', "Vowels", ('a', 5.0), ('e', 7.0), ('i', 3.0), ('o', 5.0), ('u', 7.0),
                                               ('ä', 2.0), ('ë', 5.0), ('ï', 3.0), ('ö', 6.0), ('ü', 1.0));
            Structure.AddGroup('N', "Nasal", ('m', 10.0), ('n', 10.0), ('ŋ', 10.0));
            Structure.AddGroup('P', "Plosive", ('b', 12.0), ('p', 4.0), ('d', 5.0), ('t', 3.0), ('g', 5.0), ('k', 1.0));
            Structure.AddGroup('F', "F/V", ('f', 10.0), ('v', 1.0));
            Structure.AddGroup('S', "S/SH/TH/Z/ZH", ('s', 10.0), ('ŝ', 1.0), ('Þ', 1.0), ('z', 1.0), ('ż', 0.1));
            Structure.AddGroup('A', "W/Y/H/Q", ('w', 1.0), ('y', 1.0), ('h', 1.0), ('q', 0.1)); //These contain a few (A)pproximants
            Structure.AddGroup('R', "R/L", ('r', 50), ('l', 50));

            //
            Structure.AddSyllable("V", 1.0, "Simple");

            //I define the "Simple" tag as containing no more than an onset-nucleus or nucleus-coda combination.
            Structure.AddSyllable("VN", 0.75, "Simple");
            Structure.AddSyllable("VP", 0.5, "Simple");
            Structure.AddSyllable("VR", 1.0, "Simple");
            Structure.AddSyllable("VS", 0.75, "Simple");

            Structure.AddSyllable("NV", 0.75, "Simple");
            Structure.AddSyllable("PV", 0.5, "Simple");
            Structure.AddSyllable("RV", 0.3, "Simple");
            Structure.AddSyllable("FV", 0.25, "Simple");

            //I define the "Medium" tag as containing only three letter groups (thus, a onset-nucleus-coda pattern),
            //and also double consonant ends.

            Structure.AddSyllable("NVN", 0.75, "Medium");
            Structure.AddSyllable("NVP", 0.75, "Medium");
            Structure.AddSyllable("NVR", 0.5, "Medium");
            Structure.AddSyllable("NVF", 0.25, "Medium");
            Structure.AddSyllable("NVRR", 0.25, "Medium", "DoubleEnd");

            Structure.AddSyllable("PVN", 0.5, "Medium");
            Structure.AddSyllable("PVF", 0.25, "Medium");
            Structure.AddSyllable("PVR", 0.75, "Medium");
            Structure.AddSyllable("PVRR", 0.5, "Medium", "DoubleEnd");

            Structure.AddSyllable("FVN", 0.75, "Medium");
            Structure.AddSyllable("FVP", 0.25, "Medium");
            Structure.AddSyllable("FVR", 0.85, "Medium");
            Structure.AddSyllable("FVRR", 0.25, "Medium", "DoubleEnd");

            Structure.AddSyllable("SVN", 0.75, "Medium");
            Structure.AddSyllable("SVP", 0.75, "Medium");
            Structure.AddSyllable("SVF", 0.25, "Medium");
            Structure.AddSyllable("SVR", 0.75, "Medium");
            Structure.AddSyllable("SVRR", 0.5, "Medium", "DoubleEnd");

            Structure.AddSyllable("AVN", 0.75, "Medium");
            Structure.AddSyllable("AVP", 0.5, "Medium");
            Structure.AddSyllable("AVR", 0.3, "Medium");
            Structure.AddSyllable("AVS", 0.1, "Medium");

            //I define the "Complex" tag as having more than one group in either or both of its onset or coda.
            Structure.AddSyllable("NVRN", 0.25, "Complex");
            Structure.AddSyllable("NVRS", 0.05, "Complex");

            Structure.AddSyllable("PRVN", 0.25, "Complex");
            Structure.AddSyllable("PRVP", 0.25, "Complex");
            Structure.AddSyllable("PRVS", 0.25, "Complex");
            Structure.AddSyllable("PVRN", 0.1, "Complex");
            Structure.AddSyllable("PVRP", 0.3, "Complex");
            Structure.AddSyllable("PVRS", 0.25, "Complex");
            Structure.AddSyllable("PVNS", 0.05, "Complex");

            Structure.AddSyllable("FRVN", 0.25, "Complex");
            Structure.AddSyllable("FRVP", 0.25, "Complex");
            Structure.AddSyllable("FRVS", 0.25, "Complex");

            Structure.AddSyllable("SPVN", 0.35, "Complex");
            Structure.AddSyllable("SPVR", 0.35, "Complex");
            Structure.AddSyllable("SRVN", 0.5, "Complex");
            Structure.AddSyllable("SRVP", 0.5, "Complex");
            Structure.AddSyllable("SPRVP", 0.25, "Complex");
            Structure.AddSyllable("SPRVN", 0.25, "Complex");

            SetExclusions();

            //Manual consonant doubling, depending on syllable condition—group type, letter type ('l'), syllable location.
            //(where "T" is th/sh letters, "V" are vowels, and "R" is r or l.
            /*OnLetter += (lg, word, letter) =>
                lg.LETTER_Insert(word, letter, 'l', 0,
                lg.LETTER_Syllable(letter, "TVR") &&
                lg.LETTER_Contains(letter, 'l') &&
                (letter.Syllable.SyllableIndex < word.Syllables.Count - 1));*/
        }
        private void SetExclusions()
        {
            #region 'R' doubling rules

            //Force double 'l' consonant if double 'R' group and last letter was 'l'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(last != null && lg.SELECT_GroupContains(syllable, "RR") && last.Letter.Key == 'l', 'r');

            //Force double 'r' consonant if double 'R' group and last letter was 'r'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(last != null && lg.SELECT_GroupContains(syllable, "RR") && last.Letter.Key == 'r', 'l');

            #endregion


            #region 'N' exclusion rules

            //Exclude 'ng' from 'N' group if its the first group of the syllable.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "N") &&
                lg.SELECT_Template(syllable, 0).Key == 'N', 'ŋ');

            //Exclude 'ng' from 'N' group if its not the last group of the syllable.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "NN") &&
                lg.SELECT_Template(syllable, 2).Key == 'N', 'ŋ');

            //Reduces 'ng' weight multiplier from 'n' group if its not the last syllable of the word.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_SetWeight('ŋ', 0.025, lg.SELECT_GroupContains(syllable, "N") && lg.SELECT_IsSyllableLast(syllable) == false);

            //Exclude 'ng' from 'N' group if the word has a suffix.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_IsGroupLast(syllable, 'N') &&
                (word.Suffixes != null && word.Suffixes.Count > 0), 'ŋ');

            //Exclude 'n' if the syllable is NVN and the first generated letter was 'm'
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "NVN") && current == 2 &&
                    (syllable.Letters.FirstOrDefault().Letter.Key == 'n'), 'n');

            //Exclude 'm' if the syllable is NVN and the first generated letter was 'm'
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "NVN") && current == 2 &&
                    (syllable.Letters.FirstOrDefault().Letter.Key == 'm'), 'm');

            #endregion

            #region 'R' exclusion rules

            //Exclude 'r' if the last letter equals s
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SR") && current == 1 &&
                    (last.Letter.Key == 's'), 'r');

            //Exclude 'l' if the last letter equals th
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SR") && current == 1 &&
                    (last.Letter.Key == 'Þ'), 'l');

            //Exclude 'l' if the last leter equals d or t
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "PR") && current == 1 &&
                    (last.Letter.Key == 'd' || last.Letter.Key == 't'), 'l');

            #endregion

            //Exclude 'v' from 'F' if the next group is 'R'.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "FR") && current == 0, 'v');

            //Exclude 'b', 'd', and 'g' if the last letter equals s or ŝ
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SP") && current == 1 &&
                    (last.Letter.Key == 's' || last.Letter.Key == 'ŝ'), 'b', 'd', 'g');

            //Exclude 'z' from 'S' if the next group key is 'P'
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SP") && current == 0, 'ż');

            //Exclude 'Þ' from 'S' if the next group key is 'P'
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SP") && current == 0, 'Þ');

            //Exclude 'th' from current if next group is 'P' and not last group index.
            OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(current < max && syllable.Syllable.Template[current + 1].Key == 'P', 'Þ');

            #region 'V' exclusion rules

            //Vowel rule exclusion for doubling vowels (where each vowel is it's own syllable, not as a gliding vowel):
            //  1. The last syllable ends in 'V'
            //  2. The last syllable is NOT equal to "V"
            //  3. And, the word's seed ends in 2, 5, or 7.
            OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
                lg.SELECT_Keep(syllable != null &&
                lg.SELECT_IsGroupLast(syllable, 'V') &&
                syllable.Syllable.Letters != "V" &&
                lg.SEED_EndsAny(2,5,7), "V");

            #endregion

            //Excludes the last syllable group by tag from the current selection.
            //This produces a pattern of simple-complex-medium-complex-simple-medium-etc.
            /*OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            {
                if (current > 0)
                {
                    for (int i = 0; i < selection.Count; i++)
                    {
                        if (lg.SELECT_TagsAny(selection[i], syllable.Syllable.Tags[0]))
                        {
                            selection.RemoveAt(i);
                            i--;
                        }
                    }
                }
            };*/

            //Removes all complex syllables if not the first syllable.
            //OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            //    lg.SELECT_Keep(current != 0, "VN", "VP", "VR", "VS");
            //OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            //    lg.SELECT_Exclude(current == 0, "VN", "VP", "VR", "VS");

            //
            //Double 'l' if left and right letters are vowels.

            //Last syllable ends in consonant; therefore, the current syllable *must* start with a vowel.
            OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
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
            OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
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
            OnLetterSelection += (lg, selection, word, syllable, letter, current, max) =>
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
            Lexicon.AddSyllable("leave", "VR", "FVR");

            Lexicon.AddSyllable("song", "VR", "AVN");
            Lexicon.AddSyllable("sing", "VR", "AVN");
            Lexicon.AddSyllable("sang", "VR", "AVN");
            Lexicon.AddSyllable("sung", "VR", "AVN");

            Lexicon.AddSyllable("star", "SPVR", "VN");
            Lexicon.AddSyllable("west", "AVR", "VN");
            Lexicon.AddSyllable("ho", "AVS");

            Lexicon.Vocabulary.Add("a", "o");
            Lexicon.Vocabulary.Add("an", "olt");
            Lexicon.Vocabulary.Add("the", "lo");
            Lexicon.Vocabulary.Add("as", "egel");
            Lexicon.Vocabulary.Add("is", "ha");
            Lexicon.Vocabulary.Add("was", "hel");
            Lexicon.Vocabulary.Add("were", "hend");

            Lexicon.Vocabulary.Add("of", "ha");

            /*Lexicon.Inflections.Add("in", "ul");
            Lexicon.Inflections.Add("on", "el");
            Lexicon.Inflections.Add("at", "ön");

            Lexicon.Inflections.Add("he", "bem");
            Lexicon.Inflections.Add("him", "bem");
            Lexicon.Inflections.Add("himself", "bem");
            Lexicon.Inflections.Add("she", "bem");
            Lexicon.Inflections.Add("her", "bem");
            Lexicon.Inflections.Add("herself", "bem");
            Lexicon.Inflections.Add("it", "bem");
            Lexicon.Inflections.Add("itself", "bem");

            Lexicon.Inflections.Add("i", "rende");
            Lexicon.Inflections.Add("me", "remet");
            Lexicon.Inflections.Add("myself", "rölden");
            Lexicon.Inflections.Add("we", "ken");
            Lexicon.Inflections.Add("us", "könden");
            Lexicon.Inflections.Add("them", "felk");
            Lexicon.Inflections.Add("themselves", "felken");


            Lexicon.Inflections.Add("this", "bën");
            Lexicon.Inflections.Add("that", "bön");

            Lexicon.Inflections.Add("here", "ŝën");
            Lexicon.Inflections.Add("there", "ŝen");
            Lexicon.Inflections.Add("where", "ŝun");

            Lexicon.Inflections.Add("then", "sten");
            Lexicon.Inflections.Add("when", "stul");

            Lexicon.Inflections.Add("who", "yun");
            Lexicon.Inflections.Add("what", "yöp");
            Lexicon.Inflections.Add("why", "Þal");
            Lexicon.Inflections.Add("how", "hïn");*/

            Lexicon.Vocabulary.Add("ah", "oh");
            Lexicon.Vocabulary.Add("ahh", "ohh");
            Lexicon.Vocabulary.Add("ahhh", "ohhh");

            Lexicon.Vocabulary.Add("word", "qen");
            Lexicon.Vocabulary.Add("language", "qendin");
            Lexicon.Vocabulary.Add("voice", "elis");
            Lexicon.Vocabulary.Add("speak", "qenelis");
            Lexicon.Vocabulary.Add("speech", "qeneli");
        }
        private void SetAffixes()
        {
            //Prefixes
            Lexicon.Affixes.Add(new Affix("un", "da", Affix.AffixLocation.Prefix, Affix.AffixLocation.Prefix, LetterGroups:"PV"));

            //Suffixes
            Lexicon.Affixes.Add(new Affix("'s", "-it", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VP"));
            Lexicon.Affixes.Add(new Affix("ly", "il", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VR"));
            Lexicon.Affixes.Add(new Affix("s", "en", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VN"));
            Lexicon.Affixes.Add(new Affix("less", "nöl", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "NVR"));
            Lexicon.Affixes.Add(new Affix("ing", "arel", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VRVR"));

            //Events
            OnPrefix += (lg, word, current) => lg.AFFIX_Insert(word, current, "un", "l", 2, lg.AFFIX_VowelFirst(word, current));

            OnSuffix += (lg, word, current) => lg.AFFIX_Remove(word, current, "ly", 0, 1, lg.AFFIX_VowelLast(word, current));
            OnSuffix += (lg, word, current) => lg.AFFIX_Insert(word, current, "s", "l", 0, lg.AFFIX_VowelLast(word, current));
            OnSuffix += (lg, word, current) => lg.AFFIX_Remove(word, current, "less", 0, 1, lg.AFFIX_ConsonantLast(word, current));
        }
        #endregion
    }
}
