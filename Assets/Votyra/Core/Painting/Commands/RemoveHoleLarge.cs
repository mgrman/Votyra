namespace Votyra.Core.Painting.Commands
{
    public class RemoveHoleLarge : HolePaintCommand
    {
        public RemoveHoleLarge()
            : base(2)
        {
        }

        protected override float Invoke(float value, int distance) => 0;
    }
}
