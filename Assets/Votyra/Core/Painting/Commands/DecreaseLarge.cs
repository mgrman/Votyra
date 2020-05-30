namespace Votyra.Core.Painting.Commands
{
    public class DecreaseLarge : PaintCommand
    {
        public DecreaseLarge()
            : base(2)
        {
        }

        protected override float Invoke(float value, int distance) => (value - 2) + distance;
    }
}