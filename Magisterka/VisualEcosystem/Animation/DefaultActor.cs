using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Magisterka.VisualEcosystem.Animation
{
    public class DefaultActor : IMovingActor
    {
        private readonly Rectangle _actor;

        public DefaultActor()
        {
            _actor = new Rectangle();
            _actor.Width = 20;
            _actor.Height = 20;
            Color myColor = Color.FromArgb(255, 255, 0, 0);
            SolidColorBrush myBrush = new SolidColorBrush();
            myBrush.Color = myColor;
            _actor.Fill = myBrush;
        }

        public UIElement PresentActor()
        {
            return _actor;
        }
    }
}
