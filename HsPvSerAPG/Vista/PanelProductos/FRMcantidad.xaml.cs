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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HsPvSerAPG.Vista
{
    /// <summary>
    /// Lógica de interacción para FRMcantidad.xaml
    /// </summary>
    public partial class FRMcantidad : Window
    {
        public Producto ProductoEditado { get; private set; }
        private int TipMonOriginal;
        private decimal PrecioOriginal;
        private int _monedaSeleccionada1;
        private int _monedaSeleccionada;
        private decimal _tipoCambio;
        private decimal PrecioOriginalDolares;
        private int _tipMonOriginal;
        private Producto productoParaCantidad;
        private Producto _producto;

        public FRMcantidad(Producto producto, decimal precioOriginal, int tipMonOriginal)
        {
            InitializeComponent();
            txtCantidad.Focus();
            _producto = producto;
            _tipoCambio = sisVariables.Gtipcam;
            _monedaSeleccionada = sisVariables.Gtipmon;
            _tipMonOriginal = tipMonOriginal;   
            
            ProductoEditado = new Producto
            {
                Codigo = producto.Codigo,
                Descripcion = producto.Descripcion,
                UMD = producto.UMD,
                Unit = producto.Unit,
                Cant = producto.Cant,
                D = producto.D,
                Codart = producto.Codart,
                TipMonOriginal = tipMonOriginal,
                UnitDolares = producto.UnitDolares,
                UnitSoles = producto.UnitSoles,
                Convertido = true,
                Coduni = producto.Coduni
            };

            if (tipMonOriginal == 1)
            {
                ProductoEditado.PrecioBaseSoles = precioOriginal;
                ProductoEditado.PrecioBaseDolares = Math.Round(precioOriginal / _tipoCambio, 6);
            }
            else if (tipMonOriginal == 2)
            {
                ProductoEditado.PrecioBaseDolares = precioOriginal;
                ProductoEditado.PrecioBaseSoles = Math.Round(precioOriginal * _tipoCambio, 6);
            }

            
            txtCodigo.Text = ProductoEditado.Codigo;
            txtPrecio.Text = (_monedaSeleccionada == 1
                ? ProductoEditado.PrecioBaseSoles
                : ProductoEditado.PrecioBaseDolares).ToString("F6");

            txtDescripcion.Text = ProductoEditado.Descripcion;
            txtCantidad.Text = "1";

            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            if (int.TryParse(txtCantidad.Text, out int cantidad) &&
            decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                decimal total = cantidad * precio;
                txtTotal.Text = total.ToString("F2");
            }
        }

        private void AceptarCambios()
        {
            if (int.TryParse(txtCantidad.Text, out int cantidad) && cantidad > 0 &&
                decimal.TryParse(txtPrecio.Text, out decimal nuevoPrecio))
            {
                bool valido = true;

                if (_monedaSeleccionada == 1) 
                {
                    if (nuevoPrecio < ProductoEditado.PrecioBaseSoles)
                        valido = false;
                }
                else if (_monedaSeleccionada == 2)
                {
                    if (nuevoPrecio < ProductoEditado.PrecioBaseDolares)
                        valido = false;
                }

                if (!valido)
                {
                    MessageBox.Show("El precio no puede ser menor al precio original.");
                    txtPrecio.Text = (_monedaSeleccionada == 1
                        ? ProductoEditado.UnitSoles
                        : ProductoEditado.UnitDolares).ToString("F6");
                    txtPrecio.Focus();
                    return;
                }

                ProductoEditado.Cant = cantidad;
                

                if (_monedaSeleccionada == 1) 
                {
                    ProductoEditado.UnitSoles = nuevoPrecio;
                    ProductoEditado.Unit = nuevoPrecio;
                }
                else 
                {
                    ProductoEditado.UnitDolares = nuevoPrecio;
                   ProductoEditado.Unit = nuevoPrecio;
                }

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ingrese una cantidad y precio válidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCantidad.Text = "1";
            }
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e) => AceptarCambios();


        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void txtCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                AceptarCambios();
        }

        private void txtCantidad_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!int.TryParse(txtCantidad.Text, out int cant))
            {
                cant = 0;
                e.Handled = true;
            }

            if (e.Key == Key.Up)
            {
                txtCantidad.Text = (++cant).ToString();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                txtCantidad.Text = (--cant).ToString();
                e.Handled = true;
            }
            ActualizarTotal();
        }
        private void txtPrecio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtPrecio.Text, out decimal nuevoPrecio))
            {
                bool precioValido = true;

                
                if (_monedaSeleccionada == 1) 
                {
                   
                    if (nuevoPrecio < ProductoEditado.PrecioBaseSoles)
                        precioValido = false;
                }
                else if (_monedaSeleccionada == 2) 
                {
                    if (nuevoPrecio < ProductoEditado.PrecioBaseDolares)
                        precioValido = false;
                }

                txtPrecio.Foreground = precioValido
                    ? (SolidColorBrush)Application.Current.FindResource("Text")
                    : Brushes.Red;
            }
            ActualizarTotal();
        }
        private void txtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                txtCantidad.Text = "1";
                txtCantidad.CaretIndex = txtCantidad.Text.Length;
                ActualizarTotal();
                return;
            }


            if (double.TryParse(txtCantidad.Text, out double cantidad_actual))
            {
                if (cantidad_actual < 1)
                {
                    txtCantidad.Text = "1";
                    txtCantidad.CaretIndex = txtCantidad.Text.Length;
                }
            }
        }
    }
}

