using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public sealed class Contract
    {
        [Serializable]
        public struct Status
        {
            public enum States
            {
                BLOCKING,
                WAIT_FOR_STDIN,
                BACKGROUND,
                DAEMON,
            }

            public States state;
            [Range(0, 1)] public float progress;
        }

        public readonly string name;
        public readonly Type type;
        public readonly int min_args, max_args;
        public readonly Action<Contractor> args, action;
        public readonly Func<Contractor, IEnumerator<Status>> routine;

        //----------------------------------------------------------------------------------------------------------

        public Contract(in string name, 
            in Type type = null,
            in int min_args = 0,
            in int max_args = 0,
            in Action<Contractor> args = null,
            in Action<Contractor> action = null,
            in Func<Contractor, IEnumerator<Status>> routine = null
            )
        {
            this.name = name;
            this.type = type;
            this.min_args = min_args;
            this.max_args = Mathf.Max(min_args, max_args);
            this.args = args;
            this.action = action;
            this.routine = routine;
        }
    }
}