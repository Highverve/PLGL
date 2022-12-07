/// <summary>
/// Language Reimaginer recreates an existing language through word-seeded randomness and customizable, for a powerful procedurally, hand-crafted lexicon.
/// 
/// Library Goals:
///     - To recreate languages with word-seeded randomness, constrained by the new language's requirements.
///     - For quickly generating new words for a new language as needed (e.g, game development),
///       or as a tool which aids artists and novelists to build one.
///     
/// By Density (Scope):
///     - Alphabet. The smallest component of a word.
///     - Pathways. Constrains the generator to produce favorable words through weighted flow ('p' -> 'a' -> 't' -> 'h').
///     - Structure. Constrains the generator to produce favorable words through form ('C', 'V').
///     - 
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
    /// </summary>
    public class Main
    {

    }
}