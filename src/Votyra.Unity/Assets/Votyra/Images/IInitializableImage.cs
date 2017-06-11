using UnityEngine;
using Votyra.Models;

namespace Votyra.Images
{
    public interface IInitializableImage
    {
        void StartUsing();
        void FinishUsing();
    }
}