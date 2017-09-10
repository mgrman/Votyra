using System;
using UnityEngine;
using Votyra.Models;

namespace Votyra.Images.EditableImages
{
    public interface IEditableImageAccessor2f : IDisposable
    {
        Rect2i Area { get; }

        float this[Vector2i pos] { get; set; }
    }
}