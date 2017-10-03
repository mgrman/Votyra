using UnityEngine;


namespace Votyra.Core.Images
{
    public interface IInitializableImage
    {
        void StartUsing();
        void FinishUsing();
    }
}
