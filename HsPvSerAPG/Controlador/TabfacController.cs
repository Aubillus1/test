using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Utils.Clases;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
//using System.Windows.Forms;

namespace HsPvSerAPG.Controlador
{
    public class TabfacController
    {
        private static readonly HttpClient client = new HttpClient();
        private ConsultaImpController consultaImpController = new ConsultaImpController();
        private CliproController cliproController = new CliproController();
        private TabcamController tabcamController = new TabcamController();
        public static string UltimoNumdoc { get; private set; }
        public List<TabFac> selectTabFac(int cia, int codsuc, int rango, string fecha1, string fecha2, int tipdoc, string nroser = "", string numdoc = "")
        {
            string url = $"{sisVariables.GAPI}selectTabfac?cia={cia}&codsuc={codsuc}&rango={rango}&fecha1={fecha1}&fecha2={fecha2}&tipdoc={tipdoc}&nroser={nroser}&numdoc={numdoc}";

            try
            {
                var response = client.GetAsync(url).Result; // Llamada sincrónica
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;

                var lista = JsonConvert.DeserializeObject<List<TabFac>>(responseJson);
                return lista;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al consumir API: {ex.Message}");
                return null;
            }
        }       
        public async Task<(bool sucess, string message, string numdoc)> GuardarPagoTipoDePagoAsync(List<Producto> productos,
            int codCliente, int tipDoc, string serie, float bruto, float total,
            int tipMon, int codVen, double salant, int anticipo, int stentregado)
        {
            try
            {
                if (productos == null || productos.Count == 0)
                    return (false, "", "");

                var tabcamObj = tabcamController.SelectTabcam();
                if (tabcamObj == null || tabcamObj.parale <= 0)
                {
                    System.Windows.MessageBox.Show("No se pudo obtener el tipo de cambio del día.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return (false, "", "");
                }
                double tipCam = (double)tabcamObj.parale;

                float igv = bruto * 0.18f;
                float saldoBruto = total - igv;

                TabFac factura = new TabFac
                {
                    cia = sisVariables.GCia,
                    Tipdoc = tipDoc,
                    nroser = serie,
                    Numdoc = "",
                    codigo = codCliente,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                    fecven = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
                    Tipmon = tipMon,
                    Tipcam = (float)tipCam,
                    codven = codVen,
                    Bruto = saldoBruto,
                    Igv = igv,
                    Total = total,
                    salant = (decimal)total,
                    stanticipo = sisVariables.anticipo,
                    detfacs = new List<Detfac>(),
                    stentregado = sisVariables.stentregado,
                    
                };

                int correl = 1;
                foreach (var prod in productos)
                {
                    float cantidad = (float)prod.Cant;
                    float precioUnit = (float)prod.Unit;
                    factura.detfacs.Add(new Detfac
                    {
                        correl = correl++,
                        codlin = "00",
                        codart = prod.codigo ?? "000",
                        desdet = prod.Descripcion ?? "SIN DESCRIPCION",
                        cantid = cantidad,
                        prelis = precioUnit,
                        bruto = precioUnit * cantidad,
                        precio = precioUnit,
                        neto = precioUnit * cantidad,
                        tipfac = 1,
                        coduni = prod.coduni,
                    });
                }
                return await consultaImpController.GuardarFactura(factura);
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return (false, "", "");
            }
        }
        
        
    }

}