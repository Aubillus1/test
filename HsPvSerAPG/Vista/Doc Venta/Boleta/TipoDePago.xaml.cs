using FontAwesome.WPF;
using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Utils.Clases;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
using System.Windows.Shapes;
using System.Text.Json;
//using Twilio.Rest.Numbers.V2.RegulatoryCompliance;
using static System.Net.WebRequestMethods;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace HsPvSerAPG.Vista.Doc_Venta.Boleta
{
    /// <summary>
    /// Lógica de interacción para TipoDePago.xaml
    /// </summary>
    public partial class TipoDePago : Window
    {
        private TabBanController tabBan = new TabBanController();
        private TabTarController tabTar = new TabTarController();
        private ApiTabserService apiService = new ApiTabserService();

        private List<Codven> vendedores = new List<Codven>();


        public List<PagoCalculo> pagos_a_realizar = new List<PagoCalculo>();
        public PagoViewModel viewmodel { get; set; }
        public event EventHandler LimpiarGrilla;

        private int tipmon;


        private string tippago = "";
        private string tarjeta = "";
        private string banco = "";

        private bool fecha_obl = false;

        private bool soloundoc = false;
        private int doc = 0;

        private List<TabFac> tabFac = new List<TabFac>();

        public TipoDePago(List<TabFac> tabfac)
        {
            InitializeComponent();
            tabFac = tabfac; 
            txt_valorVenta.Text = tabfac.Sum(x => x.Importe).ToString();

            
            //if (tabfac[0].Tipmon)

            Importe.Text = tabfac.Sum(x => x.Importe).ToString();
            this.tipmon = tipmon;
            TBNroLote.Text = sisVariables.GNumLote;

            if (tabfac.Count != 1)
            {
                BtnEfectivo.Visibility = Visibility.Collapsed;
                soloundoc = true;
                Importe.IsEnabled = false;

            }

            this.tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;

            CargarBancos();
            CargarTarjetas();
            CargarCBSucursales();
            try
            {
                viewmodel = new PagoViewModel(Convert.ToDouble(tabfac.Sum(x => x.Importe)));

                DataContext = viewmodel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                throw;
            }

        }

        //private readonly Dictionary<int, string> prefijosDoc = new Dictionary<int, string>
        //    {
        //        { 1, "F" },   // Factura
        //        { 3, "B" },   // Boleta
        //        { 41, "N/E" }  // Nota de Crédito
        //        //{ 8, "N/D" } // Nota de Débito
        //    };

        private void CargarCBSucursales()
        {
            TabcajaController tabcajaController = new TabcajaController();

            var sucursales = tabcajaController.selectCajaLogin(sisVariables.GCia, sisVariables.GCodusr
                );
            CBSucursal.ItemsSource = sucursales;
            CBSucursal.DisplayMemberPath = "des";
            CBSucursal.SelectedValuePath = "cod";

            var vendedorSeleccionado = sucursales.FirstOrDefault(v => v.des == tabFac[0].Dessuc);

            if (vendedorSeleccionado != null)
                CBSucursal.SelectedItem = vendedorSeleccionado;
        }

        //private void CBtipodoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (CBtipodoc.SelectedValue != null)
        //    {
        //        int codDoc = Convert.ToInt32(CBtipodoc.SelectedValue); // 🔹 Conversión segura
        //        CargarSeriesPorDoc(codDoc);
        //    }
        //}

        //private void CargarSeriesPorDoc(int elec_tipdoc)
        //{
        //    int cia = sisVariables.GCia;
        //    int codcaja = sisVariables.GCodCaja;
        //    int st_anti = 0;

        //    var listaSeries = apiService.selectTabser(cia, codcaja, elec_tipdoc, st_anti);

        //    if (listaSeries != null && listaSeries.Count > 0)
        //    {
        //        List<Tabser> seriesFiltradas;

        //        if (elec_tipdoc == 41)
        //        {
        //            seriesFiltradas = listaSeries;
        //        }
        //        else if (prefijosDoc.ContainsKey(elec_tipdoc))
        //        {
        //            string prefijo = prefijosDoc[elec_tipdoc];
        //            seriesFiltradas = listaSeries
        //                                .Where(x => x.codsun.StartsWith(prefijo))
        //                                .ToList();
        //        }
        //        else
        //        {

        //            seriesFiltradas = listaSeries;
        //        }

        //        CMBserie.ItemsSource = seriesFiltradas;
        //        CMBserie.DisplayMemberPath = "codsun";  
        //        CMBserie.SelectedValuePath = "nroser";  
        //        CMBserie.SelectedIndex = seriesFiltradas.Count > 0 ? 0 : -1;
        //    }
        //    else
        //    {

        //    }
        //    {
        //        CMBserie.ItemsSource = null;
        //    }
        //}

        private void MostrarSoloEfectivo()
        {
            fecha_obl = true;
            tippago = "Efectivo";
            //Efecto de seleccion
            BtnEfectivo.Background = Brushes.Blue;
            IconoPago.Foreground = Brushes.White;
            TBEfectivo.Foreground = Brushes.White;

            BtnTarjeta.SetResourceReference(Button.BackgroundProperty, "BtnTarjetaBackground");
            IconoTarjeta.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnTarjetaBorder");
            TBTarjeta.SetResourceReference(TextBlock.ForegroundProperty, "BtnTarjetaBorder");

            BtnBanco.SetResourceReference(Button.BackgroundProperty, "BtnBancoBackground");
            IconoBanco.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnBancoBorder");
            TBBanco.SetResourceReference(TextBlock.ForegroundProperty, "BtnBancoBorder");

            // Mostrar columnas efectivo
            colEfectivoTipo.Visibility = Visibility.Visible;
            colEfectivoImporte.Visibility = Visibility.Visible;
            colEfectivoDetalle.Visibility = Visibility.Visible;

            // Ocultar columnas tarjeta
            colTarjeta.Visibility = Visibility.Collapsed;
            colLote_operacion.Visibility = Visibility.Collapsed;

            TxtLote.Visibility = Visibility.Collapsed;
            TBNroOperacion.Visibility = Visibility.Collapsed;
            TBNroLote.Visibility = Visibility.Collapsed;

            var colorFondo = (Color)ColorConverter.ConvertFromString("#C5F291"); // azul
            var colorTexto = (Color)ColorConverter.ConvertFromString("#86c938"); // blanco

            var brushFondo = new SolidColorBrush(colorFondo);
            var brushTexto = new SolidColorBrush(colorTexto);

            // Copiamos el estilo original
            var estiloOriginal = (Style)dgPagos.Resources["CenterHeaderStyle"];
            var estiloCopia = new Style(typeof(DataGridColumnHeader), estiloOriginal);

            // Buscar si ya existían los Setters
            var setterBackground = estiloCopia.Setters
                .OfType<Setter>()
                .FirstOrDefault(s => s.Property == DataGridColumnHeader.BackgroundProperty);

            var setterBorder = estiloCopia.Setters
                .OfType<Setter>()
                .FirstOrDefault(s => s.Property == DataGridColumnHeader.BorderBrushProperty);

            // Si existen, los actualizamos
            if (setterBackground != null)
                setterBackground.Value = brushFondo;
            else
                estiloCopia.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, brushFondo));

            if (setterBorder != null)
                setterBorder.Value = brushTexto;
            else
                estiloCopia.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, brushTexto));

            // Aplicar la copia al DataGrid
            dgPagos.ColumnHeaderStyle = estiloCopia;
            TBTituloPago.Text = "Detalle de Pagos : Efectivo";

            Icon1.Visibility = Visibility.Visible;
            Icon2.Visibility = Visibility.Collapsed;
            Icon3.Visibility = Visibility.Collapsed;
            this.Title = "Pago seleccionado: Efectivo";
            CambMoneda.IsEnabled = true;
        }

        private void MostrarEfectivoYTarjeta()
        {
            //Efecto de seleccion
            BtnTarjeta.Background = Brushes.Blue;
            IconoTarjeta.Foreground = Brushes.White;
            TBTarjeta.Foreground = Brushes.White;

            BtnEfectivo.SetResourceReference(Button.BackgroundProperty, "BtnPagoBackground");
            IconoPago.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnPagoBorder");
            TBEfectivo.SetResourceReference(TextBlock.ForegroundProperty, "BtnPagoBorder");

            BtnBanco.SetResourceReference(Button.BackgroundProperty, "BtnBancoBackground");
            IconoBanco.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnBancoBorder");
            TBBanco.SetResourceReference(TextBlock.ForegroundProperty, "BtnBancoBorder");

            // Mostrar columnas efectivo y tarjeta
            colEfectivoTipo.Visibility = Visibility.Visible;
            colEfectivoImporte.Visibility = Visibility.Visible;
            colEfectivoDetalle.Visibility = Visibility.Visible;

            colTarjeta.Visibility = Visibility.Visible;
            colTarjeta.Header = "Tarjetas";
            colLote_operacion.Visibility = Visibility.Visible;

            var colorFondo = (Color)ColorConverter.ConvertFromString("#f5f992");
            var colorTexto = (Color)ColorConverter.ConvertFromString("#737632");

            var brushFondo = new SolidColorBrush(colorFondo);
            var brushTexto = new SolidColorBrush(colorTexto);

            // Copiamos el estilo original
            var estiloOriginal = (Style)dgPagos.Resources["CenterHeaderStyle"];
            var estiloCopia = new Style(typeof(DataGridColumnHeader), estiloOriginal);

            // Buscar si ya existían los Setters
            var setterBackground = estiloCopia.Setters
                .OfType<Setter>()
                .FirstOrDefault(s => s.Property == DataGridColumnHeader.BackgroundProperty);

            var setterBorder = estiloCopia.Setters
                .OfType<Setter>()
                .FirstOrDefault(s => s.Property == DataGridColumnHeader.BorderBrushProperty);

            // Si existen, los actualizamos
            if (setterBackground != null)
                setterBackground.Value = brushFondo;
            else
                estiloCopia.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, brushFondo));

            if (setterBorder != null)
                setterBorder.Value = brushTexto;
            else
                estiloCopia.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, brushTexto));

            // Aplicar la copia al DataGrid
            dgPagos.ColumnHeaderStyle = estiloCopia;
            TBTituloPago.Text = "Detalle de Pagos : Tarjeta";

            Icon1.Visibility = Visibility.Collapsed;
            Icon2.Visibility = Visibility.Visible;
            Icon3.Visibility = Visibility.Collapsed;
            this.Title = "Pago seleccionado: Tarjeta";
        }

        private void MostrarEfectivoYBanco()
        {
            //Efecto de seleccion
            BtnBanco.Background = Brushes.Blue;
            IconoBanco.Foreground = Brushes.White;
            TBBanco.Foreground = Brushes.White;

            BtnEfectivo.SetResourceReference(Button.BackgroundProperty, "BtnPagoBackground");
            IconoPago.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnPagoBorder");
            TBEfectivo.SetResourceReference(TextBlock.ForegroundProperty, "BtnPagoBorder");

            BtnTarjeta.SetResourceReference(Button.BackgroundProperty, "BtnTarjetaBackground");
            IconoTarjeta.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnTarjetaBorder");
            TBTarjeta.SetResourceReference(TextBlock.ForegroundProperty, "BtnTarjetaBorder");


            // Mostrar columnas efectivo y banco
            colEfectivoTipo.Visibility = Visibility.Visible;
            colEfectivoImporte.Visibility = Visibility.Visible;
            colEfectivoDetalle.Visibility = Visibility.Visible;

            colTarjeta.Header = "Bancos";

            // Ocultar columnas tarjeta
            colTarjeta.Visibility = Visibility.Visible;
            colLote_operacion.Visibility = Visibility.Visible;

            var colorFondo = (Color)ColorConverter.ConvertFromString("#FFB75C");
            var colorTexto = (Color)ColorConverter.ConvertFromString("#D1A164");

            var brushFondo = new SolidColorBrush(colorFondo);
            var brushTexto = new SolidColorBrush(colorTexto);

            // Copiamos el estilo original
            var estiloOriginal = (Style)dgPagos.Resources["CenterHeaderStyle"];
            var estiloCopia = new Style(typeof(DataGridColumnHeader), estiloOriginal);

            // Buscar si ya existían los Setters
            var setterBackground = estiloCopia.Setters
                .OfType<Setter>()
                .FirstOrDefault(s => s.Property == DataGridColumnHeader.BackgroundProperty);

            var setterBorder = estiloCopia.Setters
                .OfType<Setter>()
                .FirstOrDefault(s => s.Property == DataGridColumnHeader.BorderBrushProperty);

            // Si existen, los actualizamos
            if (setterBackground != null)
                setterBackground.Value = brushFondo;
            else
                estiloCopia.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, brushFondo));

            if (setterBorder != null)
                setterBorder.Value = brushTexto;
            else
                estiloCopia.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, brushTexto));

            // Aplicar la copia al DataGrid
            dgPagos.ColumnHeaderStyle = estiloCopia;
            TBTituloPago.Text = "Detalle de Pagos : Deposito";

            Icon1.Visibility = Visibility.Collapsed;
            Icon2.Visibility = Visibility.Collapsed;
            Icon3.Visibility = Visibility.Visible;
            this.Title = "Pago seleccionado: Deposito";
        }

        private void CargarTarjetas()
        {
            SubContenedoresTarjetas.Children.Clear();

            var tarjetas = tabTar.selectTabTar();
            if (tarjetas == null || tarjetas.Count == 0)
                return;

            foreach (var tarjeta in tarjetas)
            {
                var btn = new Button
                {
                    Height = 60,
                    Background = (SolidColorBrush)Application.Current.FindResource("TextBox"),
                    Foreground = Brushes.Black,
                    BorderBrush = (SolidColorBrush)Application.Current.FindResource("BordeTextBox"),
                    BorderThickness = new Thickness(2),
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Tag = tarjeta
                };
                var text = new TextBlock
                {
                    Text = tarjeta.des,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center
                };
                btn.Content = text;

                btn.Click += (s, e) =>
                {
                    BtnTarjeta.Background = Brushes.Blue;
                    IconoTarjeta.Foreground = Brushes.White;
                    TBTarjeta.Foreground = Brushes.White;

                    BtnEfectivo.SetResourceReference(Button.BackgroundProperty, "BtnPagoBackground");
                    IconoPago.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnPagoBorder");
                    TBEfectivo.SetResourceReference(TextBlock.ForegroundProperty, "BtnPagoBorder");

                    BtnBanco.SetResourceReference(Button.BackgroundProperty, "BtnBancoBackground");
                    IconoBanco.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnBancoBorder");
                    TBBanco.SetResourceReference(TextBlock.ForegroundProperty, "BtnBancoBorder");

                    foreach (var child in SubContenedoresTarjetas.Children.OfType<Button>())
                    {
                        child.Background = (SolidColorBrush)Application.Current.FindResource("TextBox");
                        if (child.Content is TextBlock tb)
                            tb.Foreground = (SolidColorBrush)Application.Current.FindResource("BordeTextBox");
                    }

                    foreach (var child in SubContenedoresBanco.Children.OfType<Button>())
                    {
                        child.Background = (SolidColorBrush)Application.Current.FindResource("TextBox");
                        if (child.Content is TextBlock tb)
                            tb.Foreground = (SolidColorBrush)Application.Current.FindResource("BordeTextBox");
                    }

                    if (s is Button clickedBtn && clickedBtn.Tag is TabTar tarjetaSel)
                    {
                        clickedBtn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bee6fd"));

                        this.tarjeta = tarjetaSel.des;
                        tippago = "Tarjeta de Cred./Debi.";
                        this.banco = "";
                    }
                    CambMoneda.IsEnabled = true;

                    TxtLote.Text = "Nro. Lote";

                    TxtLote.Visibility = Visibility.Visible;
                    TBNroLote.Visibility = Visibility.Visible;
                    colTarjeta.Visibility = Visibility.Visible;
                    TBNroOperacion.Visibility = Visibility.Collapsed;
                    MostrarEfectivoYTarjeta();
                    dgPagos.Columns[2].Header = "Nr. Lote";
                    dgPagos.Columns[3].Header = "Tarjetas";
                };
                SubContenedoresTarjetas.Children.Add(btn);
            }
        }

        private void CargarBancos()
        {
            SubContenedoresBanco.Children.Clear();

            var tabBans = tabBan.selectTabBan();
            if (tabBans == null || tabBans.Count == 0)
                return;

            foreach (var banco in tabBans)
            {
                // Crear botón
                var btn = new Button
                {
                    Content = banco.des,
                    Height = 60,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Background = (SolidColorBrush)Application.Current.FindResource("TextBox"),
                    Foreground = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    BorderBrush = (SolidColorBrush)Application.Current.FindResource("BordeTextBox"),
                    Tag = banco // guardo el objeto por si lo necesito al hacer click
                };
                btn.Click += (s, e) =>
                {
                    BtnBanco.Background = Brushes.Blue;
                    IconoBanco.Foreground = Brushes.White;
                    TBBanco.Foreground = Brushes.White;

                    BtnEfectivo.SetResourceReference(Button.BackgroundProperty, "BtnPagoBackground");
                    IconoPago.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnPagoBorder");
                    TBEfectivo.SetResourceReference(TextBlock.ForegroundProperty, "BtnPagoBorder");

                    BtnTarjeta.SetResourceReference(Button.BackgroundProperty, "BtnTarjetaBackground");
                    IconoTarjeta.SetResourceReference(ImageAwesome.ForegroundProperty, "BtnTarjetaBorder");
                    TBTarjeta.SetResourceReference(TextBlock.ForegroundProperty, "BtnTarjetaBorder");

                    foreach (var child in SubContenedoresBanco.Children.OfType<Button>())
                    {
                        child.Background = (SolidColorBrush)Application.Current.FindResource("TextBox");
                        if (child.Content is TextBlock tb)
                            tb.Foreground = (SolidColorBrush)Application.Current.FindResource("BordeTextBox");
                    }

                    foreach (var child in SubContenedoresTarjetas.Children.OfType<Button>())
                    {
                        child.Background = (SolidColorBrush)Application.Current.FindResource("TextBox");
                        if (child.Content is TextBlock tb)
                            tb.Foreground = (SolidColorBrush)Application.Current.FindResource("BordeTextBox");
                    }

                    if (s is Button clickedBtn && clickedBtn.Tag is TabBan bancoSel)
                    {
                        clickedBtn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bee6fd"));

                        this.banco = bancoSel.des;
                        tippago = "Banco";
                        this.tarjeta = "";
                    }
                    CambMoneda.SelectedIndex = banco.tipmon == 1 ? 0 : 1;
                    CambMoneda.IsEnabled = false;
                    dgPagos.Columns[2].Header = "Nr. Operación";
                    dgPagos.Columns[3].Header = "Bancos";

                    TxtLote.Text = "Nro. Operación";

                    TxtLote.Visibility = Visibility.Visible;
                    TBNroOperacion.Visibility = Visibility.Visible;
                    TBNroLote.Visibility = Visibility.Collapsed;

                    MostrarEfectivoYBanco();
                };


                SubContenedoresBanco.Children.Add(btn);
            }
        }

        private void Click_BtnTarjeta(object sender, RoutedEventArgs e)
        {
            MostrarEfectivoYTarjeta();
            tippago = "";
            fecha_obl = false;
            TxtLote.Text = "Nro. Lote";

            TxtLote.Visibility = Visibility.Visible;
            TBNroLote.Visibility = Visibility.Visible;
            TBNroOperacion.Visibility = Visibility.Collapsed;

            var fila = SegmentoTarjeta.RowDefinitions[1];

            if (fila.Height.Value == 0)
                fila.Height = GridLength.Auto; // mostrar
            else
                fila.Height = new GridLength(0);

            CambMoneda.IsEnabled = true;
            dgPagos.Columns[2].Header = "Nr. Lote";
        }

        private void Click_BtnDeposito(object sender, RoutedEventArgs e)
        {
            MostrarEfectivoYBanco();
            tippago = "Banco";
            fecha_obl = false;
            TxtLote.Text = "Nro. Operación";

            TxtLote.Visibility = Visibility.Visible;
            TBNroOperacion.Visibility = Visibility.Visible;
            TBNroLote.Visibility = Visibility.Collapsed;

            var fila = SegmentoBanco.RowDefinitions[1];

            if (fila.Height.Value == 0)
                fila.Height = GridLength.Auto; // mostrar
            else
                fila.Height = new GridLength(0);

            CambMoneda.IsEnabled = false;
            dgPagos.Columns[2].Header = "Nr. Operación";
        }

        private void Click_BtnSalir(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Está seguro que desea salir?",
               "Confirmar salida",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question
           );

            if (result == MessageBoxResult.Yes)
                this.Close();
        }

        private void BtnEfectivo_Click(object sender, RoutedEventArgs e) => MostrarSoloEfectivo();

        //BTN IMPRIMIR
        private void ImprimirPDFDirecto(string pdfPath)
        {
            try
            {
                using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        // Configurar impresora predeterminada
                        var printerSettings = new PrinterSettings
                        {
                            PrinterName = new PrinterSettings().PrinterName, // predeterminada del sistema
                            Copies = 1
                        };

                        var pageSettings = new PageSettings(printerSettings)
                        {
                            Margins = new Margins(0, 0, 0, 0)
                        };

                        //  Aplicar configuraciones a la impresión
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();

                        // Centrar automáticamente según tamaño del papel
                        printDocument.PrintPage += (s, e) =>
                        {
                            var pageSize = document.PageSizes[0];
                            int contenidoAncho = (int)(pageSize.Width / 72f * 100f); // centésimas de pulgada
                            int anchoPapel = e.PageBounds.Width;

                            float offsetX = (anchoPapel - contenidoAncho) / 2f;
                            if (offsetX > 0) e.Graphics.TranslateTransform(offsetX, 0);
                        };

                        printDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error al imprimir PDF: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        //BTN GUARDAR 
        private async void Click_BtnGrabar(object sender, RoutedEventArgs e)
        {
            if (!fecha_obl)
            {
                if (DPFechaOperacion.Text == "" || DPFechaOperacion == null)
                {
                    MessageBox.Show("Tienes que ingresar una fecha de operación", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }


            if (pagos_a_realizar == null || pagos_a_realizar.Count == 0)
            {
                MessageBox.Show("Tienes que agregar al menos un pago", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Validar que el valor de venta no sea menor a -0.09
            if (double.TryParse(Vuelto.Text, out double vueltoS))
            {
                if (vueltoS < -0.09)
                {
                    MessageBox.Show("El total de la venta no puede ser menor a -0.09", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            //if (double.TryParse(VueltoDolares.Text, out double vueltoD))
            //{
            //    if (vueltoD < -0.09)
            //    {
            //        MessageBox.Show("El total de la venta no puede ser menor a -0.02", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        return;
            //    }
            //}

            var controller_pago = new PagosController();

            //UNICO DOCUMENTO
            if (tabFac.Count == 1)
            {
                PagoUnicoDoc pagounico = new PagoUnicoDoc();

                pagounico.cia = sisVariables.GCia;

                pagounico.codsuc = Convert.ToInt32(CBSucursal.SelectedValue);

                pagounico.tipdoc = 41;

                pagounico.numdoc = tabFac[0].Numdoc;

                pagounico.codusr = tabFac[0].codusr;



                foreach (var detpagos in pagos_a_realizar)
                {
                    decimal importeSoles = 0.00m;
                    decimal importeDolares = 0.00m;
                    if (detpagos.Moneda == 1)
                        importeSoles = (decimal)detpagos.ImporteEnSoles;
                    else if (detpagos.Moneda == 2)
                        importeDolares = (decimal)detpagos.Importe;

                    string nro;
                    switch (detpagos.Codigo)
                    {
                        case 0:
                            nro = "";
                            break;
                        case 1:
                            nro = detpagos.NrOperacion;
                            break;
                        case 2:
                            nro = "OP-" + detpagos.NrOperacion;
                            break;
                        default:
                            nro = "";
                            break;
                    }


                    pagounico.detpagos.Add(new DetpagosUnicos
                    {
                        ptipopago = detpagos.Codigo,
                        Pimpsol = importeSoles,
                        pimpdol = importeDolares,
                        pvuesol = 0,
                        pvuedol = 0,
                        pcodbantar = detpagos.Codigo,
                        pnrocheope = nro,
                        pfeccheope = detpagos.FechaOperacion,
                        pfecdif = null,
                        ptipcam = sisVariables.Gtipcam,
                        pcobobs = detpagos.Detalle
                    });
                }

                var (success, message, correls) = await controller_pago.insertarCobranzasUnicoDocumento(pagounico);
                MessageBox.Show(success ? $"Éxito: {message}" : $"Fallo: {message}");
                if (success)
                {
                    if (correls == null || correls.Count == 0)
                    {
                        MessageBox.Show("No se generó ningún correlativo para imprimir.");
                        return;
                    }

                    var result = MessageBox.Show(
                        $"Se generaron {correls.Count} comprobantes.\n¿Desea imprimirlos ahora?",
                        "Imprimir Comprobantes",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result != MessageBoxResult.Yes)
                    {
                        LimpiarGrilla?.Invoke(this, EventArgs.Empty);
                        this.Close();
                        return;
                    }

                    int cia = sisVariables.GCia;
                    int tipcob = 1;
                    int codsuc = Convert.ToInt32(CBSucursal.SelectedValue);
                    string fecha = DateTime.Now.ToString("yyyyMMdd");

                    using (var cliente = new HttpClient())
                    {
                        foreach (var correl in correls)
                        {
                            try
                            {
                                string apiUrl = $"{sisVariables.GAPI}consultaImpPagare" +
                                                $"?cia={cia}&tipcob={tipcob}&codsuc={codsuc}&fecha={fecha}&correl={correl}";

                                var response = await cliente.GetAsync(apiUrl);

                                if (!response.IsSuccessStatusCode)
                                {
                                    MessageBox.Show($"Error al generar comprobante ({response.StatusCode}) del correl {correl}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    continue; 
                                }

                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                JsonDocument jsonDocument = System.Text.Json.JsonDocument.Parse(jsonResponse);
                                var doc = jsonDocument;
                                var root = doc.RootElement;

                                if (!root.TryGetProperty("pdf_link", out var pdfLinkElement))
                                {
                                    MessageBox.Show($"No se encontró el link del comprobante (correl {correl}).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    continue;
                                }

                                string pdfURL = pdfLinkElement.GetString();
                                var pdfBytes = await cliente.GetByteArrayAsync(pdfURL);
                                string fileName = System.IO.Path.GetFileName(pdfURL);
                                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);

                                System.IO.File.WriteAllBytes(tempFilePath, pdfBytes);

                                ImprimirPDFDirecto(tempFilePath);

                                System.IO.File.Delete(tempFilePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error al generar/imprimir correl {correl}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }


                    LimpiarGrilla?.Invoke(this, EventArgs.Empty);
                    this.Close();
                }

            }


            //VARIOS DOCUMENTOS
            else
            {
                Pagos pagos = new Pagos();

                pagos.cia = sisVariables.GCia;

                pagos.codsuc = Convert.ToInt32(CBSucursal.SelectedValue);

                pagos.fecha = DateTime.Now.ToString("yyyyMMdd");

                int codpago;
                switch (tippago)
                {
                    case "Efectivo":
                        codpago = 0;
                        break;
                    case "Tarjeta de Cred./Debi.":
                        codpago = 1;
                        break;
                    case "Banco":
                        codpago = 2;
                        break;
                    default:
                        codpago = -1;
                        break;
                }

                string nro;
                switch (codpago)
                {
                    case 0:
                        nro = "";
                        break;
                    case 1:
                        nro = pagos_a_realizar[0].NrOperacion;
                        break;
                    case 2:
                        nro = "OP-" + pagos_a_realizar[0].NrOperacion;
                        break;
                    default:
                        nro = "";
                        break;
                }


                //Sumatoria de los importes en el datagrid
                decimal importeSoles = 0.00m;
                decimal importeDolares = 0.00m;
                if (pagos_a_realizar[0].Moneda == 1)
                    importeSoles = (decimal)pagos_a_realizar[0].ImporteEnSoles;
                else if (pagos_a_realizar[0].Moneda == 2)
                    importeDolares = (decimal)pagos_a_realizar[0].ImporteEnSoles;

                pagos.impsol = importeSoles;

                pagos.impdol = importeDolares;

                pagos.vuesol = Convert.ToDecimal(Vuelto.Text);

                pagos.vuedol = Convert.ToDecimal(VueltoDolares.Text);

                pagos.codbantar = 1;

                pagos.nrocheope = nro;

                pagos.feccheope = DPFechaOperacion.SelectedDate?.ToString("yyyy-MM-dd");

                pagos.fecdif = null;

                pagos.tipcam = pagos_a_realizar[0].TipCam;

                pagos.cobobs = pagos_a_realizar[0].Detalle;

                foreach (var doc in tabFac)
                {
                    pagos.Documentos.Add(new Documentos
                    {
                        tipdoc = doc.Tipdoc,
                        numdoc = doc.Numdoc,
                        tipmon = doc.Tipmon,
                        importe = doc.Importe
                    });
                }

                var (success, message, correl) = await controller_pago.InsertarCobranzaVariosDocumentosAsync(pagos);
                MessageBox.Show(success ? $"Éxito: {message}" : $"Fallo: {message}");

                if (success)
                {
                    var result = MessageBox.Show("¿Desea imprimir el comprobante?", "Imprimir", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        LimpiarGrilla?.Invoke(this, EventArgs.Empty);
                        this.Close();
                        return;
                    }

                    int cia = sisVariables.GCia;
                    int tipcob = 1;
                    int codsuc = Convert.ToInt32(CBSucursal.SelectedValue);
                    string fecha = DateTime.Now.ToString("yyyyMMdd");

                    using (var cliente = new HttpClient())
                    {
                        try
                        {
                            string apiUrl = $"{sisVariables.GAPI}consultaImpPagare" +
                                            $"?cia={cia}&tipcob={tipcob}&codsuc={codsuc}&fecha={fecha}&correl={correl}";

                            var response = await cliente.GetAsync(apiUrl);
                            if (!response.IsSuccessStatusCode)
                            {
                                MessageBox.Show($"Error al generar comprobante ({response.StatusCode})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            var doc = System.Text.Json.JsonDocument.Parse(jsonResponse);
                            var root = doc.RootElement;

                            if (!root.TryGetProperty("pdf_link", out var pdfLinkElement))
                            {
                                MessageBox.Show("No se encontró el link del comprobante en la respuesta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            string pdfURL = pdfLinkElement.GetString();
                            var pdfBytes = await cliente.GetByteArrayAsync(pdfURL);
                            string fileName = System.IO.Path.GetFileName(pdfURL);
                            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
                            System.IO.File.WriteAllBytes(tempFilePath, pdfBytes);

                            ImprimirPDFDirecto(tempFilePath);
                            System.IO.File.Delete(tempFilePath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al generar o imprimir comprobante: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    LimpiarGrilla?.Invoke(this, EventArgs.Empty);
                    this.Close();
                }

            }
        }


        private void Click_AgregarPago(object sender, RoutedEventArgs e)
        {
            if (fecha_obl)
            {
                if (DPFechaOperacion.Text == "" || DPFechaOperacion == null)
                {
                    MessageBox.Show("Tienes que ingresar una fecha de operación", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }

            if (soloundoc)
            {
                if (doc == 1)
                {
                    MessageBox.Show("Solo puedes agregar un pago para este documento", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }

            if (Importe.Text == "" || Importe.Text == "0" || Importe.Text == "0.00")
            {
                MessageBox.Show("Tienes que ingresar un importe", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

                if (tippago == "")
                {
                    MessageBox.Show("Tienes que seleccionar un Tipo de Pago", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    if (tippago == "Tarjeta de Cred./Debi.")
                    {
                        if (tarjeta == "")
                        {
                            MessageBox.Show("Tienes que seleccionar una tarjeta", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(TBNroLote.Text))
                        {
                            MessageBox.Show("Debes ingresar el número de lote para pagos con tarjeta.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            TBNroLote.Focus();
                            return;
                        }
                    }
                    if (tippago == "Banco")
                    {
                        if (banco == "")
                        {
                            MessageBox.Show("Tienes que seleccionar un banco", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(TBNroOperacion.Text))
                        {
                            MessageBox.Show("Debes ingresar el número de operacion para pagos con Banco.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            TBNroLote.Focus();
                            return;
                        }
                    }
                }
            int codpago;
            switch (tippago)
            {
                case "Efectivo":
                    codpago = 0;
                    break;
                case "Tarjeta de Cred./Debi.":
                    codpago = 1;
                    break;
                case "Banco":
                    codpago = 2;
                    break;
                default:
                    codpago = -1;
                    break;
            }
            int monedaSeleccionada = CambMoneda.SelectedIndex + 1;
                string monsig = "";
                if (monedaSeleccionada == 1)
                    monsig = "S/";
                else if (monedaSeleccionada == 2)
                    monsig = "$";
                else
                    monsig = "-";
            string nroop_lot;
            switch (codpago)
            {
                case 0:
                    nroop_lot = "";
                    break;
                case 1:
                    nroop_lot = TBNroLote.Text;
                    break;
                case 2:
                    nroop_lot = TBNroOperacion.Text;
                    break;
                case -1:
                    nroop_lot = "-";
                    break;
                default:
                    nroop_lot = "";
                    break;
            }
            string tarban;
            switch (codpago)
            {
                case 0:
                    tarban = "";
                    break;
                case 1:
                    tarban = this.tarjeta;
                    break;
                case 2:
                    tarban = this.banco;
                    break;
                default:
                    tarban = "";
                    break;
            }

            PagoCalculo nuevo_pago = new PagoCalculo
                {
                    TipoPago = tippago,
                    Importe = Convert.ToDouble(Importe.Text),
                    Detalle = TBDetalle.Text,
                    Tarjeta = tarban,
                    NroLote = TBNroLote.Text,
                    NrOperacion = nroop_lot,
                    Banco = tarban,
                    FechaOperacion = DPFechaOperacion.Text,
                    Codigo = codpago,
                    TipCam = sisVariables.Gtipcam,
                    Moneda = monedaSeleccionada
                };
                viewmodel.Pagos.Add(nuevo_pago);
                pagos_a_realizar.Add(nuevo_pago);
                Importe.Text = "";
                sisVariables.GNumLote = TBNroLote.Text;
                TBNroLote.Text = "";
                TBNroOperacion.Text = "";
                TBDetalle.Text = "";
                tarjeta = "";
                banco = "";
                DPFechaOperacion.SelectedDate = DateTime.Today;

                foreach (var child in SubContenedoresTarjetas.Children.OfType<Button>())
                {
                    child.Background = (SolidColorBrush)Application.Current.FindResource("TextBox");
                    if (child.Content is TextBlock tb)
                        tb.Foreground = (SolidColorBrush)Application.Current.FindResource("BordeTextBox");
                }

                foreach (var child in SubContenedoresBanco.Children.OfType<Button>())
                {
                    child.Background = (SolidColorBrush)Application.Current.FindResource("TextBox");
                    if (child.Content is TextBlock tb)
                        tb.Foreground = (SolidColorBrush)Application.Current.FindResource("BordeTextBox");
                }

                ++doc;
        }
        

        private void Click_EliminarFila(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var pago = boton?.DataContext as PagoCalculo;

            if (pago != null)
            {
                viewmodel.Pagos.Remove(pago);
                int it = 0;
                foreach (var seleccion in pagos_a_realizar)
                {
                    if (seleccion.Codigo == pago.Codigo)
                    {
                        pagos_a_realizar.RemoveAt(it);
                        break;
                    }
                    ++it;
                }
                Importe.Text = pago.Importe.ToString();
            }
            doc = 0;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = sender as TextBox;

            bool EsAlfanumerico = char.IsLetterOrDigit(e.Text, 0);

            string currentText = textbox.Text.Remove(textbox.SelectionStart, textbox.SelectionLength);
            string Txtresultado = currentText.Insert(textbox.SelectionStart, e.Text);
            // Verifica si lo ingresado es número
            if (!EsAlfanumerico || Txtresultado.Length > 10)
                e.Handled = true;
        }

        private void CBCambiar_Seleccion(object sender, SelectionChangedEventArgs e)
        {
            if (Importe == null || Importe.Text == "" || !Importe.Text.Any(char.IsDigit))
                return;
            double valor_importe = Convert.ToDouble(Importe.Text);
            if (CambMoneda.SelectedIndex == 0)
                //soles
                Importe.Text = (valor_importe * (double)sisVariables.Gtipcam).ToString("0.00");
            else
                //dolares
                Importe.Text = (valor_importe / (double)sisVariables.Gtipcam).ToString("0.00");
        }

        private void DPFechaOperacion_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DPFechaOperacion.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("No puedes seleccionar una fecha futura", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                DPFechaOperacion.SelectedDate = DateTime.Today;
            }
        }

        private void Importe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Click_AgregarPago(BtnAgregarPago, new RoutedEventArgs());
        }
    }
}