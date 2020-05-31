namespace Votyra.Core.Painting.Commands
{
    public class RemoveHole : HolePaintCommand
    {
        public RemoveHole()
            : base(1)
        {
        }

        protected override float Invoke(float value, int distance) => 0;
    }
}
