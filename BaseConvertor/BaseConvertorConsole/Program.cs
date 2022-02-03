using System;
using BaseConvertor;

namespace BaseConvertorConsole {
    class Program {
        static void Main() {
            string input;
            do {
                Console.WriteLine("\r\n'Value, ValueBase, TargetBase' as inputs.\r\n'EXIT' to exit.\r\n");
                input = Console.ReadLine();

                if (String.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)){
                    break;
                }

                // Dummy console tester
                try {

                    var args = input.Trim().Split(',');
                    var baseNum = new BaseNum(args[0], int.Parse(args[1]));
                    var targetBase = int.Parse(args[2]);

                    var result = Convertor.Convert(baseNum, targetBase);
                    Console.WriteLine("Converted to Base " + result.Base + " with value of " + result.Value+ Environment.NewLine);

                } catch (Exception e) {
                    Console.WriteLine("An exception occured:\r\n" + e.Message);
                }

            } while (!String.Equals(input, "exit", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Press anything to exit...");
        }
    }
}
