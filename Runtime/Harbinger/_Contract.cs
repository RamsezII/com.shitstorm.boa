using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public sealed class Contract
        {
            [Serializable]
            public struct Status
            {
                [Range(0, 1)] public float progress;
                public object data;
            }

            public readonly string name;
            public readonly int min_args, max_args;
            public readonly Action<Contractor> args;
            public readonly Func<Contractor, Status> action;
            public readonly Func<Contractor, IEnumerator<Status>> routine;

            //----------------------------------------------------------------------------------------------------------

            public Contract(in string name,
                in int min_args = 0,
                in int max_args = 0,
                in Action<Contractor> args = null,
                in Func<Contractor, Status> action = null,
                in Func<Contractor, IEnumerator<Status>> routine = null
                )
            {
                this.name = name;
                this.min_args = min_args;
                this.max_args = Mathf.Max(min_args, max_args);
                this.args = args;
                this.action = action;
                this.routine = routine;
            }
        }
    }
}