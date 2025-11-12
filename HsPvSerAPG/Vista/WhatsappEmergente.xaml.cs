using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HsPvSerAPG.Vista
{
    /// <summary>
    /// Lógica de interacción para WhatsappEmergente.xaml
    /// </summary>
    public partial class WhatsappEmergente : Window
    {
        public string NumeroWhatsApp { get; private set; }
        public WhatsappEmergente(string num)
        {
            InitializeComponent();
            txtNumero.Text = num;
        }
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (txtNumero.Text == null || txtNumero.Text == "" || txtNumero.Text.Length == 9)
            {
                MessageBox.Show("Debe ingresar un número de WhatsApp válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NumeroWhatsApp = txtNumero.Text;
            this.DialogResult = true;
            this.Close();
        }

    }
}
