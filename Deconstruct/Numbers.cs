using PLGL.Construct.Elements;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Set to the input number's base. The number is processed if the output base is different from input base.
        /// </summary>
        public int InputBase { get; set; } = 10;
        /// <summary>
        /// The radix of the language's counting system.
        /// </summary>
        /// <returns>Numerals.Count</returns>
        public int OutputBase() { return Numerals.Count; }

        public char Decimal { get; set; } = '.';
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Default is 3 (thousandth place).
        /// </summary>
        public int SeparatorLength { get; set; } = 3;

        public List<Number> Numerals { get; set; } = new List<Number>();

        public void Add(params (char digit, char symbol, string name)[] number)
        {
            for (int i = 0; i < number.Length; i++)
                Numerals.Add(new Number()
                {
                    Digit = number[i].digit,
                    Symbol = number[i].symbol,
                    Name = number[i].name
                });
        }
        /// <summary>
        /// The standard base-10, in English.
        /// </summary>
        public (char digit, char symbol, string name)[] Default { get; private set; } = new (char digit, char symbol, string name)[]
        { ('0', '0', "zero"), ('1','1', "one"), ('2','2', "two"), ('3', '3', "three"), ('4', '4', "four"),
          ('5','5', "five"), ('6','6', "six"), ('7','7', "seven"), ('8', '8', "eight"), ('9', '9', "nine")};

        private string NumberFormat(string number)
        {
            string result = string.Empty;

            if (number.Contains(',')) result += "#" + Separator;
            result += "###";
            if (number.Contains(Decimal)) result += Decimal +"##";

            return result;
        }

        public void Process(LanguageGenerator lg, WordInfo word, string filterName, char decimalSymbol, char separatorSymbol)
        {
            if (word.Filter.Name.ToUpper() == filterName.ToUpper() && word.IsProcessed == false)
            {
                string format = NumberFormat(word.WordActual);
                double number = double.Parse(word.WordActual);

                //Add base processing. Have to get it to accept double.

                word.WordFinal = Format(word.WordActual, decimalSymbol, separatorSymbol);
                word.IsProcessed = true;
            }
        }
        private string Format(string number, char decimalSymbol, char separatorSymbol)
        {
            int decimalPos = number.IndexOf(Decimal);
            string whole = string.Empty, fraction = string.Empty;

            if (number.Contains(Decimal))
            {
                whole = number.Substring(0, decimalPos).Replace(Separator.ToString(), string.Empty);
                fraction = number.Substring(decimalPos + 1, number.Length - (decimalPos + 1));
            }
            else
                whole = number.Replace(Separator.ToString(), string.Empty);

            whole = Reverse(whole);
            for (int i = 1; i < whole.Length; i++)
            {
                if ((i + 1) % (SeparatorLength + 1) == 0)
                    whole = whole.Insert(i, separatorSymbol.ToString());
            }
            whole = Reverse(whole);

            string result = whole + ((string.IsNullOrEmpty(fraction) == false) ? decimalSymbol + fraction : string.Empty);

            foreach (Number n in Numerals)
                result = result.Replace(n.Digit, n.Symbol);

            return result;
        }
        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

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

            if (OutputBase() < 2 || OutputBase() > Digits.Length)
                throw new ArgumentException("The Base must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % OutputBase());
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / OutputBase();
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
        public char Digit { get; set; }
        public char Symbol { get; set; }

        public string Name { get; set; }
    }
}
