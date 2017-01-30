using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class NoiseImage : MonoBehaviour, IImage
{
    private Vector3 _old_offset = Vector3.zero;
    public Vector3 offset = Vector3.zero;
    private Vector3 _old_scale = Vector3.one;
    public Vector3 scale = Vector3.one;

    private float _old_timeRounding = 1;
    public float timeRounding = 1;
    private float _old_timeScale = 1;
    public float timeScale = 1;

    private bool _fieldsChanged = true;

    void Update()
    {
        _fieldsChanged = false;
        if (_old_offset != offset)
        {
            _old_offset = offset;
            _fieldsChanged = _fieldsChanged || true;
        }
        if (_old_scale != scale)
        {
            _old_scale = scale;
            _fieldsChanged = _fieldsChanged || true;
        }
        if (_old_timeRounding != timeRounding)
        {
            _old_timeRounding = timeRounding;
            _fieldsChanged = _fieldsChanged || true;
        }
        if (_old_timeScale != timeScale)
        {
            _old_timeScale = timeScale;
            _fieldsChanged = _fieldsChanged || true;
        }
    }

    public Range2 RangeZ { get { return new Range2(offset.z, offset.z + scale.z); } }

    public bool IsChanged(float time)
    {
        return _fieldsChanged || timeScale != 0;
    }

    public float Sample(Vector2 point, float time)
    {
        float offsetTime;
        offsetTime = timeRounding > 0 ? (time * timeScale).Round(timeRounding) : time * timeScale;


        return Mathf.PerlinNoise((point.x + offsetTime) / scale.x + offset.x, (point.y + offsetTime * 0.5f) / scale.y + offset.y) * scale.z + offset.z;
    }
}

