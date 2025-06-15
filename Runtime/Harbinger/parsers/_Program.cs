using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                reader.LocalizeError(script_path, File.ReadAllLines(script_path));

            long_error = reader.long_error;

            return success;
        }

        public bool TryParseProgram(in BoaReader reader, out Executor executor)
        {
            executor = null;

            BlockExecutor program = new(this, null);

            List<Executor> args_list = new(args.Count);
            for (int i = 0; i < args.Count; ++i)
                args_list.Add(new LiteralExecutor(this, program, args[i]));

            program._variables.Add("_args_", new BoaVariable(args_list));

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