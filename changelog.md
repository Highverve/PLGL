# PLGL Changelog

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
### 2022-1-9:
    - Added simple weight distribution to sigma selection.
    - Outdated: Improved flagging. Now it actually works, and plays nicely with lexeme deconstruction.
    - Radically improved how a sentence is deconstructed with the *Deconstructor* class. Custom filters (with name and char array).
    - Added delegates/events to LanguageGenerator, for customizable behavior. I just need to hook them up properly.
    - Removed flagging from LanguageGenerator. Punctuation is effectively handled through the deconstruction process; yet custom changes to punctuation will look very similar to how lexemes are processed.
### 2022-1-10:
    - Hooked up a few events into the word generation method. You can now effectively decide what happens to which filter types, as defined by the user. Very powerful, flexible, and well-defined. Feels good.
    - Generated words match the case of the original word as closely as possible: lowercase (default), uppercase, capitalize, or random case.
### 2022-1-11:
    - Added default OnConstruct filter methods to Language.Construction: Generate processes the text, whereas KeepAsIs doesn't.
    - Added two booleans to Language.Options: MatchCase, and AllowRandomCase.
    - Merged Construction and Deconstruction classes back into Language, because it didn't feel as clean.
    - CharacterBlocks can now be programatically merged with LanguageGenerator.EVENT_MergeBlocks. "Let's" and "10,000" are seen as one block by the generator instead of three.
    - Fixed a minor logical issue that caused capitalized single-letter words (such as "I") that generated into a word longer than one letter to be entirely uppercase rather than capitalized.
### 2022-1-12:
    - Cleaned up and split off the main LanguageGenerator.Generate method into clearer methods.
### 2022-1-13:
    - Added the Examples folder, and the "Singsonglish" language.
    - Tested and confirmed support for word escaping and simple flagging. Good foundation to improve upon.
### 2022-1-16:
    - Renamed LanguageGenerator.EVENT_(...) methods to DECONSTRUCT_(...).
    - Renamed LanguageOptions.MatchCase to AllowAutomaticCasing.
    - Added DECONSTRUCT_ContainWithin. Merges all blocks within two blocks of the specified filter. It's Merge on steroids, and it's perfect for a "Flags" filter.
    - Added CONSTRUCT_Within, for any single blocks that you need a substring of (such as an "Escape" filter).
### 2022-1-17:
    - All code from Flagging has been deleted, and replaced with drastically better code (and shorter!). Simply hook it up to the ConstructFilter, and add your flag actions.
    - Added the Punctuation class, which works very similarly to Flagging.
    - Removed Delimiters property from Language.Options, as it was no longer needed.
    - Renamed Language.ConstructFilter to Construct, brining its naming conventon inline with Deconstruct.
    - Preliminary code added to the Numbers class.
### 2022-1-18
    - Updated the Letter class, adding Name, Description, and Pronunciation as string properties, and Case as a Tuple(char, char) for enhanced control over capitalization.
    - Improvements to the Alphabet and LanguageGenerator classes which reflect the Letter class changes.
    - Added the LetterInfo class (with adjacent properties), and List<LetterInfo> GeneratedLetters to WordInfo. The generation process now adds it's letters to this list instead to WordGenerated.
    - Added OnGenerate delegate and event. This brings the level of generation control in line with the deconstruction and construction events.
### 2022-1-21
    - Finished the Numbers class for handling custom numbers (except different base support).
### 2022-1-23
    - Moved Language management to it's own class.
    - Improved how affixes are handled, and added the new AffixInfo class.
    - Affixes can now have a custom order, and can change position (suffix to prefix or prefix to suffix).
    - Added support for OnPrefix and OnSuffix events. While affixes are intiailly processed prior to word generation, these events are called afterward.
    - Added two support methods for modifying suffixes through OnSuffix: SUFFIX_Insert and SUFFIX_Remove.
    - OnSuffix support methods have slightly different behaviour compared to other events, with a boolean parameter called "condition". SUFFIX_ReturnMatch, _ReturnConsonant, and _ReturnVowel should be passed here.