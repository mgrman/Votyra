namespace Votyra.Core.Painting.Commands
{
    public class Flatten : PaintCommand
    {
        private float? centerValue;

        public Flatten()
            : base(0)
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