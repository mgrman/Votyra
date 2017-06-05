using Votyra.Common.Models;

namespace Votyra.Unity.GroupSelectors
{
    public enum UpdateAction
    {
        Recompute,
        Keep
    }

    public struct Group
    {
        public readonly Vector2i Index;
        public readonly UpdateAction UpdateAction;

        public Group(Vector2i index, UpdateAction action)
        {
            Index = index;
            UpdateAction = action;
        }
    }
}