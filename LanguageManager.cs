using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL
{
    public class LanguageManager
    {
        public Dictionary<string, Language> Languages { get; private set; } = new Dictionary<string, Language>();

        public void AddLanguage(string key, Language language)
        {
            if (Languages.ContainsKey(key) == false)
                Languages.Add(key, language);
        }
        /// <summary>
        /// Adds the language, with the nickname as it's key.
        /// </summary>
        /// <param name="language"></param>
        public void AddLanguage(Language language) { AddLanguage(language.META_Nickname, language); }

        public Language Select(string nickname)
        {
            if (Languages.ContainsKey(nickname))
                return Languages[nickname];
            return null;
        }
    }
}
