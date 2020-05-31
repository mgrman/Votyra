namespace Votyra.Core.Painting.Commands
{
    public class FlattenLarge : PaintCommand
    {
        private float? centerValue;

        public FlattenLarge()
            : base(2)
        {
        }

        protected override void OnInvocationStopping()
        {
            base.OnInvocationStopping();
            this.centerValue = null;
        }

        protected override void PrepareWithClickedValue(float clickedValue)
        {
            this.centerValue = this.centerValue ?? clickedValue;
            base.PrepareWithClickedValue(clickedValue);
        }

        protected override float Invoke(float value, int strength) => this.centerValue ?? value;
    }
}
