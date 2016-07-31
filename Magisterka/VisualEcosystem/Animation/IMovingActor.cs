using System.Windows;

namespace Magisterka.VisualEcosystem.Animation
{
    public interface IMovingActor
    {
        double Width { get; }
        double Height { get; }
        UIElement PresentActor();
    }
}
