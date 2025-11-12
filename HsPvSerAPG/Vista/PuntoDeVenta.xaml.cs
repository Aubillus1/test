using FontAwesome.WPF;
using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Utils.Clases;
using HsPvSerAPG.Vista;
using HsPvSerAPG.Vista.CajaChica;
using HsPvSerAPG.Vista.Reem_Caja;
using HsPvSerAPG.Vista.PanelProductos;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using static System.Windows.Forms.DataFormats;
using System.Collections.Generic;
using System.Linq;
using System;

namespace HsPvSerAPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PuntoDeVenta : Window
    {
        private TabgrupController tabgrupController = new TabgrupController();
        private List<Tabgrup> todoslosproductos = new List<Tabgrup>();

        private TabartController tabartController = new TabartController();
        public List<Tabart> todoslossubproductos = new List<Tabart>();
        private List<Tabart> Subproductosfiltrados = new List<Tabart>();
        private int paginaActual = 0;
        private int cantelementos = 18;
        private VersionActualController VerificarNuevaVersionAsync = new VersionActualController();

        private CliproController Clipro = new CliproController();
        private TabcamController tabcamController = new TabcamController();


        private List<Tabpr_art> lista_de_precios = new List<Tabpr_art>();

        private bool productocargado, subproductocargado = false;

        public bool ContinuarAnimacionDetalle = false;

        public bool ContinuarProcesoDetalle = true;
       

        private int monedaSeleccionada = 1;

        public ObservableCollection<Producto> productos { get; } = new ObservableCollection<Producto>();

        string Tabcam = "0.00";
        double cambio = 0;
        public void ActualizarTotalesUI()
        {
            decimal precioVenta = productos.Sum(p => p.Total);
            decimal igv = precioVenta * 0.18m / 1.18m; // si el precio ya incluye IGV
            decimal valorVenta = precioVenta - igv;
            decimal totalCantidad = productos.Sum(p => p.Cant);
            decimal valorbolsas = Convert.ToDecimal(TB_BPlastic.Text);
            decimal totalVenta = precioVenta + valorbolsas;
            //  Obtenemos el tipo de cambio desde el TextBlock TxtTabcam
            decimal tipoCambio = 1;
            decimal.TryParse(Tabcam, out tipoCambio);

            //  Conversión si está en dólares (monedaSeleccionada == 2)
            if (monedaSeleccionada == 2 && tipoCambio > 0)
            {
                precioVenta /= tipoCambio;
                totalVenta /= tipoCambio;
                igv /= tipoCambio;
                valorVenta /= tipoCambio;
                valorbolsas /= tipoCambio;
            }

            // Asignar a los TextBox
            TBValVenta.Text = valorVenta.ToString("F2");
            TBIGV.Text = igv.ToString("F2");
            TBPrcVenta.Text = precioVenta.ToString("F2");
            TBTotCanti.Text = totalCantidad.ToString();
            TBTotVenta.Text = totalVenta.ToString("F2");
            TB_BPlastic.Text = valorbolsas.ToString("F2");
        }

        public PuntoDeVenta(string compania, string caja)
        {
            InitializeComponent();

            document_number.Text = "00000000";
            TxtB_Cliente.Text = "CLIENTE";
            TxtBCorporacion.Text = compania;
            TxtBTurno.Text += caja;
            TxtBVersion.Text += sisVariables.sisVersion;
            TxtBUsuario.Text += sisVariables.GUsuario;

            this.DataContext = this;

            EstablecerHorario();
            DGTabla.ItemsSource = productos;
            ObtenerProductosDeTabla();
            document_number.Focus();
            document_number.SelectAll();
             
            //if (HsPvSerAPG.Properties.Settings.Default.IsDarkTheme)
            //{
            //    ApplyTheme("TemaOscuro.xaml");
            //    SwitchTema.IsChecked = true;
            //}
            //else
            //{
            //    ApplyTheme("TemaClaro.xaml");
            //    SwitchTema.IsChecked = false;
            //}
            
        }
        private async Task CargarMonedasAsync()
        {
            try
            {
                var controller = new Tabmon();
                var monedas = await tabartController.selectTabMonAsync();

                if (monedas == null || monedas.Count == 0)
                {
                    MessageBox.Show("No se encontraron monedas.");
                    return;
                }

                CmbMoneda.ItemsSource = monedas;
                CmbMoneda.DisplayMemberPath = "des";   // Lo que se muestra
                CmbMoneda.SelectedValuePath = "cod";
                CmbMoneda.SelectedIndex = 0;
                // El valor que se usa internamente
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando monedas: {ex.Message}");
            }
        }
        private async void CmbMoneda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CmbMoneda.SelectedItem is Tabmon selectedMoneda)
                {
                    monedaSeleccionada = selectedMoneda.cod;

                    var tabcam = await Task.Run(() => tabcamController.SelectTabcam());

                    if (tabcam == null || tabcam.parale <= 0)
                    {
                        MessageBox.Show("⚠️ No se pudo obtener un tipo de cambio válido.");
                        return;
                    }

                    sisVariables.Gtipmon = monedaSeleccionada;
                    sisVariables.Gtipcam = Math.Round(tabcam.parale, 3, MidpointRounding.AwayFromZero);


                    tabcamController.SetMoneda(monedaSeleccionada);

                    foreach (var prod in productos)
                    {
                        if (monedaSeleccionada == 1) // Soles
                        {
                            if (monedaSeleccionada == prod.TipMonOriginal && prod.Unit == prod.PrecioBaseSoles)
                            {
                                prod.UnitSoles = prod.PrecioBaseSoles;
                                prod.Unit = prod.PrecioBaseSoles;
                            }
                            else
                            {
                                prod.UnitSoles = prod.UnitDolares * sisVariables.Gtipcam;
                                prod.Unit = prod.UnitSoles;
                            }
                            prod.MonedaEditada = 1;
                        }
                        else if (monedaSeleccionada == 2) // Dólares
                        {
                            if (monedaSeleccionada == prod.TipMonOriginal && prod.Unit == prod.PrecioBaseDolares)
                            {
                                prod.UnitDolares = prod.PrecioBaseDolares;
                                prod.Unit = prod.PrecioBaseDolares;
                            }
                            else
                            {
                                prod.UnitDolares = prod.UnitSoles / sisVariables.Gtipcam;
                                prod.Unit = prod.UnitDolares;
                            }
                                prod.MonedaEditada = 2;
                        }
                        prod.OnPropertyChanged(nameof(prod.Unit));
                    }

                    tabcamController.CambiarMonedaPorSeleccion(productos, monedaSeleccionada, tabcam.parale);
                    ActualizarTotalesUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Error al cambiar moneda: {ex.Message}");
            }
        }


        private void HabyInhabProductos(bool @switch)
        {
            if (@switch)
            {
                BtnProductos.IsEnabled = false;
                UGProductos.IsEnabled = false;
            }
            else
            {
                BtnProductos.IsEnabled = true;
                UGProductos.IsEnabled = true;
            }

        }

        private async void MostrarProductos(List<Tabgrup> productos)
        {
            UGProductos.ItemsSource = null;
            UGProductos.ItemsSource = productos;
        }

        private async Task CargarProductos()
        {
            if (todoslosproductos == null || !todoslosproductos.Any())
            {
                todoslosproductos = tabgrupController.selectTabgroup(sisVariables.GCia);
            }

            // Siempre mostramos, pero no volvemos a pedir a la BD
            MostrarProductos(todoslosproductos);

            var listaProductos = new List<Producto>();
            foreach (var item in DGTabla.Items)
            {
                if (item is Producto p)
                    listaProductos.Add(p);
            }
        }

        private void KeyDown_Productos(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                BuscarProducto();
        }

        private List<Producto> ObtenerProductosDeTabla()
        {
            var listaProductos = new List<Producto>();
            foreach (var item in DGTabla.Items)
            {
                if (item is Producto p)
                    listaProductos.Add(p);
            }
            return listaProductos;
        }


        private void CerrarSubPanel_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)this.Resources["OcultarSubPanel"];
            sb.Completed += (s, ev) => 
            {
                UGSubProductos.Visibility = Visibility.Collapsed;
                SubPanel.Visibility = Visibility.Collapsed;
                TBPagina.Text = "0/0";
            };
            sb.Begin();
            HabyInhabProductos(false);
        }

        private async void EstablecerHorario()
        {
            while (true)
            {
                TxtBFecha_Hora.Text = $"FECHA / HORA: {DateTime.Now.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("HH:mm:ss")}";
                await Task.Delay(1000);
            }
        }

        private async Task<int> ObtenerCodigoClientePorDocumentoAsync(string numeroDocumento)
        {
            try
            {
                if (string.IsNullOrEmpty(numeroDocumento))
                {
                    MessageBox.Show("Número de documento no válido", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 0;
                }

                var clientes = Clipro.SelectClipro(numeroDocumento, 1); 

                if (clientes == null || clientes.Count == 0)
                {
                    Console.WriteLine($"No se encontraron clientes con documento: {numeroDocumento}");
                    MessageBox.Show($"No se encontró cliente con documento: {numeroDocumento}", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 0;
                }
                var clienteExacto = clientes.FirstOrDefault(c =>
                    c.nrodoc != null && c.nrodoc.Equals(numeroDocumento, StringComparison.OrdinalIgnoreCase));

                if (clienteExacto != null)
                {
                    return clienteExacto.codigo;
                }

                return clientes[0].codigo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener código de cliente: {ex.Message}");
                MessageBox.Show($"Error al buscar cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        private async void Click_DocVenta(object sender, RoutedEventArgs e)
        {
            if (TBTotVenta.Text == "0.00")
            {
                MessageBox.Show("Debes ingresar al menos una valor al articulo/anticipo", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (document_number.Text == "" || document_number.Text.Length < 8 || document_number.Text.Length > 9 && document_number.Text.Length < 11)
            {
                MessageBox.Show("Debes ingresar un número de documento válido en [Nro. Doc.] para acceder", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nroDoc = document_number.Text.Trim();
            string totalVenta = TBTotVenta.Text.Trim();
            string totalProductos = TBTotCanti.Text.Trim();
            string cliente = TxtB_Cliente.Text;
            string telefono = txtTelefono.Text;
            string direccion = TxtB_Direccion.Text;
            int st_anti = sisVariables.anticipo;
            int stentregado = sisVariables.stentregado;

            double igv = double.Parse(TBIGV.Text);

            int cantuni = int.Parse(TBTotCanti.Text);

            var listaProductos = new List<Producto>();
            foreach (var item in DGTabla.Items)
            {
                if (item is Producto p)
                    listaProductos.Add(p);
            }

            int codigoCliente = await ObtenerCodigoClientePorDocumentoAsync(nroDoc);

            if (codigoCliente == 0)
            {
              
                var result = MessageBox.Show($"No se encontró cliente con documento: {nroDoc}. ¿Desea continuar con cliente genérico?",
                                           "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            sisVariables.Gnrodoc = codigoCliente;

            
            Emision cobranza = new Emision(listaProductos, totalVenta, totalProductos, cliente, telefono, direccion, nroDoc, codigoCliente, cantuni, igv, st_anti, stentregado);
            cobranza.ShowDialog();



            BtnLimpiar_Click(BtnDocVenta, new RoutedEventArgs());
        }
        private void TxtB_Documento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                string nrodoc = document_number.Text.Trim();

                if (nrodoc.Length < 3)
                {
                    MessageBox.Show("Debe ingresar al menos 3 números.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Salir sin hacer más procesos
                }

                if (!string.IsNullOrEmpty(nrodoc))
                {
                    var controller = new CliproController();
                    var clipros = controller.SelectClipro(nrodoc, sisVariables.GCia);

                    if (clipros.Count == 1)
                    {
                        Clipro clipro = clipros.FirstOrDefault();

                        document_number.Text = clipro.nrodoc;
                        TxtB_Cliente.Text = clipro.razsoc;
                        TxtB_Direccion.Text = clipro.direcc;
                        txtTelefono.Text = clipro.telefo;
                        //TBCelular.Text = clipro.telefo;

                    }
                    else
                    {
                        if (clipros.Count > 1)
                        {
                            BuscarCliente ventanaCliente = new BuscarCliente(document_number.Text);
                            ventanaCliente.ShowDialog();
                            if (ventanaCliente.DialogResult == true)
                            {
                                document_number.Text = ventanaCliente.cliente.nrodoc;
                                TxtB_Cliente.Text = ventanaCliente.cliente.razsoc;
                                TxtB_Direccion.Text = ventanaCliente.cliente.direcc;
                                txtTelefono.Text = ventanaCliente.cliente.telefo;
                                TBCelular.Text = ventanaCliente.cliente.celular;
                                TBCorreo.Text = ventanaCliente.cliente.correo;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cliente no encontrado", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                            TxtB_Cliente.Text = "";
                            TxtB_Direccion.Text = "";

                            // Mostrar formulario según la longitud del documento
                            if (nrodoc.Length == 11)
                            {
                                ConsultaRUC ventanaConsulta = new ConsultaRUC();
                                ventanaConsulta.txtRUC.Text = nrodoc;
                                ventanaConsulta.ShowDialog();
                            }
                            else if (nrodoc.Length == 8 || nrodoc.Length == 9 || nrodoc.Length == 12)
                            {
                                ConsultaDNI ventanaConsulta = new ConsultaDNI();
                                ventanaConsulta.document_number.Text = nrodoc;
                                ventanaConsulta.ShowDialog();
                            }
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Ingrese un número de documento válido", "Formato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }

        private async void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            var columna = ContenendorSecundario.ColumnDefinitions[1];
            if (columna.Width.Value == 0)
            {
                TBArticulo.Focus();
                PanelDerecho.Visibility = Visibility.Visible;
                columna.Width = GridLength.Auto;
                var storyboard = (Storyboard)FindResource("MostrarPanelDerecho");
                storyboard.Begin();

                if (UGProductos.Items.Count == 0)
                {
                    try
                    {
                        EfectoCargarProductos.Visibility = Visibility.Visible;
                        await CargarProductos();
                    }
                    finally
                    {
                        EfectoCargarProductos.Visibility = Visibility.Collapsed;
                    } 
                }
                else
                    await CargarProductos();
            }
            else
            {
                var storyboard = (Storyboard)FindResource("OcultarPanelDerecho");
                storyboard.Begin();
                if (false)
                {
                    var mostrardatos = (Storyboard)this.Resources["MostrarDatosEnCasodeResolucionMenor"];
                    mostrardatos.Completed += (s, ec) => ColumnaDatos.Visibility = Visibility.Visible;
                    mostrardatos.Begin(); 
                }
                PanelDerecho.Visibility = Visibility.Collapsed;
                columna.Width = new GridLength(0);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e) 
        {
            var result = MessageBox.Show("¿Seguro que quieres salir?", "Confirmar salida", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Click_BtnClientes(object sender, RoutedEventArgs e)
        {
            BuscarCliente buscarCliente = new BuscarCliente(document_number.Text);
            buscarCliente.ShowDialog();
            if (buscarCliente.DialogResult == true)
            {
                document_number.Text = buscarCliente.cliente.nrodoc;
                TxtB_Cliente.Text = buscarCliente.cliente.razsoc;
                TxtB_Direccion.Text = buscarCliente.cliente.direcc;
                txtTelefono.Text = buscarCliente.cliente.telefo;
                TBCelular.Text = buscarCliente.cliente.celular;
                TBCorreo.Text = buscarCliente.cliente.correo;
            }
        }
        private void BuscarProducto()
        {
            string articuloFiltro = TBArticulo.Text.Trim();

            var filtrados = todoslosproductos.Where(p =>
                string.IsNullOrEmpty(articuloFiltro) ||
                (p.des != null && p.des.ToString().IndexOf(articuloFiltro, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (p.cod != null && p.cod.ToString().IndexOf(articuloFiltro, StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();

            MostrarProductos(filtrados);
        }


        private void BtnBuscarProducto(object sender, RoutedEventArgs e) => BuscarProducto();

        private async void BuscarSubProducto()
        {
            string productoFiltro = TBProducto.Text.Trim();

            var filtrados = todoslossubproductos
                .Where(p =>
                    string.IsNullOrEmpty(productoFiltro)
                    || (p.descri != null
                        && p.descri.ToString().IndexOf(productoFiltro, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (p.codart != null
                        && p.codart.IndexOf(productoFiltro, StringComparison.OrdinalIgnoreCase) >= 0)
                )
                .ToList();

            await MostrarSubProductos(filtrados, true);
        }
        private void BtnBuscarSubProducto(object sender, RoutedEventArgs e) => BuscarSubProducto();

        private void KeyDown_BuscarSubProductos(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                BuscarSubProducto();
        }

        //Calcula la altura de la pantalla para el datagrid
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarMonedasAsync();
            
            AjustarResolucionTabla();
            var tabcamController = new TabcamController();
            var tabcam = tabcamController.SelectTabcam();
            cambio = Convert.ToDouble(tabcam.parale.ToString("F2"));
            if (tabcam != null)
                // Mostrar solo el valor de la columna PARALLE
                TxtBTipoCambio.Text = $"Tipo de Cambio: {tabcam.parale.ToString("F2")}";
            else
                TxtBTipoCambio.Text = $"Tipo de Cambio: {0.00}";

        }

        private void AjustarResolucionTabla()
        {
            double alturaVentana = SystemParameters.PrimaryScreenHeight;
            double controles_encima = SPFila1.ActualHeight;
            double totales = GFilaTotales.ActualHeight + GFilaSubtotales.ActualHeight;
            double alturaSuperior = SPBotones.ActualHeight;
            double alturaInferior = GDatosInferiores.ActualHeight;
            double margen = 50;

            double alturaDisponible = alturaVentana - (alturaSuperior + alturaInferior + totales + margen + controles_encima);

            DGTabla.Height = alturaDisponible;
        }
    
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Producto producto)
            {
                producto.Cant++; // esto actualizará Total y notificará cambios
                ActualizarTotalesUI();
            }
        }

        private void BtnRestar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Producto producto && producto.Cant > 1)
            {
                producto.Cant--;
                ActualizarTotalesUI();
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Producto producto)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"¿Deseas eliminar {producto.Descripcion}?",     // Mensaje
                    "Confirmación",                         // Título
                    MessageBoxButton.YesNo,                 // Botones Sí y No
                    MessageBoxImage.Question                // Icono de pregunta
                );

                // Evaluar la respuesta del usuario
                if (result == MessageBoxResult.Yes)
                {
                    productos.Remove(producto); // elimina de la colección
                    ActualizarTotalesUI(); // actualiza totales después de eliminar
                }
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var productoSeleccionado = DGTabla.SelectedItem as Producto;
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Ocurrió un error inesperado (no es posible editar).");
                return;
            }
            var precioCtrl = new Tabpr_artController();
            var articulo = tabartController.selectTabart(sisVariables.GCia, productoSeleccionado.descripcion);
            if (precioCtrl.preciosPorArticulo(sisVariables.GCia, productoSeleccionado.Codart) == null)
            {
                DesplazarDetallesdeSubproducto(lista_de_precios, articulo[0]);
            }

            decimal precioOriginal = productoSeleccionado.TipMonOriginal == 1
                ? productoSeleccionado.PrecioBaseSoles
                : productoSeleccionado.PrecioBaseDolares;

            var frmCantidad = new FRMcantidad(productoSeleccionado, precioOriginal, productoSeleccionado.TipMonOriginal);

            if (frmCantidad.ShowDialog() != true) return;

            var productoEditado = frmCantidad.ProductoEditado;

            // Actualizamos valores editados
            productoSeleccionado.Cant = productoEditado.Cant;
            productoSeleccionado.UnitSoles = productoEditado.UnitSoles;
            productoSeleccionado.UnitDolares = productoEditado.UnitDolares;
            productoSeleccionado.TipMonOriginal = productoEditado.TipMonOriginal;

            productoSeleccionado.Unit = sisVariables.Gtipmon == 1
                ? productoSeleccionado.UnitSoles
                : productoSeleccionado.UnitDolares;

            // Notificar cambios
            productoSeleccionado.OnPropertyChanged(nameof(productoSeleccionado.Cant));
            productoSeleccionado.OnPropertyChanged(nameof(productoSeleccionado.Unit));
            productoSeleccionado.OnPropertyChanged(nameof(productoSeleccionado.UnitSoles));
            productoSeleccionado.OnPropertyChanged(nameof(productoSeleccionado.UnitDolares));

            ActualizarTotalesUI();
            DGTabla.Items.Refresh();
        }

        private void Window_Changed(object sender, SizeChangedEventArgs e) => AjustarResolucionTabla();

        private void BtnVentanaProductos_Click(object sender, RoutedEventArgs e)
        {
            BusqProducto ventana_producto = new BusqProducto(this);
            ventana_producto.ShowDialog();
        }

        public void AgregarFila(Producto producto)
        {
            // Buscar producto con mismo código, misma UMD y mismo precio
            var existente = productos.FirstOrDefault(p =>
                p.Codigo == producto.Codigo &&
                p.UMD == producto.UMD &&
                Math.Round(p.UnitSoles, 6) == Math.Round(producto.UnitSoles, 6) &&
                Math.Round(p.UnitDolares, 6) == Math.Round(producto.UnitDolares, 6)
            );

            if (existente != null)
            {
                // Existe: sumar cantidad
                existente.Cant += producto.Cant;
            }
            else
            {
                // Precio diferente: agregar nueva fila
                productos.Add(producto);
            }
        }
        public void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "¿Deseas limpiar los articulos?",
                "Confirmación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.No)
                return;

            if (DGTabla.Items.Count > 0)
            {
                try
                {
                    productos.Clear();
                    ActualizarTotalesUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                MessageBox.Show("No hay productos en la tabla para limpiar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private async void Click_BtnReenviar(object sender, RoutedEventArgs e)
        {

            if (document_number.Text == "" || document_number.Text.Length < 8 || document_number.Text.Length > 9 && document_number.Text.Length < 11)
            {
                MessageBox.Show("Debes ingresar un número de documento válido en [Nro. Doc.] para acceder", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nroDoc = document_number.Text.Trim();
            string totalVenta = TBTotVenta.Text.Trim();
            string totalProductos = TBTotCanti.Text.Trim();
            string cliente = TxtB_Cliente.Text;
            string telefono = txtTelefono.Text;
            string direccion = TxtB_Direccion.Text;

            double igv = double.Parse(TBIGV.Text);

            int cantuni = int.Parse(TBTotCanti.Text);

            // Extrae productos del DataGrid
           

            int codigoCliente = await ObtenerCodigoClientePorDocumentoAsync(nroDoc);

            if (codigoCliente == 0)
            {

                var result = MessageBox.Show($"No se encontró cliente con documento: {nroDoc}. ¿Desea continuar con cliente genérico?",
                                           "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            sisVariables.Gnrodoc = codigoCliente;

            Cobranza reenviar = new Cobranza( totalVenta, totalProductos, cliente, telefono, direccion, nroDoc, codigoCliente, cantuni, igv);
            reenviar.ShowDialog();
            if (DGTabla.Items.Count > 0)
            {
                try
                {
                    productos.Clear();
                    ActualizarTotalesUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    throw;
                }
            }
        }

        private void BtnCajaChica(object sender, RoutedEventArgs e)
        {
            Opciones caja_chica = new Opciones();
            caja_chica.ShowDialog();
        }

        private void Click_ReemCaja(object sender, RoutedEventArgs e)
        {
            Resumen resumen = new Resumen();
            resumen.ShowDialog();
        }

        private async void Click_BtnGaveta(object sender, RoutedEventArgs e)
        {
            if (Gaveta.Width == 0)
            {
                var sb = (Storyboard)FindResource("MostrarGaveta");
                sb.Begin();
                PBGavetaClave.Focus();
            } 
            else
            {
                var sb = (Storyboard)FindResource("OcultarGaveta");
                sb.Begin();
                await Task.Delay(300);
                PBGavetaClave.Clear();
            }
        }

        private void KeyDown_PBGavetaClave(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (PBGavetaClave.Password == "12345")
                {
                    MessageBox.Show("Gaveta Abierta");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c start notepad", // "/c" ejecuta y termina, "start notepad" abre Notepad
                        RedirectStandardOutput = false,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                }
                else
                    MessageBox.Show("Contraseña incorrecta, intentelo de nuevo. . .");
               
            }
        }

        private async void txBcodart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                string codartIngresado = txBcodart.Text.Trim();

                if (string.IsNullOrWhiteSpace(codartIngresado))
                {
                    MessageBox.Show("Debe ingresar un código de artículo.");
                    return;
                }
                //En caso sea un articulo el que ha ingresado
                if (codartIngresado.Any(char.IsDigit))
                {
                    int cia = sisVariables.GCia;
                    int codgru = 0;
                    string criterio = codartIngresado;
                    int esanti = 0;

                    var tabpr_artController = new Tabpr_artController();
                    var tabartController = new TabartController();

                    // Buscar producto
                    var lista_de_productos = tabartController.selectTabart(cia, criterio, codgru, esanti);
                    var producto = lista_de_productos?.FirstOrDefault();

                    if (producto == null)
                    {
                        MessageBox.Show("Producto no encontrado.");
                        return;
                    }

                    // Obtener precios
                    lista_de_precios = tabpr_artController.preciosPorArticulo(producto.cia, producto.codart);
                    if (lista_de_precios == null || lista_de_precios.Count == 0)
                    {
                        MessageBox.Show("No hay precios disponibles para este producto.");
                        return;
                    }

                    DesplazarDetallesdeSubproducto(lista_de_precios, producto);

                    // Limpiar textbox y volver a enfocar
                    txBcodart.Clear();
                    txBcodart.Focus(); 
                }
                else
                {
                    BusqProducto ventana_producto = new BusqProducto(this, codartIngresado);
                    ventana_producto.ShowDialog();
                }
            }
        }

        private void CerrarDetalleProducto_Click(object sender, RoutedEventArgs e)
        {
            ContinuarAnimacionDetalle = true;
            BtnCerrarSubpanel.IsHitTestVisible = true;
            BtnCerrarSubpanel.Opacity = 1;
        }

        /* private async void ProcesarProductoPorCodigo(int cia, int codgru, string criterio)
         {
             var tabpr_artController = new Tabpr_artController();
             var tabartController = new TabartController();

             var lista_de_productos = tabartController.selectTabart(cia, codgru, criterio);

             // Validar que exista al menos un producto
             var producto = lista_de_productos?.FirstOrDefault();
             if (producto == null)
             {
                 MessageBox.Show("Producto no encontrado.");
                 return;
             }

             // Obtener lista de precios (subproductos)
             lista_de_precios = tabpr_artController.preciosPorArticulo(producto.cia, producto.codart);
             if (lista_de_precios == null || lista_de_precios.Count == 0)
             {
                 MessageBox.Show("No hay precios disponibles para este producto.");
                 return;
             }

             // Caso: Solo un precio
             if (lista_de_precios.Count == 1)
             {
                 AbrirFormularioCantidad(producto, lista_de_precios[0]);
                 return;
             }

             // Caso: Varios precios
             if (posicion_actual_prart < 0)
                 posicion_actual_prart = 0;
             if (posicion_actual_prart >= lista_de_precios.Count)
                 posicion_actual_prart = lista_de_precios.Count - 1;

             await DesplazarDetallesdeSubproducto(lista_de_precios, posicion_actual_prart);

             // 🔹 Ahora, cuando el usuario valide el subproducto seleccionado:
             if (ContinuarProcesoDetalle) // <- aquí controlas el "validar"
             {
                 DGUnidadDeMedida.Items.Clear();
                 var subproductoSeleccionado = lista_de_precios[posicion_actual_prart];
                 AbrirFormularioCantidad(producto, subproductoSeleccionado);
             }

             ContinuarAnimacionDetalle = true;
         }*/

        private void AbrirFormularioCantidad(Tabart producto, Tabpr_art subproductoSeleccionado, decimal precio_seleccionado)
        {
            var prodExistente = this.productos.FirstOrDefault(p =>
            p.Codigo == producto.codfab &&
            p.TipMonOriginal == subproductoSeleccionado.tipmon &&
            p.UnitValorOriginal == subproductoSeleccionado.precio &&
            p.Coduni == subproductoSeleccionado.coduni);

            Producto productoParaCantidad;

            int tipMonOriginal = subproductoSeleccionado.tipmon; 

            if (prodExistente != null)
            {
                productoParaCantidad = new Producto
                {
                    Codigo = prodExistente.Codigo,
                    Descripcion = prodExistente.Descripcion,
                    UMD = subproductoSeleccionado.desuniABR,
                    Cant = prodExistente.Cant,
                    Unit = prodExistente.Unit,
                    PrecioOriginal = prodExistente.PrecioOriginal,
                    D = prodExistente.D,
                    Codart = prodExistente.Codart,
                    UnitSoles = prodExistente.UnitSoles,
                    UnitDolares = prodExistente.UnitDolares,
                    TipMonOriginal = prodExistente.TipMonOriginal,
                    Coduni = prodExistente.coduni
                };
            }
            else
            {
                productoParaCantidad = new Producto
                {
                    Codigo = producto.codart,
                    Descripcion = producto.descri,
                    UMD = subproductoSeleccionado.desuniABR,
                    Cant = 1,
                    Unit = subproductoSeleccionado.precio,
                    PrecioOriginal = subproductoSeleccionado.precio,
                    D = 0,
                    Codart = subproductoSeleccionado.codart,
                    TipMonOriginal = tipMonOriginal,
                    Coduni = subproductoSeleccionado.coduni
                };
            }

            // 🔹 Abrimos formulario con moneda original incluida
            var frmCantidad = new FRMcantidad(productoParaCantidad, precio_seleccionado, tipMonOriginal);

            bool? result = frmCantidad.ShowDialog();

            if (result == true)
            {
                var productoConCantidad = frmCantidad.ProductoEditado;

                // Validación: nunca menor al precio original
                if (productoConCantidad.UnitSoles < subproductoSeleccionado.precio)
                {
                    productoConCantidad.UnitSoles = precio_seleccionado;
                    productoConCantidad.UnitDolares = monedaSeleccionada == 1 ?  Math.Round((decimal)productoConCantidad.Unit * sisVariables.Gtipcam, 1) : (decimal)productoConCantidad.Unit; 
                    productoConCantidad.Unit = ((monedaSeleccionada == 2) ? productoConCantidad.UnitDolares : productoConCantidad.UnitSoles);
                }

                // Si existe mismo artículo con misma UMD y precio → sumar cantidad
                var prodMismoPrecio = this.productos.FirstOrDefault(p =>
                    p.Codigo == productoConCantidad.Codigo &&
                    p.UMD == productoConCantidad.UMD &&
                    Math.Round(p.Unit, 6) == Math.Round(productoConCantidad.Unit, 6) &&
                    p.Coduni == productoConCantidad.Coduni
                );

                if (prodMismoPrecio != null)
                {
                    prodMismoPrecio.Cant += productoConCantidad.Cant;
                    prodMismoPrecio.OnPropertyChanged(nameof(prodMismoPrecio.Cant));
                }
                else
                {
                    this.productos.Add(productoConCantidad);
                }

               ActualizarTotalesUI();

            }
        }

        private async Task MostrarSubProductos(List<Tabart> productos, bool reiniciarPagina = true)
        {
            if (reiniciarPagina)
                paginaActual = 0;

            Subproductosfiltrados = productos;
            BtnSiguienteArticulos.IsEnabled = Subproductosfiltrados.Count > cantelementos;
            BtnAnteriorArticulos.IsEnabled = paginaActual > 0;

            await CrearRango();
        }


        private void CargarPagina(List<Tabart> productosfiltrados)
        {
            var lista = new List<ArticuloViewModel>();
            foreach (var producto in productosfiltrados)
            {
                string ruta = $"{sisVariables.GVimg}{sisVariables.GCia}{producto.codart}.{producto.formato}";
                lista.Add(new ArticuloViewModel
                {
                    Producto = producto,
                    Imagen = ruta
                });
            }
            UGSubProductos.ItemsSource = lista;
            UGSubProductos.Visibility = Visibility.Visible;
        }

        private void VerImagen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ArticuloViewModel vm)
            {
                MostrarImagen mostrar = new MostrarImagen(vm.Producto.codart, vm.Producto.descri, vm.Producto.formato);
                mostrar.ShowDialog();
            }
        }

        //private async void Click_SubProducto(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Border btn && btn.DataContext is ArticuloViewModel vm)
        //    {
        //        var tabpr_ArtController = new Tabpr_artController();
        //        lista_de_precios = tabpr_ArtController.preciosPorArticulo(vm.Producto.cia, vm.Producto.codart);

        //        if (lista_de_precios == null)
        //        {
        //            MessageBox.Show("Este producto no tiene artículos. . .");
        //            return;
        //        }

        //        if (lista_de_precios.Count == 1)
        //        {
        //            var subproducto = lista_de_precios[0];
        //            var prodExistente = this.productos.FirstOrDefault(p => p.Codigo == vm.Producto.codfab);
        //            AbrirFormularioCantidad(vm.Producto, subproducto);
        //            return;
        //        }

        //        DGUnidadDeMedida.Items.Clear();
        //        DetalleProductoPanel.Visibility = Visibility.Visible;
        //        BtnCerrarSubpanel.IsHitTestVisible = false;
        //        BtnCerrarSubpanel.Opacity = 0.5;

        //        await DesplazarDetallesdeSubproducto(lista_de_precios, 0);

        //        if (ContinuarAnimacionDetalle && !ContinuarProcesoDetalle) return;

        //        if (ContinuarProcesoDetalle)
        //        {
        //            if (lista_de_precios[posicion_actual_prart].cia == vm.Producto.cia &&
        //                lista_de_precios[posicion_actual_prart].codart == vm.Producto.codart)
        //            {
        //                var subproductoSeleccionado = lista_de_precios[posicion_actual_prart];
        //                AbrirFormularioCantidad(vm.Producto, subproductoSeleccionado);
        //            }
        //        }

        //        posicion_actual_prart = 0;
        //    }
        //}

        private async void BtnAnteriorArticulos_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActual > 0)
            {
                paginaActual--;
                CrearRango();
                ActualizarBotones();
            }
        }


        private async void BtnSiguienteArticulos_Click(object sender, RoutedEventArgs e)
        {
            if ((paginaActual + 1) * cantelementos < Subproductosfiltrados.Count)
            {
                paginaActual++;
                CrearRango();
                ActualizarBotones();
            }
        }


        private async Task CrearRango()
        {
            if (Subproductosfiltrados == null || Subproductosfiltrados.Count == 0)
            {
                UGSubProductos.ItemsSource = null;
                TBPagina.Text = "0/0";
                return;
            }

            int totalPaginas = (int)Math.Ceiling((double)Subproductosfiltrados.Count / cantelementos);

            // Seguridad: evitar que la página se salga del rango
            if (paginaActual < 0)
                paginaActual = 0;
            if (paginaActual >= totalPaginas)
                paginaActual = totalPaginas - 1;

            int inicio = paginaActual * cantelementos;
            int cantidad = Math.Min(cantelementos, Subproductosfiltrados.Count - inicio);

            var rango = Subproductosfiltrados.GetRange(inicio, cantidad);

            // Carga en la grilla
            CargarPagina(rango);

            // Actualización del texto UI
            TBPagina.Text = $"{paginaActual + 1}/{totalPaginas}";
        }

        private void ActualizarBotones()
        {
            BtnAnteriorArticulos.IsEnabled = paginaActual > 0;
            BtnSiguienteArticulos.IsEnabled = ((paginaActual + 1) * cantelementos < Subproductosfiltrados.Count);
        }
   

        public bool DesplazarDetallesdeSubproducto(List<Tabpr_art> subpr_articulo, Tabart tabart)
        {
            if (subpr_articulo == null || subpr_articulo.Count == 0)
            {
                MessageBox.Show("No hay nada que mostrar...");
                return false;
                throw new ArgumentException("La lista de subproductos está vacía.");
            }

            FRMUnidadDeMedida UMDs = new FRMUnidadDeMedida(subpr_articulo, tabart);
            UMDs.ShowDialog();
            if (UMDs.DialogResult == true)
                AbrirFormularioCantidad(tabart, UMDs.precio_seleccionado.EntidadOriginal, UMDs.precio);
            else
                return false;
            return true;
        }

        private void Click_BtnValidarDetalles(object sender, RoutedEventArgs e)
        {
            ContinuarProcesoDetalle = true;
            BtnCerrarSubpanel.IsHitTestVisible = true;
            BtnCerrarSubpanel.Opacity = 1;
        }

        private void ApplyTheme(string themeFile)
        {
            // Limpiar diccionarios previos
            Application.Current.Resources.MergedDictionaries.Clear();
            // Cargar nuevo
            var dict = new ResourceDictionary();
            dict.Source = new System.Uri($"Temas/{themeFile}", System.UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
        //private void SwitchTemaOscuro_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplyTheme("TemaOscuro.xaml");
        //    HsPvSerAPG.Properties.Settings.Default.IsDarkTheme = true;
        //    HsPvSerAPG.Properties.Settings.Default.Save();
        //}
        //private void SwitchTemaClaro_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplyTheme("TemaClaro.xaml");
        //    HsPvSerAPG.Properties.Settings.Default.IsDarkTheme = false;
        //    HsPvSerAPG.Properties.Settings.Default.Save();
        //}

        private void AnticipoCheck(object sender, RoutedEventArgs e)
        {
            productos.Clear();
            try
            {

                if (chkAnticipo.IsChecked == true) 
                {
                    
                    sisVariables.anticipo = 1;
                    sisVariables.stentregado = 0
                    ;
                }
                else
                {
                    sisVariables.anticipo = 0;
                    
                }

                if (chkEntregado.IsChecked == true)
                    chkEntregado.IsChecked = false;

                var tabartController = new TabartController();
                var lista = tabartController.selectTabart(sisVariables.GCia, "", 0, 1);
                if (lista == null || lista.Count == 0)
                {
                    System.Windows.MessageBox.Show("No se encontraron productos.");
                    return;
                }
                var articulo = lista[0];

                var tabpr_artController = new Tabpr_artController();
                var precios = tabpr_artController.preciosPorArticulo(articulo.cia, articulo.codart);

                if (precios == null || precios.Count == 0)
                {
                    System.Windows.MessageBox.Show("No se encontraron precios para este producto.");
                    return;
                }

                var subproducto = precios[0];
                decimal precioOriginal = Math.Round(Convert.ToDecimal(subproducto.precio), 6);
                int TipMonOriginal = subproducto.tipmon;

                var productoAnticipo = new Producto
                {
                    Codigo = articulo.codart,
                    Descripcion = articulo.descri,
                    Cant = 1,
                    PrecioOriginal = precioOriginal,
                    Codart = articulo.codart,
                    TipMonOriginal = TipMonOriginal
                };

                AgregarFila(productoAnticipo);
                ActualizarTotalesUI();
                btnVentanaProductos.IsEnabled = false;
                BtnProductos.IsEnabled = false;
                txBcodart.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }
        }


        private void AnticipoCheckF(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var tabartController = new TabartController();
                var lista = tabartController.selectTabart(1, "");
                if (lista == null || lista.Count == 0)
                    return;
                var articulos = lista[0];
                var productoAnticipo = productos.FirstOrDefault(p => p.Codigo == articulos.codart);
                if (productoAnticipo != null)
                    productos.Remove(productoAnticipo);
                ActualizarTotalesUI();

                btnVentanaProductos.IsEnabled = true;
                BtnProductos.IsEnabled = true;
                txBcodart.IsEnabled = true;
                sisVariables.anticipo = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }
        }

        private void CheckBox_chkEntregado(object sender, RoutedEventArgs e)
        {
            
            if (chkEntregado.IsChecked == true)
            { 
                sisVariables.anticipo = 0;
                sisVariables.stentregado = 1;
            }
            else {
                sisVariables.stentregado = 0;
            }
                
        }
        private void CheckBox_chkEntregadoF(object sender, RoutedEventArgs e)
        {
            if  (chkEntregado.IsChecked==false)
            sisVariables.stentregado = 0;            

        }

        private void BtnBolsas_Click(object sender, RoutedEventArgs e)
        {
            var ventana_bolsas = new VentanaBolsas();
            ventana_bolsas.ShowDialog();
            decimal valor_bolsas = Convert.ToDecimal(ventana_bolsas.TBCantidad.Text) * 0.50m;
            TB_BPlastic.Text = Math.Round(valor_bolsas, 2).ToString();
            ActualizarTotalesUI();
        }

        private async void Click_SubProducto(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border btn && btn.DataContext is ArticuloViewModel vm)
            {
                var tabpr_ArtController = new Tabpr_artController();
                lista_de_precios = tabpr_ArtController.preciosPorArticulo(vm.Producto.cia, vm.Producto.codart);

                if (lista_de_precios == null)
                {
                    MessageBox.Show("Este producto no tiene artículos. . .");
                    return;
                }

                DesplazarDetallesdeSubproducto(lista_de_precios, vm.Producto);
            }

        }

        private async void Producto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is Tabgrup producto)
                {
                    var lista = tabartController.selectTabart(sisVariables.GCia, "", producto.cod);
                    todoslossubproductos = lista;
                    if (lista != null)
                    {
                        Storyboard sb = (Storyboard)this.Resources["MostrarSubPanel"];
                        sb.Begin();
                        SubPanel.Visibility = Visibility.Visible;
                        TBProducto.Focus();
                        HabyInhabProductos(true);
                        await MostrarSubProductos(lista);
                    }
                    else
                        MessageBox.Show("No hay articulos en este producto.", "Aviso", MessageBoxButton.OK);  
                }
            }
        }

        private void UMD_EnterKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {

            }
        }
    }
}
