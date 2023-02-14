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
            lang.Options.MemorizeWords = true;
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
            lang.Structure.AddGroup('N', "Nasal", ('m', 10.0), ('n', 10.0), ('ŋ', 10.0));
            lang.Structure.AddGroup('P', "Plosive", ('b', 12.0), ('p', 4.0), ('d', 5.0), ('t', 3.0), ('g', 5.0), ('k', 1.0));
            lang.Structure.AddGroup('F', "F/V", ('f', 10.0), ('v', 1.0));
            lang.Structure.AddGroup('S', "S/SH/TH/Z/ZH", ('s', 10.0), ('ŝ', 1.0), ('Þ', 1.0), ('z', 1.0), ('ż', 0.1));
            lang.Structure.AddGroup('A', "W/Y/H/Q", ('w', 1.0), ('y', 1.0), ('h', 1.0), ('q', 0.1)); //These contain a few (A)pproximants
            lang.Structure.AddGroup('R', "R/L", ('r', 50), ('l', 50));

            //
            lang.Structure.AddSyllable("V", 1.0, "Simple");

            //I define the "Simple" tag as containing no more than an onset-nucleus or nucleus-coda combination.
            lang.Structure.AddSyllable("VN", 0.75, "Simple");
            lang.Structure.AddSyllable("VP", 0.5, "Simple");
            lang.Structure.AddSyllable("VR", 1.0, "Simple");
            lang.Structure.AddSyllable("VS", 0.75, "Simple");

            lang.Structure.AddSyllable("NV", 0.75, "Simple");
            lang.Structure.AddSyllable("PV", 0.5, "Simple");
            lang.Structure.AddSyllable("RV", 0.3, "Simple");
            lang.Structure.AddSyllable("FV", 0.25, "Simple");

            //I define the "Medium" tag as containing only three letter groups (thus, a onset-nucleus-coda pattern),
            //and also double consonant ends.

            lang.Structure.AddSyllable("NVN", 0.75, "Medium");
            lang.Structure.AddSyllable("NVP", 0.75, "Medium");
            lang.Structure.AddSyllable("NVR", 0.5, "Medium");
            lang.Structure.AddSyllable("NVF", 0.25, "Medium");
            lang.Structure.AddSyllable("NVRR", 0.25, "Medium", "DoubleEnd");

            lang.Structure.AddSyllable("PVN", 0.5, "Medium");
            lang.Structure.AddSyllable("PVF", 0.25, "Medium");
            lang.Structure.AddSyllable("PVR", 0.75, "Medium");
            lang.Structure.AddSyllable("PVRR", 0.5, "Medium", "DoubleEnd");

            lang.Structure.AddSyllable("FVN", 0.75, "Medium");
            lang.Structure.AddSyllable("FVP", 0.25, "Medium");
            lang.Structure.AddSyllable("FVR", 0.85, "Medium");
            lang.Structure.AddSyllable("FVRR", 0.25, "Medium", "DoubleEnd");

            lang.Structure.AddSyllable("SVN", 0.75, "Medium");
            lang.Structure.AddSyllable("SVP", 0.75, "Medium");
            lang.Structure.AddSyllable("SVF", 0.25, "Medium");
            lang.Structure.AddSyllable("SVR", 0.75, "Medium");
            lang.Structure.AddSyllable("SVRR", 0.5, "Medium", "DoubleEnd");

            lang.Structure.AddSyllable("AVN", 0.75, "Medium");
            lang.Structure.AddSyllable("AVP", 0.5, "Medium");
            lang.Structure.AddSyllable("AVR", 0.3, "Medium");
            lang.Structure.AddSyllable("AVS", 0.1, "Medium");

            //I define the "Complex" tag as having more than one group in either or both of its onset or coda.
            lang.Structure.AddSyllable("NVRN", 0.25, "Complex");
            lang.Structure.AddSyllable("NVRS", 0.05, "Complex");

            lang.Structure.AddSyllable("PRVN", 0.25, "Complex");
            lang.Structure.AddSyllable("PRVP", 0.25, "Complex");
            lang.Structure.AddSyllable("PRVS", 0.25, "Complex");
            lang.Structure.AddSyllable("PVRN", 0.1, "Complex");
            lang.Structure.AddSyllable("PVRP", 0.3, "Complex");
            lang.Structure.AddSyllable("PVRS", 0.25, "Complex");
            lang.Structure.AddSyllable("PVNS", 0.05, "Complex");

            lang.Structure.AddSyllable("FRVN", 0.25, "Complex");
            lang.Structure.AddSyllable("FRVP", 0.25, "Complex");
            lang.Structure.AddSyllable("FRVS", 0.25, "Complex");

            lang.Structure.AddSyllable("SPVN", 0.35, "Complex");
            lang.Structure.AddSyllable("SPVR", 0.35, "Complex");
            lang.Structure.AddSyllable("SRVN", 0.5, "Complex");
            lang.Structure.AddSyllable("SRVP", 0.5, "Complex");
            lang.Structure.AddSyllable("SPRVP", 0.25, "Complex");
            lang.Structure.AddSyllable("SPRVN", 0.25, "Complex");

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
            #region 'R' doubling rules

            //Force double 'l' consonant if double 'R' group and last letter was 'l'.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(last != null && lg.SELECT_GroupContains(syllable, "RR") && last.Letter.Key == 'l', 'r');

            //Force double 'r' consonant if double 'R' group and last letter was 'r'.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(last != null && lg.SELECT_GroupContains(syllable, "RR") && last.Letter.Key == 'r', 'l');

            #endregion


            #region 'N' exclusion rules

            //Exclude 'ng' from 'N' group if its the first group of the syllable.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "N") &&
                lg.SELECT_Template(syllable, 0).Key == 'N', 'ŋ');

            //Exclude 'ng' from 'N' group if its not the last group of the syllable.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "NN") &&
                lg.SELECT_Template(syllable, 2).Key == 'N', 'ŋ');

            //Reduces 'ng' weight multiplier from 'n' group if its not the last syllable of the word.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_SetWeight('ŋ', 0.025, lg.SELECT_GroupContains(syllable, "N") && lg.SELECT_IsSyllableLast(syllable) == false);

            //Exclude 'ng' from 'N' group if the word has a suffix.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_IsGroupLast(syllable, 'N') &&
                (word.Suffixes != null && word.Suffixes.Count > 0), 'ŋ');

            //Exclude 'n' if the syllable is NVN and the first generated letter was 'm'
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "NVN") && current == 2 &&
                    (syllable.Letters.FirstOrDefault().Letter.Key == 'n'), 'n');

            //Exclude 'm' if the syllable is NVN and the first generated letter was 'm'
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "NVN") && current == 2 &&
                    (syllable.Letters.FirstOrDefault().Letter.Key == 'm'), 'm');

            #endregion

            #region 'R' exclusion rules

            //Exclude 'r' if the last letter equals s
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SR") && current == 1 &&
                    (last.Letter.Key == 's'), 'r');

            //Exclude 'l' if the last letter equals th
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "SR") && current == 1 &&
                    (last.Letter.Key == 'Þ'), 'l');

            //Exclude 'l' if the last leter equals d or t
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "PR") && current == 1 &&
                    (last.Letter.Key == 'd' || last.Letter.Key == 't'), 'l');

            #endregion

            //Exclude 'v' from 'F' if the next group is 'R'.
            lang.OnLetterSelection += (lg, selection, word, syllable, last, current, max) =>
                lg.SELECT_Exclude(lg.SELECT_GroupContains(syllable, "FR") && current == 0, 'v');

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

            #region 'V' exclusion rules

            //Vowel rule exclusion for doubling vowels (where each vowel is it's own syllable, not as a gliding vowel):
            //  1. The last syllable ends in 'V'
            //  2. The last syllable is NOT equal to "V"
            //  3. And, the word's seed ends in 2, 5, or 7.
            lang.OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
                lg.SELECT_Keep(syllable != null &&
                lg.SELECT_IsGroupLast(syllable, 'V') &&
                syllable.Syllable.Letters != "V" &&
                lg.SEED_EndsAny(2,5,7), "V");

            #endregion

            //Excludes the last syllable group by tag from the current selection.
            //This produces a pattern of simple-complex-medium-complex-simple-medium-etc.
            /*lang.OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
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
            //lang.OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            //    lg.SELECT_Keep(current != 0, "VN", "VP", "VR", "VS");
            //lang.OnSyllableSelection += (lg, selection, word, syllable, current, max) =>
            //    lg.SELECT_Exclude(current == 0, "VN", "VP", "VR", "VS");

            //
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

            lang.Lexicon.AddSyllable("star", "SPVR", "VN");
            lang.Lexicon.AddSyllable("west", "AVR", "VN");
            lang.Lexicon.AddSyllable("ho", "AVS");

            lang.Lexicon.Vocabulary.Add("a", "o");
            lang.Lexicon.Vocabulary.Add("an", "olt");
            lang.Lexicon.Vocabulary.Add("the", "lo");
            lang.Lexicon.Vocabulary.Add("as", "egel");
            lang.Lexicon.Vocabulary.Add("is", "ha");
            lang.Lexicon.Vocabulary.Add("was", "hel");
            lang.Lexicon.Vocabulary.Add("were", "hend");

            lang.Lexicon.Vocabulary.Add("of", "ha");

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

            lang.Lexicon.Vocabulary.Add("ah", "oh");
            lang.Lexicon.Vocabulary.Add("ahh", "ohh");
            lang.Lexicon.Vocabulary.Add("ahhh", "ohhh");

            lang.Lexicon.Vocabulary.Add("word", "qen");
            lang.Lexicon.Vocabulary.Add("language", "qendin");
            lang.Lexicon.Vocabulary.Add("voice", "elis");
            lang.Lexicon.Vocabulary.Add("speak", "qenelis");
            lang.Lexicon.Vocabulary.Add("speech", "qeneli");
        }
        private void SetAffixes()
        {
            //Prefixes
            lang.Lexicon.Affixes.Add(new Affix("un", "da", Affix.AffixLocation.Prefix, Affix.AffixLocation.Prefix, LetterGroups:"PV"));

            //Suffixes
            lang.Lexicon.Affixes.Add(new Affix("'s", "-it", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VP"));
            lang.Lexicon.Affixes.Add(new Affix("ly", "il", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VR"));
            lang.Lexicon.Affixes.Add(new Affix("s", "en", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VN"));
            lang.Lexicon.Affixes.Add(new Affix("less", "nöl", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "NVR"));
            lang.Lexicon.Affixes.Add(new Affix("ing", "arel", Affix.AffixLocation.Suffix, Affix.AffixLocation.Suffix, LetterGroups: "VRVR"));

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
