using System;

namespace Votyra.Core.InputHandling
{
    [Flags,]
    public enum InputModifiers : byte
    {
        None = 0,
        Extended = 1 << 0,
        Inverse = 1 << 1,
    }
}
