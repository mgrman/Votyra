namespace Votyra.Core.Painting.Commands
{
    public interface IPaintCommandFactory
    {
        string Action { get; }
        IPaintCommand Create();
    }
}