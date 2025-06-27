using System;
using UnityEngine;

namespace _BOA_
{
    [Serializable]
    public sealed class LintTheme
    {
        public static readonly LintTheme
            theme_dark = new()
            {

            },
            theme_light = new()
            {

            };

        public Color
            argument = Color.deepPink,
            argument_coma = Color.lightPink,
            point = Color.yellow,
            flags = Color.beige,
            options = Color.bisque,
            option_args = Color.sandyBrown,
            operators = Color.lightGray,
            contracts = Color.darkSlateBlue,
            sub_contracts = Color.darkSlateBlue,
            functions = Color.deepSkyBlue,
            variables = Color.mediumPurple,
            paths = Color.ivory,
            comments = Color.darkOliveGreen,
            command_separators = Color.softYellow,
            keywords = Color.magenta,
            bracket_0 = Color.yellow,
            bracket_1 = Color.rebeccaPurple,
            bracket_2 = Color.navyBlue,
            literal = Color.limeGreen,
            constants = Color.deepSkyBlue,
            strings = Color.orange,
            quotes = Color.yellowNice,
            error = Color.red,
            fallback_default = Color.gray
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