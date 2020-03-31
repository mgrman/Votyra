using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IMaskConstraint2e
    {
        IEnumerable<int> Priorities { get; }

        void Initialize(IEditableMask2e mask);

        Range2i FixMask(IEditableMask2e mask, MaskValues[,] editableMatrix, Range2i invalidatedImageArea);
    }
}
