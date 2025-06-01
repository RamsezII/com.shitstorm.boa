using System.Collections.Generic;

namespace _BOA_
{
    internal static partial class Harbinger
    {
        public static List<Instruction> ParseInstructions(in string text)
        {
            List<Instruction> instructions = new();
            ParseInstructions(text, 0, instructions);
            return instructions;
        }

        static void ParseInstructions(in string text, in int read_i, in List<Instruction> instructions)
        {

        }
    }
}