using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using FontAwesome.WPF;
using FontAwesome = FontAwesome.WPF.FontAwesome;

namespace Magisterka.VisualEcosystem.Animation
{
    public class UserIconActor : IMovingActor
    {
        public double Width { get; }
        public double Height { get; }

        private readonly Ellipse _actor;

        public UserIconActor()
        {
            _actor = new Ellipse
            {
                Width = Width = 20,
                Height = Height = 20
            };
            ImageAwesome icon = new ImageAwesome()
            {
                Icon = FontAwesomeIcon.User,
                Foreground = new SolidColorBrush(Color.FromRgb(191,85,236))
            };
            ImageBrush iconBrush = new ImageBrush(icon.Source);
            _actor.Fill = iconBrush;
            
        }

        public UIElement PresentActor()
        {
            return _actor;
        }
    }
}
