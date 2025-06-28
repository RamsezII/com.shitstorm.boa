using System.Collections.Generic;
using System.Linq;

public static partial class Util_boa
{
    public static string ToBoaString(this object data) => data switch
    {
        IEnumerable<string> es => es.Join(", ", "[ ", " ]"),
        IEnumerable<object> eo => eo.Select(o => o?.ToString()).ToBoaString(),
        _ => data?.ToString(),
    };
}