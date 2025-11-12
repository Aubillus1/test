using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Vista.Control_de_Usuario;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HsPvSerAPG.Vista
{
    public partial class BuscarCliente : Window
    {
        private readonly CliproController controller = new CliproController();
        private readonly int cia = sisVariables.GCia;
        private readonly int codven = sisVariables.GCodven;

        public Clipro cliente = new Clipro();

        public BuscarCliente(string doc)
        {
            InitializeComponent();
            txtCriterio.Text = doc;
            btnBuscar_Click(BtnBuscar, new RoutedEventArgs());
        }

        private void TxtCriterio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                string criterio = txtCriterio.Text.Trim();

                if (string.IsNullOrEmpty(criterio))
                {
                    _ = CargarClientesAsync("");
                    e.Handled = true;
                    return;
                }

                if (criterio.Length < 3)
                {
                    MessageBox.Show("Ingrese al menos 3 caracteres para realizar la búsqueda.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCriterio.Focus();
                    e.Handled = true;
                    return;
                }

                _ = RealizarBusquedaAsync(criterio);
                e.Handled = true;
            }
        }

        private async Task CargarClientesAsync(string codigo)
        {
            try
            {
                var clientes = await Task.Run(() => controller.SelectClipro(codigo, cia));

                if (clientes == null || !clientes.Any())
                {
                    tbInfo.ItemsSource = null;
                    ActualizarContador(0);
                    return;
                }

                tbInfo.ItemsSource = clientes;
                if (tbInfo.ItemsSource != null)
                {
                    tbInfo.SelectedIndex = 0;
                    tbInfo.Focus();
                    tbInfo.ScrollIntoView(tbInfo.SelectedItem);
                }
                ActualizarContador(clientes.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarContador(int cantidad) => txtReg.Text = cantidad.ToString("D5");

        private void btn_Nuevo(object sender, RoutedEventArgs e)
        {
            var nuevaVentana = new RegistroClipro();
            nuevaVentana.ShowDialog();
        }

        private void btn_Salir(object sender, RoutedEventArgs e) => this.Close();


        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (tbInfo.SelectedItem is Clipro seleccionado)
            {
                var ventana = new RegistroClipro();
                ventana.CargarDatosParaEdicion(seleccionado);
                ventana.ShowDialog();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un cliente para modificar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string criterio = txtCriterio.Text.Trim();

            if (string.IsNullOrEmpty(criterio))
            {
                MessageBox.Show("Ingrese un criterio válido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                EfectoCargar.Visibility = Visibility.Visible;
                await RealizarBusquedaAsync(criterio);
            }
            finally
            {
                EfectoCargar.Visibility = Visibility.Collapsed;
            }
        }

        private async Task RealizarBusquedaAsync(string criterio)
        {
            var resultado = await Task.Run(() => controller.SelectClipro(criterio, cia));

            if (resultado != null && resultado.Any())
            {
                await CargarClientesAsync(criterio);
            }
            else
            {
                if (long.TryParse(criterio, out _) && criterio.Length == 8 | criterio.Length <= 8)
                {
                    var ventanaDNI = new ConsultaDNI();
                    ventanaDNI.document_number.Text = criterio;
                    ventanaDNI.ShowDialog();
                }
                else if (long.TryParse(criterio, out _) && criterio.Length == 11 | criterio.Length <= 11)
                {
                    var ventanaRUC = new ConsultaRUC();
                    ventanaRUC.txtRUC.Text = criterio;
                    ventanaRUC.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Cliente no encontrado y el criterio no es válido como DNI o RUC.", "No encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Btn_Sunat(object sender, RoutedEventArgs e)
        {
            var consultaRUC = new ConsultaRUC();
            consultaRUC.ShowDialog();
        }

        private void Btn_Reniec(object sender, RoutedEventArgs e)
        {
            var consultaDNI = new ConsultaDNI();
            consultaDNI.ShowDialog();
        }

        private void tbInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e) => PasaDatosAPrincipal();

        private void PasaDatosAPrincipal()
        {
            if (tbInfo.SelectedItem is Clipro clipro)
            {
                if (clipro == null)
                {
                    MessageBox.Show("No se ha seleccionado ningún cliente.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                cliente = clipro;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void tbInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                PasaDatosAPrincipal();
                e.Handled = true; // Evita que el evento se propague y cause comportamientos no deseados
            }
        }

        private void tbInfo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var grid = (DataGrid)sender;

                // Asegúrate de que hay una fila seleccionada
                if (grid.SelectedItem != null)
                {
                    PasaDatosAPrincipal();
                }
                // Evita que el ENTER haga que cambie de fila
                e.Handled = true;
            }
        }
    }
}
