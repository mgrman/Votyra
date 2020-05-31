namespace Votyra.Core.Painting.Commands
{
    public class IncreaseLarge : PaintCommand
    {
        public IncreaseLarge()
            : base(2)
        {
        }

        protected override float Invoke(float value, int distance) => (value + 2) - distance;
    }
}
