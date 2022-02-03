using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseConvertor.Exceptions;

namespace BaseConvertor {
    public static class Convertor {

        private static readonly char[] Sexagesimal_Table = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x'};

        public static BaseNum Convert(string value, int baseValue, int toBase) {

            if (toBase == baseValue) {
                return new BaseNum(value, toBase);
            }

            if (baseValue == 10) {
                if (!int.TryParse(value, out int b10)) {
                    throw new InvalidBaseValueException("Value given does not match with the given base.");
                } else {
                    return FromBase10ToX(b10, toBase);
                }
            } else if (toBase == 10) {
                return ToBase10(value, baseValue);
            } else {
                BaseNum base10 = ToBase10(value, baseValue);
                return FromBase10ToX(int.Parse(base10.Value), toBase);
            }
        }

        public static BaseNum Convert(BaseNum baseNum, int toBase) {
            return Convert(baseNum.Value, baseNum.Base, toBase);
        }

        public static BaseNum FromBase10ToX(int base10Value, int targetBase) {
            StringBuilder result = new StringBuilder();

            do {
                result.Append(Sexagesimal_Table[base10Value % targetBase]);
                base10Value /= targetBase;
            } while (base10Value > 0);

            return new BaseNum(result.ToString().Reverse().ToString(), targetBase);
        }

        public static BaseNum ToBase10(string value, int baseValue) {
            TryMessage tryMessage = TryGetRawValue(value, baseValue, out List<int> rawValue);

            if (!tryMessage.Success) {
                throw tryMessage.Exception;
            }

            int rawResult = 0;

            for (int i = 0; i < rawValue.Count; ++i) {
                // Only multiply by baseValue if not the last digit.
                rawResult += rawValue[i] * (i == rawValue.Count - 1 ? 1 : baseValue);
            }

            return new BaseNum(rawResult.ToString(), 10);
        }

        public static BaseNum ToBase10(BaseNum baseNum) {
            return ToBase10(baseNum.Value, baseNum.Base);
        }

        /// <summary>
        /// ie. Convert AB to [11, 12]
        /// </summary>
        private static TryMessage TryGetRawValue(string value, int baseValue, out List<int> rawValue) {
            rawValue = new List<int>();

            foreach (var c in value) {
                int cValue;

                if (Char.IsNumber(c)) {
                    cValue = (int)Char.GetNumericValue(c);
                } else {
                    // Handles 'A-Z' characters
                    // Follows ASCII table https://www.asciitable.com/

                    cValue = (int)c;
                    if (c >= 65 && c <= 90) {
                        // 'A-Z'
                        cValue -= 54;
                    } else if (c >= 97 && c <= 122) {
                        // 'a-z'; Notice the case-sensitivity.
                        cValue -= 60;
                    } else {
                        // Out of library's ASCII range to handle.
                        return new TryMessage(false, new ASCIIOutOfRangeException("Out of ASCII range to handle; Only '0-9', 'A-Z' and 'a-z' are handled."));
                    }

                }

                if (cValue >= baseValue) {
                    // One of the value doesn't match up with the Base provided.
                    return new TryMessage(false, new InvalidBaseValueException("Value given does not match with the given base."));
                }

                rawValue.Insert(0, cValue);
            }

            return new TryMessage(true);
        }
    }
}
