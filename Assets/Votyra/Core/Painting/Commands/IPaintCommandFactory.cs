using Votyra.Core.InputHandling;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public interface IPaintCommandFactory
    {
        string Action { get; }
        IPaintCommand Create();
    }
}