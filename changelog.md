﻿# PLGL Changelog

### 2022-12-6:
- Added weighted suffixes to each letter for diverse branching.
- Moved 1.1 changes into a separate class: Pathways. May be subject to a name change later.
- Created the Structure class for word formation (as "CV", "VC", "CVC", "CC", etc) by a weighted distribution. Good start, but more work to do.
- Added the foundation of the Lexemes class, that answers the question: "How do we handle lexemes?"
### 2022-12-7:
- Created the Punctuation class, for processing and manipulating punctuation marks.
- Added SyllableInfo and WordInfo. These classes are used by the generator for additional context.
- Added a few WordGenerator methods.
- Added the Markings class.
### 2022-12-8:
- Renamed WordGenerator to LanguageGenerator.
- Changed the Syllable class to Sigma, and improved how the data is structured. It now uses an onset-nucleus-coda approach, adding another layer above the "CVC" structure.
- Changed SyllableInfo to SigmaInfo, and it's structure to reflesh 1.10's changes.
- Temporarily gutted Structure.cs while I plan it's code upgrade.
### 2022-12-9:
- Designed an ideal scaffolding for letter pathing. It utilizes filters for word position, syllable position, and weighted distribution to pick the next letter.
### 2022-12-10:
- Moved the more broad options in Language to an Options class for better clarity.
- Renamed Markers to Flagging to clarify it's different from punctuation marks.
- Selected affixes (in Lexemes) are now ordered from longest to shortest. This prevent in- stopping inter- from parsing (in·ter·cept).
- Added "Skip" and "Possessive" booleans to WordInfo (though I should consider making Possessive flagging more modular).
- Added text position targeting to Flags: BeforePrefix, AfterPrefix, BeforeSuffix, AfterSuffix. Useful for flags that add text.
### 2022-12-12:
- Removed Skip and Possessive booleans from WordInfo, as I realized it was redundant.
- Progress in the generating method(s) for language generation.
- Defined SigmaWeight; a author-specified class that helps the generator decide which sigma to pick.
- Added InputConsonants/Vowels to Language.Options. This is for estimating syllable count.
- Added and tested a method to count syllables for any input word. According to my testing, it only has trouble with words that use y as a vowel.
- Small changes and additional variables to WordInfo.
- Added supporting methods to Structure.
- Added the Affix class, and supporting methods to Lexemes.
- Added a few classes inherited from Flagging: SkipGenerate, SkipLexemes, Add, and Remove.
### 2022-12-13:
- Coded in the barebone framework for word generation. Currently, it strips a word from it's affixes, generates the seed from the word root, compiles the sigma template (no weighting yet), sets the letters (without pathways yet), and outputs the result. Good progress, with more to accomplish.
### 2022-12-15:
- Added letter pathing support into the generator itself.
- Added the Lexicon class, which supports custom words from the root to the inflection ("lexeme") level.
- Added MemorizeWords boolean to Language.Options. If true, it will add newly generated words to the Lexicon's InflectedWords dictionary.
- Added syllable count skewing to the language generation. Set SigmaSkewMin and SigmaSkewMax in Language.Options.
- Tracked down a bug that prevented the generator from adding characters to the sigma's onset.
- Fixed a bug that was causing an extra vowel to be added if the nucleus was the start of the word.
### 2022-12-16:
- Merged Lexicon with Lexemes, and renamed Lexemes to Lexicon.
- Added Language switching to LanguageGenerator.
- Cleaned up the code in LanguageGenerator. Much nicer.
- Minor cleanup in various classes.
- Deleted SyllableGenerator as it's been made Redundant.
### 2022-12-17:
- Deleted Main class (as it was empty), and moved the changelog to it's own markdown file.
- Deleted RandomGenerator, moving it's components into LanguageGenerator.
- Moved LanguageGenerator a level above the Operators folder, and deleted that folder.
- Cleaned up the LanguageGenerator.Generate method even further. A few aspects of the method have been separated out into new methods for enhanced clarity.
### 2023-1-9:
- Added simple weight distribution to sigma selection.
- Outdated: Improved flagging. Now it actually works, and plays nicely with lexeme deconstruction.
- Radically improved how a sentence is deconstructed with the *Deconstructor* class. Custom filters (with name and char array).
- Added delegates/events to LanguageGenerator, for customizable behavior. I just need to hook them up properly.
- Removed flagging from LanguageGenerator. Punctuation is effectively handled through the deconstruction process; yet custom changes to punctuation will look very similar to how lexemes are processed.
### 2023-1-10:
- Hooked up a few events into the word generation method. You can now effectively decide what happens to which filter types, as defined by the user. Very powerful, flexible, and well-defined. Feels good.
- Generated words match the case of the original word as closely as possible: lowercase (default), uppercase, capitalize, or random case.
### 2023-1-11:
- Added default OnConstruct filter methods to Language.Construction: Generate processes the text, whereas KeepAsIs doesn't.
- Added two booleans to Language.Options: MatchCase, and AllowRandomCase.
- Merged Construction and Deconstruction classes back into Language, because it didn't feel as clean.
- CharacterBlocks can now be programatically merged with LanguageGenerator.EVENT_MergeBlocks. "Let's" and "10,000" are seen as one block by the generator instead of three.
- Fixed a minor logical issue that caused capitalized single-letter words (such as "I") that generated into a word longer than one letter to be entirely uppercase rather than capitalized.
### 2023-1-12:
- Cleaned up and split off the main LanguageGenerator.Generate method into clearer methods.
### 2023-1-13:
- Added the Examples folder, and the "Singsonglish" language.
- Tested and confirmed support for word escaping and simple flagging. Good foundation to improve upon.
### 2023-1-16:
- Renamed LanguageGenerator.EVENT_(...) methods to DECONSTRUCT_(...).
- Renamed LanguageOptions.MatchCase to AllowAutomaticCasing.
- Added DECONSTRUCT_ContainWithin. Merges all blocks within two blocks of the specified filter. It's Merge on steroids, and it's perfect for a "Flags" filter.
- Added CONSTRUCT_Within, for any single blocks that you need a substring of (such as an "Escape" filter).
### 2023-1-17:
- All code from Flagging has been deleted, and replaced with drastically better code (and shorter!). Simply hook it up to the ConstructFilter, and add your flag actions.
- Added the Punctuation class, which works very similarly to Flagging.
- Removed Delimiters property from Language.Options, as it was no longer needed.
- Renamed Language.ConstructFilter to Construct, brining its naming conventon inline with Deconstruct.
- Preliminary code added to the Numbers class.
### 2023-1-18
- Updated the Letter class, adding Name, Description, and Pronunciation as string properties, and Case as a Tuple(char, char) for enhanced control over capitalization.
- Improvements to the Alphabet and LanguageGenerator classes which reflect the Letter class changes.
- Added the LetterInfo class (with adjacent properties), and List<LetterInfo> GeneratedLetters to WordInfo. The generation process now adds it's letters to this list instead to WordGenerated.
- Added OnGenerate delegate and event. This brings the level of generation control in line with the deconstruction and construction events.
### 2023-1-21
- Finished the Numbers class for handling custom numbers (except different base support).
### 2023-1-23
- Moved Language management to it's own class.
- Improved how affixes are handled, and added the new AffixInfo class.
- Affixes can now have a custom order, and can change position (suffix to prefix or prefix to suffix).
- Added support for OnPrefix and OnSuffix events. While affixes are intiailly processed prior to word generation, these events are called afterward.
- Added two support methods for modifying suffixes through OnSuffix: SUFFIX_Insert and SUFFIX_Remove.
- OnSuffix support methods have slightly different behaviour compared to other events, with a boolean parameter called "condition". SUFFIX_ReturnMatch, _ReturnConsonant, and _ReturnVowel should be passed here.
### 2023-1-24
- The language generator now uses the cases specified by the Language, rather than just char.ToUpper. However, if the letter key isn't found in your alphabet, it defaults to ToUpper.
- Added three default casing methods: CASE_UpperDefault, CASE_CapitalizeDefault, and CASE_RandomDefault.
- Changed the action case methods Uppercase, Capitalize, and RandomCase to follow other naming conventions: CASE_Upper, CASE_Capitalize, and CASE_Random.
- Cleaned up folders and namespaces, and moved "subclasses" that were used by other classes into their own .cs file.
- Code clean-up in aisle LanguageGenerator.
### 2023-1-25
- Renamed SUFFIX_ methods to AFFIX_, as I realized these methods function perfectly for both prefixes and suffixes.
### 2023-1-27
- Added Syllable, SyllableInfo, LetterGroup classes.
- Replaced old syllable and letter generation code in LanguageGenerator with a stronger system.
### 2023-1-28
- Erased old syllable classes and selection code: LetterPath, Sigma, SigmaBlock, SigmaPath, SigmaInfo.
- Renamed OnConstruct to OnLetter, and added OnSyllable.
### 2023-1-29
- Renamed GENERATE_ methods to LETTER_, and added two new condition methods: LETTER_Contains and LETTER_Syllable.
- Added SyllableInfo reference to LetterInfo for use by language events. This is set when the letter is chosen.
### 2023-1-30
- Added Diagnostics class for logging word generation and supporting methods that diagnose your language.
- Added logging information to most LanguageGenerator methods. If IsLogging is true, the logging details are saved to the Diagnostics.LogName .txt file.
### 2023-2-1
- Renamed LETTER_Contains to LETTER_Any, and LETTER_Syllable to SYLLABLE_Any.
- Added SYLLABLE_Replace method, and SYLLABLE_Any, SYLLABLE_Starts, and SYLLABLE_Ends boolean methods.
- Removed unnecessary CharacterBlock parameters from DECONSTRUCT_ methods (left and right adjacent references). Just access them from the CharacterBlock reference.
- Added WORD_LastByFilter and WORD_NextByFilter methods. These return the previous or next occurrence relative to the current word. Perfect for custom markers and sentence-level context.
### 2023-2-4
- Flags are now case-insensitive.
- Added SkipLexemes boolean to WordInfo. This is set to true if an inflection or root is found in Lexicon, but can also be triggered manually by a flag.
### 2023-2-7
- Minor refactoring to LanguageGenerator.PopulateLetters method. Moved part of the method into it's own: SelectLetter.
- Slight improvement to english syllable estimation. It will now remove "es" and "ed" from the end of a word before estimation, as the "e" is typically silent.
- Added CONSTRUCT_Replace, which replaces input characters with their respective output characters. It takes a params of Tuple(char input, char output) array.
- Added OnSyllableSelect. This crucial change allows the language author to modify what syllables can be selected based on criteria. Perfect for matching ending consonants to starting vowels, and vice versa (supporting methods to follow).
### 2023-2-8
- Added weight multipliers to Syllable and Letter classes. These doubles are set by the OnSyllableSelect and OnLetterSelect events to influence weights, and are reset before the next syllable or letter is chosen.
- Implemented support for the OnLetterSelect event. This behaves similar to OnSyllableSelect.
- Added a LetterGroup reference to LetterInfo. Now it knows where it came from.
- Added a LetterInfo list to SyllableInfo. This is useful for the OnLetterSelect and OnSyllableSelect events.
### 2023-2-9
- Added support methods for OnLetterSelect and OnSyllableSelect, under SELECT_: Exclude, Keep, and SetWeight. A few of these are overloaded to support both syllables and letters.
- Added conditional methods: SELECT_GroupsContains, SELECT_GroupsAny, SELECT_IsSyllableFirst, SELECT_IsSyllableLast, SELECT_IsSyllableMiddle, SELECT_IsGroupFirst, SELECT_IsGroupLast, SELECT_IsGroupMiddle.
- Added returning methods for LetterGroup and LetterInfo: SELECT_Template and SELECT_Letter.
- Added Syllables to Lexicon. You can now strictly define the syllable structure of words, while letting the generator choose the letters. This is really nice for inflections like "sing", "sang", "sung".
### 2023-2-10
- Renamed Inflections to Vocabulary.
- Added support methods for Vocabulary and Roots.
- Added SELECT_GroupExcept to LanguageGenerator, which returns all of the LetterGroup keys (as a char array), minus the chars passed through the parameter.
- Fixed an issue which caused memorized words to have another set of affixes applied on the second attempt.
- Added a few methods to the diagnostic log.
### 2023-2-11
- Fixed an issue caused by the double affix fix from yesterday, which allowed the generator to overwrite custom vocabulary words.
- Renamed Syllable.Groups to Letters, and cleaned up the Syllable class.
- Added string Group to Syllable. This can be anything, and is useful for excluding specific syllables in the OnSyllableSelect event.
- Deleted SyllablePath, as it wasn't needed.
- Added conditional methods: SELECT_TagsAny and SELECT_TagsAll. As an example, these can be used to exclude syllables by tag in the OnSyllableSelect event.
### 2023-2-13
- Renamed old sigma methods in LanguageGenerator to syllable.
- Added TEST_UniqueGeneration method to Diagnostics. This generates the specified words, adds them to Diagnostics.Uniques, and returns an duplicate occurence percentage.
- Added TEST_UniquesByCommonality method, which returns all of the duplicates generated by TEST_UniqueGeneration.
### 2023-2-15
- Moved syllable function and helping methods from LanguageGenerator to Language.
- Changed LanguageOptions' syllable skew min and max from doubles to Func<int, double> (where the int is the estimated syllable count). Allows for greater flexibility in skewing the final syllable count.
- Deleted support for OnSyllable and OnLetter events (not to be confused with OnSyllableSelect and OnLetterSelect). These methods acted on the letters *after* they were generated, thus breaking syllabic structure. I felt like it was bad practice, so I removed it.
- Started work on a second example language. This one attempts to mimic the Japanese language.
### 2023-2-16
- Removed an old language.
- Official launch.
- Removed LanguageGenerator.LinkLetterLeft and LinkLetterRight, as it was no longer necessary.
- Removed IsAlive and IsProcessed from LetterInfo.
- Removed LETTER_Insert and LETTER_Replace. These were old support methods for the now-deleted OnLetter event.
- Renamed LETTER_Any to SELECT_LetterAny.