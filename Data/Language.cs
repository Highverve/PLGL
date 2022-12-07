using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    public class Language
    {
        /// <summary>
        /// The name of the language.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The description of the language.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// The author of the language.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// A global seed used to offset seed generated for each word. Default is 0.
        /// Increasing or decreasing will change every word of the generated language.
        /// </summary>
        public int SeedOffset { get; set; } = 0;

        //Classes
        public Alphabet Alphabet { get; private set; }
        public Structure Structure { get; private set; }
        public Pathways Pathways { get; private set; }
        //Needs work:
        public Grammar Grammar { get; private set; }
        public Syntax Syntax { get; private set; }

        public void Initialize()
        {

        }

        public string Generate(string phrase)
        {
            return phrase;
        }

        public void SaveJSON(string path)
        {
            //JSON to save goes here.
        }
        public void LoadJSON(string path)
        {

        }
    }
}
