/// <summary>
/// Language Reimaginer recreates an existing language through word-seeded randomness and customizable, for a powerful procedurally, hand-crafted lexicon.
/// 
/// Library Goals:
///     - To recreate languages with word-seeded randomness, constrained by the language author's requirements.
///     - For quickly generating new words for a new language as needed (e.g, game development),
///       or as a tool which aids artists and novelists to build one.
///     
/// By Density (Scope):
///     - Alphabet. The smallest component of a word.
///     - Pathways. Constrains the generator to produce favorable words through weighted flow ('p' -> 'a' -> 't' -> 'h').
///     - Structure. Constrains the generator to produce favorable words through form ('C', 'V').
///     - 
///     
/// An input sentence goes through two processes.
///     
///     1. Deconstruction. The sentence is analyzed and parsed. Custom markers are read, punctuation
///         marks manipulated, and lexemes processed.
///     2. Construction. The sentence's words are generated, according to the alphabetical characters,
///        alphabetical pathways, and structure constraints set by the language author.
///     
/// Although, there is a bit of overlap between these two processes.
///         
/// </summary>
namespace LanguageReimaginer
{
    /// <summary>
    /// Changelog:
    ///     2022-12-6:
    ///         - 1.1 Added weighted suffixes to each letter for diverse branching.
    ///         - 1.2 Moved 1.1 changes into a separate class: Pathways. May be subject to a name change later.
    ///         - 1.3 Created the Structure class for word formation (as "CV", "VC", "CVC", "CC", etc) by a weighted distribution. Good start, but more work to do.
    ///         - 1.4 Added the foundation of the Lexemes class, that answers the question: "How do we handle lexemes?"
    ///     2022-12-7:
    ///         - 1.5 Created the Punctuation class, for processing and manipulating punctuation marks.
    ///         - 1.6 Added SyllableInfo and WordInfo. These classes are used by the generator for additional context.
    ///         - 1.7 Added a few WordGenerator methods.
    ///         - 1.8 Added the Markings class.
    ///     2022-12-8:
    ///         - 1.9 Renamed WordGenerator to LanguageGenerator.
    ///         - 1.10 Changed the Syllable class to Sigma, and improved how the data is structured. It now uses an onset-nucleus-coda approach, adding another layer above the "CVC" structure.
    ///         - 1.11 Changed SyllableInfo to SigmaInfo, and it's structure to reflesh 1.10's changes.
    ///         - 1.12 Temporarily gutted Structure.cs while I plan it's code upgrade.
    ///     2022-12-9:
    ///         - 1.13 Designed an ideal scaffolding for letter pathing. It utilizes filters for word position, syllable position, and weighted distribution to pick the next letter.
    ///     2022-12-10:
    ///         - 1.14 Moved the more broad options in Language to an Options class for better clarity.
    ///         - 1.15 Renamed Markers to Flagging to clarify it's different from punctuation marks.
    ///         - 1.16 Selected affixes (in Lexemes) are now ordered from longest to shortest. This prevent in- stopping inter- from parsing (in·ter·cept).
    ///         - 1.17 Added "Skip" and "Possessive" booleans to WordInfo (though I should consider making Possessive flagging more modular).
    ///         - 1.18 Added text position targeting to Flags: BeforePrefix, AfterPrefix, BeforeSuffix, AfterSuffix. Useful for flags that add text.
    ///     2022-12-12:
    ///         - 1.19 Removed Skip and Possessive booleans from WordInfo, as I realized it was redundant.
    ///         - 1.20 Progress in the generating method(s) for language generation.
    ///         - 1.21 Defined SigmaWeight; a author-specified class that helps the generator decide which sigma to pick.
    ///         - 1.22 Added InputConsonants/Vowels to Language.Options. This is for estimating syllable count.
    ///         - 1.23 Added and tested a method to count syllables for any input word. According to my testing, it only has trouble with words that use y as a vowel.
    ///         - 1.24 Small changes and additional variables to WordInfo.
    ///         - 1.25 Added supporting methods to Structure.
    ///         - 1.26 Added the Affix class, and supporting methods to Lexemes.
    ///         - 1.27 Added a few classes inherited from Flagging: SkipGenerate, SkipLexemes, Add, and Remove.
    /// </summary>
    public class Main
    {

    }
}