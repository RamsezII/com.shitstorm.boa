using System.Collections.Generic;

partial class Util_boa
{
    public static IEnumerable<string> ECompletionMatches(this IEnumerable<string> candidates, string argument)
    {
        foreach (string candidate in candidates)
            if (MatchesCompletion(candidate, argument))
                yield return candidate;
    }

    public static bool MatchesCompletion(this string completion, in string argument, in bool ignore_case = true)
    {
        var ordinal = ignore_case.ToOrdinal();
        int last = 0, ic = 0, matches = 0;
        while (ic < argument.Length)
        {
            int i = completion.IndexOf(argument[ic..++ic], last, completion.Length - last, ordinal);
            if (i >= 0)
            {
                last = i + 1;
                ++matches;
            }
        }
        return matches >= argument.Length;
    }
}