using Votyra.Core.Models;

namespace Votyra.Core
{
    public class StateModel : IStateModel
    {
        public bool IsEnabled { get; set; } = true;
    }
}