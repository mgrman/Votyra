using UnityEngine;
using Votyra.Core.Images.Constraints;

namespace Votyra.Core.Unity.Config
{
    public class ConstraintConfig : IConstraintConfig
    {
        public ConstraintConfig([ConfigInject("simpleSampleScaleFactor")] ScaleFactor scaleFactor)
        {
            SimpleSampleScaleFactor = Mathf.Clamp((int)scaleFactor,1,2);
        }

        public int SimpleSampleScaleFactor { get; }
    }

    public enum ScaleFactor
    {
        One=1,
        Two=2
    }
}