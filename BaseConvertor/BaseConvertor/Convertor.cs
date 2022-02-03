using System;
using System.Collections.Generic;
using System.Text;
using BaseConvertor.Exceptions;

namespace BaseConvertor {
    public static class Convertor {

        private class RawValue {

            private List<int> beforeDecimal;

            private List<int> afterDecimal;

            public RawValue() {
                beforeDecimal = new List<int>();
                afterDecimal = new List<int>();
            }

            public RawValue(List<int> beforeDecimal, List<int> afterDecimal) {
                this.beforeDecimal = beforeDecimal;
                this.afterDecimal = afterDecimal;
            }

            public int[] GetAfterDecimal() {
                return afterDecimal.ToArray();
            }

            public int[] GetBeforeDecimal() {
                return beforeDecimal.ToArray();
            }
        }

        private static readonly char[] Sexagesimal_Table = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x'};

        /// <summary>
        /// Convert value from Base N to base M
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="baseValue">Base of the Value</param>
        /// <param name="toBase">Base to convert to.</param>
        /// <returns></returns>
        public static BaseNum Convert(string value, int baseValue, int toBase) {

            if (toBase == baseValue) {
                return new BaseNum(value, toBase);
            }

            if (baseValue == 10) {
                if (!double.TryParse(value, out double b10)) {
                    throw new InvalidBaseValueException("Value given does not match with the given base.");
                } else {
                    return FromBase10ToX(b10, toBase);
                }
            } else if (toBase == 10) {
                return ToBase10(value, baseValue);
            } else {
                BaseNum base10 = ToBase10(value, baseValue);
                return FromBase10ToX(double.Parse(base10.Value), toBase);
            }
        }

        /// <summary>
        /// Convert value from Base N to base M
        /// </summary>
        /// <param name="baseNum">Value and it's Base</param>
        /// <param name="toBase">Base to convert to.</param>
        /// <returns></returns>
        public static BaseNum Convert(BaseNum baseNum, int toBase) {
            return Convert(baseNum.Value, baseNum.Base, toBase);
        }

        public static BaseNum FromBase10ToX(double base10Value, int targetBase) {

            var base10String = base10Value.ToString();
            if (base10String.Contains('.')) {

                // Split the decimals.
                var decimalSplit = base10String.Split('.');
                var beforeDecimal = int.Parse(decimalSplit[0]);
                var afterDecimal = double.Parse("0." + decimalSplit[1]);

                string result = Reverse(OperateValueBeforeDecimal(beforeDecimal)) + "." + OperateValueAfterDecimal(afterDecimal);
                return new BaseNum(result, targetBase);
            } else {
                return new BaseNum(OperateValueBeforeDecimal((int)base10Value), targetBase);
            }

            #region Local_Function

            string OperateValueAfterDecimal(double afterDecimalValue) {
                StringBuilder afterDecimalResult = new StringBuilder();
                int maxLoopCycle = 25;
                int i = 0;
                do {
                    afterDecimalValue *= targetBase;
                    int overflow = (int)Math.Truncate(afterDecimalValue);
                    afterDecimalResult.Append(Sexagesimal_Table[overflow % targetBase]);
                    afterDecimalValue -= Math.Truncate(afterDecimalValue);

                    ++i;
                } while (afterDecimalValue > 0 && i <= maxLoopCycle);

                return afterDecimalResult.ToString();
            }

            string OperateValueBeforeDecimal(int beforeDecimalValue) {
                StringBuilder result = new StringBuilder();
                do {
                    result.Append(Sexagesimal_Table[(int)beforeDecimalValue % targetBase]);
                    beforeDecimalValue /= targetBase;
                } while (beforeDecimalValue > 0);

                return Reverse(result.ToString());
            }
            #endregion
        }

        public static BaseNum ToBase10(string value, int baseValue) {
            TryMessage tryMessage = TryGetRawValue(value, baseValue, out RawValue rawValue);

            if (!tryMessage.Success) {
                throw tryMessage.Exception;
            }

            double result = 0;

            var beforeDecimal = rawValue.GetBeforeDecimal();
            for (int i = 0; i < beforeDecimal.Length; ++i) {
                result += beforeDecimal[i] * Math.Pow(baseValue, i);
            }

            var afterDecimal = rawValue.GetAfterDecimal();
            for (int i = 0; i < afterDecimal.Length; ++i) {
                result += afterDecimal[i] / Math.Pow(baseValue, i);
            }

            return new BaseNum(result.ToString(), 10);
        }

        public static BaseNum ToBase10(BaseNum baseNum) {
            return ToBase10(baseNum.Value, baseNum.Base);
        }

        /// <summary>
        /// ie. Convert AB to [11, 12]
        /// </summary>
        private static TryMessage TryGetRawValue(string value, int baseValue, out RawValue rawValue) {
            rawValue = new RawValue();
            var beforeDecimal = new List<int>();
            var afterDecimal = new List<int>();

            bool isBeforeDecimal = true;

            foreach (var c in value) {
                int cValue;

                if (Char.IsNumber(c)) {
                    cValue = (int)Char.GetNumericValue(c);
                } else if (c.Equals('.')) {

                    // There was already a decimal point beforehand
                    if (!isBeforeDecimal) {
                        return new TryMessage(false, new InvalidOperationException("There was more than one decimal point in the given value."));
                    }

                    isBeforeDecimal = false;
                    continue;
                } else {
                    // Handles 'A-Z' characters
                    // Follows ASCII table https://www.asciitable.com/

                    cValue = (int)c;
                    if (c >= 65 && c <= 90) {
                        // 'A-Z'
                        cValue -= 55;
                    } else if (c >= 97 && c <= 122) {
                        // 'a-z'; Notice the case-sensitivity.
                        cValue -= 61;
                    } else {
                        // Out of library's ASCII range to handle.
                        return new TryMessage(false, new ASCIIOutOfRangeException("Out of ASCII range to handle; Only '0-9', 'A-Z' and 'a-z' are handled, Base62."));
                    }

                }

                if (cValue > baseValue) {
                    // One of the value doesn't match up with the Base provided.
                    return new TryMessage(false, new InvalidBaseValueException("Value given does not match with the given base."));
                }

                if (isBeforeDecimal) {
                    beforeDecimal.Insert(0, cValue);
                } else {
                    afterDecimal.Insert(0, cValue);
                }
            }

            rawValue = new RawValue(beforeDecimal, afterDecimal);
            return new TryMessage(true);
        }

        private static string Reverse(string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
