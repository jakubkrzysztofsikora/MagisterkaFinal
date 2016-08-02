using Magisterka.Domain.Adapters;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public interface ICommandValidator
    {
        void ValidateConfiguration(MapAdapter mapAdapter, object[] enumParams);
    }
}
