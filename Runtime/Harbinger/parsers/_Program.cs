using System.IO;

namespace _BOA_
{
    partial class Harbinger
    {

        //----------------------------------------------------------------------------------------------------------

        public bool TryRunScript(out Executor executor, out string error, out string error_long, bool strict_syntax)
        {
            BoaReader reader = new(strict_syntax, File.ReadAllText(script_path));

            if (!TryParseProgram(reader, out executor, out error) || error != null)
            {
                error_long = reader.LocalizeError(error, File.ReadAllLines(script_path));
                return false;
            }

            error_long = null;
            return true;
        }

        public bool TryParseProgram(in BoaReader reader, out Executor executor, out string error)
        {
            executor = null;

            BlockExecutor program = new(this, null);

            while (TryParseBlock(reader, program, out var sub_block, out error))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (error != null)
                goto failure;

            if (reader.TryPeekChar_out(out char peek))
            {
                error ??= $"could not parse '{peek}'";
                goto failure;
            }

            executor = program;
            return true;

        failure:
            return false;
        }
    }
}