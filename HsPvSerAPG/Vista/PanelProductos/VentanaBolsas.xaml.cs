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

namespace HsPvSerAPG.Vista.PanelProductos
{
    /// <summary>
    /// Lógica de interacción para VentanaBolsas.xaml
    /// </summary>
    public partial class VentanaBolsas : Window
    {
        public VentanaBolsas()
        {
            InitializeComponent();
            TBCantidad.Focus();
        }

        private void Restar_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = int.Parse(TBCantidad.Text);
            cantidad--;
            TBCantidad.Text = cantidad.ToString();
        }

        private void Sumar_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = int.Parse(TBCantidad.Text);
            cantidad++;
            TBCantidad.Text = cantidad.ToString();
        }

        private void TBCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.Close();
        }

        private void TBCantidad_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!int.TryParse(TBCantidad.Text, out int cant))
            {
                cant = 0;
                e.Handled = true;
            }

            if (e.Key == Key.Up)
            {
                TBCantidad.Text = (++cant).ToString();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                TBCantidad.Text = (--cant).ToString();
                e.Handled = true;
            }
        }

        private void TBCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBCantidad.Text))
            {
                TBCantidad.Text = "0";
                TBCantidad.CaretIndex = TBCantidad.Text.Length;
                return;
            }


            if (double.TryParse(TBCantidad.Text, out double cantidad_actual))
            {
                if (cantidad_actual < 1)
                {
                    TBCantidad.Text = "0";
                    TBCantidad.CaretIndex = TBCantidad.Text.Length;
                }
            }
        }
    }
}
