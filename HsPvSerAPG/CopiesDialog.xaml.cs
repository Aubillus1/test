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

namespace HsPvSerAPG
{
    /// <summary>
    /// Lógica de interacción para CopiesDialog.xaml
    /// </summary>
    public partial class CopiesDialog : Window
    {
        public int Copias { get; private set; } = 3;
        public CopiesDialog()
        {
            InitializeComponent();
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCopias.Text, out int val))
            {
                txtCopias.Text = (val + 1).ToString();
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCopias.Text, out int val) && val > 1)
            {
                txtCopias.Text = (val - 1).ToString();
            }
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtCopias.Text, out int copias) || copias <= 0)
            {
                MessageBox.Show("Ingrese un número válido.");
                return;
            }

            Copias = copias;
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
