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

namespace HsPvSerAPG.Vista.CajaChica
{
    /// <summary>
    /// Lógica de interacción para Opciones.xaml
    /// </summary>
    public partial class Opciones : Window
    {
        public Opciones()
        {
            InitializeComponent();
        }

        private void BtnSalir(object sender, RoutedEventArgs e) => this.Close();

        // Abrir formulario de Ingreso
        private void BtnIngreso(object sender, RoutedEventArgs e)
        {
            RegistroIngreso ingreso = new RegistroIngreso(0); // 0 = Ingreso
            ingreso.ShowDialog();
        }

        // Abrir formulario de Egreso
        private void BtnEgreso(object sender, RoutedEventArgs e)
        {
            RegistroIngreso ingreso = new RegistroIngreso(1); // 0 = Ingreso
            ingreso.ShowDialog();
        }

    }
}
    