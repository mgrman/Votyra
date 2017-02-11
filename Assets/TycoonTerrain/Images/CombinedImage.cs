//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//class CombinedImage : MonoBehaviour, IImage2
//{
//    public MonoBehaviour ImageA;
//    public MonoBehaviour ImageB;

//    private IImage2 _imageA;
//    private IImage2 _imageB;

//    public enum Operations
//    {
//        Add,
//        Subtract,
//        Multiply,
//        Divide,
//        ImageAAddToXInImageB,
//        ImageAAddToYInImageB

//    }
//    public Operations Operation = Operations.Add;


//    public Range2 RangeZ
//    {
//        get
//        {

//            _imageA = ImageA as IImage2;
//            _imageB = ImageB as IImage2;

//            return _imageA.RangeZ.UnionWith(_imageB.RangeZ);
//        }
//    }

//    public bool IsChanged(float time)
//    {
//        _imageA = ImageA as IImage2;
//        _imageB = ImageB as IImage2;

//        return _imageA.IsChanged(time) || _imageB.IsChanged(time);
//    }

//    public float Sample(Vector2 point, float time)
//    {
//        if (_imageA == null || _imageB == null)
//        {
//            return 0;
//        }
//        float a = _imageA.Sample(point, time);
//        float b = _imageB.Sample(point, time);
//        switch (Operation)
//        {
//            case Operations.Add:
//                return a + b;

//            case Operations.Subtract:
//                return a - b;

//            case Operations.Multiply:
//                return a * b;

//            case Operations.Divide:
//                return a / b;

//            case Operations.ImageAAddToXInImageB:
//                return _imageB.Sample(new Vector2(point.x + a, point.y), time);

//            case Operations.ImageAAddToYInImageB:
//                return _imageB.Sample(new Vector2(point.x, point.y + a), time);

//            default:
//                return 0;
//        }

//    }
//}

