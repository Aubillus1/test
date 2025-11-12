using FontAwesome.WPF;
using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IOPath = System.IO.Path;
using ShapesPath = System.Windows.Shapes.Path;

namespace HsPvSerAPG.Vista
{
    /// <summary>
    /// Lógica de interacción para BusqProducto.xaml
    /// </summary>
    public partial class BusqProducto : Window
    {
        private PuntoDeVenta ventanamenu;

        private List<Tabart> tabart = new List<Tabart>();
        private TabBolController tabbolController = new TabBolController();
        //int codart = 0;
        public BusqProducto(PuntoDeVenta puntodeventa, string articulo = null)
        {
            InitializeComponent();
            ventanamenu = puntodeventa;
            if (articulo != null)
            {
                txtCriterio.Text = articulo;
                BtnBuscar_Click(BtnBuscar, new RoutedEventArgs());
            }
        }

        private void DGProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProcesarProductoSeleccionado();
        }
        private void Btn_Click_Salir(object sender, RoutedEventArgs e)
        {
            // Cierra sin agregar producto, dialog result es false o null
            this.DialogResult = false;
            this.Close();
        }
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cargando.Visibility = Visibility.Visible;
                string criterio = txtCriterio.Text.Trim().ToLower();

                var tabartController = new TabartController();
                tabart = await Task.Run(() => tabartController.selectTabart(1, ""));

                var tabartFiltrado = tabart.Where(p => string.IsNullOrEmpty(criterio) ||
                       p.codfab.ToLower().Contains(criterio) ||
                       p.descri.ToLower().Contains(criterio))
            .ToList();

                var productos = new List<Producto>();

                tabart = tabartFiltrado;
                foreach (var producto in tabart)
                {
                    if (!string.IsNullOrEmpty(criterio) &&
                        !(producto.codfab.ToLower().Contains(criterio) || producto.descri.ToLower().Contains(criterio)))
                    {
                        continue;
                    }

                    productos.Add(new Producto
                    {
                        Codigo = producto.codart,
                        Descripcion = producto.descri,
                        Cant = 1,
                        Unit = 0, // precio se obtiene luego
                        PrecioOriginal = 0,
                        D = 0,
                        CodFab = producto.codfab,
                        Codart = producto.codart
                    });
                }

                DGProductos.ItemsSource = productos;

                if (productos.Count > 0)
                {
                    DGProductos.SelectedIndex = 0;
                    DGProductos.Focus();
                    var item = DGProductos.SelectedItem;
                    if (item != null)
                    {
                        DGProductos.ScrollIntoView(item);
                    }
                }

            }
            finally
            {
                Cargando.Visibility = Visibility.Collapsed;
            }
        }
        private void txtCriterio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                BtnBuscar_Click(null, null);
            }
        }
        private async void ProcesarProductoSeleccionado()
        {
            var producto_seleccionado = DGProductos.SelectedItem as Producto;
            if (producto_seleccionado == null)
                return;


            var productoParaProcesar = producto_seleccionado;

            // Ejecutar el resto del flujo después de cerrar la ventana
            await Dispatcher.BeginInvoke(new Action(async() =>
            {
                var tabartController = new TabartController();
                var lista_de_productos = tabartController.selectTabart(sisVariables.GCia, "");

                var productoOriginal = lista_de_productos.FirstOrDefault(p =>
                    p.codfab == productoParaProcesar.Codigo || p.descri == productoParaProcesar.Descripcion);

                if (productoOriginal == null)
                {
                    MessageBox.Show("No se pudo encontrar el producto original.");
                    return;
                }

                var tabpr_artController = new Tabpr_artController();
                var lista_de_precio = tabpr_artController.preciosPorArticulo(sisVariables.GCia, productoOriginal.codart);

                if (ventanamenu.DesplazarDetallesdeSubproducto(lista_de_precio, productoOriginal))
                    this.Close();

            }));
        }
        private void DGProductos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true; // evita que el DataGrid cambie la fila
                if (DGProductos.SelectedItem != null)
                {
                    ProcesarProductoSeleccionado();
                }
            }
        }
        private static string BuildArchivoName(int cia, string codart, string extension)
        {
            string ciaStr = cia.ToString();
            string codartStr = codart?.Trim() ?? "000000";   

            string ext = (extension ?? "").Trim().TrimStart('.').ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = "jpg";

            return $"{ciaStr}{codartStr}.{ext}";
        }

        private async void BtnSubirImagen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Identificamos el producto de la fila
                var button = sender as Button;
                var producto = button?.DataContext as Producto;
                if (producto == null)
                {
                    MessageBox.Show("No se pudo obtener el artículo de la fila.");
                    return;
                }


                var tabartController = new TabartController();
                var lista_de_productos = tabartController.selectTabart(sisVariables.GCia, "");

                var productoOriginal = lista_de_productos.FirstOrDefault(p =>
                    p.codfab == producto.Codigo || p.descri == producto.Descripcion);

                if (productoOriginal == null)
                {
                    MessageBox.Show("No se encontró el producto en la BD.");
                    return;
                }

                string codart = productoOriginal.codart; 


                var ofd = new OpenFileDialog
                {
                    Filter = "Archivos de imagen (*.*)|*.*"
                };

                if (ofd.ShowDialog() != true) return;

                string rutaImagen = ofd.FileName;

                byte[] bytesImagen = File.ReadAllBytes(rutaImagen);
                string imagenBase64 = Convert.ToBase64String(bytesImagen);

 
                string extension = IOPath.GetExtension(rutaImagen).TrimStart('.').ToLowerInvariant();

                int cia = sisVariables.GCia;
                

                string archivoName = BuildArchivoName(cia, codart, extension);


                bool ok = await tabartController.SubirImagenTabartAsync(cia, codart.ToString(), imagenBase64, extension);

                if (ok)
                {

                    bool updok = await tabartController.updateTabartFormato(cia, codart.ToString(), extension);

                    if (updok)
                    {
                        MessageBox.Show($"Imagen guardada correctamente");

                        BtnBuscar_Click(null, new RoutedEventArgs());

                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar formato de la imagen");

                    }
                }

                else
                    MessageBox.Show("Error al guardar la imagen");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
           


        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext;
            int index = DGProductos.Items.IndexOf(item);

            // Buscar el Icon dentro del botón
            var icon = button?.Content as ImageAwesome;
            if (icon != null)
            {
                if (icon.Icon == FontAwesomeIcon.Eye)
                {
                    icon.Icon = FontAwesomeIcon.EyeSlash;
                    //icon.Foreground = Brushes.White;
                    //button.Background = Brushes.Black;
                    //button.BorderBrush = Brushes.White;

                    // LOGICA PARA MOSTRAR LA IMAGEN
                    MostrarImagen mostrarimagen = new MostrarImagen(tabart[index].codart, tabart[index].descri, tabart[index].formato);
                    mostrarimagen.ShowDialog();

                    icon.Icon = FontAwesomeIcon.Eye;
                }
            }
        }

        private async void DGProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg && dg.SelectedItem is Producto producto)
            {
                try
                {
                    CargandoStock.Visibility = Visibility.Visible;
                    var lista_ticket = await Task.Run(() => tabbolController.selectSaldosByCodart(sisVariables.GCia, producto.codart));

                    //var lista_ticket = tabbolController.selectSaldosByCodart(sisVariables.GCia, producto.Codart);

                    DGstock.ItemsSource = lista_ticket;
                }
                finally
                {
                    CargandoStock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}