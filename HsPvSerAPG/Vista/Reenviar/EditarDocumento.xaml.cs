using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
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
using System.IO;

namespace HsPvSerAPG.Vista.Reenviar
{
    /// <summary>
    /// Lógica de interacción para EditarDocumento.xaml
    /// </summary>
    public partial class EditarDocumento : Window
    {
        private List<DetalleDocumento> lista = new List<DetalleDocumento>();

        private ConsultaImpController consultaImpController = new ConsultaImpController();
        private TabfacController tabfacController = new TabfacController();
        private CliproController clipro = new CliproController();   

        public EditarDocumento(TabFac tabfac, List<DetalleDocumento> detalles)

        {

            InitializeComponent();
            LlenarDatos(tabfac);
            CargarDatagrid(tabfac, detalles);
            CargarSeries();

        }
        private void CargarSeries()
        {
            var listaSeries = new Controlador.ApiTabserService().selectTabser(sisVariables.GCia, sisVariables.GCodCaja, 0, 0);
            TBNumeroSerie.Text = listaSeries[0].codsun;
        }

        private void Click_BtnEliminar(object sender, RoutedEventArgs e)
        {
            // Obtener los elementos seleccionados (checkbox marcado)
            var seleccionados = lista.Where(d => d.IsSelected).ToList();

            if (seleccionados.Count == 0)
            {
                MessageBox.Show("No hay filas seleccionadas para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("¿Estas seguro de que deseas eliminar las filas seleccionadas?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Eliminar de la lista principal
                foreach (var item in seleccionados)
                {
                    lista.Remove(item);
                }

                // Actualizar el DataGrid
                dataGridDetalle.ItemsSource = null;
                dataGridDetalle.ItemsSource = lista;

                // Actualizar los totales
                ActualizarTotales(lista);
            }
        }

        private void Click_BtnSalir(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void ActualizarTotales(List<DetalleDocumento> detalles)
        {
            if (detalles == null || detalles.Count == 0)
            {
                txtValorBruto.Text = "0.00";
                txtValorVenta.Text = "0.00";
                txtIGV.Text = "0.00";
                txtImporteTotal.Text = "0.00";
                return;
            }
            decimal totalVenta = lista.Where(p => p.IsSelected).Sum(p => p.Total);
            double igv = Convert.ToDouble(totalVenta) * 0.18 / 1.18; // si el precio ya incluye IGV
            double valorVenta = Convert.ToDouble(totalVenta) - igv;
            decimal totalCantidad = lista.Where(p => p.IsSelected).Sum(p => p.Cantidad);

            txtValorBruto.Text = valorVenta.ToString("N2");
            txtValorVenta.Text = valorVenta.ToString("N2");
            txtIGV.Text = igv.ToString("N2");
            txtImporteTotal.Text = totalVenta.ToString("N2");
        }

        private void LlenarDatos(TabFac tabFac)
        {
            TBSucursal.Text = tabFac.Dessuc;
            TBRazSocial.Text = tabFac.Razsoc;

            // Obtener los primeros 4 dígitos y el resto de desdoc
            string desdoc = tabFac.Numdoc ?? string.Empty;
            string primerosCuatro = desdoc.Length >= 4 ? desdoc.Substring(0, 4) : desdoc;
            string resto = desdoc.Length > 4 ? desdoc.Substring(4) : string.Empty;

            TBDoc.Text = primerosCuatro;
            TBNumDoc.Text = resto;

            Codven vendedor = consultaImpController.ConsultarVendedor(sisVariables.GCia, tabFac.Tipdoc, tabFac.Numdoc);

            if (vendedor != null)
            {
                TBNumVendedor.Text = vendedor.cod.ToString();
                TBVendedor.Text = vendedor.des;
                
            }
            else
            {
                TBNumVendedor.Text = string.Empty;
                TBVendedor.Text = "Vendedor no encontrado";

            }
            TBFecha.Text = tabFac.Fecha;

           // TBNumVendedor.Text = consulta.codven.ToString();
            //TBVendedor.Text = consulta.desven;
            //TBFecha.Text = consulta.Fecha;
           
            TBTipMon.Text = tabFac.Tipmon == 1 ? "S/." : "$";


        }
        
        private void CargarDatagrid(TabFac tabFac, List<DetalleDocumento> detalles)
        {
            lista = detalles.Select(item => new DetalleDocumento
            {
                Codigo = item.Codigo,
                Producto = item.Producto,
                desuni = item.desuni,
                Cantidad = item.Cantidad,
                Precio = item.Precio,
                Total = item.Cantidad * item.Precio,
                Coduni = item.Coduni
            }).ToList();

            foreach (var detalle in lista)
            {
                detalle.CantidadOriginal = detalle.Cantidad;
            }

            dataGridDetalle.ItemsSource = lista;
        }

        private void Click_DGCheckBox(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb.DataContext is DetalleDocumento detalle)
                {
                    if (cb.IsChecked == true)
                        detalle.IsSelected = true;
                    else
                        detalle.IsSelected = false;
                    ActualizarTotales(lista);
                }
            }
        }
        private void dataGridDetalle_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.Row.Item is DetalleDocumento detalle)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    detalle.Total = detalle.Cantidad * detalle.Precio;

                    dataGridDetalle.Items.Refresh();

                    ActualizarTotales(lista);
                }), 
                System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void ComboBoxUMD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is DetalleDocumento detalle)
            {
                if (comboBox.SelectedItem is string selectedUMD)
                {
                    detalle.desuni = selectedUMD;
                }
            }
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


        private async void BTNGUARDAR(object sender, RoutedEventArgs e)
        {
            try
            {
                cargar.Visibility = Visibility.Visible;

                var productosSeleccionados = lista
                    .Where(p => p.IsSelected)
                    .ToList();

                if (productosSeleccionados == null || productosSeleccionados.Count == 0)
                {
                    MessageBox.Show("Para guardar el documento debes seleccionar al menos un artículo a editar",
                        "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal totalVentaDecimal = productosSeleccionados.Sum(p => (decimal)p.Total);
                float tipcam = sisVariables.Gtipcam > 0 ? (float)sisVariables.Gtipcam : 0f;
                int tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;

                int codigoCliente = await clipro.ObtenerCodigoClientePorDocumentoAsync(TBNumDoc.Text);

                // Construcción del DTO
                var dto = new TabFacUpdateDto
                {
                    cia = 1,
                    tipdoc = 77,
                    nroser = TBNumeroSerie.Text,
                    numdoc = "",
                    codigo = 1,
                    fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                    fecven = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"),
                    tipmon = tipmon,
                    tipcam = tipcam,
                    numref = TBNumeroSerie.Text + TBNumDoc.Text,
                    codven = int.Parse(TBNumVendedor.Text),
                    bruto = (float)productosSeleccionados.Sum(p => (decimal)p.Total / 1.18m),
                    igv = (float)productosSeleccionados.Sum(p => (decimal)p.Total) - (float)productosSeleccionados.Sum(p => (decimal)p.Total / 1.18m),
                    total = (float)totalVentaDecimal,
                    stanticipo = 0,
                    salant = (float)totalVentaDecimal,
                    detfacs = productosSeleccionados.Select((d, i) => new DetFacDto
                    {
                        correl = i + 1,
                        codlin = "00",
                        codart = d.Codigo,
                        desdet = d.Producto,
                        cantid = (double)d.Cantidad,
                        prelis = (double)d.Precio,
                        bruto = (double)d.Total,
                        precio = (double)d.Precio,
                        neto = (double)d.Total,
                        tipfac = 1,
                        coduni = d.Coduni
                    }).ToList()
                };

                var (exito, numdocGenerado) = await consultaImpController.EditarFactura(dto);
                if (!exito)
                {
                    MessageBox.Show("Error al actualizar los productos seleccionados.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Producto editado correctamente. NumDoc generado: {numdocGenerado}",
                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                var result = MessageBox.Show(
                    "¿Desea imprimir documento?",
                    "Imprimir Documento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result != MessageBoxResult.Yes)
                    return;
                string numdoc = numdocGenerado;
                int tipdoc = 77 ;
                int cia = sisVariables.GCia;

                string apiUrl = $"{sisVariables.GAPI}consultaImp?cia={cia}&tipdoc={tipdoc}&numdoc={numdoc}";

                using (var client = new HttpClient())
                {
                    await Task.Delay(1000);

                    var httpResponse = await client.GetAsync(apiUrl); // Renombrado para evitar conflicto
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show("No se pudo obtener la URL del PDF.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                    string pdfUrl = data.pdf_links.ticket;

                    if (string.IsNullOrEmpty(pdfUrl))
                    {
                        MessageBox.Show("No se encontró el enlace del PDF.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var pdfBytes = await client.GetByteArrayAsync(pdfUrl);
                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(pdfUrl));

                    File.WriteAllBytes(tempFilePath, pdfBytes);

                    // Imprimir PDF directamente
                    ImprimirPDFDirecto(tempFilePath);
                    File.Delete(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                cargar.Visibility = Visibility.Collapsed;
            }
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is DetalleDocumento documento)
            {
                // Intenta convertir el texto a número decimal (o int si tus cantidades son enteras)
                if (!decimal.TryParse(tb.Text, out decimal valor))
                    return;

                // Obtiene el binding actual del TextBox
                var bindingExpr = tb.GetBindingExpression(TextBox.TextProperty);

                // Teclas que deben validar o confirmar
                if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Enter)
                {
                    // Validación de rango
                    if (valor < 1 || valor > documento.CantidadOriginal)
                    {
                        MessageBox.Show($"La cantidad debe estar entre 1 y {documento.CantidadOriginal}.",
                                        "Cantidad inválida", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Restaura el valor anterior mostrado
                        tb.Text = documento.CantidadOriginal.ToString();
                        e.Handled = true;
                        return;
                    }

                    // ✅ Si está dentro del rango, actualiza el binding
                    bindingExpr?.UpdateSource();

                    // Recalcula totales al presionar Enter
                    if (e.Key == Key.Enter)
                        ActualizarTotales(lista);
                }
            }
        }
    }
}
