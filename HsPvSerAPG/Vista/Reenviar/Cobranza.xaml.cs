using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Vista.Doc_Venta.Boleta;
using HsPvSerAPG.Vista.Reenviar;
using HsPvSerAPG.Vista.Reenviar.Anticipo;
using PdfiumViewer;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using static System.Resources.ResXFileRef;



namespace HsPvSerAPG.Vista
{
    public partial class Cobranza : Window
    {
        private ConsultaImpController consultaImpController = new ConsultaImpController();
        private TabfacController tabfacController = new TabfacController();
        private PDFcontroller PDFcontroller = new PDFcontroller();
        private TabFac documentoSeleccionado = null;

        private string Total;
        private string totalProductos;
        private string document_number;
        private string totalVenta;
        public string nroDoc;
        private string cliente;
        private string telefono;
        private string direccion;
        public int codigoCliente;
        private int codigoVendedor;

        private int cantiuni;
        public double igv;

        List<TabFac> listaDocs = new List<TabFac>();
        public Cobranza(string totalVenta, string totalProductos,
                        string cliente, string telefono, string direccion, string nroDoc, int codigoCliente, int uni, double igv)
        {
            InitializeComponent();
            this.totalVenta = totalVenta;
            this.totalProductos = totalProductos;
            this.cliente = cliente;
            this.telefono = telefono;
            this.direccion = direccion;
            this.nroDoc = nroDoc;
            this.codigoCliente = codigoCliente;
            this.cantiuni = uni;
            this.igv = igv;

            //Carga sucursales
            CargarSucursales();
            //DPFechaInicio.SelectedDate = DateTime.Now.AddDays(-7);
        }

        private void CalcularSumatoria()
        {
            decimal totalsoles =  Math.Round(listaDocs.Where(p => p.IsSelected).Sum(p => p.Importe), 2);
            decimal totaldolares =  Math.Round(listaDocs.Where(p => p.IsSelected).Sum(p => p.ImporteDolares), 2);
            txtTotalSOLES.Text = totalsoles.ToString("F2");
            txtTotalDOLARES.Text = totaldolares.ToString("F2");
        }
        private void CargarSucursales()
        {
            TabcajaController TabcajaController = new TabcajaController();

            var sucursales = TabcajaController.selectCajaLogin(sisVariables.GCia, sisVariables.GCodusr);
            CBSucursal.ItemsSource = sucursales;
            CBSucursal.DisplayMemberPath = "des";
            CBSucursal.SelectedValuePath = "cod";

            CBSucursal.SelectedIndex = 0;

            var vendedorSeleccionado = sucursales.FirstOrDefault(v => v.cod == sisVariables.GCodSuc);

            if (vendedorSeleccionado != null)
                CBSucursal.SelectedItem = vendedorSeleccionado;
        }

        private void Click_BtnWhatsApp(object sender, RoutedEventArgs e)
        {
            WhatsappEmergente whatsapp = new WhatsappEmergente(telefono);
            bool? result = whatsapp.ShowDialog();

            string numeroWhatsApp;
            if (result != true)
            {
                numeroWhatsApp = whatsapp.NumeroWhatsApp;
                string sid = "";
                var service = new WhatsAppService(sid, "");
                string fromNumber = "whatsapp:+";
                //string nombreArchivo = sisVariables.GCia + "-" + xtipdoc + "-" + XNumdoc.Substring(0, 4) + "-" + XNumdoc.Substring(4, 8) + ".pdf";

                string baseUrl = "http://192.168.18.36:8000/docvoucherfacturas/";
                string numdoc = documentoSeleccionado.Numdoc;

                //const string accountSid = "";
                //const string authToken = "";

                string link = $"{baseUrl}Factura__{numdoc}.pdf"; // Ajusta según el formato real del enlace
                string link2 = "https://www.google.com";


                try
                {
                    if (sender is Button btn)
                    {
                        if (btn.DataContext is ConsultaImp filaseleccionada)
                        {
                            service.EnviarWhatsappConTemplateYPDF(
                                fromNumber,
                                "whatsapp:+",
                                sid,
                                new[] { filaseleccionada.RazonSocial.Trim(), filaseleccionada.desdoc, filaseleccionada.Fecha, "1-1-F001-00056787.pdf" }
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"XD: {ex.Message}");
                    throw;
                }

                MessageBox.Show($"Mensaje enviado con SID: {sid}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                //    var service = new WhatsAppService("", "");
                //    string fromNumber = "whatsapp:+‬";
                //    string contentSid = "";
                //    //string nombreArchivo = sisVariables.GCia + "-" + xtipdoc + "-" + XNumdoc.Substring(0, 4) + "-" + XNumdoc.Substring(4, 8) + ".pdf";

                //    string baseUrl = "http://192.168.18.36:8000/docvoucherfacturas/";
                //    string nroser = txtSerie.Text.Trim();
                //    string numdoc = documentoSeleccionado.NumDoc;

                //    //const string accountSid = "";
                //    //const string authToken = "";

                //    string link = $"{baseUrl}Factura_{nroser}_{numdoc}.pdf"; // Ajusta según el formato real del enlace
                //    string link2 = "https://www.google.com";


                //    //service.EnviarWhatsappConTemplateYPDF(
                //    //    fromNumber,
                //    //    "whatsapp:" + 970593573,
                //    //    contentSid,
                //    //    new[] { objDato.RAZSOC.Trim(), objDato.desdoc, DateTime.Parse(objDato.FECHA).ToString("dd/MM/yyyy"), nombreArchivo }
                //    //);

                //    //MessageBox.Show($"Mensaje enviado con SID: {message.Sid}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void ImprimirPDFDirecto(string pdfPath)
        {
            try
            {
                var dlg = new CopiesDialog();
                dlg.Owner = Application.Current.MainWindow;

                bool? result = dlg.ShowDialog();
                if (result != true) return;

                int copias = dlg.Copias;

                using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        var printerSettings = new PrinterSettings
                        {
                            PrinterName = new PrinterSettings().PrinterName,
                            Copies = (short)copias
                        };

                        var pageSettings = new PageSettings(printerSettings)
                        {
                            Margins = new Margins(0, 0, 0, 0)
                        };

                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();

                        printDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al imprimir PDF: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        private async void Click_BtnImprimir(object sender, RoutedEventArgs e)
        {
            try
            {
                if (documentoSeleccionado == null)
                {
                    MessageBox.Show("Selecciona un documento del listado para imprimir.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "¿Desea imprimir documento?",
                    "Imprimir Documento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result != MessageBoxResult.Yes)
                    return;

                string numdoc = documentoSeleccionado.Numdoc;
                string tipdoc = documentoSeleccionado.Tipdoc.ToString();
                int cia = documentoSeleccionado.cia;

                string apiUrl = $"{sisVariables.GAPI}consultaImp?cia={sisVariables.GCia}&tipdoc={tipdoc}&numdoc={numdoc}";

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e) => this.Close();  


        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG.SelectedItem is TabFac docSeleccionado)
            {
                documentoSeleccionado = docSeleccionado; // guardamos selección

                int cia = sisVariables.GCia;
                int tipdoc = docSeleccionado.Tipdoc;
                string numdoc = docSeleccionado.Numdoc ?? "";

                var detalles = consultaImpController.ConsultarDetalle(cia, tipdoc, numdoc);
                DGDetalles.ItemsSource = (detalles != null && detalles.Count > 0) ? detalles : null;
            }
            else
            {
                documentoSeleccionado = null;
                DGDetalles.ItemsSource = null;

            }
        }


        //btn buscar documetos
        private async void Button_Click(object sender, RoutedEventArgs e) => await Buscar();

        private async Task Buscar()
        {
            try
            {
                
                CargarGeneral.Visibility = Visibility.Visible;
                int cia = sisVariables.GCia;
                int sucursalValue = Convert.ToInt32(CBSucursal.SelectedValue?.ToString());
                int rango = 1;
                int tipdoc = 41;
                string fecha1 = DPFechaInicio.SelectedDate?.ToString("yyyy-MM-dd") ?? "";
                string fecha2 = DPFechaFin.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

                decimal saldo = 0.00M;
                listaDocs = await Task.Run(() =>
                {
                    return tabfacController.selectTabFac(cia, sucursalValue, rango, fecha1, fecha2, tipdoc);
                });

                if (listaDocs == null || listaDocs.Count == 0)
                {
                    DG.ItemsSource = null;
                    return;
                }
                // Filtrar por cliente si TBCliente.Text no está vacío
                if (!string.IsNullOrWhiteSpace(TBCliente.Text))
                {
                    string filtroCliente = TBCliente.Text.Trim().ToLower();
                    listaDocs = listaDocs
                        .Where(d => !string.IsNullOrEmpty(d.Razsoc) && d.Razsoc.ToLower().Contains(filtroCliente))
                        .ToList();
                }

                // Filtrado adicional según estado
                if (CBTodos.IsChecked == true)
                {
                    // No filtrar, mostrar todos
                }
                else if (CBCancelado.IsChecked == true)
                {
                    // Mostrar solo los cancelados (total == 0)
                    listaDocs = listaDocs.Where(d => d.Importe == 0).ToList();
                }
                else if (CBPendiente.IsChecked == true)
                {
                    // Mostrar solo los pendientes (total != 0)
                    listaDocs = listaDocs.Where(d => d.Importe > 0).ToList();
                }

                //Guardar el valor del saldo original
                foreach (var item in listaDocs)
                    item.SaldoOriginal = item.Saldo;

                DG.ItemsSource = listaDocs;
                txtTotalSOLES.Text = "0.00";
                txtTotalDOLARES.Text = "0.00";
            }
            finally
            {
                CargarGeneral.Visibility = Visibility.Collapsed;
            }           
        }

        private void Click_BtnEditar(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.DataContext == null)
                {
                    MessageBox.Show("FALLO NULL");
                    return;
                }
                if (btn.DataContext is TabFac consulta)
                {
                    var detalles = consultaImpController.ConsultarDetalle(sisVariables.GCia, consulta.Tipdoc, consulta.Numdoc);
                    if (detalles == null || detalles.Count == 0)
                    {
                        MessageBox.Show("El documento no tiene detalles para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    EditarDocumento editar = new EditarDocumento(consulta, detalles);
                    editar.ShowDialog();  
                }
            }
        }
        private void Click_DGCheckBox(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb.DataContext is TabFac tabfac)
                    tabfac.IsSelected = cb.IsChecked == true;
            }
            CalcularSumatoria();
        }

        private void BtnDetallar_CLick(object sender, RoutedEventArgs e)
        {
            //LOGICA CHECK
            var documentos_seleccionados = new List<TabFac>();

            foreach (var item in DG.Items)
            {
                if (item is TabFac doc && doc.IsSelected)
                {
                    documentos_seleccionados.Add(doc);
                }
            }

            if (documentos_seleccionados == null || documentos_seleccionados.Count == 0)
            {
                MessageBox.Show("Debes seleccionar un documento", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            foreach (var doc in documentos_seleccionados)
            {
                if (documentos_seleccionados[0].Razsoc != doc.Razsoc)
                {
                    MessageBox.Show("Debes elegir documentos con la misma razon social", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var tipoDePagoWindow = new TipoDePago(documentos_seleccionados);
            tipoDePagoWindow.ShowDialog();
            Buscar();
        }

        private async void BtnAnticipo_CLick(object sender, RoutedEventArgs e)
        {
            var documentos_seleccionados = new List<TabFac>();

            foreach (var item in DG.Items)
            {
                if (item is TabFac doc && doc.IsSelected)
                {
                    documentos_seleccionados.Add(doc);
                }
            }

            if (documentos_seleccionados == null || documentos_seleccionados.Count == 0)
            {
                MessageBox.Show("Debes seleccionar un documento", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (documentos_seleccionados.Count != 1)
            {
                MessageBox.Show("Selecciona un único documento del listado para registrar anticipo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var documento = documentos_seleccionados.ToList()[0];

            if (documento.Saldo < 0)
            {
                MessageBox.Show("El documento ya está cancelado.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppAnticipo anticipo = new AppAnticipo(documento);
            anticipo.ShowDialog();

            await Buscar();
        }

        private void TBCliente_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Buscar();
        }

        private void DPFechaFin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DPFechaFin.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("No puedes seleccionar una fecha futura", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                DPFechaFin.SelectedDate = DateTime.Today;
            }
        }

        private void DPFechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DPFechaFin != null)
            {
                if (DPFechaInicio.SelectedDate > DPFechaFin.SelectedDate)
                {
                    MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha fin", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DPFechaInicio.SelectedDate = DPFechaFin.SelectedDate;
                } 
            }
        }

        private async void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    CargarActualizando.Visibility = Visibility.Visible;

                    var tb = (TextBox)sender;
                    var bindingExpr = tb.GetBindingExpression(TextBox.TextProperty);

                    if (decimal.TryParse(tb.Text, out decimal valor) && tb.DataContext is TabFac documento)
                    {
                        string header = DG.CurrentColumn.Header?.ToString();

                        bool valido = true;
                        string mensaje = "";

                        if (header == "Imp. S/.")
                        {
                            if (documento.Tipmon == 1 && valor > documento.SaldoOriginal)
                            {
                                valido = false;
                                mensaje = $"El IMPORTE SOLES no debe ser mayor al saldo original: \n(S/. {documento.SaldoOriginal})";
                            }
                            else if (documento.Tipmon == 2 && (valor / (decimal)documento.Tipcam) > documento.SaldoOriginal)
                            {
                                valido = false;
                                mensaje = $"El IMPORTE SOLES no debe ser mayor al saldo original: \n($ {documento.SaldoOriginal})";
                            }
                        }
                        else if (header == "Imp. $")
                        {
                            if (documento.Tipmon == 1 && (valor * (decimal)documento.Tipcam) > documento.SaldoOriginal)
                            {
                                valido = false;
                                mensaje = $"El IMPORTE DÓLARES no debe ser mayor al saldo original: \n(S/. {documento.SaldoOriginal})";
                            }
                            else if (documento.Tipmon == 2 && valor > documento.SaldoOriginal)
                            {
                                valido = false;
                                mensaje = $"El IMPORTE DÓLARES no debe ser mayor al saldo original: \n($ {documento.SaldoOriginal})";
                            }
                        }

                        if (!valido)
                        {
                            bindingExpr.UpdateTarget(); // vuelve al valor original
                            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            var itemActual = DG.SelectedItem ?? documento;
                            int index = DG.Items.IndexOf(itemActual);

                            bindingExpr.UpdateSource(); // acepta el nuevo valor
                            await Task.Delay(500);
                            DG.Items.Refresh();

                            // Restaura la selección
                            if (index >= 0 && index < DG.Items.Count)
                            {
                                DG.SelectedIndex = index;
                                DG.Focus();
                                DG.ScrollIntoView(DG.SelectedItem);
                            }
                        }
                        CalcularSumatoria();
                        e.Handled = true; // evita que el DataGrid mueva selección
                    }
                }
                finally
                {
                    CargarActualizando.Visibility = Visibility.Collapsed;
                }
            }
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0) || e.Text == "."))
                e.Handled = true;
        }

        private async void CBSucursal_SelectionChanged(object sender, SelectionChangedEventArgs e) => await Buscar();
    }
}