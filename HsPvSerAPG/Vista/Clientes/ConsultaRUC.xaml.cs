using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;

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
    /// Lógica de interacción para ConsultaRUC.xaml
    /// </summary>
    public partial class ConsultaRUC : Window
    {
        private CliproController _controller = new CliproController();
        public ConsultaRUC()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btn_ConsultaSunat_Click(object sender, RoutedEventArgs e) => ConsultarRUC();

        private void txtRUC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ConsultarRUC();
        }

        private async Task<Clipro> ObtenerClientePorRuc(string ruc)
        {
            int cia = 1; // Usa el valor real de tu empresa si lo tienes
            var clientes = await Task.Run(() => _controller.SelectClipro(ruc, cia));
            return clientes?.FirstOrDefault();
        }

        private async void Btn_Grabar(object sender, RoutedEventArgs e)
        {
            string ruc = txtRUC.Text.Trim();

            var clipro = new Clipro
            {
                nrodoc = ruc,
                razsoc = txtRazonSocial.Text.Trim(),
                direcc = txtDireccion.Text.Trim(),
                cia = sisVariables.GCia,
            };

            var clienteExistente = await ObtenerClientePorRuc(ruc);

            var resultado = await _controller.RegistrarClipro(clipro);

            if (resultado != null)
            {
                if (clienteExistente == null)
                {
                    MessageBox.Show("Cliente registrado correctamente.", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Cliente actualizado correctamente.", "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Error al registrar o actualizar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }

        private void ConsultarRUC()
        {
            string ruc = txtRUC.Text.Trim();
            if (ruc.Length != 11)
            {
                MessageBox.Show("Ingrese un RUC válido de 11 dígitos.");
                return;
            }

            var api = new ApiReniecController();
            var sunatInfo = api.ConsultarRUC(ruc);

            if (sunatInfo != null)
            {
                //txtRUCResult.Text = sunatInfo.ruc ?? "";
                txtRazonSocial.Text = sunatInfo.razsoc ?? "";
                txtDireccion.Text = sunatInfo.direccion ?? "";
                txtEstadoContribuyente.Text = sunatInfo.estado ?? "";
                txtCondicionContribuyente.Text = sunatInfo.condicion ?? "";

                txtTelefonoFijo.Text = "";
                txtTelefonoCelular.Text = "";
                txtWhatsApp.Text = "";
                txtCorreoElectronico.Text = "";
            }
        }
    }
    
}
