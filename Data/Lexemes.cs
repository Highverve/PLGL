using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    /// <summary>
    /// A constraining class, that aims to answer a tricky question: "How do we handle lexemes?"
    /// 
    /// Scenario:
    ///     1. The generator is processing a sentence, when a word ("running") matches a Dictionary key.
    ///     2. The usual generation process is circumvented, and the word undergoes a strictly defined process.
    ///     3. The root word is split from it's modifying suffix ("runn" and "ing").
    ///     4. The word ("runn", or "run" if double consonant is removed) is processed through the generator.
    ///     5. An affix and/or punctuation mark is added, according to the style preferences of the language author.
    /// 
    /// If the root word has multiple affixes (on either end), the process loops through each one and
    /// applies the style preference accordingly.
    /// </summary>
    public class Lexemes
    {
        /// <summary>
        /// Marks at the end of a word that the generator uses to distinguish a Dictionary key. Examples:
        ///     Plural: -s, -es
        ///     Past participles: -ed, -d, -t
        ///     Into adjectives: -ly, -ness, -
        ///
        /// </summary>
        public string[] Suffixes { get; set; }
        /// <summary>
        /// Marks at the start of a word that the generator uses to distinguish a Dictionary key. Examples:
        ///     a-, an-, co-, ex-, in-, dis-, de-, un-
        ///
        /// </summary>
        public string[] Prefixes { get; set; }

        public Dictionary<string, Func<string>> Functions { get; private set; } = new Dictionary<string, Func<string>>();
    }
}
