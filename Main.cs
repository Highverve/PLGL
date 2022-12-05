/// <summary>
/// Library Goals:
///     - To recreate languages with word-seeded randomness, constrained by the new language's requirements.
///     - For quickly generating new words for a new language as needed (e.g, game development),
///       or as a tool which aids artists and novelists to build one.
///     
/// Library Structure:
///     ELEMENTS.
///         - Symbol, which derives into Consonant and Vowel.
///         - Syllable.
///         
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
///         
/// </summary>
namespace LanguageReimaginer
{
    public class Main
    {

    }
}