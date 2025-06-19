using System;

namespace _BOA_
{
    enum SIG_ENUM : byte
    {
        _kill,
        _lint,
        _check,
        _exec,
        _tick,
        _submit,
        _double,
        _cpl,
        _tab,
        _list,
        _alt,
        _hist,
        _up,
        _down,
        _left,
        _right,
        _last_,
    }

    [Flags]
    public enum SIG_FLAGS_new : ushort
    {
        _none_,
        KILL = 1 << SIG_ENUM._kill,
        LINT = 1 << SIG_ENUM._lint,
        CHECK = 1 << SIG_ENUM._check,
        EXEC = 1 << SIG_ENUM._exec,
        TICK = EXEC | 1 << SIG_ENUM._tick,
        SUBMIT = EXEC | 1 << SIG_ENUM._submit,
        DOUBLE = 1 << SIG_ENUM._double,
        CPL = 1 << SIG_ENUM._cpl,
        TAB = CPL | 1 << SIG_ENUM._tab,
        LIST = CPL | 1 << SIG_ENUM._list,
        ALT = CPL | 1 << SIG_ENUM._alt,
        HIST = 1 << SIG_ENUM._hist,
        UP = 1 << SIG_ENUM._up,
        RIGHT = 1 << SIG_ENUM._right,
        DOWN = 1 << SIG_ENUM._down,
        LEFT = 1 << SIG_ENUM._left,

        UP_DOWN = UP | DOWN,
        LEFT_RIGHT = LEFT | RIGHT,

        ALT_UP = ALT | UP,
        ALT_RIGHT = ALT | RIGHT,
        ALT_DOWN = ALT | DOWN,
        ALT_LEFT = ALT | LEFT,

        HIST_UP = HIST | UP,
        HIST_DOWN = HIST | DOWN,
    }
}