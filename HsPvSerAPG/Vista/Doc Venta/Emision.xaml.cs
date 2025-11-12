using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Utils.Clases;
using HsPvSerAPG.Vista.Control_de_Usuario;
using HsPvSerAPG.Vista.Doc_Venta.Boleta;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;

namespace HsPvSerAPG
{
    /// <summary>
    /// Lógica de interacción para Cobranza.xaml
    /// </summary>
    public partial class Emision : Window
    {
        private string Total;
        private string totalProductos;
        private string document_number;
        private string totalVenta;
        public string nroDoc;

        public string numdoc = "";

        private string cliente;
        private string telefono;
        private string direccion;
        public int st_anti;
        // public int st;

        public event EventHandler LimpiarGrilla;
        
        private List<Producto> productos;
        public int codigoCliente;
        private int codigoVendedor;

        private TabfacController Tabfac = new TabfacController();
        private CliproController Clipro = new CliproController();
        private ConsultaImpController consultaImpController = new ConsultaImpController();
        private TabfacController tabfacController = new TabfacController();

        private int stentregado = sisVariables.stentregado;

        public static double salant = sisVariables.salant;

       // private ConsultaImpController consultaImpController = new ConsultaImpController();

        private int cantiuni;
        public double igv;

        private List<Codven> vendedores = new List<Codven>();

        public Emision(List<Producto> productos, string totalVenta, string totalProductos,
                        string cliente, string telefono, string direccion, string nroDoc, int codigoCliente, int uni, double igv, int st_anti, int stentregado)
        {
            InitializeComponent();

            this.stentregado= stentregado;
            //this.st = sisVariables.anticipo;
            this.st_anti = st_anti;
            this.productos = productos;
            this.totalVenta = totalVenta;
            this.totalProductos = totalProductos;
            this.cliente = cliente;
            this.telefono = telefono;
            this.direccion = direccion;
            this.nroDoc = nroDoc;
            this.codigoCliente = codigoCliente;
            this.cantiuni = uni;
            this.igv = igv;
            CargarSeries(sisVariables.GCia, sisVariables.GCodCaja, 0, 0);
            CargarCBVendedores();
            this.stentregado = stentregado;
        }

        private void Click_BtnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
                            Copies = 3
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

        public void MostrarAlertaDePago(string ultimoDoc)
        {
            MessageBoxResult resultado = System.Windows.MessageBox.Show(
                "¿Desea realizar el pago ahora?",
                "Confirmación de pago",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resultado == MessageBoxResult.Yes)
            {
                int doc = sisVariables.GCodSuc;                

                // Ya no haces await ni consultas la base de datos.
                var ultimoregistro = tabfacController.selectTabFac(
                    sisVariables.GCia,
                    doc,
                    1,
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    41,
                    numdoc,
                    ultimoDoc 
                );

                TipoDePago frmPago = new TipoDePago(ultimoregistro);
                frmPago.ShowDialog();
                this.Close();
            }
            else
            {
                LimpiarGrilla?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
        }


        private void CargarCBVendedores()
        {
            vendedores = CodvenController.selectTabven(sisVariables.GCia, sisVariables.GCodven);
            CBVendedor.ItemsSource = vendedores;
            CBVendedor.DisplayMemberPath = "des";
            CBVendedor.SelectedValuePath = "cod";

            CBVendedor.SelectedIndex = 0;
        }

        private void CargarSeries(int cia, int codcaja, int electipdoc, int st_anti)
        {

            cia = sisVariables.GCia;
            codcaja = sisVariables.GCodCaja;
            electipdoc = 0;
            st_anti = sisVariables.anticipo;

            var listaSeries = new Controlador.ApiTabserService().selectTabser(cia, codcaja, electipdoc, st_anti);

            if (listaSeries != null && listaSeries.Count > 0)
            {

                var seriesValidas = listaSeries.Where(s => s.codsun.StartsWith("")).ToList();
                CMBXnroser.Text = seriesValidas[0].codsun;

            }
            else
            {
                System.Windows.MessageBox.Show("No se encontraron series disponibles.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CMBXnroser.Text = "0000";
            }
            
            CMBXnroser.ItemsSource = listaSeries;

            CMBXnroser.DisplayMemberPath = "nroser";
            CMBXnroser.SelectedValuePath = "codsun";

            CMBXnroser.SelectedIndex = 0;
        }

        //funciones descartadas
        private void BtnProforma_Click(object sender, RoutedEventArgs e)
        {
            //Proforma proforma = new Proforma();
            //proforma.ShowDialog();
        }

        private void Click_BtnBoleta(object sender, RoutedEventArgs e)
        {
          //  int tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;
          //  float tipcam = sisVariables.Gtipcam > 0 ? (float)sisVariables.Gtipcam : 0f;
          //  // Convertir el totalVenta a decimal
          //  if (!decimal.TryParse(totalVenta, out decimal totalVentaDecimal))
          //  {
          //      System.Windows.MessageBox.Show("Error al leer el total de la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          //      return;
          //  }

          //  // Validar si total es mayor a 700
          //  if (totalVentaDecimal > 699)
          //  {
          //      // Validar documento obligatorio
          //      if (string.IsNullOrWhiteSpace(nroDoc) ||
          //          !(nroDoc.Length == 8 || nroDoc.Length >= 9 && nroDoc.Length == 12))
          //      {
          //          System.Windows.MessageBox.Show("Para montos mayores a S/ 700.00 es obligatorio ingresar un número de documento válido (DNI, Pasaporte o Carnet de Extranjería).", "Documento requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
          //          return; // Evita que continúe
          //      }
          //  }

          ////  TipoDePago formasDePago = new TipoDePago(productos, totalVenta, nroDoc, totalProductos, cliente, telefono, direccion, cantiuni, igv, tipmon, tipcam, 1);
          // // formasDePago.ShowDialog();
        }
        private void BtnFactura_Click(object sender, RoutedEventArgs e)
        {
            //int tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;
            //float tipcam = sisVariables.Gtipcam > 0 ? (float)sisVariables.Gtipcam : 0f;
            //decimal.TryParse(totalVenta, out decimal totalVentaDecimal);


            ////TipoDePago tipopago = new TipoDePago(productos, totalVenta, nroDoc, totalProductos,
            //                                   cliente, telefono, direccion, cantiuni, igv, tipmon, tipcam, 0);
            ////tipopago.ShowDialog();

        }

        private async void BtnNotaDeVenta_Click(object sender, RoutedEventArgs e)
        {
            string ultimoDocGenerado = "";
            int tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;
            float tipcam = sisVariables.Gtipcam > 0 ? (float)sisVariables.Gtipcam : 0f;
            if (!decimal.TryParse(totalVenta, out decimal totalVentaDecimal))
            {
                System.Windows.MessageBox.Show("Error al leer el total de la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                int codVen = (CBVendedor.SelectedItem as Codven)?.cod ?? sisVariables.GCodven;
                try
                {
                    Cargar.Visibility = Visibility.Visible;
                    GTodo.IsHitTestVisible = false;
                    GTodo.Focusable = false;
                    int codigoCliente = await Clipro.ObtenerCodigoClientePorDocumentoAsync(nroDoc);
                }
                finally
                {
                    Cargar.Visibility = Visibility.Collapsed;
                    GTodo.IsHitTestVisible = true;
                    GTodo.Focusable = true;
                }


                try
                {
                    Cargar.Visibility = Visibility.Visible;
                    GTodo.IsHitTestVisible = false;
                    GTodo.Focusable = false;
                    var (success, message, numdoc) = await Tabfac.GuardarPagoTipoDePagoAsync(productos, codigoCliente, 41, CMBXnroser.Text,
                                                                            (float)(totalVentaDecimal - (decimal)igv), (float)totalVentaDecimal,
                                                                            tipmon, codVen, salant, sisVariables.anticipo, stentregado);
                    ultimoDocGenerado = numdoc;

                   

                    string apiUrl = $"{sisVariables.GAPI}consultaImp?cia={sisVariables.GCia}&tipdoc={41}&numdoc={ultimoDocGenerado}";

                    using (var client = new HttpClient())
                    {
                        await Task.Delay(1000);

                        var response = await client.GetAsync(apiUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("No se pudo obtener la URL del PDF.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                        string pdfUrl = data.pdf_links.ticket;

                        if (string.IsNullOrEmpty(pdfUrl))
                        {
                            MessageBox.Show("No se encontró el enlace del PDF.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        var pdfBytes = await client.GetByteArrayAsync(pdfUrl);
                        string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(pdfUrl));

                        File.WriteAllBytes(tempFilePath, pdfBytes);

                        // Imprimir PDF directamente
                        ImprimirPDFDirecto(tempFilePath);
                        File.Delete(tempFilePath);

                        if (!success)
                        {
                            System.Windows.MessageBox.Show("Error al registrar la cobranza ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                finally
                {
                    Cargar.Visibility = Visibility.Collapsed;
                    GTodo.IsHitTestVisible = true;
                    GTodo.Focusable = true;

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al procesar la nota de venta: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            System.Windows.MessageBox.Show($"Nota de venta registrada correctamente\nN° Doc: {ultimoDocGenerado}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            MostrarAlertaDePago(ultimoDocGenerado);

            //string numdoc = Txtnrodoc.Text;
            //string nroser = CMBXnroser.Text;
            //string fileName = $"Ticket_{nroser}_{numdoc}.pdf";
        }
    }
}
