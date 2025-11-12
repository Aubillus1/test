using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Windows;

namespace HsPvSerAPG.Vista
{
    public partial class RegistroClipro : Window
    {
        private readonly CliproController _controller;

        public RegistroClipro()
        {
            InitializeComponent();
            _controller = new CliproController(); // instancia controlador, no la ventana
        }

        private async void BtnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show("Debe ingresar el número de documento.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellidoPaterno.Text) && string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                MessageBox.Show("Debe ingresar al menos Apellido Paterno o Nombres.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var clipro = new Clipro
            {
                cia = sisVariables.GCia,
                nrodoc = txtDocumento.Text.Trim(),
                razsoc = $"{txtApellidoPaterno.Text} {txtApellidoMaterno.Text} {txtNombres.Text}".Trim(),
                razcom = "",
                nomcom = txtNombres.Text.Trim(),
                direcc = txtDireccion.Text.Trim(),
                telefo = txtTelefonoFijo.Text.Trim(),
                fax = txtWhatsapp.Text.Trim(),
                correo = txtCorreoElectronico.Text.Trim(),
                codven = sisVariables.GCodven,
                codusr = sisVariables.GCodusr
            };

            clipro  = await _controller.RegistrarClipro(clipro);

            if (clipro != null )
            {
                MessageBox.Show("Cliente registrado correctamente con codigo  " +clipro.codigo , "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            else
            {
                MessageBox.Show("Ocurrió un error al registrar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        public void CargarDatosParaEdicion(Clipro clipro)
        {
            txtDocumento.Text = clipro.nrodoc;
            txtDocumento.IsEnabled = false;

            txtApellidoPaterno.Text = clipro.apepat;
            
            txtApellidoMaterno.Text = clipro.apemat; 
            txtNombres.Text = clipro.nombre;
            txtDireccion.Text = clipro.direcc;
            txtTelefonoFijo.Text = clipro.telefo;
            txtWhatsapp.Text = clipro.fax;
            txtCorreoElectronico.Text = clipro.correo;
        }
        

    }
}
