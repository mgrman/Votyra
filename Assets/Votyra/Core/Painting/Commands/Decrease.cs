namespace Votyra.Core.Painting.Commands
{
    public class Decrease : PaintCommand
    {
        public Decrease()
            : base(0)
        {
        }

        protected override float Invoke(float value, int distance) => value - 1;
    }
}
