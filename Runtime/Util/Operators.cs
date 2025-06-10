using System;

namespace _BOA_
{
    enum OperatorsE : byte
    {
        assign,
        not,
        add, sub,
        mul, div, div_int, mod,
        eq, gt, lt,
        and, or, xor,
    }

    [Flags]
    public enum OperatorsM : ushort
    {
        unknown,
        assign = 1 << OperatorsE.assign,
        not = 1 << OperatorsE.not,
        add = 1 << OperatorsE.add,
        sub = 1 << OperatorsE.sub,
        mul = 1 << OperatorsE.mul,
        div = 1 << OperatorsE.div,
        div_int = 1 << OperatorsE.div_int,
        mod = 1 << OperatorsE.mod,
        eq = 1 << OperatorsE.eq,
        neq = not | eq,
        gt = 1 << OperatorsE.gt,
        lt = 1 << OperatorsE.lt,
        ge = gt | eq,
        le = lt | eq,
        and = 1 << OperatorsE.and,
        or = 1 << OperatorsE.or,
        xor = 1 << OperatorsE.xor,
    }
}