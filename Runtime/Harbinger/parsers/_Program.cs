using System.IO;

namespace _BOA_
{
    partial class Harbinger
    {

        //----------------------------------------------------------------------------------------------------------

        public bool TryRunScript(out Executor executor, out string error, out string long_error)
        {
            BoaReader reader = new(strict_syntax, File.ReadAllText(script_path));

            bool success = TryParseProgram(reader, out executor);
            error = reader.error;

            if (reader.error != null && reader.long_error == null)
                reader.LocalizeError(File.ReadAllLines(script_path));

            long_error = reader.long_error;

            return success;
        }

        public bool TryParseProgram(in BoaReader reader, out Executor executor)
        {
            executor = null;

            BlockExecutor program = new(this, null);

            while (TryParseBlock(reader, program, out var sub_block))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (reader.error != null)
                goto failure;

            if (reader.TryPeekChar_out(out char peek))
            {
                reader.error ??= $"could not parse everything ({nameof(peek)}: '{peek}')";
                goto failure;
            }

            executor = program;
            return true;

        failure:
            return false;
        }
    }
}