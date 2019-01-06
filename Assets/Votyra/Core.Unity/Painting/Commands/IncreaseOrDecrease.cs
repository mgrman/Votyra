namespace Votyra.Core.Painting.Commands
{
    public class IncreaseOrDecrease : PaintCommand
    {
        protected override float Invoke(float value, int strength) => value + strength;
    }
}