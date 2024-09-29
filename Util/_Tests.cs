#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace _BOA_
{
    internal static class _Tests
    {
        [MenuItem("Assets/" + nameof(_BOA_) + "/" + nameof(TestBoaParserExecute))]
        static void TestBoaParserExecute()
        {
            string line = "  123 456 789";
            line.TryReadWord(out string newline, out string word, true);
            Debug.Log($"\"{word}\"");
            Debug.Log($"\"{newline}\"");
        }
    }
}
#endif