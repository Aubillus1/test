using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HsPvSerAPG.Vista.Reem_Caja
{
    /// <summary>
    /// Lógica de interacción para Resumen.xaml
    /// </summary>
    public partial class Resumen : Window
    {
        public Resumen()
        {
            InitializeComponent();
        }

        private void Click_BtnSalir(object sender, RoutedEventArgs e) => this.Close();
    }
}
