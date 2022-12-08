using LanguageReimaginer.Data;
using LanguageReimaginer.Data.Elements;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageReimaginer.Operators
{
    public class WordGenerator
    {
        internal RandomGenerator RanGen { get; set; }
        internal SyllableGenerator SyllableGen { get; set; }

        List<WordInfo> wordInfo = new List<WordInfo>();
        StringBuilder sentenceBuilder = new StringBuilder();

        public Language Language { get; set; }

        public WordGenerator(Language Language) { this.Language = Language; }

        public string Generate(string sentence, out List<WordInfo> info)
        {
            sentenceBuilder.Clear();
            wordInfo.Clear();

            string pattern = "@\"(?<=[" + string.Join("", Language.Delimiters) + "])\"";
            string[] words = Regex.Split(sentence, pattern);//sentence.Split(' '); //Temporary ' ' char. Replace with customizable option(s).

            //Loop through split words and add to wordInfo list.
            foreach (string s in words)
            {
                WordInfo word = new WordInfo();
                word.WordActual = s;

                wordInfo.Add(word);
            }

            //Link adjacent words.
            for (int i = 0; i < wordInfo.Count; i++)
            {
                if (i != 0)
                    wordInfo[i].AdjacentLeft = wordInfo[i - 1];
                if (i != wordInfo.Count)
                    wordInfo[i].AdjacentRight = wordInfo[i - 1];
            }

            foreach (WordInfo part in wordInfo)
            {
                //Check for flags
                //Check for punctuation marks. If the sentence contains any, then: isolate (Add before or after
                //Check for and process lexemes.

                //sentenceBuilder.Append(NextWord(s) + " ");
            }

            info = wordInfo;
            return sentenceBuilder.ToString();
        }
    }
}
