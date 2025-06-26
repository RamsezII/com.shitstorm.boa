using _ARK_;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseProgram(in BoaReader reader, in ScopeNode scope, out bool background, out Executor executor)
        {
            background = false;
            executor = null;

            BlockExecutor program = new(this, scope ?? new ScopeNode(null, true));
            program.scope.SetVariable("_args_", new BoaVariable(args));
            program.scope.SetVariable("_app_dir_", new BoaVariable(NUCLEOR.game_path.DOS2UNIX_full()));

            while (TryParseBlock(reader, program.scope, out var sub_block))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (reader.sig_error != null)
                goto failure;

            background = reader.TryReadChar_match('&', lint: reader.lint_theme.command_separators);

            if (reader.TryPeekChar_out(out char peek, out _))
            {
                reader.Stderr($"could not parse everything ({nameof(peek)}: '{peek}').");
                goto failure;
            }

            executor = program;
            return true;

        failure:
            reader.LocalizeError();
            return false;
        }
    }
}