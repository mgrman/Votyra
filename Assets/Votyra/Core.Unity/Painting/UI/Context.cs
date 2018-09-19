using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Votyra.Core.Painting.Commands;
using System;

namespace Votyra.Core.Painting.UI
{
    public interface IContext
    {
        object Value { get; set; }
    }

    public class Context : MonoBehaviour, IContext
    {
        public object Value { get; set; }
    }
}