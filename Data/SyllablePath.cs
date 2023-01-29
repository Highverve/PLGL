using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Data
{
    public class SyllablePath
    {
        /// <summary>
        /// How likely the generator will choose this sigma. Default is 1.0.
        /// </summary>
        public double SelectionWeight { get; set; } = 1.0;

        /// <summary>
        /// How likely this sigma will start a word. Default is 1.0.
        /// </summary>
        public double StartingWeight { get; set; } = 1.0;
        /// <summary>
        /// How likely this sigma will end a word. Default is 1.0.
        /// </summary>
        public double EndingWeight { get; set; } = 1.0;

        /// <summary>
        /// How likely this sigma will follow a sigma that ends with a vowel. Default is 1.0.
        /// </summary>
        public double LastVowelWeight { get; set; } = 1.0;
        /// <summary>
        /// How likely this sigma will follow a sigma that ends with a consonant. Default is 1.0.
        /// </summary>
        public double LastConsonantWeight { get; set; } = 1.0;
    }
}
