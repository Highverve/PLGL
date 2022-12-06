using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageReimaginer.Data.Elements;

namespace LanguageReimaginer.Data
{
    public enum Clustering { None, Vowels, Consonants, Both }
    public record class Language
    {
        #region Syllable scope
        public List<Consonant> Consonants { get; private set; } = new List<Consonant>();
        public List<Vowel> Vowels{ get; private set; } = new List<Vowel>();

        #endregion

        #region Letter clustering
        /// <summary>
        /// Determines if double consonants or vowels are generated.
        /// </summary>
        public Clustering Clustering { get; set; } = Clustering.Both;
        /// <summary>
        /// A percentage value between 0f and 1f. Set to 0 for no clustering as the first syllable.
        /// </summary>
        public float ClusterStartChance { get; set; } = 0f;
        /// <summary>
        /// A percentage value between 0f and 1f. Set to 0 for no clustering in any middle syllable.
        /// </summary>
        public float ClusterMiddleChance { get; set; } = .15f;
        /// <summary>
        /// A percentage value between 0f and 1f. Set to 0 for no clustering as the last syllable.
        /// </summary>
        public float ClusterEndChance { get; set; } = .1f;

        /// <summary>
        /// If the syllable generated is a cluster, this determines if it's a consonant or a vowel. Weighted against ClusterVowelWeight.
        /// </summary>
        public double ClusterConsonantWeight { get; set; } = 5.0;
        /// <summary>
        /// If the syllable generated is a cluster, this determines if it's a consonant or a vowel. Weighted against ClusterConsonantWeight.
        /// </summary>
        public double ClusterVowelWeight { get; set; } = 5.0;

        #endregion

        #region Word structuring
        
        public int SyllableCountMax { get; set; }
        public int SyllableCountMin { get; set; }
        public List<string> Delimiters { get; private set; } = new List<string>();

        #endregion
    }
}
