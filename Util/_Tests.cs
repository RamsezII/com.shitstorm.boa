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
            void Waw(in string value, out string output)
            {
                Debug.Log("in1: " + value);
                output = "zzup Vega";
                Debug.Log("in2: " + value);
            }

            string value = "Hello Kong";
            Waw(value, out value);
            Debug.Log("out: " + value);
        }
    }
}
#endif