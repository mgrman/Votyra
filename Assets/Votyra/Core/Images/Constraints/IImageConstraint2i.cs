using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public interface IImageConstraint2I
    {
        IEnumerable<int> Priorities { get; }

        void Initialize(IEditableImage2F image);

        Range2i FixImage(IEditableImage2F image, float[,] editableMatrix, Range2i invalidatedImageArea, Direction direction);
    }
}
