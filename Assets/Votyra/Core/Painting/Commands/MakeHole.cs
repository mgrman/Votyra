namespace Votyra.Core.Painting.Commands
{
    public class MakeHole : HolePaintCommand
    {
        public MakeHole()
            : base(0)
        {
        }

        protected override float Invoke(float value, int distance) => float.NaN;
    }
}