using UnityEngine;
using System.Collections;
using Votyra.Common.Models;
using Votyra.Unity.Images;

namespace Votyra.Unity.Behaviours
{
    public class ClickToPaint : MonoBehaviour
    {
        private const float Period = 0.1f;
        private float lastTime;
        private Vector2i lastCell;

        private MaxtrixImageBehaviour _image;
        void Start()
        {
            _image = GetComponent<MaxtrixImageBehaviour>();
        }

        private void OnCellClick(Vector2 position)
        {
            var cell = new Vector2i(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

            if (cell == lastCell && Time.time < lastTime + Period)
            {
                return;
            }
            lastCell = cell;
            lastTime = Time.time;

            int offset = 0;

            if (Input.GetMouseButton(0))
            {
                offset = 1;
            }
            else if (Input.GetMouseButton(1))
            {
                offset = -1;
            }
            // Debug.LogFormat("Changing pixel {0},{1} by {2} .", cell.x, cell.y, offset);

            int maxDist;
            if (Input.GetButton("Modifier1"))
                maxDist = 3;
            else
                maxDist = 1;

            for (int ox = -maxDist; ox <= maxDist; ox++)
            {
                for (int oy = -maxDist; oy <= maxDist; oy++)
                {
                    var dist = Mathf.Max(Mathf.Abs(ox), Mathf.Abs(oy));
                    _image.SetByOffsetValue(cell + new Vector2i(ox, oy), offset * (maxDist - dist));
                }
            }
        }
    }
}