using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
using System.Net.Http;

namespace HsPvSerAPG.Vista.Reenviar.Anticipo
{
    /// <summary>
    /// Lógica de interacción para AppAnticipo.xaml
    /// </summary>
    public partial class AppAnticipo : Window
    {
        private TabFacAnticiposController TabFacanticiposcontroller = new TabFacAnticiposController();

        private TabFac tabFac = new TabFac();
        private bool @switch = false;


        public AppAnticipo(TabFac consulta)
        {
            InitializeComponent();
            tabFac = consulta;
            TBTipCam.Text = sisVariables.Gtipcam.ToString("F6");
            CB_TM.Text = consulta.Desmon; //
            decimal valor_a_calcular = consulta.Tipmon == 1 ? consulta.Importe : consulta.ImporteDolares;
            TBImporteSOLES.Text = consulta.Tipmon == 1 ? $"{valor_a_calcular:F2}" : $"{valor_a_calcular * sisVariables.Gtipcam:F2}"; //
            TBImporteDOLARES.Text = consulta.Tipmon == 1 ? $"{valor_a_calcular / sisVariables.Gtipcam:F2}" : $"{valor_a_calcular:F2}";
            TBSOLESdocumento.Text = consulta.Tipmon == 1 ? $"{consulta.SaldoOriginal:F2}" : $"{consulta.SaldoOriginal * sisVariables.Gtipcam:F2}"; //
            TBDOLARESdocumento.Text = consulta.Tipmon == 1 ? $"{consulta.SaldoOriginal / sisVariables.Gtipcam:F2}" : $"{consulta.SaldoOriginal:F2}"; //
        }

        private void BtnClick_Emision(object sender, RoutedEventArgs e)
        {
            EmisionNotaPedido anticipo = new EmisionNotaPedido(tabFac);

            foreach (var anticipocomparar in anticipo.lista_anticipos)
            {
                if (anticipocomparar.Saldo > tabFac.Saldo)
                {
                    MessageBox.Show("El saldo es mayor");
                    return;
                }
            }
            anticipo.ShowDialog();
            if (anticipo.TienAnticipo == true)
            {
                TabFac Tipmon = anticipo.Tipmon;
                TBTipDoc.Text = anticipo.anticipo.TipDoc;
                TBNumDoc.Text = anticipo.anticipo.NumDoc;
                TBSaldoSOLES.Text = anticipo.anticipo.Saldo.ToString("F2");
                TBSaldoDOLARES.Text = anticipo.anticipo.Saldo.ToString("F2");
                Calcular();
                TBImporteSOLES.Text = TBImporteSOLES.Text;
                TBImporteDOLARES.Text = TBImporteDOLARES.Text;
            }
            
        }

        private void Calcular()
        {
            TBSaldoDOLARES.Text = Math.Round(Convert.ToDecimal(TBSaldoSOLES.Text) / Convert.ToDecimal(TBTipCam.Text), 6).ToString("F2");
            @switch = true;

            TBImporteDOLARES.Text = Math.Round(Convert.ToDecimal(TBImporteSOLES.Text) / Convert.ToDecimal(TBTipCam.Text), 6).ToString("F2");
            @switch = true;

        }

        private void Click_BtnSalir(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Está seguro que desea salir?",
                "Confirmar salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                this.Close(); 
                
            }
        }

        private void TBImporteSOLES_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (@switch)
                {
                    decimal soles;
                    if (decimal.TryParse(TBImporteSOLES.Text, out soles))
                    {
                        if (soles > Convert.ToDecimal(TBSaldoSOLES.Text))
                        {
                            TBImporteSOLES.Text = TBSOLESdocumento.Text;
                            TBImporteDOLARES.Text = TBDOLARESdocumento.Text;
                            MessageBox.Show("El importe a aplicar no puede ser mayor al saldo del anticipo", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        // Reemplaza la línea problemática en el método TBImporteSOLES_KeyDown:
                        decimal tipoCambio = Convert.ToDecimal(TBTipCam.Text);
                        decimal dolares = tipoCambio > 0 ? Math.Round(soles / tipoCambio, 2) : 0;
                        TBImporteDOLARES.Text = dolares.ToString("F2");
                    }
                    else
                    {
                        TBImporteSOLES.Text = "0.00";
                        TBImporteDOLARES.Text = "0.00";
                        MessageBox.Show("Ingrese un valor correcto", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                    MessageBox.Show("Ingrese primero un anticipo", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TBImporteDOLARES_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (@switch)
                {
                    decimal dolares;
                    if (decimal.TryParse(TBImporteDOLARES.Text, out dolares))
                    {
                        
                        decimal tipoCambio = Convert.ToDecimal(TBTipCam.Text);
                        decimal soles = tipoCambio > 0 ? Math.Round(dolares * tipoCambio, 6) : 0;
                        TBImporteSOLES.Text = soles.ToString("F2");
                    }
                    else
                    {
                        TBImporteSOLES.Text = "0.00";
                    }
                }
            }
        }

        private void TBTipCam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                Calcular();
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
        private async void BtnClick_Guardar(object sender, RoutedEventArgs e)
        {
            if (@switch)
            {
                if (tabFac.Saldo <= 1)
                {
                    MessageBox.Show("No puedes Aplicar");
                    return;
                }
                if (tabFac.Saldo > tabFac.Importe)
                {
                    MessageBox.Show("El saldo no puede ser mayor al importe");
                    return;
                }

                // Validar que importe no supere saldo del anticipo
                if (Convert.ToDecimal(TBImporteSOLES.Text) > Convert.ToDecimal(TBSaldoSOLES.Text) || Convert.ToDecimal(TBImporteDOLARES.Text) > Convert.ToDecimal(TBSaldoDOLARES.Text))
                {
                    MessageBox.Show("El importe a aplicar no puede ser mayor al saldo del anticipo");
                    return;
                }

                // Validar que importe no supere el saldo del documento
                if (Convert.ToDecimal(TBImporteSOLES.Text) > Convert.ToDecimal(TBSOLESdocumento.Text) || Convert.ToDecimal(TBImporteDOLARES.Text) > Convert.ToDecimal(TBImporteDOLARES.Text))
                {
                    MessageBox.Show("El importe a aplicar no puede ser mayor al saldo del documento");
                    return;
                }

                AnticipoRequest anticipo_request = new AnticipoRequest
                {
                    cia = sisVariables.GCia.ToString(),
                    tipdoc = 41.ToString(),
                    numdoc = TBTipDoc.Text + TBNumDoc.Text,
                    correl = 1,
                    docref = 41,
                    numref = tabFac.Numdoc,
                    codcaja = sisVariables.GCodCaja,
                    tipcam = Convert.ToDecimal(TBTipCam.Text),
                    impsol = Convert.ToDecimal(TBImporteSOLES.Text),
                    impdol = Convert.ToDecimal(TBImporteDOLARES.Text),
                    usrins = sisVariables.GUsuario
                };
                var (succes, correl, message) = await TabFacanticiposcontroller.aplicarAnticipo(anticipo_request);
                MessageBox.Show("Se aplico correctamente el anticipo", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                if (succes)
                {
                    var result = MessageBox.Show("¿Desea imprimir el comprobante?", "Imprimir", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        this.Close();
                        return;
                    }

                    int cia = sisVariables.GCia;
                        int tipcob = 9;
                        int codsuc = sisVariables.GCodSuc;
                        int tipdoc = 41;
                    string fecha = DateTime.Now.ToString("yyyyMMdd");

                        using (var cliente = new HttpClient())
                        {
                            try
                            {

                                string apiUrl = $"{sisVariables.GAPI}consultaImpPagare" +
                                                $"?cia={cia}&tipcob={tipcob}&codsuc={codsuc}&fecha={fecha}&correl={correl}&tipdoc={tipdoc}&numdoc={tabFac.Numdoc}";
                            

                            var response = await cliente.GetAsync(apiUrl);

                                if (!response.IsSuccessStatusCode)
                                {
                                    System.Windows.MessageBox.Show($"Error al generar comprobante ({response.StatusCode})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                string jsonResponse = await response.Content.ReadAsStringAsync();

                                var doc = System.Text.Json.JsonDocument.Parse(jsonResponse);
                                var root = doc.RootElement;

                                if (!root.TryGetProperty("pdf_link", out var pdfLinkElement))
                                {
                                    System.Windows.MessageBox.Show("No se encontró el link del comprobante en la respuesta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                                System.Windows.MessageBox.Show($"Error al generar o imprimir comprobante: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo aplicar el anticipo. \n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    //var anticipo = TabFacanticiposcontroller.aplicarAnticipo(anticipo_request);

                    TBTipCam.Text = sisVariables.Gtipcam.ToString("F6");
                }
                else
                    MessageBox.Show("Ingrese primero un anticipo", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        

        private void TBTipCam_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0) || e.Text == "."))
                e.Handled = true;
        }

        private void TBImporteSOLES_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBImporteSOLES.Text) || string.IsNullOrWhiteSpace(TBSaldoSOLES.Text) || string.IsNullOrWhiteSpace(TBSOLESdocumento.Text))
                return;

            if (decimal.TryParse(TBImporteSOLES.Text, out decimal nuevoPrecio))
            {
                bool precioValido = true;

                if (Convert.ToDecimal(TBImporteSOLES.Text) > Convert.ToDecimal(TBSaldoSOLES.Text) || Convert.ToDecimal(TBImporteSOLES.Text) > Convert.ToDecimal(TBSOLESdocumento.Text))
                    precioValido = false;

                TBImporteSOLES.Foreground = precioValido
                    ? (SolidColorBrush)Application.Current.FindResource("Text")
                    : Brushes.Red;
            }
        }

        private void TBImporteDOLARES_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBImporteSOLES.Text) || string.IsNullOrWhiteSpace(TBSaldoDOLARES.Text) || string.IsNullOrWhiteSpace(TBDOLARESdocumento.Text))
                return;

            if (string.IsNullOrWhiteSpace(TBImporteSOLES.Text))
                return;

            if (decimal.TryParse(TBImporteDOLARES.Text, out decimal nuevoPrecio))
            {
                bool precioValido = true;

                if (Convert.ToDecimal(TBImporteDOLARES.Text) > Convert.ToDecimal(TBSaldoDOLARES.Text) || Convert.ToDecimal(TBImporteDOLARES.Text) > Convert.ToDecimal(TBDOLARESdocumento.Text))
                    precioValido = false;

                TBImporteDOLARES.Foreground = precioValido
                    ? (SolidColorBrush)Application.Current.FindResource("Text")
                    : Brushes.Red;
            }
        }
    }
}
