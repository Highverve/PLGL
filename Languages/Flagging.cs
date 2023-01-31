using PLGL.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Languages
{
    /// <summary>
    /// Markers allow the end-user to specify custom behaviour.
    /// Mark a word in your input text, and the matching marker will apply.
    /// 
    /// Example:
    ///     - There's a name in the sentence that you want the generator to skip.
    ///     - The SkipGenerate flag has been assigned to the 's' character.
    ///     - In the input text, you'd add "**s" to the end of the name: "Trevor**s"
    ///     
    ///     - To add multiple flags to a word, add more characters. If the character key
    ///       exists in the Markers dictionary, the function will apply to the word.
    ///     - Let's add an PrependMarker with the 'a' key. This will add a fictional title "syl-" to the start of a name.
    ///     - Now add the extra character to the name: "Trevor**sa", and will output "syl-Trevor"
    /// </summary>
    public class Flagging
    {
        public Dictionary<string, Action<LanguageGenerator, WordInfo>> Actions { get; set; }
            = new Dictionary<string, Action<LanguageGenerator, WordInfo>>();

        public void Add(string flagKey, Action<LanguageGenerator, WordInfo> action)
        {
            if (Actions.ContainsKey(flagKey) == false)
                Actions.Add(flagKey, action);
        }
        public void Process(LanguageGenerator lg, WordInfo word, string filterName, string delimiter = ",", int substringIndex = 1, int substringSubtract = 2)
        {
            if (word.Filter.Name.ToUpper() == filterName && word.IsProcessed == false)
            {
                string[] command = word.WordActual.Substring(substringIndex, word.WordActual.Length - substringSubtract).Split(delimiter);

                foreach (string s in command)
                {
                    if (Actions.ContainsKey(s))
                        Actions[s](lg, word);
                }

                lg.Diagnostics.LogBuilder.AppendLine();
            }
        }

        /// <summary>
        /// Hides the left adjacent word.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        public void ACTION_HideLeft(LanguageGenerator lg, WordInfo word)
        {
            if (word.AdjacentLeft != null)
            {
                word.AdjacentLeft.WordFinal = string.Empty;
                word.AdjacentLeft.IsProcessed = true;
                word.IsProcessed = true;

                if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    FlagLog(lg, $"{word.AdjacentLeft.WordActual} is hidden.");
            }
        }
        /// <summary>
        /// Hides the right adjacent word.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        public void ACTION_HideRight(LanguageGenerator lg, WordInfo word)
        {
            if (word.AdjacentRight != null)
            {
                word.AdjacentRight.WordFinal = string.Empty;
                word.AdjacentRight.IsProcessed = true;
                word.IsProcessed = true;

                if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    FlagLog(lg, $"{word.AdjacentRight.WordActual} is hidden.");
            }
        }
        /// <summary>
        /// Hides the left and right adjacent words.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        public void ACTION_HideAdjacents(LanguageGenerator lg, WordInfo word) { ACTION_HideLeft(lg, word); ACTION_HideRight(lg, word); }

        /// <summary>
        /// Sets the left adjacent word's final string to the actual (initial) string, effectively escaping the word out of generation.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        public void ACTION_NoGenerate(LanguageGenerator lg, WordInfo word)
        {
            if (word.AdjacentLeft != null)
            {
                word.AdjacentLeft.WordFinal = word.AdjacentLeft.WordActual;
                word.IsProcessed = true;

                if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    FlagLog(lg, $"{word.AdjacentLeft.WordActual} generated word is set to actual.");
            }
        }
        public void ACTION_NoAffixes(LanguageGenerator lg, WordInfo word)
        {
            if (word.AdjacentLeft != null)
            {
                word.AdjacentLeft.WordFinal = word.AdjacentLeft.WordGenerated;
                word.IsProcessed = true;

                if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    FlagLog(lg, $"{word.AdjacentLeft.WordActual} affixes removed.");
            }
        }

        /// <summary>
        /// Replaces the left adjacent word with a dynamically compiled string. This may be helpful for player/character names.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        /// <param name="result"></param>
        public void ACTION_ReplaceLeft(LanguageGenerator lg, WordInfo word, Func<string> result)
        {
            if (word.AdjacentLeft != null)
            {
                string r = result();
                word.AdjacentLeft.WordFinal = r;
                word.IsProcessed = true;

                if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    FlagLog(lg, $"{word.AdjacentLeft.WordActual} replaced with {r}.");
            }
        }
        /// <summary>
        /// Replaces the right adjacent word with a dynamically compiled string. This may be helpful for player/character names.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        /// <param name="result"></param>
        public void ACTION_ReplaceRight(LanguageGenerator lg, WordInfo word, Func<string> result)
        {
            if (word.AdjacentRight != null)
            {
                string r = result();
                word.AdjacentRight.WordFinal = r;
                word.IsProcessed = true;

                if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                    FlagLog(lg, $"{word.AdjacentRight.WordActual} replaced with {r}.");
            }
        }
        /// <summary>
        /// Replaces the current word with a dynamically compiled string. This may be helpful for player/character names.
        /// </summary>
        /// <param name="lg"></param>
        /// <param name="word"></param>
        /// <param name="result"></param>
        public void ACTION_ReplaceCurrent(LanguageGenerator lg, WordInfo word, Func<string> result)
        {
            string r = result();
            word.WordFinal = r;
            word.IsProcessed = true;

            if (lg.Diagnostics.IsConstructEventLog && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                FlagLog(lg, $"{word.WordActual} replaced with {r}.");
        }

        public void FlagLog(LanguageGenerator lg, string info) { lg.Diagnostics.LOG_Subheader($"FLAG: {info}"); }
    }
}

