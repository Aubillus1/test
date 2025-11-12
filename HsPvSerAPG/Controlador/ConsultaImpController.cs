using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace HsPvSerAPG.Controlador
{
    public class ConsultaImpController
    {
        private static readonly HttpClient client = new HttpClient();

        public List<ConsultaImp> ConsultarCabecera(int cia = 1, int tipdoc = 0, string  numdoc = "", string nroser = "", string fecha = "", int rango = 1)
        {
            string url = $"{sisVariables.GAPI}selectTabfac" + $"?cia={cia}" + $"&rango={rango}" +
                         $"&fecha={fecha}" +
                         $"&tipdoc={tipdoc}" +
                         $"&nroser={nroser}" +
                         $"&numdoc={numdoc}";

            Console.WriteLine($"[DEBUG] URL generada => {url}");

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"[DEBUG] JSON recibido => {json}");

                return JsonConvert.DeserializeObject<List<ConsultaImp>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return null;
            }
        
        }
        public List<DetalleDocumento> ConsultarDetalle(int cia, int tipdoc, string numdoc)
        {
            string url = $"{sisVariables.GAPI}selectDetfac?cia={cia}&tipdoc={tipdoc}&numdoc={Uri.EscapeDataString(numdoc)}";

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("JSON recibido (detalle): " + json);

               
                var detallesCrudos = JsonConvert.DeserializeObject<List<dynamic>>(json);

                var detalles = detallesCrudos?.Select(d => new DetalleDocumento
                {
                    Codigo = d.codart,
                    Producto = d.desdet,
                    desuni = d.desuni,
                    Cantidad = d.cantid,
                    Precio = d.prelis, 
                    Total = d.neto,
                    Coduni = d.coduni
                }).ToList();

                return detalles;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en consulta detalle: " + ex.Message);
                return null;
            }
        }
        public async Task<(bool sucess, string message, string numdoc)> GuardarFactura(TabFac cabecera)
        {
            string msg = "", num = "0";
            try
            {
                string url = $"{sisVariables.GAPI}insertUpdateTabfac";

                using (var cliente = new HttpClient())
                {
                    
                    var json = JsonConvert.SerializeObject(cabecera);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

               
                    var response = await cliente.PostAsync(url, content);
                   
                    if (response.IsSuccessStatusCode)
                    {
                        var respuestaJson = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Respuesta del API: {respuestaJson}");

                        try
                        {
                            var resultado = JsonConvert.DeserializeObject<dynamic>(respuestaJson);
                            msg = resultado.message.ToString();
                            num = resultado.numdoc != null ? resultado.numdoc.ToString() : "0";
                            if (resultado != null && resultado.success == true)
                            {
                                string numdoc = resultado.numdoc ?? ""; 
                                string tipdoc = cabecera.Tipdoc.ToString();
                                string cia = cabecera.cia.ToString();

                                string urlVoucher = $"{sisVariables.GAPI}consultaImp?cia={cia}&tipdoc={tipdoc}&numdoc={numdoc}";
                                Console.WriteLine($"Llamando a: {urlVoucher}");

                                var voucherResponse = await cliente.GetAsync(urlVoucher);
                                if (voucherResponse.IsSuccessStatusCode)
                                {
                                    var voucherJson = await voucherResponse.Content.ReadAsStringAsync();
                                    var obj = JsonConvert.DeserializeObject<dynamic>(voucherJson);
                                    bool ok = obj.success == true;
                                    Console.WriteLine($"PDF generado correctamente: {voucherJson}");
                                }
                                else
                                {
                                    Console.WriteLine($"Error al generar voucher: {voucherResponse.StatusCode}");
                                }
                                
                                return (true, msg, num);
                            }
                            else
                            {
                                Console.WriteLine($"API retornó success=false: {respuestaJson}");
                                return (false, msg, num);
                            }
                        }
                        catch (Exception jsonEx)
                        {
                            Console.WriteLine($"Error deserializando respuesta: {jsonEx.Message}");
                            return (true, msg, num); // Si no podemos deserializar, asumimos éxito de inserción
                        }
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error HTTP al guardar factura: {response.StatusCode} - {error}");
                        return (false, msg, num);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en GuardarFactura: {ex.Message}");
                return (false, msg, num);
            }
        }

        public async Task<(bool success, string numdoc)> EditarFactura(TabFacUpdateDto dto)
        {
            try
            {
                string url = $"{sisVariables.GAPI}insertUpdateTabfac";

                using (var cliente = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(dto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await cliente.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var respuestaJson = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Respuesta del API: {respuestaJson}");

                        try
                        {
                            var resultado = JsonConvert.DeserializeObject<dynamic>(respuestaJson);

                            if (resultado != null && resultado.success == true)
                            {
                                // Recuperar numdoc generado
                                string numdoc = resultado.numdoc != null ? (string)resultado.numdoc : dto.numdoc;

                                return (true, numdoc);
                            }
                            else
                            {
                                Console.WriteLine($"API retornó success=false: {respuestaJson}");
                                return (false, null);
                            }
                        }
                        catch (Exception jsonEx)
                        {
                            Console.WriteLine($"Error deserializando respuesta: {jsonEx.Message}");
                            return (false, null);
                        }
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error HTTP al editar factura: {response.StatusCode} - {error}");
                        return (false, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en EditarFactura: {ex.Message}");
                return (false, null);
            }
        }


        public Codven ConsultarVendedor(int cia = 1, int tipdoc = 0, string numdoc = "")
        {
            string url = $"{sisVariables.GAPI}consultaImp" + $"?cia={cia}" + $"&tipdoc={tipdoc}" + $"&numdoc={numdoc}";

            Console.WriteLine($"[DEBUG] URL generada => {url}");

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"[DEBUG] JSON recibido => {json}");

                // Usar JObject para ir directo al nodo "factura"
                var jObject = JObject.Parse(json);
                var factura = jObject["factura"];

                if (factura == null) return null;

                var vendedor = new Codven
                {
                    cod = factura.Value<int>("codven"),
                    des = factura.Value<string>("desven") ?? ""
                };

                return vendedor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return null;
            }
        }
        public ConsultaImp consultaImp(int cia = 1, int tipdoc = 0, string numdoc = "")
        {
            string url = $"{sisVariables.GAPI}consultaImp" + $"?cia={cia}" + $"&tipdoc={tipdoc}" + $"&numdoc={numdoc}";

            Console.WriteLine($"[DEBUG] URL generada => {url}");

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"[DEBUG] JSON recibido => {json}");

                // Usar JObject para ir directo al nodo "factura"
                var jObject = JObject.Parse(json);
                var factura = jObject["factura"];

                if (factura == null) return null;

                // Deserializar el nodo "factura" a ConsultaImp
                var consultaImp = factura.ToObject<ConsultaImp>();
                return consultaImp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return null;
            }
        }

    }
}
