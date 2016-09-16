using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Magisterka.VisualEcosystem.Animation
{
    public class DefaultActor : IMovingActor
    {
        public double Width { get; }
        public double Height { get; }

        private readonly Rectangle _actor;

        public DefaultActor()
        {
            _actor = new Rectangle
            {
                Width = Width = 20,
                Height = Height = 20
            };
            Color myColor = Color.FromArgb(255, 255, 0, 0);
            SolidColorBrush myBrush = new SolidColorBrush
            {
                Color = myColor
            };
            _actor.Fill = myBrush;
        }

        public UIElement PresentActor()
        {
            return _actor;
        }
    }
}
