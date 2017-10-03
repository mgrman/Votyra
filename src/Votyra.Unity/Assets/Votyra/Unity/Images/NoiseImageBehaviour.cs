// using Votyra.Images;
// using UnityEngine;

// namespace Votyra.Unity.Images
// {
//     internal class NoiseImageBehaviour : MonoBehaviour, IImage2iProvider
//     {
//         private Vector3 _old_offset = Vector3.zero;
//         public Vector3 offset = Vector3.zero;
//         private Vector3 _old_scale = Vector3.one;
//         public Vector3 scale = Vector3.one;

//         private float _old_timeRounding = 1;
//         public float timeRounding = 1;
//         private float _old_timeScale = 1;
//         public float timeScale = 1;

//         private RoundImage _image = null;

//         public IImage2i CreateImage()
//         {

//             return _image;

//         }

//         private bool _fieldsChanged = true;

//         private void Start()
//         {
//             SetImage();
//         }

//         private void Update()
//         {
//             _fieldsChanged = false;
//             if (_old_offset != offset)
//             {
//                 _old_offset = offset;
//                 _fieldsChanged = _fieldsChanged || true;
//             }
//             if (_old_scale != scale)
//             {
//                 _old_scale = scale;
//                 _fieldsChanged = _fieldsChanged || true;
//             }
//             if (_old_timeRounding != timeRounding)
//             {
//                 _old_timeRounding = timeRounding;
//                 _fieldsChanged = _fieldsChanged || true;
//             }
//             if (_old_timeScale != timeScale)
//             {
//                 _old_timeScale = timeScale;
//                 _fieldsChanged = _fieldsChanged || true;
//             }
//             if (_fieldsChanged)
//             {
//                 SetImage();
//             }
//         }

//         private void SetImage()
//         {
//             // TODO Animation has to be done here!!!!
//             // float offsetTime = TimeRounding > 0 ? (time * TimeScale).Round(TimeRounding) : time * TimeScale;

//             // _image = new RoundImage(new NoiseImage(offset, scale, timeRounding, timeScale));
//         }
//     }
// }