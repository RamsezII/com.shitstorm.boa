using UnityEngine;

namespace _BOA_
{
    public sealed class LintTheme
    {
        public static LintTheme
            theme_dark = new()
            {

            },
            theme_light = new()
            {

            };

        public static readonly Color lint_default = Color.white;

        public Color
            argument_coma = Color.lightPink,
            operators = Color.lightGray,
            contracts = Color.darkSlateBlue,
            functions = Color.deepSkyBlue,
            variables = Color.mediumPurple,
            paths = Color.lightYellow,
            comments = Color.darkOliveGreen,
            command_separators = Color.softYellow,
            keywords = Color.magenta,
            bracket_0 = Color.yellow,
            bracket_1 = Color.rebeccaPurple,
            bracket_2 = Color.navyBlue,
            literal = Color.limeGreen,
            strings = Color.yellowNice,
            quotes = Color.orange
            ;

        //----------------------------------------------------------------------------------------------------------

        public LintTheme()
        {
            // int x = (((1 + 1 * 1) * 1 + 1) * 1) + 1;
            // int[] t = new int
            // Debug.Log(x);
        }
    }
}