using UnityEngine;

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