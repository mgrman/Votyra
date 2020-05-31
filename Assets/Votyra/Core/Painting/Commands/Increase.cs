namespace Votyra.Core.Painting.Commands
{
    public class Increase : PaintCommand
    {
        public Increase()
            : base(0)
        {
        }

        protected override float Invoke(float value, int distance) => value + 1;
    }
}
