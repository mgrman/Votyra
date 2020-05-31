namespace Votyra.Core.Painting.Commands
{
    public class MakeHoleLarge : HolePaintCommand
    {
        public MakeHoleLarge()
            : base(2)
        {
        }

        protected override float Invoke(float value, int distance) => float.NaN;
    }
}
