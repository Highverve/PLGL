using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PLGL.Deconstruct
{
    public class Numbers
    {
        //Support for any base conversion (3, 5, 15, etc.)
        //Support for individual number names, and multiplier names (hundred, thousand, million, etc.).
        //String format extraction. Include , and . parsing options (for Europeans).
        //Options for rounding, decimal removal, etc.

        /// <summary>
        /// The radix of the language's counting system.
        /// </summary>
        /// <returns>Numerals.Count</returns>
        public int Base() { return Numerals.Count; }
        public char Decimal { get; set; } = '.';
        public char Separator { get; set; } = ',';
        /// <summary>
        /// Default is 3 (thousandth).
        /// </summary>
        public int SeparatorLength { get; set; } = 3;

        public List<Number> Numerals { get; set; } = new List<Number>();

        public void Add(params (char symbol, string name)[] number)
        {
            for (int i = 0; i < number.Length; i++)
                Numerals.Add(new Number() { Symbol = number[i].symbol, Name = number[i].name });
        }
        /// <summary>
        /// The standard base-10, in English.
        /// </summary>
        public (char symbol, string name)[] Default { get; private set; } = new (char symbol, string name)[]
        { ('0', "zero"), ('1', "one"), ('2', "two"), ('3', "three"), ('4', "four"),
          ('5', "five"), ('6', "six"), ('7', "seven"), ('8', "eight"), ('9', "nine")};

        /*private string NumberFormat(string number)
        {
            string result = string.Empty;

            if (number.Contains(',')) result += "#" + Separator;
            result += "###";
            if (number.Contains(Decimal)) result += Decimal +"##";

            return result;
        }*/

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// 
        /// Code borrowed from: https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net
        /// Code author: Pavel Vladov
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (between 2 and 36).</param>
        /// <returns></returns>
        public string DecimalToArbitrarySystem(long decimalNumber)
        {
            int BitsInLong = 64;
            string Digits = NumeralOrder();

            if (Base() < 2 || Base() > Digits.Length)
                throw new ArgumentException("The Base must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % Base());
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / Base();
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
                result = "-" + result;

            return result;
        }
        private string NumeralOrder()
        {
            string result = string.Empty;
            for (int i = 0; i < Numerals.Count; i++)
                result += Numerals[i].Symbol;
            return result;
        }
    }

    public class Number
    {
        public char Symbol { get; set; }
        public string Name { get; set; }
    }
}
