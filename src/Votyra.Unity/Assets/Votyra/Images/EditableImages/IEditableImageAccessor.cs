using System;
using UnityEngine;
using Votyra.Models;

namespace Votyra.Images.EditableImages
{
    public interface IEditableImageAccessor : IDisposable
    {
        Rect2i Area { get; }

        int this[Vector2i pos] { get; set; }
    }
}