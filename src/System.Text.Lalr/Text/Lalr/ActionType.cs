using System;

namespace System.Text.Lalr
{
    internal enum ActionType
    {
        Shift,
        Accept,
        Reduce,
        Error,
        SSConflict,
        SRConflict,
        RRConflict,
        SHResolved,
        RDResolved,
        NotUsed,
    }
}
