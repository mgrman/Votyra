using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IEditableMask2e
    {
        IEditableMaskAccessor2e RequestAccess(Range2i area);
    }
}