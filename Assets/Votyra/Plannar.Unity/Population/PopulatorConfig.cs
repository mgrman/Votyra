using Votyra.Core;

namespace Votyra.Plannar.Unity
{
    public interface IPopulatorConfig : ISharedConfig
    {
        PopulatorConfigItem[] ConfigItems { get; }
    }

    public class PopulatorConfig : IPopulatorConfig
    {
        public PopulatorConfig([ConfigInject("populationConfigItems"),]
            PopulatorConfigItem[] configItems)
        {
            this.ConfigItems = configItems;
        }

        public PopulatorConfigItem[] ConfigItems { get; }
    }
}
