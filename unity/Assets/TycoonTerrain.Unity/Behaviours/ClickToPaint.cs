using UnityEngine;
using System.Collections;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Unity.Images;

namespace TycoonTerrain.Unity.Behaviours
{
    public class ClickToPaint : MonoBehaviour
    {
        private MaxtrixImageBehaviour _image;
        void Start()
        {
            _image = GetComponent<MaxtrixImageBehaviour>();
        }

        private void OnCellClick(Vector2 cell)
        {
            int offset = 0;

            if (Input.GetMouseButton(0))
            {
                offset = 1;
            }
            else  if (Input.GetMouseButton(1))
            {
                offset = -1;
            }
            Debug.LogFormat("Changing pixel {0},{1} by {2} .", cell.x, cell.y, offset);
            
            _image.SetByOffsetValue(new Vector2i(Mathf.FloorToInt(cell.x), Mathf.FloorToInt(cell.y)), offset);
        }

    }
}