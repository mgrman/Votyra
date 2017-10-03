using UnityEngine;
using Votyra.Models;

namespace Votyra.Core.Images
{
    public interface IInitializableImage
    {
        void StartUsing();
        void FinishUsing();
    }
}
