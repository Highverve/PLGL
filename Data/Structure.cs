using LanguageReimaginer.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageReimaginer.Data
{
    public enum Clustering { None, Vowels, Consonants, Both }

    /// <summary>
    /// Tells the generator how to structure a word.
    /// </summary>
    public class Structure
    {
        #region Clustering

        /// <summary>
        /// Determines if double consonants or vowels are generated.
        /// </summary>
        public Clustering Clustering { get; set; } = Clustering.Both;
        /// <summary>
        /// Tells the generator how often a cluster structure is picked. A percentage value between 0.0 and 1.0.
        /// </summary>
        public double ClusterChance { get; set; } = 0.1;

        /// <summary>
        /// A value weighed against the other two weights. Set to 0 for no clustering as the first syllable.
        /// </summary>
        public double ClusterStartWeight { get; set; } = 0;
        /// <summary>
        /// A value weighed against the other two weights. Set to 0 for no clustering in any middle syllable.
        /// </summary>
        public double ClusterMiddleWeight { get; set; } = 0.15;
        /// <summary>
        /// A value weighed against the other two weights. Set to 0 for no clustering as the last syllable.
        /// </summary>
        public double ClusterEndWeight { get; set; } = 0.1;

        /// <summary>
        /// If the syllable generated is a cluster, this determines if it's a consonant or a vowel. Weighted against ClusterVowelWeight.
        /// </summary>
        public double ClusterConsonantWeight { get; set; } = 5.0;
        /// <summary>
        /// If the syllable generated is a cluster, this determines if it's a consonant or a vowel. Weighted against ClusterConsonantWeight.
        /// </summary>
        public double ClusterVowelWeight { get; set; } = 5.0;

        public void SetClustering(Clustering type, double clusterChance, double startWeight, double midWeight, double endWeight,
                                  double consonantWeight, double vowelWeight)
        {
            this.Clustering = type;
            this.ClusterChance = clusterChance;
            this.ClusterStartWeight = startWeight;
            this.ClusterMiddleWeight = midWeight;
            this.ClusterEndWeight = endWeight;
            this.ClusterConsonantWeight = consonantWeight;
            this.ClusterVowelWeight = vowelWeight;
        }

        #endregion

        public Dictionary<string, Form> LetterForms { get; private set; } = new Dictionary<string, Form>();
        public void AddForm(params Form[] forms)
        {
            for (int i = 0; i < forms.Length; i++)
            {
                if (LetterForms.ContainsKey(forms[i].Display) == false)
                    LetterForms.Add(forms[i].Display, forms[i]);
            }

            AddForm(new Form(10.0, 'C', 'V'), new Form(10.0, 'V', 'C'), new Form(5.0, 'C', 'C'),
                    new Form(2.0, 'V', 'V'), new Form(1.0, 'C', 'V', 'C'), new Form(1.0, 'V', 'C', 'V'));
        }

        public int SyllableCountMax { get; set; }
        public int SyllableCountMin { get; set; }
        public List<string> Delimiters { get; private set; } = new List<string>();
    }

    public class Form
    {
        public char[] Order { get; private set; }
        public double Weight { get; set; }

        public string Display { get; private set; } = string.Empty;
        public int Length { get { return Order.Length; } }
        public Clustering Cluster { get; private set; }

        public Type Select(int index) { return (Char.ToUpper(Order[index]) == 'C') ? typeof(Consonant) : typeof(Vowel); }
        public Type First() { return Select(0); }
        public Type Last() { return Select(Length); } //-1? Double check

        public bool IsConsonant(int index) { return Select(index) == typeof(Consonant); }
        public bool IsVowel(int index) { return Select(index) == typeof(Vowel); }

        private Clustering SetCluster()
        {
            if (DoubleForm('C')) return Clustering.Consonants;
            if (DoubleForm('V')) return Clustering.Vowels;

            return Clustering.None;
        }
        private bool DoubleForm(char c) { return Display.Contains((c + "" + c), StringComparison.OrdinalIgnoreCase); }

        /// <summary>
        /// The structure a syllable will take.
        /// </summary>
        /// <param name="Weight">The chance for </param>
        /// <param name="Order">Must be either C (for 'consonant') or V (for 'vowel'). Limited to three characters.</param>
        public Form(double Weight, params char[] Order)
        {
            this.Weight = Weight;
            this.Order = Order;

            for (int i = 0; i < Order.Length; i++)
                Display += Order[i];
            Cluster = SetCluster();
        }
    }
}
