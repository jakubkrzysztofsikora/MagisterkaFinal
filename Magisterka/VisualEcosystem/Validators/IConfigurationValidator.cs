using GraphX.Controls;

namespace Magisterka.VisualEcosystem.Validators
{
    public interface IConfigurationValidator
    {
        bool ValidateCanBeDefinedPosition(VertexControl vertex);
    }
}
