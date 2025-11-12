using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Lógica de interacción para FRMUnidadDeMedida.xaml
    /// </summary>
    public partial class FRMUnidadDeMedida : Window
    {
        private List<TablaUMD> UMDs { get; set; } = new List<TablaUMD>();

        private Tabart _producto;

        public TablaUMD precio_seleccionado;

        public decimal precio;
        public FRMUnidadDeMedida(List<Tabpr_art> tabpr_Arts, Tabart articulo)
        {
            InitializeComponent();
            this.DataContext = this;
            this._producto = articulo;

            //this.productoBase = producto;

            foreach (var umd in tabpr_Arts) 
            {
                var nuevaUmd = new TablaUMD
                {
                    Codigo = umd.coduni,
                    Desuni = umd.desuni,
                    EntidadOriginal = umd
                };

                if (umd.tipmon == 1) // Los precios originales  SOLES
                {
                    nuevaUmd.precioSOLES = umd.precio;
                    nuevaUmd.precio_mayorSOLES = umd.precio_mayor;
                    nuevaUmd.precio_minSOLES = umd.precio_min;
                    nuevaUmd.precio_paseSOLES = umd.precio_pase;
                    nuevaUmd.precio_pase1SOLES = umd.precio_pase1;

                    nuevaUmd.precioDOLARES = umd.precio / sisVariables.Gtipcam;
                    nuevaUmd.precio_mayorDOLARES = umd.precio_mayor / sisVariables.Gtipcam;
                    nuevaUmd.precio_minDOLARES = umd.precio_min / sisVariables.Gtipcam;
                    nuevaUmd.precio_paseDOLARES = umd.precio_pase / sisVariables.Gtipcam;
                    nuevaUmd.precio_pase1DOLARES = umd.precio_pase1 / sisVariables.Gtipcam;
                }
                else if (umd.tipmon == 2) // Los precios originales DOLARES
                {
                    nuevaUmd.precioDOLARES = umd.precio;
                    nuevaUmd.precio_mayorDOLARES = umd.precio_mayor;
                    nuevaUmd.precio_minDOLARES = umd.precio_min;
                    nuevaUmd.precio_paseDOLARES = umd.precio_pase;
                    nuevaUmd.precio_pase1DOLARES = umd.precio_pase1;

                    nuevaUmd.precioSOLES = umd.precio * sisVariables.Gtipcam;
                    nuevaUmd.precio_mayorSOLES = umd.precio_mayor * sisVariables.Gtipcam;
                    nuevaUmd.precio_minSOLES = umd.precio_min * sisVariables.Gtipcam;
                    nuevaUmd.precio_paseSOLES = umd.precio_pase * sisVariables.Gtipcam;
                    nuevaUmd.precio_pase1SOLES = umd.precio_pase1 * sisVariables.Gtipcam;
                }
                UMDs.Add(nuevaUmd);
            }

            DGUMD.ItemsSource = UMDs;
            DGUMD.Focus();
        }

        private void Click_Columna(object sender, MouseButtonEventArgs e)
        {
            DataGridCell clickedCell = EncontrarPadre<DataGridCell>(e.OriginalSource as DependencyObject);

            if (clickedCell != null)
            {
                DataGridColumn column = clickedCell.Column;
                int index = DGUMD.Columns.IndexOf(column);

                var umdSeleccionada = DGUMD.SelectedItem as TablaUMD;

                decimal _precio;

                switch (index)
                {
                    case 2:
                    case 3:
                        _precio = umdSeleccionada.precioSOLES;
                        break;
                    case 4:
                    case 5:
                        _precio = umdSeleccionada.precio_mayorSOLES;
                        break;
                    case 6:
                    case 7:
                        _precio = umdSeleccionada.precio_minSOLES;
                        break;
                    case 8:
                    case 9:
                        _precio = umdSeleccionada.precio_paseSOLES;
                        break;
                    case 10:
                    case 11:
                        _precio = umdSeleccionada.precio_pase1SOLES;
                        break;
                    default:
                        _precio = 0; // valor por defecto
                        break;
                }

                if (umdSeleccionada != null)
                {
                    GestionarSeleccionUMD(umdSeleccionada, _precio);
                } 
            }
        }
        public static T EncontrarPadre<T>(DependencyObject elementoHijo) where T : DependencyObject
        {
            // Obtener el padre visual
            DependencyObject padre = VisualTreeHelper.GetParent(elementoHijo);

            while (padre != null && !(padre is T))
            {
                padre = VisualTreeHelper.GetParent(padre);
            }
            return padre as T;
        }        
        private void GestionarSeleccionUMD(TablaUMD umdSeleccionada, decimal precioSeleccionado)
        {
            var producto = new Producto()
            {
                Codigo = _producto.codart,
                Descripcion = _producto.descri,
                Cant = 1,
                Unit = precioSeleccionado,
                PrecioOriginal = precioSeleccionado,
                D = 0,
                UMD = umdSeleccionada.Desuni,
                Codart = _producto.codart,
                TipMonOriginal = _producto.tipmon
            };

            this.DialogResult = true;
            precio_seleccionado = DGUMD.SelectedItem as TablaUMD;
            precio = precioSeleccionado;
            this.Close();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.F1 || e.Key > Key.F5)
                return;

            if (DGUMD.SelectedItem == null) return;

            e.Handled = true;

            TablaUMD fila = (TablaUMD)DGUMD.SelectedItem;

            decimal valor;

            switch (e.Key)
            {
                case Key.F1:
                    valor = fila.precioSOLES;
                    break;
                case Key.F2:
                    valor = fila.precio_mayorSOLES;
                    break;
                case Key.F3:
                    valor = fila.precio_minSOLES;
                    break;
                case Key.F4:
                    valor = fila.precio_paseSOLES;
                    break;
                case Key.F5:
                    valor = fila.precio_pase1SOLES;
                    break;
                default:
                    valor = 0m;
                    break;
            }


            GestionarSeleccionUMD(fila, valor);
        }

        private void DgUMDFocus(object sender, RoutedEventArgs e)
        {
            if (DGUMD.Items.Count > 0)
            {
                DGUMD.SelectedIndex = 0;
                DGUMD.Focus();
                DGUMD.ScrollIntoView(DGUMD.SelectedItem);
            }
        }
    }
}
