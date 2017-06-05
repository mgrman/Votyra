using UnityEngine;
using Votyra.Common.Models;

namespace Votyra.Images
{
    public interface IInitializableImage
    {
        void StartUsing();
        void FinishUsing();
    }
}