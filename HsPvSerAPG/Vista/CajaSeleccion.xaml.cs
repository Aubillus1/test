using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
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

namespace HsPvSerAPG.Vista
{
    /// <summary>
    /// Lógica de interacción para CompañiaSelección.xaml
    /// </summary>
    public partial class CompañiaSelección : Window
    {
        private string compania;
        private TabcajaController tabcajaController = new TabcajaController();

        public CompañiaSelección(string compania)
        {
            InitializeComponent();
            caja();
            this.compania = compania;
            NumLote.Focus();
        }

        private void caja()
        {
            var cajas = tabcajaController.selectCajaLogin(sisVariables.GCia, sisVariables.GCodusr);

            if (cajas != null && cajas.Count > 0)
            {
                CBCaja.ItemsSource = cajas;
                CBCaja.DisplayMemberPath = "des";
                CBCaja.SelectedValuePath = "cod"; // ← importante: usar "cod" como clave interna
                CBCaja.SelectedIndex = 0;
            }
        }

        private void Ingresar()
        {
            if (CBCaja.SelectedItem == null)
            {
                MessageBox.Show("No tienes cajas asignadas. Comunícate con un administrador.",
                                "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Condicional en caso el numero de lote no sea solo numeros
            if (!NumLote.Text.All(char.IsDigit))
            {
                MessageBox.Show("Escribe un número de lote válido",
                                "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Tabcaja tabcaja = (Tabcaja)CBCaja.SelectedItem;
            sisVariables.GCodCaja = tabcaja.cod;
            sisVariables.GCodSucursal = tabcaja.cod;
            sisVariables.GCodSuc = tabcaja.cod;
            sisVariables.GNumLote = NumLote.Text;
            sisVariables.GSucursal = CBCaja.Text;
            PuntoDeVenta puntoDeVenta = new PuntoDeVenta(compania, CBCaja.Text);
            this.Close();
            puntoDeVenta.Show();
        }

        private void BtnIngresar_Click(object sender, RoutedEventArgs e) => Ingresar();

        private void Click_BtnSalir(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void NumLote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                Ingresar();
        }
    }
}