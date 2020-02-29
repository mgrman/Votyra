using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint2i
    {
        void Initialize(IEditableImage2f image);
        IEnumerable<int> Priorities { get; }
        Range2i FixImage(IEditableImage2f image, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction);
    }
}