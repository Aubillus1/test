using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HsPvSerInstaller2.Controlador
{
    public class AnimationController
    {
        private readonly ContentControl _contenedor;
        public AnimationController(ContentControl contenedor) => _contenedor = contenedor;

        public void CambiarVista(UserControl nuevaVista)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) =>
            {
                _contenedor.Content = nuevaVista;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                nuevaVista.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            if (_contenedor.Content is UIElement actual)
                actual.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            else
                _contenedor.Content = nuevaVista;
        }
    }
}
