using System;

namespace Votyra.Core.InputHandling
{
    [Flags]
    public enum InputActions:short
    {
        LeftRight = 1 << 1,
        ForwardBackward=1<<2,
        ExtendedModifier = 1 << 3,
        InverseModifier = 1 << 4,
        UpDown = 1 << 5,
        Jump = 1 << 6,
        Rotate = 1 << 7,
        Action = 1 << 8
    }
}