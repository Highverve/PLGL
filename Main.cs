/// <summary>
/// Language Reimaginer recreates an existing language through word-seeded randomness and customizable, for a powerful procedurally, hand-crafted lexicon.
/// 
/// Library Goals:
///     - To recreate languages with word-seeded randomness, constrained by the new language's requirements.
///     - For quickly generating new words for a new language as needed (e.g, game development),
///       or as a tool which aids artists and novelists to build one.
///     
/// Library Structure:
///     OPERATORS.
///         - SyllableGenerator.
///             Defined structure. "CV", "VC", "CVC", "VCV"
///             Syllabificate(). Returns the syllable structure of a word.
///         - WordGenerator.
///             Random. Seeded with the current word when Next() is called.
///             private Next(string).
///             public Reimagine(string). The final method utilized. Splits string, applies grammatical rules (e.g, replacing punctuation),
///                 iterates string into Next(), StringBuilds, and returns result.
///     DATA.
///         - Symbol, which derives into Consonant and Vowel.
///         - Syllable.
///         
/// 
///     
///
/// EXTERNAL SETUP EXAMPLE:
///     SyllableStructure("CV", "VC", "CVC", "VCV");
/// 
///     AddVowels('a', 'e', 'i', 'o', 'u');
///     AddConsonants('c', 'b', 'd', 'f', 'g', etc.);
/// 
///     AddCommonality('a', 100);
///     AddCommonality('e', 80);
///     AddCommonality('i', 70);
///     AddCommonality('o', 120);
///     AddCommonality('u', 90);
/// 
///     Blacklist('t', 'o');
/// 
/// SYLLABLES:
///     IsVowel('c'); false
///     IsConsonant('x'); true
///         
/// </summary>
namespace LanguageReimaginer
{
    public class Main
    {

    }
}